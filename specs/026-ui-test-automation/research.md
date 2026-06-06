# Research: UI Test Automation with Playwright

## Decision: Use Playwright for E2E Testing

**Rationale**: 
- Playwright is the industry standard for browser automation with excellent TypeScript support
- Built-in support for API mocking via `page.route()` enables testing without backend services
- First-class video and screenshot capture capabilities
- Runs headlessly in CI environments
- Excellent cross-browser support (Chromium, Firefox, WebKit)

**Alternatives considered**:
- **Cypress**: Excellent but browser automation is more complex without network access; Playwright's mocking is more flexible
- **Puppeteer**: Only supports Chromium; Playwright provides multi-browser testing
- **Vitest E2E with Web Driver IO**: More complex setup; Playwright offers better out-of-box experience

## Decision: Mock API Responses Using `page.route()`

**Rationale**:
- Playwright's route mocking allows intercepting fetch/XMLHttpRequest calls and returning predefined responses
- No backend startup required for local testing
- Mock data can be versioned alongside tests
- Supports both static responses and dynamic response generation based on request

**Implementation approach**:
```typescript
// Example pattern
await page.route('**/api/graphql', async (route) => {
  const request = route.request();
  if (request.method() === 'POST') {
    await route.fulfill({
      status: 200,
      contentType: 'application/json',
      body: JSON.stringify(mockData),
    });
  } else {
    await route.continue();
  }
});
```

## Decision: Organize Tests Per Application

**Rationale**:
- Each webapp (`webapp`, `webapp-spaces`, `webapp-teams`) has distinct functionality
- Separate test suites allow independent execution and debugging
- Follows existing project structure pattern (each app has its own `src/test/` directory)

**Structure**:
```
apps/webapp/tests/e2e/
  ├── auth/
  │   ├── login.spec.ts
  │   └── logout.spec.ts
  ├── spaces/
  │   └── create-space.spec.ts
  └── teams/
      └── invite-member.spec.ts

apps/webapp/tests/mocks/
  ├── graphql-responses.ts
  └── api-paths.ts
```

## Decision: Media Capture as Optional Feature

**Rationale**:
- Video/screenshot capture is resource-intensive and slow
- Should be disabled by default for CI efficiency
- Enable via environment variable (`PLAYWRIGHT_RECORD_VIDEO=true`)

**Implementation**:
```typescript
// Configure per-test based on env var
const recordVideo = process.env.PLAYwright_RECORD_VIDEO === 'true';
const videoDir = process.env.VIDEO_OUTPUT_DIR || './tests/videos';

await browser.launch({
  headless: !recordVideo, // headed if recording
});
```

## Decision: Output Media to Configurable Directory

**Rationale**:
- Prevents media files from cluttering source directories
- CI environments can mount volumes for artifact storage
- Local developers can exclude from git via `.gitignore`

**Default paths**:
- Videos: `./.test-artifacts/videos/`
- Screenshots: `./.test-artifacts/screenshots/`
- Logs: `./.test-artifacts/logs/`

## Decision: CI Pipeline with GitHub Actions

**Rationale**:
- Existing infrastructure (project uses GitHub)
- No additional tooling costs
- Can run on pull request events
- Supports parallel test execution for faster feedback

**Workflow approach**:
```yaml
name: UI Tests
on: [pull_request]
jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
        with:
          node-version: '20'
      - run: pnpm install
      - run: pnpm test:e2e --run
```

## Decision: Test Coverage Target of 80%

**Rationale**:
- High enough to catch regressions without over-testing
- Focus on core user scenarios (booking flow, authentication, key CRUD operations)
- Edge cases covered by integration/unit tests instead

**Coverage areas**:
1. Authentication flows (login, logout, session persistence)
2. Main navigation and routing
3. Critical user journeys (create space, invite member, etc.)
4. Form validation and submission
5. Error handling and empty states

## Open Questions Resolved

| Question | Resolution |
|----------|------------|
| Which web apps to test? | `webapp`, `webapp-spaces`, `webapp-teams` (public-web excluded) |
| Media format for screenshots/videos? | Standard formats: MP4 video, PNG screenshots |
| Backend required? | No - API mocking via Playwright route interception |
