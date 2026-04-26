# SP26-Dungeoneers (Dungeoneers VR Study)

This repository contains Unity projects for the Dungeoneers VR cybersickness/acclimatization study.

## Repository layout

- `Cybersickness/` — primary project for the Dungeoneers environment.
- `hw1/` — earlier prototype/homework project assets and scripts.

## Reproducibility baseline

Use the exact versions below when possible:

- Unity Editor version: `6000.3.10f1`
	- Source: [Cybersickness/ProjectSettings/ProjectVersion.txt](Cybersickness/ProjectSettings/ProjectVersion.txt)
- Unity package set / dependencies:
	- Source: [Cybersickness/Packages/manifest.json](Cybersickness/Packages/manifest.json)

Key XR packages currently configured in the project include:

- `com.meta.xr.sdk.all` `85.0.0`
- `com.unity.xr.openxr` `1.16.1`
- `com.unity.inputsystem` `1.18.0`
- `com.unity.render-pipelines.universal` `17.3.0`

## Bootstrap / first-time setup

1. Install Unity Hub.
2. Install Unity Editor `6000.3.10f1`.
3. In Unity Hub, open the `Cybersickness` folder as a Unity project.
4. Allow package restore/import to complete.
5. Confirm no package errors in Console.

## Meta Quest setup (Quest 2)

1. Put headset in Developer Mode.
2. Connect headset via USB-C and allow debugging prompt in-headset.
3. In Unity, switch platform to Android.
4. Ensure OpenXR + Meta XR packages remain enabled (from `manifest.json`).
5. Build and run to device.

## Running in Editor

1. Open scene assets under `Cybersickness/Assets` (for example `scene1.unity`).
2. Press Play.
3. Verify XR rig and interactions initialize without missing script warnings.

## Data/output note

Some experiment scripts write CSV output under `Assets/` (for example `Dungeoneers_Outputfile.csv` in the hw1 project). For reproducible runs, document participant/session metadata with each exported CSV.

## Additional reproducibility checklist

See [REPRODUCIBILITY.md](REPRODUCIBILITY.md).