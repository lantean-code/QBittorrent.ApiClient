# Changelog

## 1.3.0

- Added qBittorrent 5.3.0 support, validated against the final `release-5.3.0` source tag and Web API `2.16.2`.
- Added path free-space queries, RSS rule export/import and cloning, batch speed limits, session pause/resume, and individual torrent file downloads.
- Added category share limits and download-path inheritance, torrent share-limit modes, and torrent creation dotfile options.
- Added application preferences with feature-specific compatibility checks and automatic translation of renamed backup and SMTP settings.
- Extended response models for search job details, session and transfer statistics, peer contribution, metadata file priorities, and torrent creation timestamps.
- Added documentation guides and updated dependencies.
- Moved repository tests to Microsoft.Testing.Platform with native Coverlet integration.

## 1.0.0

- First release candidate of `QBittorrent.ApiClient`.
- Added a result-based .NET client for the qBittorrent Web API.
- Added structured failures and version-aware compatibility handling for supported qBittorrent releases.
- Added standalone NuGet packaging plus independent CI and release automation.
