# ValetaxWebApi (FxNet Test API)

ASP.NET Core 8 REST API with tree management and exception journaling.

## 🚀 Features

- ✅ **Tree Management**: Create and manage independent trees with hierarchical nodes
- ✅ **Exception Journaling**: Automatic logging of all exceptions with full context
- ✅ **PostgreSQL Database**: Code-first approach with Entity Framework Core
- ✅ **JWT Authentication**: Standard JWT token-based authentication (RFC 7519)
- ✅ **Swagger Documentation**: Interactive API documentation with XML comments
- ✅ **Custom Error Handling**: SecureException for user-friendly error messages
- ✅ **Thread-Safe Event IDs**: Collision-free event ID generation

## 📋 Prerequisites

- .NET 8 SDK
- PostgreSQL database

## ⚙️ Configuration

Update `appsettings.json` with your database connection and JWT settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=fxnet_test;Username=postgres;Password=postgres"
  },
  "Authentication": {
    "Enabled": true
  },
  "Jwt": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!ChangeThisInProduction",
    "Issuer": "FxNetTestApi",
    "Audience": "FxNetTestApiUsers",
    "ExpirationMinutes": 1440
  }
}
```

### JWT Configuration

- **Secret**: Symmetric key for signing tokens (min 32 characters, **change in production!**)
- **Issuer**: Who creates the token (your API identifier)
- **Audience**: Who can use the token (your API consumers)
- **ExpirationMinutes**: Token lifetime (1440 = 24 hours)

### Authentication Mode

Set `Authentication:Enabled` to:
- `true` - Enable JWT authentication (recommended for production)
- `false` - Disable authentication for testing (uses bypass handler that auto-succeeds)

**Note**: When disabled, all `[Authorize]` attributes are bypassed automatically - you can call any endpoint without a token.

## 🛠️ Setup

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Run the application**:
   ```bash
   dotnet run
   ```

The database will be **automatically migrated** on startup.

## 📚 API Endpoints

### Authentication
- `POST /api.user.partner.rememberMe` - Get JWT token (public endpoint)

### Tree Management (requires auth if enabled)
- `POST /api.user.tree.get` - Get or create a tree
- `POST /api.user.tree.node.create` - Create a new node
- `POST /api.user.tree.node.delete` - Delete a node (children must be deleted first)
- `POST /api.user.tree.node.rename` - Rename a node

### Exception Journal (requires auth if enabled)
- `POST /api.user.journal.getRange` - Get paginated journal entries with filtering
- `POST /api.user.journal.getSingle` - Get single journal entry by ID

## 💡 Usage Examples

### 1. Get JWT Token

```bash
curl -X POST "https://localhost:7000/api.user.partner.rememberMe?code=myuniqueuser"
```

Response (JWT Token):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI0MiIsInVuaXF1ZV9uYW1lIjoibXl1bmlxdWV1c2VyIiwianRpIjoiYWJjMTIzLi4uIiwibmFtZWlkIjoiNDIiLCJuYW1lIjoibXl1bmlxdWV1c2VyIiwiZXhwIjoxNzM1MTQwMDAwLCJpc3MiOiJGeE5ldFRlc3RBcGkiLCJhdWQiOiJGeE5ldFRlc3RBcGlVc2VycyJ9.signature..."
}
```

**Token Format**: `header.payload.signature` (standard JWT structure)

### 2. Create a Root Node

```bash
curl -X POST "https://localhost:7000/api.user.tree.node.create?treeName=MyTree&nodeName=RootNode" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### 3. Create a Child Node

```bash
curl -X POST "https://localhost:7000/api.user.tree.node.create?treeName=MyTree&parentNodeId=1&nodeName=ChildNode" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

### 4. Get Tree Structure

```bash
curl -X POST "https://localhost:7000/api.user.tree.get?treeName=MyTree" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

Response:
```json
{
  "id": 0,
  "name": "root",
  "children": [
    {
      "id": 1,
      "name": "RootNode",
      "children": [
        {
          "id": 2,
          "name": "ChildNode",
          "children": []
        }
      ]
    }
  ]
}
```

### 5. Query Exception Journal

```bash
curl -X POST "https://localhost:7000/api.user.journal.getRange?skip=0&take=10" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"from": "2025-01-01T00:00:00Z", "to": "2025-12-31T23:59:59Z", "search": "error"}'
```

## ⚠️ Error Handling

### SecureException (Business Rules - User-Friendly)
```json
{
  "type": "Secure",
  "id": "638136064526554554",
  "data": {
    "message": "You have to delete all children nodes first"
  }
}
```

### System Exceptions (Internal Errors)
```json
{
  "type": "Exception",
  "id": "638136064187111634",
  "data": {
    "message": "Internal server error ID = 638136064187111634"
  }
}
```

All exceptions are automatically logged to the exception journal with:
- Unique Event ID
- Timestamp
- Query/Body parameters (as JSONB)
- Full stack trace

## 🔐 JWT Authentication

### How JWT Works

1. **Get Token**: User calls `rememberMe` with unique code
2. **Token Structure**: `header.payload.signature`
   - **Header**: `{"alg": "HS256", "typ": "JWT"}`
   - **Payload**: Contains claims (userId, username, expiration)
   - **Signature**: HMAC-SHA256 signature for verification
3. **Use Token**: Send token in `Authorization: Bearer {token}` header
4. **Validation**: API validates signature, expiration, issuer, audience

### Token Claims

```json
{
  "sub": "42",                    // Partner ID
  "unique_name": "myuniqueuser",  // User code
  "jti": "unique-token-id",       // JWT ID
  "nameid": "42",                 // Name identifier
  "name": "myuniqueuser",         // Name
  "exp": 1735140000,              // Expiration timestamp
  "iss": "FxNetTestApi",          // Issuer
  "aud": "FxNetTestApiUsers"      // Audience
}
```

### Security Features

- ✅ **Stateless**: No database lookup on every request (only on token generation)
- ✅ **Tamper-Proof**: Signature verification prevents token modification
- ✅ **Expiration**: Tokens expire after configured time (default 24 hours)
- ✅ **Standard**: RFC 7519 compliant JWT implementation
- ✅ **Claims-Based**: Rich user identity information in token

### Decode JWT Token

You can decode (but not verify) JWT tokens at: https://jwt.io

**⚠️ Security Note**: Never share your `Jwt:Secret` - change it in production!

## 🗄️ Database Schema

### Trees
- Independent tree containers
- Each tree has a unique name
- Auto-created when first referenced

### Nodes
- Hierarchical tree nodes
- Mandatory name field (unique among siblings)
- Foreign key to tree (ensures tree independence)
- Self-referencing parent-child relationship
- Cascade delete restricted (must delete children first)

### Exception Journal
- Automatic exception logging
- EventId (thread-safe, collision-free)
- CreatedAt timestamp with timezone
- Parameters stored as JSONB
- Stack trace preservation

### Partners
- User authentication records
- Code → JWT Token mapping
- Token updated on each rememberMe call
- CreatedAt timestamp

## 📐 Business Rules

1. **Node Uniqueness**: Node names must be unique among siblings
2. **Tree Isolation**: All child nodes must belong to the same tree as their parent
3. **Delete Protection**: Nodes with children cannot be deleted (throw SecureException)
4. **Cascading Restrictions**: Database enforces `RESTRICT` on parent deletion
5. **Auto Tree Creation**: Trees are created automatically when first referenced

## 📖 Swagger UI

Access interactive API documentation at:
```
https://localhost:7000/swagger
```

Features:
- Full API documentation with XML comments
- "Try it out" functionality
- Authentication support (Bearer token)
- Schema definitions

## 🔧 Development Notes

### Improvements Implemented
1. **JWT Authentication**: RFC 7519 compliant with HMAC-SHA256 signing
2. **Thread-Safe Event IDs**: Using `Interlocked.Increment` to prevent collisions
3. **Enhanced Body Reading**: Better error handling with fallback logging
4. **JSON Serialization**: CamelCase naming policy for spec compliance
5. **Swagger Annotations**: XML documentation on all endpoints
6. **Optional Authentication**: Configurable via `appsettings.json`
7. **XML Documentation**: Generated for better IntelliSense and Swagger
8. **Database Constraints**: `RESTRICT` instead of `CASCADE` for explicit control

### Architecture Highlights
- **Middleware-based Exception Handling**: Centralized error processing
- **Service Layer**: Business logic separated from controllers
- **DTO Pattern**: Clean API contracts
- **Repository Pattern**: via EF Core DbContext
- **SOLID Principles**: Dependency injection, interface segregation

## 🛡️ Technologies

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL with Npgsql
- **JWT Bearer Authentication** (System.IdentityModel.Tokens.Jwt)
- Swashbuckle (Swagger/OpenAPI)
- PostgreSQL with Npgsql
- Swashbuckle (Swagger/OpenAPI)
- Custom Authentication Handler

## 📝 License

This is a test project for technical assessment.
