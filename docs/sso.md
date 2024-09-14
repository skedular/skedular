```mermaid
sequenceDiagram
    participant User
    participant React Frontend
    participant .NET Backend
    participant Azure AD

    User->>React Frontend: Clicks "Login with Microsoft"
    React Frontend->>Azure AD: Redirect to /authorize (OAuth 2.0)
    Azure AD-->>User: Presents Microsoft login page
    User-->>Azure AD: Submits login credentials
    Azure AD-->>React Frontend: Redirect back with authorization code

    React Frontend->>.NET Backend: Send authorization code
    .NET Backend->>Azure AD: Exchange code for access token (/token)
    Azure AD-->>.NET Backend: Responds with access token and ID token

    .NET Backend->>Azure AD: (Optional) Fetch user details with access token
    Azure AD-->>.NET Backend: Responds with user information

    .NET Backend-->>React Frontend: Return user details (e.g., name, email)
    React Frontend-->>User: Displays user info (e.g., "Welcome, User!")

