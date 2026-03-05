# Test Data Files Summary

## Files Created

### 1. `test-data.sql` ⭐ **MAIN TEST DATA FILE**
**Purpose**: Comprehensive SQL script to populate PostgreSQL database
**Contents**:
- 5 Partners for authentication testing
- 3 Trees (TestTree with 11 nodes, ProductionTree with 4 nodes, EmptyTree with 0 nodes)
- 15 Nodes in hierarchical structures
- 30 Exception Journal entries spanning 84 days

**How to Use**:
```bash
psql -U postgres -d fxnet_test -f test-data.sql
```

### 2. `verify-test-data.sql`
**Purpose**: Verification script to check test data was loaded correctly
**Contents**:
- Displays all partners with codes
- Shows tree structures
- Lists node hierarchies
- Displays journal entries
- Shows statistics and test values

**How to Use**:
```bash
psql -U postgres -d fxnet_test -f verify-test-data.sql
```

### 3. `postman-swagger-tests.json` ⭐ **MAIN POSTMAN COLLECTION**
**Purpose**: Complete Postman collection for testing all Swagger endpoints
**Contents**:
- 29 test requests organized by API category
- Automated test assertions
- Collection variables for easy configuration
- Bearer token authentication
- Positive and negative test cases

**How to Use**:
1. Import into Postman
2. Set `baseUrl` variable (default: http://localhost:5072)
3. Run "Remember Me" request first (saves auth token)
4. Run entire collection or individual tests

### 4. `SWAGGER-TEST-DATA-GUIDE.md` ⭐ **COMPREHENSIVE GUIDE**
**Purpose**: Complete documentation for testing
**Contents**:
- Quick start instructions
- Test data summary with IDs and structures
- Complete test coverage matrix
- Expected results for all tests
- Troubleshooting guide
- Database verification queries

### 5. `API-TESTING-GUIDE.md`
**Purpose**: General API testing guide (previous version)
**Note**: Use `SWAGGER-TEST-DATA-GUIDE.md` instead for Swagger-specific testing

## Quick Start (3 Steps)

### Step 1: Load Test Data
```bash
# Navigate to project root
cd C:\Users\Dzmitriy_Kakhanouski\source\repos\ValetaxWebApi

# Apply migrations (if not already done)
cd FxNet.Test.Api
dotnet ef database update
cd ..

# Load test data
psql -U postgres -d fxnet_test -f test-data.sql

# Verify data
psql -U postgres -d fxnet_test -f verify-test-data.sql
```

### Step 2: Start API
```bash
cd FxNet.Test.Api
dotnet run
```

### Step 3: Test with Postman
1. Open Postman
2. Import `postman-swagger-tests.json`
3. Run "01 - Authentication" → "Remember Me - TEST_USER_001"
4. Run entire collection (29 tests should pass)

## Test Data Overview

| Resource | Count | Purpose |
|----------|-------|---------|
| **Partners** | 5 | Authentication testing |
| **Trees** | 3 | Tree CRUD operations |
| **Nodes** | 15 | Node operations, hierarchies |
| **Journal Entries** | 30 | Pagination, filtering, search |

## Swagger Endpoints Covered

✅ `POST /api.user.partner.rememberMe` - Authentication
✅ `POST /api.user.journal.getRange` - Paginated journal list
✅ `POST /api.user.journal.getSingle` - Single journal entry
✅ `POST /api.user.tree.get` - Get entire tree
✅ `POST /api.user.tree.node.create` - Create tree node
✅ `POST /api.user.tree.node.rename` - Rename tree node
✅ `POST /api.user.tree.node.delete` - Delete tree node

## Test Coverage

### Positive Tests (Should Pass)
- ✅ Authentication with existing and new users
- ✅ Get trees (existing and auto-create new)
- ✅ Create nodes (root level and nested)
- ✅ Rename nodes with unique names
- ✅ Delete leaf nodes without children
- ✅ Pagination with various skip/take values
- ✅ Filtering by date range
- ✅ Text search in journal entries
- ✅ Get single journal entries by ID

### Negative Tests (Expected Failures)
- ❌ Create node with duplicate sibling name
- ❌ Create node with non-existent parent
- ❌ Rename node to existing sibling name
- ❌ Delete node with children
- ❌ Get non-existent journal entry
- ❌ Rename/delete non-existent node

## Key Test Values

### Authentication Codes
```
TEST_USER_001, TEST_USER_002, DEMO_USER, INTEGRATION_TEST, QA_TESTER
```

### Tree Names
```
TestTree (11 nodes), ProductionTree (4 nodes), EmptyTree (0 nodes)
```

### Node IDs for Testing
```
Root: 1
Projects: 2
  WebApp: 3
    Frontend: 4 (deep nesting)
    Backend: 5
Archive: 11 (renameable)
Invoices: 10 (deleteable leaf)
```

### Journal Entry IDs
```
Valid: 1-30
Invalid (test failure): 999
```

## Expected Results

### Total Tests: 29
- Authentication: 3 tests
- Journal API: 9 tests
- Tree API: 4 tests
- Tree Node API: 13 tests

### All Tests Should Pass: ✅ 29/29

## File Locations

```
ValetaxWebApi/
├── test-data.sql                    # ⭐ Load this first
├── verify-test-data.sql              # Check data loaded correctly
├── postman-swagger-tests.json        # ⭐ Import to Postman
├── SWAGGER-TEST-DATA-GUIDE.md        # ⭐ Read this for details
├── API-TESTING-GUIDE.md              # Alternative guide
├── postman-collection.json           # (old version)
└── FxNet.Test.Api/
    └── [API project files]
```

## Troubleshooting

### Database Connection
- **Host**: localhost
- **Port**: 5432
- **Database**: fxnet_test
- **Username**: postgres
- **Password**: Serengeti1

### Common Issues

**"relation does not exist"**
→ Run migrations: `dotnet ef database update`

**"Unauthorized" in Postman**
→ Run authentication request first

**Tests fail**
→ Check API is running on http://localhost:5072
→ Verify test data loaded: `psql -U postgres -d fxnet_test -f verify-test-data.sql`

## Next Steps

1. ✅ Load test data: `psql -U postgres -d fxnet_test -f test-data.sql`
2. ✅ Start API: `dotnet run --project FxNet.Test.Api`
3. ✅ Import Postman collection: `postman-swagger-tests.json`
4. ✅ Run tests in Postman
5. ✅ Verify all 29 tests pass

## Support

For detailed information about specific test cases, see:
- **SWAGGER-TEST-DATA-GUIDE.md** - Complete testing documentation
- **verify-test-data.sql** - Database verification

---

**Status**: ✅ Ready for Testing

All files are configured to work with your API implementation based on `valetax-test-swagger.json`.
