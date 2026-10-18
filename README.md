# CPCompanion

CPCompanion is a .NET 9 REST API that links a user's Codeforces account to a personal problem-solving tracker. It imports accepted submissions, stores the solved-problem history, tracks goals (by tag, target count, and deadline), and exposes dashboard statistics such as tag breakdown and solving streaks.

> **Status:** under active development.

## Features

- **Authentication:** registration and login with ASP.NET Core Identity and JWT bearer tokens.
- **Codeforces integration:** fetches the user's current/max rating and accepted submissions from the public Codeforces API.
- **Problem history:** stores each solved problem once per user, with its name, difficulty, tags, and solve time.
- **Goals:** create goals with a target count, an optional tag, and an optional deadline. Progress is updated automatically after each sync.
- **Dashboard stats:** total solved, per-tag breakdown, current and longest daily streak, and rating.
- **Manual and scheduled sync:** `POST /api/auth/sync-now` for on-demand sync, plus a hosted background service that syncs all users every 6 hours (configurable).
- **Validation and error handling:** FluentValidation through an action filter, and a global exception-handling middleware that maps domain exceptions to HTTP status codes.
- **Data integrity:** a unique index on `(UserId, ProblemId)` prevents duplicate problem logs.

## Tech Stack

| Area | Technology |
| --- | --- |
| Platform | C#, .NET 9, ASP.NET Core Web API |
| Data access | Entity Framework Core 9, LINQ |
| Database | Microsoft SQL Server |
| Auth | ASP.NET Core Identity, JWT (HMAC-SHA256) |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Docs / tooling | Swagger (Swashbuckle), Git, Visual Studio |

## Project Structure

```text
CPCompanion/
├── CPCompanion.API/             # Controllers, middleware, validation filter, Program.cs
├── CPCompanion.Application/     # DTOs, service interfaces, validators, exceptions, mappings
├── CPCompanion.Domain/          # Entities (User, ProblemLog, Goal, SyncLog) and enums
├── CPCompanion.Infrastructure/  # EF Core DbContext and migrations, services,
│                                # Codeforces client, background sync worker
└── CPCompanion.sln
```

## API Endpoints

All endpoints except register and login require the header `Authorization: Bearer <token>`.

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/auth/register` | Create an account linked to a Codeforces handle |
| POST | `/api/auth/login` | Log in and receive a JWT |
| GET | `/api/auth/profile` | Get the current user's profile |
| PUT | `/api/auth/profile` | Update the Codeforces handle |
| POST | `/api/auth/sync-now` | Sync the current user's Codeforces data immediately |
| POST | `/api/goals` | Create a goal |
| GET | `/api/goals` | List the current user's goals |
| GET | `/api/goals/{id}` | Get a goal by ID |
| PUT | `/api/goals/{id}` | Update a goal and recalculate its progress |
| DELETE | `/api/goals/{id}` | Delete a goal |
| GET | `/api/stats/dashboard` | Total solved, tag breakdown, streaks, and ratings |

### Example requests

Register (the password needs at least 8 characters, with an uppercase letter, a lowercase letter, a digit, and a special character):

```json
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "Str0ng!Pass",
  "codeforcesHandle": "tourist"
}
```

Login:

```json
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "Str0ng!Pass"
}
```

Response:

```json
{
  "token": "<jwt>",
  "expiration": "2026-10-18T12:00:00Z"
}
```

Create a goal:

```json
POST /api/goals
{
  "title": "Solve 20 DP problems",
  "targetTag": "dp",
  "targetCount": 20,
  "deadline": "2026-12-31T00:00:00Z"
}
```

`targetTag` and `deadline` are optional. The deadline must be in the future.

## How Goal Progress Works

A problem counts toward a goal when:

1. it was solved on or after the goal's creation time,
2. it was solved before the goal's deadline (if one is set), and
3. it contains the goal's tag (if one is set; matching is case-insensitive).

After every sync, newly imported problems are counted against each active goal, and a goal is marked completed once its count reaches the target. When a goal is edited, its progress is recalculated from the full stored problem history.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server (LocalDB, SQL Server Express, or any SQL Server instance)
- EF Core CLI tools: `dotnet tool install --global dotnet-ef`

### 1. Clone and restore

```bash
git clone https://github.com/Zyad-Emad/CPCompanion.git
cd CPCompanion
dotnet restore
```

### 2. Configure secrets

`appsettings.json` already contains the JWT issuer, audience, and expiry. The connection string and JWT signing key must be provided through User Secrets (never commit them):

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=CPCompanionDb;Trusted_Connection=True;TrustServerCertificate=True;" --project CPCompanion.API
dotnet user-secrets set "JwtSettings:SecretKey" "replace-with-a-random-string-of-at-least-32-characters" --project CPCompanion.API
```

The signing key must be at least 32 characters long, otherwise token generation throws an error.

### 3. Apply migrations

```bash
dotnet ef database update --project CPCompanion.Infrastructure --startup-project CPCompanion.API
```

### 4. Run

```bash
dotnet run --project CPCompanion.API
```

The API listens on `http://localhost:5057` (and `https://localhost:7254` with the `https` launch profile). In the Development environment, Swagger UI is available at `/swagger`.

## Configuration Reference

| Key | Default | Description |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | none (required) | SQL Server connection string |
| `JwtSettings:SecretKey` | none (required) | HMAC signing key, at least 32 characters |
| `JwtSettings:Issuer` | `CPCompanion` | Token issuer |
| `JwtSettings:Audience` | `CPCompanionUsers` | Token audience |
| `JwtSettings:ExpiryMinutes` | `60` | Token lifetime in minutes |
| `BackgroundServiceSettings:SyncIntervalHours` | `6` | Interval of the scheduled sync |

## Roadmap

- Automated unit and integration tests for goal rules, sync, and validation.
- Rate limiting for outgoing Codeforces API calls.
- Safer handling of concurrent syncs and goal-progress updates.
- Docker and Docker Compose setup.

## Author

**Zyad Emad** — [GitHub](https://github.com/Zyad-Emad) · [LinkedIn](https://www.linkedin.com/in/zyad-emad-5bb1a1339/)
