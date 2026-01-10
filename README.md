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
| ❤️ **Health Checks** | PostgreSQL va DbContext monitoring (`/health` endpoint) |
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
│   │   ├── Attributes/           # Custom attributes (RequirePermission)
│   │   ├── Authentication/       # Auth DTOs, Handler, Provider
│   │   │   ├── Handler/          # PermissionAuthorizationHandler
│   │   │   └── Provider/         # PermissionPolicyProvider, PermissionRequirement
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
│       │   └── Permissions/
│       │       ├── DTOs/         # PermissionDto, CreatePermissionDto, UpdatePermissionDto
│       │       └── Interface/    # IPermissionService, IPermissionRepository
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
│       ├── BaseService/          # CrudService (generic CRUD operations)
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
    ├── Extensions/               # DI, Swagger, Policies, ResultExtensions, Filters
    ├── Middleware/               # Exception, Token validation
    ├── wwwroot/                  # Static files
    └── logs/                     # Serilog log files
```

### 📐 Qatlamlar orasidagi bog'liqlik

```
┌─────────────────────────────────────────────────────────┐
│                    ProjectBase.Web                      │
│              (Controllers, Middleware, DI)              │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                     Application                         │
│            (Services, Mappers, Extensions)              │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                    Infrastructure                       │
│      (DbContext, Repositories, Configurations)          │
└─────────────────────────┬───────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────┐
│                       Domain                            │
│    (Entities, Interfaces, Errors, Business Rules)       │
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
| `AspNetCore.HealthChecks.NpgSql` | Latest | PostgreSQL health check |

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
  "SeedSettings": {
    "EnableInProduction": false
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
| `GET` | `/health` | Health check endpoint |

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
return Result.Success();                              // { "isSuccess": true, "message": "Successfully" }
return Result.Success("Muvaffaqiyatli saqlandi");     // Custom message bilan
return Result<User>.Success(user);                    // { "isSuccess": true, "data": {...}, "message": "Successfully" }
return Result<User>.SuccessWithMessage(user, "User topildi");

// Xato natija (Error yashirilgan - JSON da ko'rinmaydi)
return Result.Failure(Error.NotFound);                // { "isSuccess": false, "message": "Not found" }
return Result<User>.Failure("USER_NOT_FOUND", "User not found");

// Controller'da ishlatish (ResultExtensions bilan)
return await _service.GetAsync(id).ToActionResultAsync();           // Avtomatik status code
return await _service.CreateAsync(dto).ToCreatedResultAsync();      // 201 Created
return await _service.DeleteAsync(id).ToNoContentResultAsync();     // 204 No Content
return await _service.LoginAsync(dto).ToActionResultSafeAsync(_logger); // Exception handling bilan
```

> ⚠️ **Muhim:** `Error` property `[JsonIgnore]` bilan belgilangan - JSON javobda ko'rinmaydi, faqat `message` ko'rsatiladi.

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

### AutoMapper Integration

```csharp
// Mapping profile yaratish
public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}

// Service'da ishlatish
public class UserService
{
    private readonly IMapper _mapper;
    
    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }
}
```

### ResultExtensions (Controller uchun)

```csharp
// Oddiy result
return result.ToActionResult();                    // 200 OK yoki error status

// Custom status code
return result.ToActionResult(HttpStatusCode.Created);  // 201 Created
return result.ToNoContentResult();                     // 204 No Content

// Async bilan
return await _service.GetAsync(id).ToActionResultAsync();
return await _service.CreateAsync(dto).ToCreatedResultAsync();

// Exception handling bilan (try-catch avtomatik)
return await _service.LoginAsync(dto).ToActionResultSafeAsync(_logger);

// Match pattern
return result.Match(
    onSuccess: data => Ok(data),
    onFailure: error => BadRequest(error.Message)
);
```

> Controller'da if/else va try-catch yozish shart emas - `ResultExtensions` barchasini avtomatik bajaradi.

---

## 🔒 Permission System

Loyihada to'liq permission-based authorization tizimi mavjud.

### Permission Entity

```csharp
public class Permission : AuditableEntity<Guid>
{
    public string Name { get; set; }        // "users.create"
    public string Description { get; set; } // "Foydalanuvchi yaratish"
    public string Resource { get; set; }    // "users"
    public string Action { get; set; }      // "create"
}
```

### RequirePermission Attribute

```csharp
// String bilan
[RequirePermission("users.create")]
[HttpPost]
public async Task<IActionResult> CreateUser() { }

// Enum bilan
[RequirePermission(UserPermissions.Create)]
[HttpPost]
public async Task<IActionResult> CreateUser() { }

// Bir nechta permission
[RequirePermission("users.view")]
[RequirePermission("users.edit")]
[HttpPut]
public async Task<IActionResult> UpdateUser() { }
```

### IPermissionService

```csharp
public interface IPermissionService
{
    Task<Result<List<PermissionDto>>> GetByUserIdAsync(Guid userId);
    Task<Result<List<PermissionDto>>> GetByRoleIdAsync(Guid roleId);
    Task<Result<bool>> UserHasPermissionAsync(Guid userId, string permissionName);
    Task<Result<bool>> UserHasAllPermissionsAsync(Guid userId, IEnumerable<string> permissions);
    Task<Result<bool>> UserHasAnyPermissionAsync(Guid userId, IEnumerable<string> permissions);
}
```

### Permission Flow

```
1. User login qilganda JWT token olinadi
        │
        ▼
2. Token'da permission claim'lar mavjud
   { "permission": ["users.view", "users.create", ...] }
        │
        ▼
3. [RequirePermission] attribute so'rovni tekshiradi
        │
        ▼
4. PermissionAuthorizationHandler permission borligini tasdiqlaydi
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

### 🐳 Docker

Loyihada **bitta unified** Docker Compose fayl mavjud - **profiles** orqali turli rejimlarni boshqarish:

| Fayl | Tavsif |
|------|--------|
| `Dockerfile` | Multi-stage production build |
| `docker-compose.yml` | **Unified** - barcha rejimlar bitta faylda |
| `.env.example` | Environment variables namunasi |
| `.dockerignore` | Docker build uchun ignore fayllar |

### 📌 Profiles

| Profile | Buyruq | Servislar |
|---------|--------|-----------|
| `dev` | `docker-compose --profile dev up -d` | PostgreSQL + Redis + pgAdmin |
| `full` | `docker-compose --profile full up -d` | API + PostgreSQL + Redis + pgAdmin (hardcoded) |
| `prod` | `docker-compose --profile prod up -d` | API + PostgreSQL + Redis (.env dan) |

### 💡 Smart Defaults

`.env` da host ko'rsatilmasa, avtomatik **ichki konteyner** ishlatiladi:

| `.env` holati | Natija |
|---------------|--------|
| `POSTGRES_HOST=` (bo'sh) | ✅ Ichki `postgres` konteyner |
| `POSTGRES_HOST=db.example.com` | ✅ Tashqi serverga ulanadi |
| `REDIS_HOST=` (bo'sh) | ✅ Ichki `redis` konteyner |
| `REDIS_HOST=redis.aws.com` | ✅ Tashqi Redis ga ulanadi |

### 🚀 Tez boshlash

```bash
# 1. Development (faqat DB, API lokal)
docker-compose --profile dev up -d
cd ProjectBase.Web && dotnet run

# 2. Full Stack (hammasi Docker ichida)
docker-compose --profile full up -d

# 3. Production (.env bilan)
cp .env.example .env
# .env ni to'ldiring
docker-compose --profile prod up -d

# To'xtatish
docker-compose --profile <profile> down
```

### 🔐 CI/CD bilan Deploy (Secrets bilan)

**Loyihada secrets hech qachon kodda yozilmaydi!** Barcha maxfiy ma'lumotlar CI/CD tizimidan olinadi:

```bash
# 1. .env.example ni nusxalang
cp .env.example .env

# 2. .env faylini to'ldiring (git ga push QILMANG!)
nano .env

# 3. Production ishga tushiring
docker-compose --profile prod up -d
```

### GitHub Actions bilan Deploy

1. GitHub repo → **Settings** → **Secrets and variables** → **Actions**
2. Quyidagi secrets qo'shing:

| Secret | Tavsif |
|--------|--------|
| `DOCKER_USERNAME` | Docker Hub username |
| `DOCKER_PASSWORD` | Docker Hub password/token |
| `SSH_HOST` | Server IP manzili |
| `SSH_USERNAME` | SSH username |
| `SSH_PRIVATE_KEY` | SSH private key |
| `POSTGRES_PASSWORD` | Database password |
| `JWT_SECRET_KEY` | JWT secret key (32+ belgi) |

3. `main` branch ga push qilganingizda avtomatik deploy bo'ladi

### Jenkins bilan Deploy

1. **Manage Jenkins** → **Credentials** → **System** → **Global credentials**
2. Quyidagi credentials qo'shing:
   - `ssh-credentials` (SSH Username with private key)
   - `ssh-host` (Secret text - Server IP)
   - `postgres-password` (Secret text)
   - `jwt-secret-key` (Secret text - 32+ belgi)

3. Pipeline yarating va `Jenkinsfile` ni ishlatishini belgilang

> 💡 **Jenkins serverda build qiladi** - Docker Hub ga push qilmaydi, to'g'ridan-to'g'ri production serverda docker-compose bilan deploy qiladi.

### 🖥️ HTTP bilan (Docker'siz)

Docker o'rnatmasdan ham loyihani ishga tushirish mumkin:

```bash
# 1. PostgreSQL va Redis o'rnating (Windows)
# PostgreSQL: https://postgresql.org/download/windows/
# Redis: https://github.com/tporadowski/redis/releases

# 2. appsettings.Development.json to'ldiring
# 3. API ni ishga tushiring
cd ProjectBase.Web
dotnet run
```

### Servislar va Portlar

| Servis | Port | URL |
|--------|------|-----|
| API | 5000 | http://localhost:5000 |
| Swagger | 5000 | http://localhost:5000/swagger |
| PostgreSQL | 5432 | localhost:5432 |
| Redis | 6379 | localhost:6379 |
| pgAdmin | 5050 | http://localhost:5050 |

### 🌐 Tashqi Database ga ulanish

Agar sizda allaqachon PostgreSQL server mavjud bo'lsa, `.env` faylida host ni ko'rsating:

```bash
# .env
POSTGRES_HOST=db.example.com
POSTGRES_PASSWORD=your_secure_password
REDIS_HOST=redis.example.com
```

> 💡 Host bo'sh qoldirilsa, Docker ichki konteynerlardan foydalanadi.

### Faqat API image yaratish

```bash
# Image yaratish
docker build -t projectbase-api:latest .

# Konteyner ishga tushirish (tashqi DB bilan)
docker run -d -p 5000:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=projectbase;Username=postgres;Password=postgres" \
  -e ConnectionStrings__Redis="host.docker.internal:6379" \
  --name projectbase-api \
  projectbase-api:latest
```

### Dockerfile (Multi-stage)

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ProjectBase.Web.sln", "./"]
# ... restore & build ...

# Stage 2: Runtime (Alpine - kichik hajm)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD wget --spider http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "ProjectBase.WebApi.dll"]
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
