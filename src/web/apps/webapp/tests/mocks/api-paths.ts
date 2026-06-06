// API Path Patterns for Playwright Route Mocking

export const GRAPHQL_ENDPOINT = '/graphql';

export const REST_API_PATHS = {
  users: (id?: string) => id ? `/api/users/${id}` : '/api/users',
  spaces: (id?: string) => id ? `/api/spaces/${id}` : '/api/spaces',
  members: (spaceId: string) => `/api/spaces/${spaceId}/members`,
};

// GraphQL query/mutation name patterns for mocking
export const GRAPHQL_PATTERNS = {
  GET_SPACES: 'GetSpaces',
  GET_USER_PROFILE: 'GetUserProfile',
  CREATE_SPACE: 'CreateSpace',
  INVITE_MEMBER: 'InviteMember',
  LOGIN: 'Login',
  LOGOUT: 'Logout',
};

// Route matching patterns for Playwright
export const ROUTE_PATTERNS = {
  graphql: '**/graphql',
  api: '**/api/*',
  static: '**/*.js, **/*.css, **/*.png, **/*.jpg, **/*.svg',
};
