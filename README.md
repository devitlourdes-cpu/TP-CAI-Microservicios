# TP-CAI-Microservicios

TP de Construcción de Aplicaciones Informáticas — E-commerce con microservicios en .NET 10.

## Requisitos

SDK de .NET 10.

## Levantar todo

```bash
./scripts/run-all.sh
```

Compila y levanta todos los microservicios de `src/`. Ctrl+C para bajarlos.
En Windows se corre desde Git Bash.

## Levantar uno solo

```bash
dotnet run --project src/Users.API
```

## Compilar

```bash
dotnet build ECommerce.sln
```
