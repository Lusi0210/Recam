# Azure Blob Storage Configuration

This document describes the Azure Blob Storage setup used for media file
management (images, videos, floor plans, VR tours) in the RECAM platform.

## Storage Account

| Item            | Value                          |
|-----------------|--------------------------------|
| Account name    | recamstorage2026               |
| Resource group  | recam-rg                       |
| Region          | Australia East                 |
| Performance     | Standard                       |
| Account kind    | StorageV2 (general purpose v2) |
| Replication     | Locally-redundant storage (LRS)|
| Access tier     | Hot                            |

## Container Configuration

| Item                    | Value                       |
|-------------------------|-----------------------------|
| Container name          | media                       |
| Public access level     | Private (no anonymous access)|

All media files are stored in the `media` container. Anonymous public access
is disabled at both the account and container level. Files are never served
through a public URL directly.

## Access Policy

- Anonymous blob access is **disabled**. The container is private.
- The application authenticates to the storage account using the account
  **connection string**, stored in configuration under `BlobStorage:ConnectionString`.
- Clients never receive the connection string or account key. To let a client
  view or download a file, the API issues a short-lived **SAS (Shared Access
  Signature) URL** with read-only permission and a limited expiry. This gives
  secure, time-limited access without exposing credentials.
- Minimum TLS version is 1.2; secure transfer (HTTPS) is required.

## Secrets Handling

- The real connection string is stored **only** in `appsettings.Development.json`,
  which is excluded from source control via `.gitignore`.
- `appsettings.json` (committed) contains an empty `ConnectionString` placeholder
  so the configuration shape is documented without leaking the secret.
- For production, the connection string should be supplied via environment
  variables or Azure Key Vault rather than a file.

## Backup / Data Protection

The following data-protection features are enabled on the storage account:

- **Blob soft delete** — retention 7 days. Deleted blobs can be restored within
  this window.
- **Container soft delete** — retention 7 days. Deleted containers can be
  restored within this window.

Because the application performs **soft deletes** on `MediaAsset` records
(setting `IsDeleted = true`) rather than removing the blob immediately, the
database record and the underlying blob can both be recovered if needed.

For stronger durability, replication can be upgraded from LRS to GRS
(geo-redundant) later; this is a configuration change on the storage account
and does not require code changes.
