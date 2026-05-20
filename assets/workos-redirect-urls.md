# WorkOS Redirect URLs

This is the redirect URL reference for the WorkOS clients. The apps currently use these WorkOS redirect paths:

- `/callback` for the standard AuthKit sign-in/sign-up and protected-route flow in every app.
- `/api/auth/oauth/callback` for the custom Google/Microsoft OAuth flow in `webapp` only.

## Staging

Register these on the staging/non-production WorkOS client.

```text
http://localhost:15000/callback
http://localhost:15000/api/auth/oauth/callback
http://localhost:15002/callback
http://localhost:15004/callback

https://staging.skedular.app/callback
https://staging.skedular.app/api/auth/oauth/callback

https://spacesstaging.skedular.app/callback
https://teamsstaging.skedular.app/callback

https://skedularmarketplacetrial.staging.skedular.app/callback
https://skedularmarketplacetrial.staging.skedular.app/api/auth/oauth/callback

https://assembly.staging.skedular.app/callback
https://assembly.staging.skedular.app/api/auth/oauth/callback
```

## Production

Register these on the production WorkOS client.

```text
https://skedular.app/callback
https://skedular.app/api/auth/oauth/callback

https://spaces.skedular.app/callback

https://teams.skedular.app/callback

https://skedularmarketplacetrial.skedular.app/callback
https://skedularmarketplacetrial.skedular.app/api/auth/oauth/callback

https://assembly.skedular.app/callback
https://assembly.skedular.app/api/auth/oauth/callback
```
