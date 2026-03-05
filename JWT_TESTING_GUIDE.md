# 🔐 JWT Authentication Testing Guide

## Quick Start

### 1. Start the API
```bash
cd FxNet.Test.Api
dotnet run
```

The API will be available at: `https://localhost:7xxx` (check console for exact port)

---

## 2. Get JWT Token

### Request
```bash
curl -X POST "https://localhost:7000/api.user.partner.rememberMe?code=testuser123" \
  -H "Content-Type: application/json"
```

### Response
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwidW5pcXVlX25hbWUiOiJ0ZXN0dXNlcjEyMyIsImp0aSI6IjEyMzQ1Njc4LTEyMzQtMTIzNC0xMjM0LTEyMzQ1Njc4OTAxMiIsIm5hbWVpZCI6IjEiLCJuYW1lIjoidGVzdHVzZXIxMjMiLCJleHAiOjE3MzUyMjY4MDAsImlzcyI6IkZ4TmV0VGVzdEFwaSIsImF1ZCI6IkZ4TmV0VGVzdEFwaVVzZXJzIn0.signature_here"
}
```

**Save this token** - you'll need it for all other requests!

---

## 3. Decode JWT Token (Optional)

Visit **https://jwt.io** and paste your token to see:

### Header
```json
{
  "alg": "HS256",
  "typ": "JWT"
}
```

### Payload (Claims)
```json
{
  "sub": "1",                       // Partner ID (Subject)
  "unique_name": "testuser123",     // User code
  "jti": "12345678-1234-...",       // JWT ID (unique token identifier)
  "nameid": "1",                    // Name identifier
  "name": "testuser123",            // User name
  "exp": 1735226800,                // Expiration (Unix timestamp)
  "iss": "FxNetTestApi",            // Issuer
  "aud": "FxNetTestApiUsers"        // Audience
}
```

### Signature
```
HMACSHA256(
  base64UrlEncode(header) + "." + base64UrlEncode(payload),
  your-secret-key
)
```

---

## 4. Use JWT Token

### Create Tree Node
```bash
curl -X POST "https://localhost:7000/api.user.tree.node.create?treeName=MyTree&nodeName=RootNode" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Get Tree
```bash
curl -X POST "https://localhost:7000/api.user.tree.get?treeName=MyTree" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

### Response
```json
{
  "id": 0,
  "name": "root",
  "children": [
    {
      "id": 1,
      "name": "RootNode",
      "children": []
    }
  ]
}
```

---

## 5. Test Without Token (Should Fail)

```bash
curl -X POST "https://localhost:7000/api.user.tree.get?treeName=MyTree"
```

### Expected Response: **401 Unauthorized**

---

## 6. Test With Invalid Token (Should Fail)

```bash
curl -X POST "https://localhost:7000/api.user.tree.get?treeName=MyTree" \
  -H "Authorization: Bearer invalid.token.here"
```

### Expected Response: **401 Unauthorized**

---

## 7. Test With Expired Token

Wait for token expiration (default: 24 hours) or change `ExpirationMinutes` to 1 minute in `appsettings.json`:

```json
{
  "Jwt": {
    "ExpirationMinutes": 1
  }
}
```

Then wait 1 minute and try using the token:

```bash
curl -X POST "https://localhost:7000/api.user.tree.get?treeName=MyTree" \
  -H "Authorization: Bearer YOUR_EXPIRED_TOKEN"
```

### Expected Response: **401 Unauthorized**

---

## 8. Test in Swagger UI

1. Open: `https://localhost:7000/swagger`
2. Click **"Authorize"** button (🔒 icon at top-right)
3. Paste your JWT token (without "Bearer " prefix)
4. Click **"Authorize"**
5. Now all requests will include the token automatically
6. Try any endpoint by clicking **"Try it out"**

---

## Testing Scenarios

### ✅ Valid Scenarios

| Test Case | Expected Result |
|-----------|----------------|
| Get token with unique code | 200 OK + JWT token |
| Use valid token on protected endpoint | 200 OK + data |
| Get token twice with same code | 200 OK + new JWT token |
| Decode token at jwt.io | See all claims |

### ❌ Invalid Scenarios

| Test Case | Expected Result |
|-----------|----------------|
| Call protected endpoint without token | 401 Unauthorized |
| Call protected endpoint with invalid token | 401 Unauthorized |
| Call protected endpoint with expired token | 401 Unauthorized |
| Modify token payload manually | 401 Unauthorized (signature invalid) |

---

## Configuration Options

### Enable/Disable Authentication

**`appsettings.json`**:
```json
{
  "Authentication": {
    "Enabled": false  // Set to false for testing without tokens
  }
}
```

### Change Token Expiration

```json
{
  "Jwt": {
    "ExpirationMinutes": 1440  // 24 hours (default)
  }
}
```

Common values:
- `5` = 5 minutes (for testing expiration)
- `60` = 1 hour
- `1440` = 24 hours
- `10080` = 7 days

### Change Secret Key

```json
{
  "Jwt": {
    "Secret": "YourNewSuperSecretKeyHere..."
  }
}
```

**⚠️ Important**: 
- Must be at least 32 characters
- Use strong random string in production
- Never commit production secrets to Git

---

## Security Best Practices

1. ✅ **Use HTTPS**: Always use SSL/TLS in production
2. ✅ **Strong Secret**: Use 64+ character random string
3. ✅ **Rotate Secrets**: Change JWT secret periodically
4. ✅ **Short Expiration**: Use shorter expiry for sensitive operations
5. ✅ **Environment Variables**: Store secrets in environment variables, not appsettings.json
6. ✅ **Refresh Tokens**: Implement refresh tokens for long-lived sessions (future enhancement)

---

## Troubleshooting

### "401 Unauthorized" with valid token

**Check**:
1. Token not expired? Decode at jwt.io
2. Secret matches between token generation and validation?
3. Issuer and Audience match configuration?
4. Authentication enabled in appsettings.json?

### "Invalid signature" error

**Cause**: JWT secret used for generation differs from validation secret

**Fix**: Ensure `Jwt:Secret` is identical in all environments

### Token works but claims are wrong

**Fix**: Delete the partner from database and get a new token:
```sql
DELETE FROM partners WHERE code = 'yourcode';
```

---

## Example: Full Workflow

```bash
# 1. Get JWT token
TOKEN=$(curl -s -X POST "https://localhost:7000/api.user.partner.rememberMe?code=alice" | jq -r '.token')

# 2. Create tree
curl -X POST "https://localhost:7000/api.user.tree.node.create?treeName=Companies&nodeName=TechCorp" \
  -H "Authorization: Bearer $TOKEN"

# 3. Add child nodes
curl -X POST "https://localhost:7000/api.user.tree.node.create?treeName=Companies&parentNodeId=1&nodeName=Engineering" \
  -H "Authorization: Bearer $TOKEN"

curl -X POST "https://localhost:7000/api.user.tree.node.create?treeName=Companies&parentNodeId=1&nodeName=Sales" \
  -H "Authorization: Bearer $TOKEN"

# 4. Get full tree
curl -X POST "https://localhost:7000/api.user.tree.get?treeName=Companies" \
  -H "Authorization: Bearer $TOKEN" | jq
```

**Expected Result**:
```json
{
  "id": 0,
  "name": "root",
  "children": [
    {
      "id": 1,
      "name": "TechCorp",
      "children": [
        {
          "id": 2,
          "name": "Engineering",
          "children": []
        },
        {
          "id": 3,
          "name": "Sales",
          "children": []
        }
      ]
    }
  ]
}
```

---

## Next Steps

- ✅ Test all endpoints with JWT
- ✅ Verify token expiration works
- ✅ Test error scenarios
- ✅ Deploy to production with secure secret
- 🔄 Consider implementing refresh tokens (optional)
- 🔄 Add user roles/permissions (optional)

**Your API now has production-ready JWT authentication!** 🎉🔐
