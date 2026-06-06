# UI Test Automation - webapp-spaces

This directory contains Playwright end-to-end tests for the spaces product.

## Running Tests

### Local Development (without backend)
```bash
pnpm test:e2e --run
```

## Test Organization

- `auth/` - Login, logout, and authentication flows  
- `spaces/` - Space management and listing
- `media/` - Media capture integration tests

## Writing New Tests

1. Create a new file in the appropriate subdirectory
2. Use Playwright's testing API with route mocking for API responses
3. Follow patterns from existing test files

## CI Integration

Tests run automatically on pull requests via `.github/workflows/ui-tests.yml`.
