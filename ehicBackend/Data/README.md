# Database Setup Guide

## Entity Framework Core Configuration

This project uses Entity Framework Core 9.0 with SQL Server.

## Database Migrations

### Create a new migration
```bash
dotnet ef migrations add <MigrationName>
```

### Apply migrations to the database
```bash
dotnet ef database update
```

### Remove the last migration (if not applied)
```bash
dotnet ef migrations remove
```

### List all migrations
```bash
dotnet ef migrations list
```

## Connection Strings

- **Production**: `appsettings.json` - Uses `EhicExamsDb`
- **Development**: `appsettings.Development.json` - Uses `EhicExamsDb_Dev`

## Database Schema

### Tables

#### Users
- `Id` (int, Primary Key)
- `Username` (nvarchar(100), Unique, Required)
- `Email` (nvarchar(255), Unique, Required)
- `PasswordHash` (nvarchar(max), Required)
- `FirstName` (nvarchar(100), Required)
- `LastName` (nvarchar(100), Required)
- `RoleId` (int, Foreign Key to Roles)
- `IsActive` (bit, Default: true)

#### Roles
- `Id` (int, Primary Key)
- `Name` (nvarchar(50), Unique, Required)
- `Description` (nvarchar(255))
- `IsActive` (bit, Default: true)

### Seeded Data

The database is seeded with the following default roles:
1. **Admin** - Administrator role with full access
2. **User** - Standard user role
3. **Student** - Student role for exam taking

## DbContext Features

- **Automatic Audit Fields**: The `ApplicationDbContext` automatically updates `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy` fields for entities that inherit from `AuditBaseEntity`.
- **Soft Delete Support**: Entities have an `IsActive` field for soft delete functionality.
