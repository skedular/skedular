# FileStorage Module — Agent Notes

## Purpose

Provides CDN (public file serving) and private file storage behind two interfaces:
`ICdnService` and `IFileService`. Implementations switch between a local filesystem backend
(development/file-server mode) and Cloudflare R2 (production) based on configuration.

## Registration

```csharp
services.AddFileStorage(
    configuration,
    publicCdnFileEndpoint: "https://cdn.example.com/files/",
  fileEndpoint: "https://api.example.com/files/private/");
```

The two endpoint strings are provided by the calling host because endpoint URLs are host-specific.

**Config section key:** `FileStorage` — see `FileStorageConfiguration.cs`.

## What Gets Registered

| Condition                           | `ICdnService` impl     | `IFileService` impl     |
| ----------------------------------- | ---------------------- | ----------------------- |
| `FileStorage:UseFileServer = true`  | `LocalCdnService`      | `LocalFileService`      |
| `FileStorage:UseFileServer = false` | `CloudflareCdnService` | `CloudflareFileService` |

When running locally, the service automatically creates the required directories under
`~/wwwroot/cdn` and `~/wwwroot/private` if they do not exist.

## Configuration Reference

```json
{
  "FileStorage": {
    "UseFileServer": true,
    "FileServerPublicFilePath": "",
    "FileServerFilePath": "",
    "MaxFileSize": 10485760
  },
  "Cloudflare": {
    "AccountId": "...",
    "AccessKey": "...",
    "SecretKey": "...",
    "CdnR2BucketName": "...",
    "FileR2BucketName": "...",
    "CdnBaseUrl": "https://..."
  }
}
```

`FileServerPublicFilePath` and `FileServerFilePath` default to `~/wwwroot/cdn` and `~/wwwroot/private`
when left empty.

`PublicCdnFileEndpoint` and `FileEndpoint` are populated by `AddFileStorage(...)` from the host-specific
route prefixes and are used to build returned file URLs.

`MaxFileSize` is applied as the `MultipartBodyLengthLimit` for file upload endpoints.

## Rules

- Always call `AddFileStorage` with the host-specific endpoint strings rather than reading them
  inside the module — the module does not know its own public URL.
- Do not reference `Cloudflare` configuration directly in domain code — inject `ICdnService` or
  `IFileService` instead.
- Cloudflare R2 access is implemented through the AWS S3 SDK (`AWSSDK.S3`) against the R2
  S3-compatible endpoint; keep that dependency aligned with both `CloudflareCdnService` and
  `CloudflareFileService`.
