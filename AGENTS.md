# AGENTS.md — Notes for AI agents (gregMod.NoCostShop)

Repo: https://github.com/mleem97/gregMod.NoCostShop · License: Apache-2.0 · Version: see `VERSION` (1.0.2).

MelonMod for Data Center. Makes shop items free (derived from an MIT-licensed
original — see `src/ORIGINAL_LICENSE_MIT.txt`, keep that file intact).

## Duties

1. **Read first:** `README.md`, `docs/INDEX.md`, `docs/ARCHITECTURE.md` — only then make changes.
2. **Do not commit secrets** (keys, tokens, `.env`). Use keys only via environment variables.
3. **Preserve history:** no `push --force`, no history rewrite without instruction.
4. **Verify changes:** before reporting done, build the mod (`dotnet build gregMod.NoCostShop.csproj -c Release` or `./build.sh NoCostShop` from `ModRepositories/`).
5. **Keep docs in sync:** for new features update `README.md` + `docs/` + `CHANGELOG.md` (Unreleased).
6. **Conventions:** Conventional Commits (`feat:`, `fix:`, `docs:`, `chore:` …), one logical change per commit.
7. **When unsure:** stop and ask instead of guessing — especially for deletes, migrations, CI.

## Build and references

- Target: `net6.0`, x64. Game: Data Center (`MelonGame("Waseku", "Data Center")`).
- `references/` holds absolute symlinks into the Steam Data Center install.
  Never commit `references/*.dll`, `bin/`, or `obj/`.
- After a fresh clone, run `../tools/sync-melon-assemblies.sh`.
- Deploy only with `./build.sh NoCostShop --deploy`.

## Hard rules

- Price overrides go through the game's own shop methods so HUD and save stay
  consistent — never write currency fields directly.
- Keep `src/ORIGINAL_LICENSE_MIT.txt` and its attribution intact.
- **Never** touch gregCore types outside a soft-probe/JIT-split bridge — the
  mod must load without `gregCore.dll`.

## Layout

- `src/Core.cs` — MelonMod entry. `src/ItemData.cs`, `src/Enums/` — shop data.
- `src/Options/` — prefs.
