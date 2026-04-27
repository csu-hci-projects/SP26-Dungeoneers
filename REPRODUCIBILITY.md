# Reproducibility Checklist — Dungeoneers

This checklist is for consistent project setup, execution, and reporting across team members.

## 1) Pin the software baseline

- Unity Editor: `6000.3.10f1`
  - Reference: [Cybersickness/ProjectSettings/ProjectVersion.txt](Cybersickness/ProjectSettings/ProjectVersion.txt)
- Package dependency baseline:
  - Reference: [Cybersickness/Packages/manifest.json](Cybersickness/Packages/manifest.json)

Recommended verification before running experiments:

- Editor version matches exactly.
- `manifest.json` has no unreviewed changes.
- Project imports with no package resolution errors.

## 2) Hardware/runtime baseline

- Device: Meta Quest 2
- Runtime mode: Android build target
- Controllers: standard Quest controllers

Before each session:

- Headset charged.
- Developer mode enabled.
- USB debugging authorized.
- Same physical play-space lighting and boundary conditions if possible.

## 3) Build + run consistency

- Open the `Cybersickness` project from Unity Hub.
- Confirm Android target before device deployment.
- Run the same scene(s) for all participants in a cohort.
- Record build date/time and commit hash used.

## 4) Data collection consistency

For every participant/session, log:

- Participant ID (anonymized)
- Session timestamp
- Condition order (counterbalancing order)
- Build commit hash
- Device used
- Output CSV filename/path

## 5) Change control

Before merging experiment-impacting changes:

- Open a PR with a clear “experimental impact” note.
- Include before/after behavior summary.
- Include affected scenes/scripts.
- Re-run a short pilot smoke test.

## 6) Minimum report metadata

Each report/paper draft should include:

- Unity editor version
- Package baseline reference
- Headset/device model
- Trial protocol version
- Data export format version
