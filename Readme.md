# MyApi

A simple **ASP.NET Core Web API** project using **Entity Framework Core** and **PostgreSQL**.  
Supports CRUD operations with auto-generated database tables.

---

## 📦 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL installed and running (e.g. via Docker or local install)

---

## 🚀 Setup Instructions

### 1. Restore dependencies
```bash
dotnet restore
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Swashbuckle.AspNetCore
# asp_prj
