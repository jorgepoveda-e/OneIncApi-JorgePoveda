
#### Key Principles:
1. **Dependency Rule**:  
   Outer layers depend on inner layers (Presentation → Services (Application) → Domain ← Infrastructure).
2. **Decoupled Components**:  
   Database access is abstracted via `IUserRepository`.
3. **Testability**:  
   Layers are isolated for unit/integration testing.

---

## Best Practices
### Implemented
| Practice                          | Description                                                                 |
|-----------------------------------|-----------------------------------------------------------------------------|
| **FluentValidation**              | Separation of validation rules from models                                 |
| **Dependency Injection**          | Explicit dependencies via constructor injection                           |
| **Layer Separation**              | Clear boundaries between Domain/Application/Infrastructure/Presentation    |
| **Global Exception Handling**     | Centralized error handling middleware                                      |
| **Parameterized Queries**         | SQL injection prevention                                                   |
| **Async/Await**                   | Non-blocking database operations                                           |
| **Unit/Integration Tests**        | 85%+ test coverage with NUnit and Moq                                      |

### Excluded (Per Requirements)
- HTTPS
- Authentication/Authorization
- Rate limiting

---

## Setup
### Prerequisites
- .NET 8 SDK
- SQL Server
- IDE (VS 2022+, Rider, or VSCode)

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/jalexispoveda/OneIncApi.Test.git
2. Execute database script from Scripts folder, in SQL server and update the connection string.