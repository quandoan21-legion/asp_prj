using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Students",
                newName: "StudentID");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Students",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Students",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Students",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "Students",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "RollNumberGlobal",
                table: "Students",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsNew = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseID);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourseRegistration",
                columns: table => new
                {
                    RegistrationID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentID = table.Column<int>(type: "integer", nullable: false),
                    CourseID = table.Column<int>(type: "integer", nullable: false),
                    ExamID = table.Column<int>(type: "integer", nullable: false),
                    RollNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ApplicationStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourseRegistration", x => x.RegistrationID);
                    table.ForeignKey(
                        name: "FK_StudentCourseRegistration_Courses_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Courses",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourseRegistration_Exams_ExamID",
                        column: x => x.ExamID,
                        principalTable: "Exams",
                        principalColumn: "ExamID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourseRegistration_Students_StudentID",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExamResults",
                columns: table => new
                {
                    ResultID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistrationID = table.Column<int>(type: "integer", nullable: false),
                    MarksObtained = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    MaxMarks = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    AssignedClass = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CourseFee = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PaymentDeadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaymentStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ExtraLabOpted = table.Column<bool>(type: "boolean", nullable: false),
                    ExtraLabFee = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamResults", x => x.ResultID);
                    table.ForeignKey(
                        name: "FK_ExamResults_StudentCourseRegistration_RegistrationID",
                        column: x => x.RegistrationID,
                        principalTable: "StudentCourseRegistration",
                        principalColumn: "RegistrationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_RegistrationID",
                table: "ExamResults",
                column: "RegistrationID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseRegistration_CourseID",
                table: "StudentCourseRegistration",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseRegistration_ExamID",
                table: "StudentCourseRegistration",
                column: "ExamID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourseRegistration_StudentID",
                table: "StudentCourseRegistration",
                column: "StudentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamResults");

            migrationBuilder.DropTable(
                name: "StudentCourseRegistration");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "RollNumberGlobal",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "StudentID",
                table: "Students",
                newName: "Id");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Students",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
