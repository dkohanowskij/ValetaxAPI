# Complete Test Data Guide for valetax-test-swagger.json

## Overview
This guide provides complete instructions for populating your database with test data that will pass **ALL** tests defined in your `valetax-test-swagger.json` file.

## Quick Start

### 1. Apply Database Migrations
```bash
cd FxNet.Test.Api
dotnet ef database update
```

### 2. Load Test Data
```bash
cd ..
psql -U postgres -d fxnet_test -f test-data.sql
```

### 3. Verify Test Data
```bash
psql -U postgres -d fxnet_test -f verify-test-data.sql
```

### 4. Start API
```bash
cd FxNet.Test.Api
dotnet run
```

### 5. Import Postman Collection
- Open Postman
- Import `postman-swagger-tests.json`
- Set `baseUrl` variable to `http://localhost:5072` (or your API URL)

## Test Data Summary

### Partners (5 records)
| ID | Code | Purpose |
|----|------|---------|
| 1 | TEST_USER_001 | Primary test user |
| 2 | TEST_USER_002 | Secondary test user |
| 3 | DEMO_USER | Demo account |
| 4 | INTEGRATION_TEST | Integration testing |
| 5 | QA_TESTER | QA testing |

**API Endpoint**: `/api.user.partner.rememberMe?code={CODE}`
- ✅ Existing codes return JWT token
- ✅ New codes auto-create partner and return JWT token

### Trees (3 records)

#### TestTree (ID: 1) - Complex Hierarchical Structure
```
Root (ID: 1)
├── Projects (ID: 2)
│   ├── WebApp (ID: 3)
│   │   ├── Frontend (ID: 4)
│   │   └── Backend (ID: 5)
│   ├── MobileApp (ID: 6)
│   └── API (ID: 7)
├── Documents (ID: 8)
│   ├── Reports (ID: 9)
│   └── Invoices (ID: 10)
└── Archive (ID: 11)
```
**Total Nodes**: 11
**Purpose**: Comprehensive tree testing, node operations

#### ProductionTree (ID: 2) - Simple Structure
```
Services (ID: 12)
├── Database (ID: 13)
├── Cache (ID: 14)
└── Queue (ID: 15)
```
**Total Nodes**: 4
**Purpose**: Basic operations, simple structure testing

#### EmptyTree (ID: 3)
**Total Nodes**: 0
**Purpose**: Edge case testing, empty tree scenarios

**API Endpoint**: `/api.user.tree.get?treeName={TREE_NAME}`
- ✅ Existing trees return hierarchical structure
- ✅ Non-existent trees are auto-created (empty)

### Nodes (15 records)
**Test Cases Covered**:
- ✅ Root-level nodes (no parent)
- ✅ Single-level children
- ✅ Multi-level hierarchies (3+ levels deep)
- ✅ Multiple siblings
- ✅ Leaf nodes (no children)
- ✅ Parent nodes (with children)

**API Endpoints**:
- `/api.user.tree.node.create` - Create node (root or child)
- `/api.user.tree.node.rename` - Rename node (unique among siblings)
- `/api.user.tree.node.delete` - Delete node (must have no children)

### Exception Journal (30 records)
**Date Range**: Last 84 days to 1 hour ago
**Event IDs**: 1001-1030
**Test Coverage**:
- ✅ Pagination (skip/take)
- ✅ Date filtering (from/to)
- ✅ Text search
- ✅ Combined filters
- ✅ Diverse error messages

**API Endpoints**:
- `/api.user.journal.getRange?skip={N}&take={M}` - Paginated list
- `/api.user.journal.getSingle?id={ID}` - Single journal entry

## Swagger Test Coverage Matrix

### ✅ Authentication Tests (user.partner)

| Test Case | Endpoint | Expected Result | Test Data |
|-----------|----------|-----------------|-----------|
| Remember Me - Existing User | `POST /api.user.partner.rememberMe?code=TEST_USER_001` | 200, JWT token | Partner ID 1 |
| Remember Me - New User | `POST /api.user.partner.rememberMe?code=NEW_CODE` | 200, JWT token, auto-create | N/A |
| Remember Me - Demo User | `POST /api.user.partner.rememberMe?code=DEMO_USER` | 200, JWT token | Partner ID 3 |

### ✅ Journal Tests (user.journal)

| Test Case | Endpoint | Expected Result | Test Data |
|-----------|----------|-----------------|-----------|
| Get Range - All | `POST /api.user.journal.getRange?skip=0&take=10` | 200, 10 items | IDs 1-30 |
| Get Range - Page 2 | `POST /api.user.journal.getRange?skip=10&take=10` | 200, next 10 items | IDs 1-30 |
| Get Range - Filter by Date | `POST /api.user.journal.getRange` + body | 200, filtered items | IDs 1-30 |
| Get Range - Search Text | `POST /api.user.journal.getRange` + body | 200, matching items | IDs with "node" |
| Get Single - ID 1 | `POST /api.user.journal.getSingle?id=1` | 200, full details | Journal ID 1 |
| Get Single - ID 15 | `POST /api.user.journal.getSingle?id=15` | 200, full details | Journal ID 15 |
| Get Single - ID 30 | `POST /api.user.journal.getSingle?id=30` | 200, full details | Journal ID 30 |
| Get Single - Not Found | `POST /api.user.journal.getSingle?id=999` | 500, SecureException | N/A |

### ✅ Tree Tests (user.tree)

| Test Case | Endpoint | Expected Result | Test Data |
|-----------|----------|-----------------|-----------|
| Get Tree - TestTree | `POST /api.user.tree.get?treeName=TestTree` | 200, 11 nodes | Tree ID 1 |
| Get Tree - ProductionTree | `POST /api.user.tree.get?treeName=ProductionTree` | 200, 4 nodes | Tree ID 2 |
| Get Tree - EmptyTree | `POST /api.user.tree.get?treeName=EmptyTree` | 200, 0 nodes | Tree ID 3 |
| Get Tree - Auto-Create | `POST /api.user.tree.get?treeName=NewTree` | 200, 0 nodes, created | N/A |

### ✅ Tree Node Tests (user.tree.node)

| Test Case | Endpoint | Expected Result | Test Data |
|-----------|----------|-----------------|-----------|
| **Create Node** | | | |
| Create Root Node | `POST /api.user.tree.node.create?treeName=TestTree&nodeName=NewRoot` | 200 | Tree ID 1 |
| Create Child Node | `POST /api.user.tree.node.create?treeName=TestTree&parentNodeId=1&nodeName=Child` | 200 | Node ID 1 |
| Create Deep Node | `POST /api.user.tree.node.create?treeName=TestTree&parentNodeId=4&nodeName=Deep` | 200 | Node ID 4 |
| Create Duplicate (Fail) | `POST /api.user.tree.node.create?treeName=TestTree&parentNodeId=1&nodeName=Projects` | 500 | Node ID 2 exists |
| Create Invalid Parent (Fail) | `POST /api.user.tree.node.create?treeName=TestTree&parentNodeId=999&nodeName=Node` | 500 | N/A |
| **Rename Node** | | | |
| Rename Success | `POST /api.user.tree.node.rename?nodeId=11&newNodeName=NewArchive` | 200 | Node ID 11 |
| Rename Duplicate (Fail) | `POST /api.user.tree.node.rename?nodeId=5&newNodeName=Frontend` | 500 | Node ID 4 exists |
| Rename Not Found (Fail) | `POST /api.user.tree.node.rename?nodeId=999&newNodeName=Name` | 500 | N/A |
| **Delete Node** | | | |
| Delete Leaf Node | `POST /api.user.tree.node.delete?nodeId=10` | 200 | Node ID 10 (Invoices) |
| Delete With Children (Fail) | `POST /api.user.tree.node.delete?nodeId=1` | 500 | Node ID 1 (Root) has children |
| Delete Not Found (Fail) | `POST /api.user.tree.node.delete?nodeId=999` | 500 | N/A |

## Running Tests in Postman

### Step 1: Authentication
1. Run "Remember Me - TEST_USER_001"
2. Token automatically saved to collection variable `authToken`
3. All subsequent requests use this token (Bearer authentication)

### Step 2: Run All Tests
- Click collection name "FxNet.Test.Api - Swagger Complete Tests"
- Click "Run" button
- Select all folders
- Click "Run FxNet.Test.Api..."
- **Expected**: 30+ tests pass

### Step 3: Review Results
- ✅ Green tests = Pass
- ❌ Red tests = Review error message
- Check API logs for detailed error information

## Test Data by Endpoint

### `/api.user.partner.rememberMe`
**Test Values**:
```
✅ TEST_USER_001
✅ TEST_USER_002
✅ DEMO_USER
✅ INTEGRATION_TEST
✅ QA_TESTER
✅ ANY_NEW_CODE (auto-creates)
```

### `/api.user.tree.get`
**Test Values**:
```
✅ TestTree (11 nodes)
✅ ProductionTree (4 nodes)
✅ EmptyTree (0 nodes)
✅ ANY_NEW_TREE_NAME (auto-creates empty)
```

### `/api.user.tree.node.create`
**Valid Test Combinations**:
```
✅ treeName=TestTree, nodeName=UniqueNewName (root node)
✅ treeName=TestTree, parentNodeId=1, nodeName=UniqueChildName
✅ treeName=TestTree, parentNodeId=4, nodeName=DeepChild
✅ treeName=ProductionTree, parentNodeId=12, nodeName=NewService
```

**Invalid Test Cases (Expected to Fail)**:
```
❌ treeName=TestTree, parentNodeId=1, nodeName=Projects (duplicate sibling)
❌ treeName=TestTree, parentNodeId=999, nodeName=Node (parent not found)
❌ treeName=TestTree, parentNodeId=12, nodeName=Node (parent in different tree)
```

### `/api.user.tree.node.rename`
**Valid Test Values**:
```
✅ nodeId=11, newNodeName=RenamedArchive
✅ nodeId=9, newNodeName=NewReports
✅ nodeId=15, newNodeName=MessageQueue
```

**Invalid Test Cases (Expected to Fail)**:
```
❌ nodeId=5, newNodeName=Frontend (duplicate sibling)
❌ nodeId=999, newNodeName=Name (node not found)
```

### `/api.user.tree.node.delete`
**Valid Deletions (Leaf Nodes)**:
```
✅ nodeId=10 (Invoices - no children)
✅ nodeId=9 (Reports - no children)
✅ nodeId=15 (Queue - no children)
```

**Invalid Deletions (Expected to Fail)**:
```
❌ nodeId=1 (Root - has children)
❌ nodeId=2 (Projects - has children)
❌ nodeId=8 (Documents - has children)
❌ nodeId=999 (not found)
```

### `/api.user.journal.getRange`
**Pagination Test Values**:
```
✅ skip=0, take=10 (first page)
✅ skip=10, take=10 (second page)
✅ skip=20, take=10 (third page)
✅ skip=0, take=30 (all entries)
```

**Filter Test Values** (request body):
```json
// Date filter only
{
  "from": "2025-01-01T00:00:00Z",
  "to": "2025-12-31T23:59:59Z"
}

// Search filter only
{
  "search": "node"
}

// Combined filters
{
  "from": "2025-01-01T00:00:00Z",
  "to": "2025-12-31T23:59:59Z",
  "search": "not found"
}

// Empty filter (all entries)
{}
```

### `/api.user.journal.getSingle`
**Valid Test Values**:
```
✅ id=1 (EventID 1001)
✅ id=15 (EventID 1015)
✅ id=30 (EventID 1030)
✅ Any ID from 1-30
```

**Invalid Test Values (Expected to Fail)**:
```
❌ id=999 (not found)
❌ id=0 (not found)
```

## Troubleshooting

### Issue: "Partner code cannot be empty"
**Solution**: Ensure you're passing a non-empty `code` parameter

### Issue: "Node X not found"
**Solution**: Verify node ID exists in database using verify script

### Issue: "You have to delete all children nodes first"
**Solution**: Expected behavior - delete child nodes first before parent

### Issue: "A node named 'X' already exists"
**Solution**: Expected behavior - use unique names among siblings

### Issue: "Journal entry X not found"
**Solution**: Valid IDs are 1-30, use one of these IDs

### Issue: "Unauthorized" (401)
**Solution**: Run authentication request first to get JWT token

## Database Verification Queries

```sql
-- Check partners
SELECT * FROM partners ORDER BY id;

-- Check trees and node counts
SELECT t.name, COUNT(n.id) as node_count
FROM trees t
LEFT JOIN nodes n ON t.id = n.tree_id
GROUP BY t.id, t.name
ORDER BY t.id;

-- Check TestTree structure
SELECT n.id, n.name, n.parent_node_id, p.name as parent_name
FROM nodes n
LEFT JOIN nodes p ON n.parent_node_id = p.id
WHERE n.tree_id = 1
ORDER BY n.id;

-- Check journal entries
SELECT id, event_id, created_at, LEFT(text, 50) as text_preview
FROM exception_journal
ORDER BY created_at DESC
LIMIT 10;

-- Check totals
SELECT 
  (SELECT COUNT(*) FROM partners) as partners,
  (SELECT COUNT(*) FROM trees) as trees,
  (SELECT COUNT(*) FROM nodes) as nodes,
  (SELECT COUNT(*) FROM exception_journal) as journals;
```

## Expected Test Results

### Authentication: 3/3 Pass
✅ Remember Me - TEST_USER_001
✅ Remember Me - New User (Auto-Create)
✅ Remember Me - DEMO_USER

### Journal: 9/9 Pass
✅ Get Range - All Entries
✅ Get Range - Pagination Page 2
✅ Get Range - With Date Filter
✅ Get Range - With Search Filter
✅ Get Range - Combined Filters
✅ Get Single - ID 1
✅ Get Single - ID 15
✅ Get Single - ID 30
✅ Get Single - Not Found (Expected Failure)

### Tree: 4/4 Pass
✅ Get Tree - TestTree
✅ Get Tree - ProductionTree
✅ Get Tree - EmptyTree
✅ Get Tree - Auto-Create New Tree

### Tree Node: 13/13 Pass
✅ Create Node - Root Level
✅ Create Node - With Parent
✅ Create Node - Deep Level
✅ Create Node - Duplicate Name (Expected Failure)
✅ Create Node - Parent Not Found (Expected Failure)
✅ Rename Node - Success
✅ Rename Node - Duplicate Sibling (Expected Failure)
✅ Rename Node - Not Found (Expected Failure)
✅ Delete Node - Leaf Node
✅ Delete Node - With Children (Expected Failure)
✅ Delete Node - Not Found (Expected Failure)

**Total Expected: 29/29 Tests Pass** ✅

## Summary

Your database now contains comprehensive test data that covers:
- ✅ All endpoints in `valetax-test-swagger.json`
- ✅ Positive test cases (success scenarios)
- ✅ Negative test cases (expected failures)
- ✅ Edge cases (empty trees, auto-creation)
- ✅ Pagination and filtering
- ✅ Hierarchical data structures
- ✅ Unique constraint validations

**All tests should pass!** 🎉

If any tests fail, review the specific test case in this guide and verify your API implementation matches the expected behavior.
