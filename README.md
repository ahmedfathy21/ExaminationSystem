# 📝 Examination System API

A modular, production-grade REST API for managing online examinations — built with **Vertical Slice Architecture** and **CQRS** on ASP.NET Core (.NET 10).

---

## 🏗️ Architecture

This project follows **Vertical Slice Architecture** instead of traditional N-Tier layering. Each feature is self-contained with its own Controller, Command/Query, Handler, and Validator — organized by business domain rather than technical concern.

```
ExaminationSystem/
├── Program.cs                             # App entry point (Composition Root)
├── appsettings.json                       # Configuration (DB, JWT, Email)
│
├── Common/                                # Shared cross-cutting infrastructure
│   ├── Data/AppDbContext.cs               # EF Core context (7 DbSets, Fluent API)
│   ├── Models/                            # Domain entities
│   │   ├── BaseEntity.cs                  # Id + CreatedAt + UpdatedAt
│   │   ├── User.cs                        # Auth fields + refresh/reset tokens
│   │   ├── Diploma.cs                     # Exam category/course
│   │   ├── Quiz.cs                        # Exam under a Diploma
│   │   ├── Question.cs                    # Quiz question
│   │   ├── Option.cs                      # Multiple choice option
│   │   ├── Attempt.cs                     # Student's exam attempt
│   │   ├── Answer.cs                      # Student's chosen answer
│   │   ├── JwtSettings.cs                 # JWT config POCO
│   │   └── EmailSettings.cs               # SMTP config POCO
│   ├── Services/                          # Shared business services
│   │   ├── IJwtProvider.cs                # JWT generation interface
│   │   ├── JwtProvider.cs                 # Access + refresh token generation
│   │   ├── IEmailService.cs               # Email interface
│   │   └── EmailService.cs                # SMTP email sender
│   ├── Exceptions/                        # Custom exception hierarchy
│   │   ├── AppException.cs                # Base + NotFoundException, UnauthorizedException, etc.
│   │   └── ErrorCode.cs                   # Enum (400–500)
│   ├── Middleware/
│   │   └── GlobalExceptionMiddleware.cs   # Catches all exceptions → JSON responses
│   ├── Wrappers/
│   │   └── ApiResponse.cs                 # Uniform response envelopes
│   └── Extension/
│       └── ServiceExtensions.cs           # DI composition (DB, JWT, Auth, Email)
│
├── Features/                              # Vertical slices by business domain
│   └── Auth/                              # ✅ Complete
│       ├── Register/                      # User registration + email verification code
│       ├── Verify/                        # Email verification
│       ├── Login/                         # JWT access token + HttpOnly refresh cookie
│       ├── Refresh/                       # Token rotation
│       ├── ForgotPassword/                # Password reset code via email
│       └── ResetPassword/                 # Password reset with strong validation
│
├── Migrations/                            # EF Core auto-generated migrations
└── ExaminationSystem.IntegrationTests/    # Integration tests (Testcontainers)
```

---

## ⚙️ Tech Stack

| Layer | Technology | Version |
|---|---|---|
| **Runtime** | .NET | 10.0 |
| **Framework** | ASP.NET Core | 10.0 |
| **Database** | PostgreSQL | Latest (Docker) |
| **ORM** | Entity Framework Core | 10.0.5 |
| **Auth** | JWT Bearer + BCrypt | — |
| **Validation** | FluentValidation | 12.1.1 |
| **CQRS** | MediatR | 14.1.0 |
| **Mapping** | AutoMapper | 16.1.1 |
| **API Docs** | Scalar (OpenAPI) | 2.13.20 |
| **Testing** | Testcontainers | — |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://docs.docker.com/get-docker/) (for PostgreSQL)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/ExaminationSystem.git
cd ExaminationSystem
```

### 2. Start PostgreSQL (Docker)

```bash
docker run -d \
  --name pg-exam \
  -e POSTGRES_PASSWORD=your_password \
  -e POSTGRES_DB=examination_db \
  -p 5433:5432 \
  postgres:latest
```

### 3. Configure the Application

Edit `appsettings.json` with your settings:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=examination_db;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "Secret": "YOUR_SUPER_SECRET_KEY_CHANGE_IN_PRODUCTION_MIN_32_CHARS",
    "Issuer": "ExaminationSystem",
    "Audience": "ExaminationSystemUsers",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "SenderEmail": "noreply@examsystem.com",
    "SenderName": "Examination System",
    "Username": "your_email@gmail.com",
    "Password": "your_app_password"
  }
}
```

> **Note:** The app auto-applies pending migrations on startup in Development mode. No manual `dotnet ef database update` needed.

### 4. Run the Application

```bash
dotnet run
```

The API will be available at:
- **HTTPS:** `https://localhost:7227`
- **HTTP:** `http://localhost:5161`

### 5. Explore the API

Open the interactive API docs in your browser:

```
https://localhost:7227/scalar/v1
```

---

## 🔐 Authentication & Identity — API Reference

The complete auth flow: **Register → Verify → Login → (Refresh | Forgot Password → Reset Password)**

### `POST /api/auth/register`

Creates a new user account and sends a 6-digit verification code via email.

**Request:**
```json
{
  "fullName": "Ahmed Fathy",
  "email": "ahmed@example.com",
  "password": "MyPass123!"
}
```

**Response:** `200 OK`
```json
{ "success": true, "message": "Success", "errors": null }
```

**Validation Rules:**
| Field | Rules |
|---|---|
| `fullName` | Required, max 150 chars |
| `email` | Required, valid email format |
| `password` | Required, 6–25 chars |

---

### `POST /api/auth/verify`

Verifies the user's email with the 6-digit code received via email.

**Request:**
```json
{
  "email": "ahmed@example.com",
  "code": "123456"
}
```

**Response:** `200 OK`
```json
{ "success": true, "message": "Success", "errors": null }
```

**Error Cases:**
| Scenario | Status | Message |
|---|---|---|
| User not found | 404 | `"User"` |
| Already verified | 409 | `"User is already verified."` |
| Wrong code | 401 | `"Invalid verification code."` |
| Expired code | 401 | `"Verification code has expired."` |

---

### `POST /api/auth/login`

Authenticates the user and returns a JWT access token. A refresh token is set via `HttpOnly` secure cookie.

**Request:**
```json
{
  "email": "ahmed@example.com",
  "password": "MyPass123!"
}
```

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs..."
  },
  "errors": null
}
```

**Response Headers:**
```
Set-Cookie: refreshToken=<token>; HttpOnly; Secure; SameSite=Strict; Expires=...
```

**Error Cases:**
| Scenario | Status | Message |
|---|---|---|
| Wrong credentials | 401 | `"Invalid email or password."` |
| Not verified | 403 | `"Account is not verified."` |

---

### `POST /api/auth/refresh`

Rotates the refresh token (from `HttpOnly` cookie) and issues a new access token.

**Request:** No body needed — the refresh token is read from the cookie.

**Response:** `200 OK`
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs..."
  },
  "errors": null
}
```

**Response Headers:**
```
Set-Cookie: refreshToken=<new_token>; HttpOnly; Secure; SameSite=Strict; Expires=...
```

---

### `POST /api/auth/forgot-password`

Sends a 6-digit password reset code to the user's email. The code expires in **15 minutes**.

> **Security:** Always returns `200 OK` regardless of whether the email exists — prevents email enumeration attacks.

**Request:**
```json
{
  "email": "ahmed@example.com"
}
```

**Response:** `200 OK`
```json
{ "success": true, "message": "Success", "errors": null }
```

---

### `POST /api/auth/reset-password`

Resets the user's password using the code from the forgot-password email. On success, the refresh token is **revoked** — forcing re-login on all devices.

**Request:**
```json
{
  "email": "ahmed@example.com",
  "code": "632860",
  "newPassword": "NewSecure1!"
}
```

**Response:** `200 OK`
```json
{ "success": true, "message": "Success", "errors": null }
```

**Password Requirements:**
| Rule | Example |
|---|---|
| At least 6 characters | ✅ `Ab1!xx` |
| At most 25 characters | — |
| At least one uppercase letter | `A` |
| At least one lowercase letter | `a` |
| At least one digit | `1` |
| At least one special character | `!@#$%` |

**Error Cases:**
| Scenario | Status | Message |
|---|---|---|
| User not found | 404 | `"No account found with this email."` |
| Wrong code | 401 | `"Invalid reset code."` |
| Expired code | 401 | `"Reset code has expired. Please request a new one."` |
| Weak password | 400 | Specific validation error messages |

---

## 🗄️ Database Schema

The system uses 7 core tables with the following relationships:

| Table | Description | Key Relationships |
|---|---|---|
| `users` | User accounts with auth fields | Has many `attempts` |
| `diplomas` | Exam categories/courses | Has many `quizzes` |
| `quizzes` | Exams under a diploma | Belongs to `diploma`, has many `questions` and `attempts` |
| `questions` | Quiz questions | Belongs to `quiz`, has many `options` and `answers` |
| `options` | Multiple choice options | Belongs to `question` |
| `attempts` | Student exam attempts | Belongs to `user` and `quiz`, has many `answers` |
| `answers` | Student's chosen answers | Belongs to `attempt`, `question`, and `option`. Unique constraint on `(attempt_id, question_id)` |

### Key Schema Decisions

- **UUIDs** for all primary keys (via `Guid`)
- **Soft timestamps** — `CreatedAt` and `UpdatedAt` auto-managed by `AppDbContext`
- **String-based enums** — `Role`, `Status`, `Type` stored as readable strings
- **Cascade deletes** — Diploma → Quiz → Question → Option chain
- **Restrict deletes** — User/Quiz → Attempt (protect exam history)

---

## 🛡️ Security Design

| Feature | Implementation |
|---|---|
| **Password Storage** | BCrypt hashing with automatic salt |
| **Access Tokens** | JWT with HMAC-SHA256, 60-min expiry, zero clock skew |
| **Refresh Tokens** | Cryptographically random, stored in DB, HttpOnly + Secure + SameSite=Strict cookie |
| **Token Rotation** | Old refresh token invalidated on each refresh |
| **Password Reset** | 6-digit code, 15-min expiry, one-time use, revokes all sessions |
| **Email Enumeration Prevention** | Forgot-password always returns 200 |
| **Role-Based Access** | `AdminOnly` and `StudentOnly` authorization policies |
| **Global Error Handling** | Exception middleware catches all errors → structured JSON |

---

## 🧩 Design Patterns

| Pattern | Where | Purpose |
|---|---|---|
| **Vertical Slice Architecture** | `Features/` directory | Each feature is self-contained — no cross-feature dependencies |
| **CQRS** | MediatR Commands/Queries | Separates write operations (Commands) from read operations (Queries) |
| **Mediator** | MediatR `IRequestHandler` | Decouples controllers from business logic |
| **Composition Root** | `ServiceExtensions.cs` | Single place for all DI registration — keeps `Program.cs` clean |
| **Options Pattern** | `IOptions<JwtSettings>` | Strongly-typed configuration binding |
| **Repository Pattern** | `AppDbContext` (via EF Core) | Data access abstraction |
| **Response Envelope** | `ApiResponse<T>` | Consistent API response format |
| **Exception Hierarchy** | `AppException` → typed children | Maps domain errors to HTTP status codes |

---

## 🧪 Running Tests

```bash
# Integration tests (requires Docker for Testcontainers)
cd ExaminationSystem.IntegrationTests
dotnet test
```

---

## 📋 Project Roadmap

| Epic | Status | Description |
|---|---|---|
| 🏗️ Foundation & Infrastructure | ✅ Complete | DB, models, middleware, exceptions, DI, OpenAPI |
| 🔐 Authentication & Identity | ✅ Complete | Register, Verify, Login, Refresh, Forgot/Reset Password |
| 📚 Diploma Management | ⬜ Not Started | CRUD for exam categories |
| 📝 Quiz Management | ⬜ Not Started | CRUD + publish workflow for exams |
| ❓ Question & Option Management | ⬜ Not Started | MCQ question + option CRUD |
| 🎯 Exam Attempt & Grading | ⬜ Not Started | Start, answer, submit, auto-grade |
| 📊 Admin Dashboard & Reporting | ⬜ Not Started | User management, analytics, export |

---

## 📄 License

This project is licensed under the MIT License.
