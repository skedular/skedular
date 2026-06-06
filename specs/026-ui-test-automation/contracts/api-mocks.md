# API Contracts: UI Test Mock Responses

This document defines the contract for mocked API endpoints used in UI test automation.

## GraphQL Endpoints

All GraphQL requests are intercepted and respond with predefined data.

### POST /graphql

**Request Headers**:
- `Content-Type`: `application/json`
- `Authorization`: Bearer token (optional, tests can provide or omit)

**Mock Response Format**:

```json
{
  "data": { ... },
  "errors": [] // Optional
}
```

## REST API Endpoints

### GET /api/users/:id

Returns user profile data.

**Response**:
```json
{
  "id": "user_123",
  "name": "Test User",
  "email": "test@example.com",
  "avatarUrl": "https://example.com/avatar.png"
}
```

### POST /api/spaces

Creates a new space.

**Request Body**:
```json
{
  "name": string,
  "description?: string,
  "location?: string
}
```

**Response**:
```json
{
  "id": "space_123",
  "name": "Test Space",
  "created_at": "2026-06-06T12:00:00Z"
}
```

### GET /api/spaces/:id/members

Returns space members.

**Response**:
```json
{
  "members": [
    {
      "userId": "user_123",
      "role": "owner" | "member",
      "joinedAt": "2026-06-06T12:00:00Z"
    }
  ]
}
```

## Mock Data Configuration

Each webapp has a `tests/mocks/graphql-responses.ts` file that maps query names to response data:

```typescript
export const mockResponses = {
  GetSpaces: {
    spaces: [{ id: '1', name: 'Test Space' }],
  },
  GetUserProfile: {
    user: { id: '1', name: 'John Doe', email: 'john@example.com' },
  },
};
```

## Error Scenarios

**Network Error**: Return status 503 with empty body
**Unauthorized**: Return status 401 with `{ error: 'Unauthorized' }`
**Validation Failure**: Return status 400 with `{ errors: [...] }`
