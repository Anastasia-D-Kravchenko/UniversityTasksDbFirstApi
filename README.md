# UniversityTasksDbFirstApi

ASP.NET Core (.NET 10) Web API built **database-first**: entities and DbContext scaffolded from an existing SQL Server schema (courses, students, enrollments, assignments and submissions) with Entity Framework Core.

## Endpoints

| Method | Path | Description |
|---|---|---|
| GET | `/api/courses/{idCourse}/assignments` | Assignments of a course |
| POST | `/api/submissions` | Submit work for an assignment |
| PUT | `/api/submissions/{idSubmission}/grade` | Grade a submission |
| DELETE | `/api/submissions/{idSubmission}` | Delete a submission |
| GET | `/api/students/{idStudent}/dashboard` | Student dashboard (enrollments, assignments, submissions) |

Structure: Controllers, DTOs, Models (scaffolded), `ModelExtensions`, Data and Migrations.

## Run

Start SQL Server in Docker:

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```
The connection string in `appsettings.json` uses this local placeholder password; change both together, and use an environment variable (`ConnectionStrings__DefaultConnection`) or user-secrets for anything beyond a local demo.

Create the database (name in `appsettings.json`), run the schema/migrations, then:

```bash
cd UniversityTasksDbFirstApi
dotnet ef database update
dotnet run
```

## License

MIT, see [LICENSE](LICENSE).
