# 🚀 ProjectBase - .NET 10 Clean Architecture Template

<p align="center">
  <img src="https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 10"/>
  <img src="https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL"/>
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis"/>
  <img src="https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT"/>
  <img src="https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black" alt="Swagger"/>
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License"/>
</p>

## 📋 Loyiha haqida

**ProjectBase** - bu .NET 10 platformasida **Clean Architecture** prinsiplariga asoslangan, production-ready Web API loyihasi uchun asos bo'lib xizmat qiluvchi shablon. Ushbu loyiha barcha dasturchilar uchun yangi loyihalarni tez va sifatli boshlash imkonini beradi.

### ✨ Asosiy xususiyatlar

| Xususiyat | Tavsif |
|-----------|--------|
| 🏗️ **Clean Architecture** | Domain, Application, Infrastructure va WebApi qatlamlari |
| 🔐 **JWT Authentication** | Access va Refresh token asosida xavfsiz autentifikatsiya |
| 🛡️ **Role-Based Authorization** | Permissions va Roles orqali foydalanuvchi huquqlarini boshqarish |
| 📊 **Serilog** | Fayl va konsolga loglash (daily rolling) |
| ⚡ **Rate Limiting** | API so'rovlarini cheklash (Fixed Window) |
| 📚 **Swagger/OpenAPI** | Interaktiv API hujjatlari (IP cheklovi bilan) |
| 🗄️ **Entity Framework Core** | PostgreSQL bilan ishlash, Migrations, Seeding |
| 📦 **Redis** | SignalR uchun distributed cache |
| 🗜️ **Response Compression** | Gzip va Brotli siqish |
| ❤️ **Health Checks** | PostgreSQL va DbContext monitoring |
| 🌍 **IP Geolocation** | Foydalanuvchi joylashuvini aniqlash |
| 🔄 **AutoMapper** | Object mapping |
| ⚠️ **Global Exception Handling** | ProblemDetails standarti bilan xatolarni boshqarish |

---

## 🏛️ Arxitektura

Loyiha **Clean Architecture** (Onion Architecture) asosida tuzilgan:

```
ProjectBase/
├── 📁 Domain/                    # Core business logic
│   ├── Abstraction/              # Interfaces, Base classes, Errors
│   │   ├── Attributes/           # Custom attributes
│   │   ├── Authentication/       # Auth DTOs, Permissions
│   │   ├── Base/                 # Entity, AuditableEntity, IBaseRepository
│   │   ├── Configuration/        # Auth configuration models
│   │   ├── Consts/               # Constants (Status, Gender, Countries...)
│   │   ├── Errors/               # Error handling (Result pattern)
│   │   ├── Extensions/           # String, Enum extensions
│   │   ├── Helpers/              # Utility helpers
│   │   ├── Interface/            # Service interfaces
│   │   ├── Jwt/                  # JWT options
│   │   ├── Models/               # Domain models
│   │   ├── Options/              # Rate limit options
│   │   └── Results/              # Result<T> pattern
│   └── EfClasses/                # Entity classes
│       ├── Authentication/       # Permission, Role, UserRole
│       ├── Enums/                # EnumStatus, EnumGender...
│       ├── Info/                 # Country, Region, District...
│       ├── Person/               # Person entity
│       ├── Tokens/               # Token, DeviceInfo
│       └── User/                 # User entity
│
├── 📁 Application/               # Application services
│   ├── Extensions/               # HttpContext, DeviceInfo extractors
│   ├── Mappers/                  # AutoMapper profiles
│   └── Service/                  # Business services
│       ├── Authentication/       # AuthService, JwtTokenService
│       ├── BaseService/          # CrudServiceBase
│       └── IpGeolocationService/ # IP location service
│
├── 📁 Infrastructure/            # Data access & external services
│   ├── Configuration/            # EF Core entity configurations
│   ├── Context/                  # ApplicationDbContext
│   ├── Migrations/               # EF Core migrations
│   ├── Repositories/             # Repository implementations
│   │   ├── Base/                 # BaseRepository, UnitOfWork
│   │   ├── Permission/           # PermissionRepository
│   │   ├── Token/                # TokenRepository
│   │   └── User/                 # UserRepository
│   └── Seeds/                    # Data seeding
│       ├── SeedDefaultEnums.cs
│       ├── SeedDefaultInfo.cs
│       ├── SeedDefaultPersonAndUser.cs
│       └── SeedPermissionsAndRoles.cs
│
└── 📁 ProjectBase.Web/           # API Layer
    ├── Controllers/              # API endpoints
    ├── Extensions/               # DI, Swagger, Policies, Filters
    ├── Middleware/               # Exception, Token validation
    ├── wwwroot/                  # Static files
    └── logs/                     # Serilog log files
```

### 📐 Qatlamlar orasidagi bog'liqlik

```
┌─────────────────────────────────────────────────────────┐
│                    ProjectBase.Web                       │
│              (Controllers, Middleware, DI)               │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                     Application                          │
│            (Services, Mappers, Extensions)               │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                    Infrastructure                        │
│      (DbContext, Repositories, Configurations)           │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                       Domain                             │
│    (Entities, Interfaces, Errors, Business Rules)        │
└─────────────────────────────────────────────────────────┘
```

---

## 🔧 Texnologiyalar

### Backend
- **.NET 10** - Eng so'nggi .NET versiyasi
- **ASP.NET Core Web API** - RESTful API
- **Entity Framework Core 10** - ORM
- **PostgreSQL** - Ma'lumotlar bazasi
- **Redis** - Distributed cache & SignalR backplane

### Kutubxonalar
| Paket | Versiya | Tavsif |
|-------|---------|--------|
| `Serilog` | 4.3.0 | Structured logging |
| `AutoMapper` | 16.0.0 | Object-to-object mapping |
| `Swashbuckle` | Latest | Swagger/OpenAPI |
| `Microsoft.AspNetCore.Identity` | 2.3.1 | Password hashing |
| `System.IdentityModel.Tokens.Jwt` | 8.15.0 | JWT token handling |
| `StackExchange.Redis` | Latest | Redis client |
| `Newtonsoft.Json` | 13.0.3 | JSON serialization |

---

## 🚀 O'rnatish va ishga tushirish

### Talablar

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/) (v14+)
- [Redis](https://redis.io/download) (ixtiyoriy, SignalR uchun)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) yoki [VS Code](https://code.visualstudio.com/)

### 1️⃣ Repositoriyni klonlash

```bash
git clone https://github.com/BakhodirovDev/ProjectBase.git
cd ProjectBase
```

### 2️⃣ Ma'lumotlar bazasini sozlash

`appsettings.json` faylida connection string ni o'zgartiring:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=your_database;Username=postgres;Password=your_password"
  }
}
```

### 3️⃣ Migratsiyalarni qo'llash

```bash
cd ProjectBase.Web
dotnet ef database update --project ../Infrastructure
```

### 4️⃣ Loyihani ishga tushirish

```bash
dotnet run
```

Yoki Visual Studio'da `F5` tugmasini bosing.

### 5️⃣ Swagger UI

Development muhitida Swagger avtomatik yoqiladi:
```
https://localhost:5001/swagger/v1/index.html
```

> ⚠️ **Eslatma:** Swagger faqat `AllowedSwaggerIPs` ro'yxatidagi IP lardan ochiladi.

---

## ⚙️ Konfiguratsiya

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=mydb;Username=postgres;Password=postgres",
    "Redis": "localhost:6379"
  },
  "JwtOptions": {
    "Key": "your-super-secret-key-at-least-32-characters",
    "Issuer": "api.example.com",
    "Audience": "api.example.com",
    "ExpiresInMinutes": 60
  },
  "RateLimiter": {
    "GlobalLimiter": {
      "PermitLimit": 5,
      "WindowInMinutes": 0.01667,
      "QueueLimit": 0
    }
  },
  "SwaggerSettings": {
    "Enabled": true,
    "RoutePrefix": "swagger",
    "Version": "v1",
    "Title": "ProjectBase API",
    "AllowedSwaggerIPs": ["127.0.0.1", "::1"]
  },
  "AllowedOrigins": [
    "https://your-frontend.com"
  ],
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log_.log",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

---

## 🔐 Autentifikatsiya

### JWT Token Flow

```
1. POST /Auth/SignIn (login, password)
        │
        ▼
2. Server JWT Access Token + Refresh Token qaytaradi
        │
        ▼
3. Client Access Token ni Authorization header da yuboradi
   Authorization: Bearer <access_token>
        │
        ▼
4. Access Token muddati tugaganda:
   GET /Auth/RefreshToken?refreshToken=<token>
        │
        ▼
5. Yangi token juftligi qaytariladi
```

### API Endpoints

| Method | Endpoint | Tavsif |
|--------|----------|--------|
| `POST` | `/Auth/SignIn` | Tizimga kirish |
| `GET` | `/Auth/RefreshToken` | Tokenni yangilash |
| `GET` | `/Auth/Logout` | Tizimdan chiqish |
| `GET` | `/Auth/IsSecure` | Autentifikatsiyani tekshirish |

---

## 🏗️ Domain Layer - Asosiy tushunchalar

### Entity Base Class

```csharp
public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Id { get; private set; }
    // Equality implementation...
}
```

### AuditableEntity

```csharp
public abstract class AuditableEntity<TId> : Entity<TId>
{
    public DateTime CreatedAt { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public Guid? UpdatedBy { get; private set; }
    public int StatusId { get; private set; }
}
```

### Result Pattern

```csharp
// Muvaffaqiyatli natija
return Result.Success();
return Result<User>.Success(user);

// Xato natija
return Result.Failure(Error.NotFound);
return Result<User>.Failure("USER_NOT_FOUND", "User not found");
```

### Repository Pattern

```csharp
public interface IBaseRepository<TEntity, TId>
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    Task<(List<TEntity> Items, int TotalCount)> GetPagedAsync(...);
    Task<TEntity> AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    // ...
}
```

---

## 🔒 Permission System

### Permission Attribute

```csharp
[PermissionModule("System", "Tizim sozlamalari")]
public enum SystemPermissions
{
    [PermissionInfo("system.view", IsReadOnly = true)]
    View,

    [PermissionInfo("system.update", IsCritical = true)]
    Update,

    [PermissionInfo("system.backup", IsCritical = true)]
    Backup
}
```

### Controller'da ishlatish

```csharp
[Authorize(Policy = "system.view")]
[HttpGet]
public async Task<IActionResult> GetSettings() { }
```

---

## 📊 Middleware

### Exception Handling

Barcha xatolar `ProblemDetails` formatida qaytariladi:

```json
{
  "type": "https://developer.mozilla.org/en-US/docs/Web/HTTP/Status/404",
  "title": "NotFound",
  "status": 404,
  "detail": "User not found",
  "instance": "/api/users/123",
  "extensions": {
    "timestamp": "2026-01-05T12:00:00Z",
    "traceId": "00-abc123..."
  }
}
```

### Token Validation Middleware

Har bir so'rovda JWT token validatsiyasi amalga oshiriladi.

---

## 📁 Yangi modul qo'shish

### 1. Entity yaratish (Domain)

```csharp
// Domain/EfClasses/Products/Product.cs
public class Product : AuditableEntity<Guid>
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }

    public Product(Guid id, string name, decimal price) : base(id)
    {
        Name = name;
        Price = price;
    }
}
```

### 2. Interface yaratish (Domain)

```csharp
// Domain/EfClasses/Products/Interface/IProductRepository.cs
public interface IProductRepository : IBaseRepository<Product>
{
    Task<List<Product>> GetByPriceRangeAsync(decimal min, decimal max);
}
```

### 3. Configuration yaratish (Infrastructure)

```csharp
// Infrastructure/Configuration/Product/ProductConfiguration.cs
public class ProductConfiguration : AuditableEntityConfiguration<Product>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);
        builder.ToTable("Products");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Price).HasPrecision(18, 2);
    }
}
```

### 4. Repository yaratish (Infrastructure)

```csharp
// Infrastructure/Repositories/Product/ProductRepository.cs
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(DbContext context, ILogger<Repository<Product>> logger) 
        : base(context, logger) { }

    public async Task<List<Product>> GetByPriceRangeAsync(decimal min, decimal max)
    {
        return await GetQueryable()
            .Where(p => p.Price >= min && p.Price <= max)
            .ToListAsync();
    }
}
```

### 5. DI ga ro'yxatdan o'tkazish

```csharp
// Infrastructure/DependencyInjection.cs
services.AddScoped<IProductRepository, ProductRepository>();
```

---

## 🧪 Test qilish

```bash
# Unit testlarni ishga tushirish
dotnet test

# Code coverage bilan
dotnet test --collect:"XPlat Code Coverage"
```

---

## 📦 Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ProjectBase.WebApi.dll"]
```

### Docker Compose

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    depends_on:
      - db
      - redis
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=mydb;Username=postgres;Password=postgres

  db:
    image: postgres:14
    environment:
      POSTGRES_DB: mydb
      POSTGRES_PASSWORD: postgres

  redis:
    image: redis:7-alpine
```

---

## 🤝 Hissa qo'shish

1. Fork qiling
2. Feature branch yarating (`git checkout -b feature/AmazingFeature`)
3. Commit qiling (`git commit -m 'Add some AmazingFeature'`)
4. Push qiling (`git push origin feature/AmazingFeature`)
5. Pull Request oching

---

## 📄 Litsenziya

Ushbu loyiha [MIT License](LICENSE.txt) ostida tarqatiladi.

---

## 👨‍💻 Muallif

**Bahodirov Behruz**

- Telegram: [@bbahodirov](https://bbahodirov.t.me/)
- GitHub: [@BakhodirovDev](https://github.com/BakhodirovDev)

---

## ⭐ Qo'llab-quvvatlash

Agar loyiha sizga foydali bo'lsa, ⭐ yulduzcha qo'ying!
