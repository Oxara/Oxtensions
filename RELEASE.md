# Release process

## Invariants

- `src/Oxtensions/Oxtensions.csproj`, `CHANGELOG.md` ve `vMAJOR.MINOR.PATCH` etiketi aynı sürümü taşır.
- Yayın etiketi yalnız testleri geçmiş `main` commit'i üzerinde oluşturulur ve sonradan taşınmaz.
- Tag push, NuGet ve GitHub Release yayınını tetikler; yerel makineden paket yayımlanmaz.
- `NUGET_API_KEY` yalnız GitHub Actions secret olarak kullanılır ve loglara yazılmaz.
- Başarısız veya hatalı yayın yeni patch sürümüyle düzeltilir.

## Prepare

```powershell
dotnet restore
dotnet list Oxtensions.sln package --vulnerable --include-transitive
dotnet build --configuration Release --no-restore
dotnet test --configuration Release --no-build
dotnet pack src/Oxtensions/Oxtensions.csproj --configuration Release --no-build --output artifacts/release-candidate
```

1. Tüm hedef framework testlerini ve paket içeriğini doğrula.
2. `CHANGELOG.md` içindeki `Unreleased` maddelerini tarihli sürüme taşı.
3. Package version ile planlanan tag değerinin aynı olduğunu doğrula.
4. NuGet ikonunun, README'nin ve sembol paketinin release candidate içinde bulunduğunu kontrol et.
5. Commit, tag ve push için ayrıca sahip onayı al.

CI, tag ile proje sürümü farklıysa NuGet publish adımından önce yayını reddeder.
