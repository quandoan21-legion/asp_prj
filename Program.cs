using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using MyApi.Data;

var builder = WebApplication.CreateBuilder(args);

// ===== Services =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with Bearer token support
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new() { Title = "MyApi", Version = "v1" });

    var bearerScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Paste JWT only (without the word 'Bearer') when using Swagger's Authorize dialog."
    };
    o.AddSecurityDefinition("Bearer", bearerScheme);
    o.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        [bearerScheme] = Array.Empty<string>()
    });
});

// PostgreSQL EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===== JWT Authentication =====
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var cfg = builder.Configuration.GetSection("Jwt");

        options.RequireHttpsMetadata = false; // dev only
        options.SaveToken = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = cfg["Issuer"], // "MyApi"
            ValidAudience = cfg["Audience"], // "MyApiUsers"
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(cfg["Key"]!)),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.Name,
            ClockSkew = TimeSpan.Zero
        };

        // 🧹 TỰ LÀM SẠCH TOKEN trước khi validate
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                // 1) Lấy token từ header Authorization
                var auth = ctx.Request.Headers["Authorization"].ToString();

                if (!string.IsNullOrWhiteSpace(auth) &&
                    auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var raw = auth.Substring("Bearer ".Length);

                    // 2) Trim khoảng trắng/newline lạ
                    raw = raw.Trim();

                    // 3) Nếu dán kèm dấu ngoặc kép "token"
                    if (raw.Length >= 2 && raw.StartsWith("\"") && raw.EndsWith("\""))
                        raw = raw.Substring(1, raw.Length - 2).Trim();

                    // 4) Loại bỏ ký tự whitespace không chuẩn (Unicode categories)
                    raw = new string(raw.Where(c => !char.IsControl(c) && !char.IsWhiteSpace(c)).ToArray());

                    // 5) Nếu ai đó dán "Bearer Bearer <token>" → chỉ lấy phần cuối
                    var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 1)
                        raw = parts.Last();

                    ctx.Token = raw;
                }

                // (Tuỳ chọn) cũng cho phép lấy từ query "access_token" (SignalR, v.v.)
                if (string.IsNullOrEmpty(ctx.Token))
                {
                    var q = ctx.Request.Query["access_token"].ToString();
                    if (!string.IsNullOrWhiteSpace(q))
                        ctx.Token = q.Trim().Trim('"');
                }

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"[JWT] Auth failed: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },

            OnChallenge = ctx =>
            {
                Console.WriteLine($"[JWT] Challenge: {ctx.Error} - {ctx.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ===== Pipeline =====
if (app.Environment.IsDevelopment())
{
    IdentityModelEventSource.ShowPII = true; // chỉ bật khi dev để xem chi tiết lỗi mã hoá
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Log nhanh Authorization header & đếm số dấu chấm (giúp soi lỗi “1 chấm”)
app.Use(async (ctx, next) =>
{
    var auth = ctx.Request.Headers["Authorization"].ToString();
    if (!string.IsNullOrEmpty(auth))
    {
        var dotCount = auth.Count(c => c == '.');
        Console.WriteLine($"[AUTH HEADER] {auth}");
        Console.WriteLine($"[AUTH DOTS] {dotCount}");
    }

    await next();
});

app.UseHttpsRedirection();

// 🔐 Auth middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Public test endpoints
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/", () => "Hello from My API!").AllowAnonymous();

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast(
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            )).ToArray();

        return forecast;
    })
    .WithName("GetWeatherForecast")
    .AllowAnonymous();

app.Run();

// ===== Records =====
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}