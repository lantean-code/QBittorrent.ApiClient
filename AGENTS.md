# AGENTS.md (root)

> Scope: This file gives high-level context and guardrails for the repository root. Deeper folders may add their own `AGENTS.md` files which take precedence for their subtrees (for example `./src/AGENTS.md` and `./test/AGENTS.md`).

## Project overview
- `QBittorrent.ApiClient` is a standalone .NET client library for the qBittorrent Web API.
- Primary goals: faithful qBittorrent semantics, reliability, strong test coverage, and NuGet-ready packaging.
- Non-goals: app-specific UI behavior, qBittorrent WebUI replacement features, or changes to qBittorrent semantics without explicit approval.

## Repository layout
- Solution: `QBittorrent.ApiClient.slnx`
- Projects:
  - `QBittorrent.ApiClient` — the package source.
  - `QBittorrent.ApiClient.Test` — unit tests for the package.
- Config/conventions: `.editorconfig`, `.gitignore`, `global.json`, `GitVersion.yml`.

## Build, test, publish
- Prerequisites: .NET SDK version pinned by `global.json`.
  - Agents must verify the pinned SDK is available in the current environment before running restore/build/test commands.
  - Agents must include `--artifacts-path=/tmp/artifacts/qbittorrent-apiclient` on all `dotnet` commands.
- Restore & build:
  - `dotnet restore --artifacts-path=/tmp/artifacts/qbittorrent-apiclient`
  - `dotnet build --artifacts-path=/tmp/artifacts/qbittorrent-apiclient`
- Run tests:
  - `dotnet test --artifacts-path=/tmp/artifacts/qbittorrent-apiclient`
- Create packages:
  - `dotnet pack --artifacts-path=/tmp/artifacts/qbittorrent-apiclient`
- After each behavior-affecting set of changes:
  - Run `dotnet test --artifacts-path=/tmp/artifacts/qbittorrent-apiclient`.
  - Behavior-affecting includes edits to production code, test code, project/package/build configuration, or other runtime-impacting assets.
  - Docs-only/report-only/markdown-only edits do not require restore/build/test unless explicitly requested.

## Coding and test standards
- Source code rules and generation constraints live in `./src/AGENTS.md`.
- Unit test rules live in `./test/AGENTS.md`.
- If rules conflict, the deeper file wins; otherwise, follow both.

## Frequently violated rules
- Non-public constants must use `_camelCase`; only public constants use PascalCase.
- If you add a constant and the name starts looking like `PascalCase`, stop and check whether it is actually public.

## Line endings
- Use CRLF line terminators for any files you write or modify.

## Git permissions
- Agents must not perform git write operations unless the user gives explicit permission in the current conversation.
- Git write operations include (but are not limited to): `commit`, `push`, `pull`, `merge`, `rebase`, `cherry-pick`, `reset`, `revert`, `checkout`/`switch` that changes branch or files, tag creation/deletion, and branch creation/deletion.
- Until explicit permission is granted, only read-only git commands are allowed.

## How to work in this repo
1. Read this file, then the relevant folder `AGENTS.md`.
2. When validating behavior against qBittorrent source:
   - Ensure qBittorrent is checked out under `./ref/qBittorrent`.
   - Check out a tagged release, not a branch.
   - Prefer the most recent relevant release tag for the compatibility behavior being validated.
3. Before modifying code:
   - Confirm SDK target, nullable context, analyzers, and editorconfig rules.
   - Keep the public package surface consistent unless the user explicitly requests a breaking change.
   - Keep qBittorrent Web API request and response semantics aligned with upstream behavior.
   - Treat `ApiResult` success as a completed, usable operation outcome, not merely receipt of an HTTP success status. If qBittorrent reports an accepted, pending, queued, or otherwise incomplete state that still requires polling or retry to obtain the real result, model that as a failed `ApiResult` with an explicit failure kind rather than a successful value wrapper.
4. When generating code:
   - Follow `./src/AGENTS.md` exactly.
   - Prefer minimal, maintainable changes and avoid churn to unrelated files.
   - After modifying source, test, project, or solution files, format the changed files to match `.editorconfig` before finishing. Prefer the smallest formatting scope that covers the edited files.
5. When writing tests:
   - Follow `./test/AGENTS.md` exactly.
6. Before opening a PR or preparing a release:
   - Build succeeds and tests are green.
   - Public XML docs are added or updated where required.
   - Package metadata, README, release notes, and changelog are consistent with the change.

## PR and review checklist
- [ ] Change is scoped and justified; no unrelated edits.
- [ ] Code adheres to `./src/AGENTS.md` standards.
- [ ] Tests adhere to `./test/AGENTS.md` and preserve full coverage expectations.
- [ ] No secrets, tokens, or user-specific paths are committed.
- [ ] `dotnet restore`, `build`, `test`, and `pack` succeed with the pinned SDK.
- [ ] Error messages and logs are clear and actionable.

## Communication & assumptions
- Do not guess. If any requirement, API contract, or behavior is unclear, ask for clarification.
- Prefer concise diffs and explicit rationale in commit messages and PR descriptions.
