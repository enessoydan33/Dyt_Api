# Dyt_Api

Diyetisyen yazılımının backend API'si (ASP.NET Core .NET 10, PostgreSQL, EF Core, ASP.NET Identity + JWT).

## Yerel kurulum

Gereksinimler: .NET 10 SDK, PostgreSQL ve EF Core araçları (`dotnet tool install --global dotnet-ef`).

Gizli bilgiler repoda tutulmaz. Development ortamında User Secrets ile verilir. Komutları `Dyt_Api/Dyt_Api` proje klasöründe çalıştırın:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=<database>;Username=<username>;Password=<password>"
```

```bash
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 -A 64)"
```

Veritabanı tablolarını oluşturun ve uygulamayı başlatın:

```bash
dotnet ef database update
```

```bash
dotnet run --launch-profile http
```

API `http://localhost:5218` adresinde açılır. Swagger: `http://localhost:5218/swagger`.

Tüm ayarların örneği için [appsettings.Example.json](Dyt_Api/appsettings.Example.json) dosyasına bakın. Bu dosya uygulama tarafından okunmaz, sadece referanstır.

## Yapılandırma

| Ayar | Development | Production |
|---|---|---|
| `ConnectionStrings:DefaultConnection` | User Secrets | Ortam değişkeni |
| `Jwt:Key` (en az 32 byte) | User Secrets | Ortam değişkeni |
| `Jwt:Issuer`, `Jwt:Audience`, `Jwt:AccessTokenMinutes` | `appsettings.json` | `appsettings.json` |
| `Cors:AllowedOrigins` | `appsettings.Development.json` | Ortam değişkeni |

Sonraki kaynak öncekini ezer: `appsettings.json` → `appsettings.{Ortam}.json` → User Secrets (sadece Development) → ortam değişkenleri.

Production'da ortam değişkeni adlarında `:` yerine `__` kullanılır:

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<connection-string>
Jwt__Key=<secret>
Cors__AllowedOrigins__0=https://<frontend-domain>
```

`Jwt:Key` eksik veya 32 byte'tan kısaysa uygulama başlamaz. `Cors:AllowedOrigins` boşsa hiçbir frontend adresine izin verilmez.

Gerçek şifreleri, connection string'leri veya JWT anahtarlarını `appsettings*.json` dosyalarına yazmayın ve commit etmeyin.
