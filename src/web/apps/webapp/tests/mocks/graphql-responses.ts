// Mock GraphQL Responses for UI Tests

export const mockResponses = {
  // GetSpaces query response
  GetSpaces: {
    data: {
      spaces: [
        {
          id: 'space_1',
          name: 'Test Workspace',
          description: 'A test workspace for UI testing',
          createdAt: '2026-06-06T12:00:00Z',
          membersCount: 3,
        },
        {
          id: 'space_2',
          name: 'Development Team Space',
          description: 'Space for development team collaboration',
          createdAt: '2026-05-15T08:30:00Z',
          membersCount: 5,
        },
      ],
    },
  },

  // GetUserProfile query response
  GetUserProfile: {
    data: {
      user: {
        id: 'user_123',
        name: 'John Doe',
        email: 'john.doe@example.com',
        avatarUrl: 'https://example.com/avatar.png',
        role: 'admin',
      },
    },
  },

  // CreateSpace mutation response
  CreateSpace: {
    data: {
      createSpace: {
        id: 'space_new',
        name: 'New Test Space',
        description: null,
        createdAt: '2026-06-06T12:00:00Z',
      },
    },
  },

  // Login mutation response
  Login: {
    data: {
      login: {
        token: 'test-jwt-token-12345',
        user: {
          id: 'user_123',
          name: 'John Doe',
          email: 'john.doe@example.com',
        },
      },
    },
  },

  // Logout mutation response
  Logout: {
    data: {
      logout: true,
    },
  },

  // InviteMember mutation response
  InviteMember: {
    data: {
      inviteToSpace: {
        invitationId: 'inv_123',
        status: 'pending',
      },
    },
  },
};

// Error responses for testing error handling
export const errorResponses = {
  networkError: null,
  unauthorized: {
    errors: [
      {
        message: 'Unauthorized - invalid or expired token',
        extensions: { code: 'UNAUTHENTICATED' },
      },
    ],
  },
  validationError: {
    data: null,
    errors: [
      {
        message: 'Validation failed',
        extensions: { code: 'BAD_USER_INPUT' },
      },
    ],
  },
};
