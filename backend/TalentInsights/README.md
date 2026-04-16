# TalentInsights

## Creación de dbcontext y entidades de base de datos

Ejecutar esto para actualizar el contexto, o realizar la creación del contexto y entidades.

```shell
dotnet ef dbcontext scaffold "Server=localhost,1433;User=sa;Password=Admin1234@;Database=TalentInsights;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer --project TalentInsights.Domain --startup-project TalentInsights.WebApi --no-build --force --context-dir Database/SqlServer/Context --output-dir Database/SqlServer/Entities --no-onconfiguring
```

## Secretos de la aplicación

Estos datos, se almacenan usando `user-secrets` para resguardarlos, ya que son sensibles. Abajo, tendrá un ejemplo, con el que puede empezar.

```json
{
  "Jwt": {
    "PrivateKey": "private-key"
  },
  "SMTP": {
    "Host": "host",
    "From": "example@example.com",
    "Port": 587,
    "User": "user",
    "Password": "password"
  },
  "ConnectionStrings": {
    "Database": "connection-string"
  }
}
```

- Creado por alenj0x1
