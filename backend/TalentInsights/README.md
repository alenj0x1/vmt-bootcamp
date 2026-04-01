# TalentInsights

## Creación de dbcontext y entidades de base de datos

Ejecutar esto para actualizar el contexto, o realizar la creación del contexto y entidades.

```shell
dotnet ef dbcontext scaffold "Server=localhost,1433;User=sa;Password=Admin1234@;Database=TalentInsights;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --project TalentInsights.Domain --startup-project TalentInsights.WebApi --no-build --force --context-dir Database/SqlServer/Context --output-dir Database/SqlServer/Entities
```
