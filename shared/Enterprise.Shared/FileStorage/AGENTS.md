# FileStorage Module — Agent Notes

## Purpose

Provides CDN (public file serving) and private file storage behind two interfaces:
`ICdnService` and `IPrivateFileService`. Implementations switch between a local filesystem backend
(development) and Cloudflare (production) based on configuration.

## Registration

```csharp
services.AddFileStorage(
    configuration,
    publicCdnFileEndpoint: "https://cdn.example.com/files/",
    privateFileEndpoint: "https://api.example.com/files/private/");
```

The two endpoint strings are provided by the calling host because endpoint URLs are host-specific.

**Config section key:** `FileStorage` — see `FileStorageConfiguration.cs`.

## What Gets Registered

| Condition                      | `ICdnService` impl     | `IPrivateFileService` impl     |
|--------------------------------|------------------------|--------------------------------|
| `FileStorage:UseLocal = true`  | `LocalCdnService`      | `LocalPrivateFileService`      |
| `FileStorage:UseLocal = false` | `CloudflareCdnService` | `CloudflarePrivateFileService` |

When running locally, the service automatically creates the required directories under
`~/wwwroot/cdn` and `~/wwwroot/private` if they do not exist.

## Configuration Reference

```json
{
  "FileStorage": {
    "UseLocal": true,
    "LocalCdnPath": "",
    "LocalPrivateFilePath": "",
    "MaxFileSize": 10485760
  },
  "Cloudflare": {
    "AccountId": "...",
    "ApiToken": "...",
    "BucketName": "...",
    "PublicUrl": "https://..."
  }
}
```

`LocalCdnPath` and `LocalPrivateFilePath` default to `~/wwwroot/cdn` and `~/wwwroot/private`
when left empty.

`MaxFileSize` is applied as the `MultipartBodyLengthLimit` for file upload endpoints.

## Rules

- Always call `AddFileStorage` with the host-specific endpoint strings rather than reading them
  inside the module — the module does not know its own public URL.
- Do not reference `Cloudflare` configuration directly in domain code — inject `ICdnService` or
  `IPrivateFileService` instead.
- AWS S3 (`AWSSDK.S3`) is a declared dependency but is not used by the current implementations;
  do not add S3 service registrations here unless an `S3*` implementation is also added.
