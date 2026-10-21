# Documentation site

The site uses MkDocs Material with the same colours, theme controls, search, and GitHub Pages deployment pattern as qbtmud.

## Build and preview

Use Python 3.12. From the repository root:

```bash
python -m venv .venv-docs
```

Activate the environment:

```bash
# Linux / macOS
source .venv-docs/bin/activate
```

```powershell
# Windows PowerShell
.\.venv-docs\Scripts\Activate.ps1
```

Install the pinned dependencies and build:

```bash
python -m pip install -r docs/docs-requirements.txt
python -m mkdocs build --strict --config-file docs/mkdocs.yml
```

Preview the site:

```bash
python -m mkdocs serve --config-file docs/mkdocs.yml
```

Open `http://127.0.0.1:8000/QBittorrent.ApiClient/`. Build output goes to the ignored `site/` directory.

## GitHub Pages

In the repository's **Settings > Pages**, select **GitHub Actions** as the build and deployment source. The `docs-pages.yml` workflow validates documentation pull requests targeting `develop` and deploys documentation changes pushed to `develop`. It can also be run manually from `develop`.

The published address is `https://lantean-code.github.io/QBittorrent.ApiClient/`. Deployment requires the workflow to be pushed and GitHub Pages to be enabled; a local build does not publish anything.

## Editing

- Put public guides in `docs/content/` and add them to `nav` in `docs/mkdocs.yml`.
- Keep examples aligned with the current public client surface.
- Describe development-branch behaviour explicitly when it differs from a released package.
- Link to the existing release notes and GitHub releases rather than maintaining duplicate release histories.
- Run the strict build before submitting changes.

API reference generation is deferred. There is no reference generator or placeholder reference section in this setup.
