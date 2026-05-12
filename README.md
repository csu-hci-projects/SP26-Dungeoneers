# Dungeoneers: Investigating Cybersickness Acclimatization in Virtual Reality

Dungeoneers is a Unity-based virtual reality research platform developed to investigate whether controlled exposure to visually intense environments can accelerate acclimatization to cybersickness.

Built for the Meta Quest 2, the system is able to guide participants through a series of tunnel environments with progressively different levels of visual intensity. Before beginning the experimental trials, participants completed a pre-study survey designed to assess their prior virtual reality experience and baseline susceptibility to motion sickness. After each trial, participants complete questionnaires based on the Virtual Reality Sickness Questionnaire (VRSQ) and provide open-ended reflections that are analyzed using reflexive thematic analysis.

This project was developed as the final project for **CS 465: Multimodal Interaction for 3D Interfaces** at **Colorado State University (Spring 2026)**.

---

## Research Question

**Does prior exposure to a high-intensity virtual reality environment accelerate acclimatization to lower-intensity VR environments?**

---

## Key Findings

* The **purple/yellow** condition consistently produced the strongest reports of vertigo, eye strain, and disorientation.
* The **blue/pink** condition was generally perceived as the most comfortable and easiest to navigate.
* Several participants reported becoming more comfortable over the course of the study, suggesting short-term acclimatization.
* Participants with prior VR experience tended to report fewer symptoms than first-time users.
* Some participants experienced lingering symptoms, such as stomach discomfort, after removing the headset.

---

## Repository Structure

* `Cybersickness/` — Main Unity project used in the final experiment.
* `hw1/` — Earlier prototype and homework assets.
* `docs/` — Figures, screenshots, final paper, and presentation materials.
* `Latex Source/` — ACM LaTeX source files for the final report.
* `PDFs-LiteratureSurvey/` — Research papers used in the literature review.
* `Videos/` — Links to all project videos.
* `REPRODUCIBILITY.md` — Detailed instructions for rebuilding and running the project.

---

## Technology Stack

* Unity 6 (`6000.3.10f1`)
* Meta Quest 2
* Meta XR SDK (`85.0.0`)
* Meta Voice SDK / Wit.ai
* Universal Render Pipeline (URP)
* C#
* ACM LaTeX (`acmart` template)

---

## Reproducibility Baseline

Use the exact versions below when possible:

* Unity Editor version: `6000.3.10f1`

	* Source: `Cybersickness/ProjectSettings/ProjectVersion.txt`
* Unity package dependencies:

	* Source: `Cybersickness/Packages/manifest.json`

Key packages include:

* `com.meta.xr.sdk.all` `85.0.0`
* `com.unity.xr.openxr` `1.16.1`
* `com.unity.inputsystem` `1.18.0`
* `com.unity.render-pipelines.universal` `17.3.0`

---

## Bootstrap / First-Time Setup

1. Install Unity Hub.
2. Install Unity Editor `6000.3.10f1`.
3. Open the `Cybersickness/` folder in Unity Hub.
4. Allow all packages to restore.
5. Confirm that there are no package or compilation errors.

---

## Meta Quest 2 Setup

1. Enable Developer Mode on the headset.
2. Connect the headset to your computer using a USB-C cable.
3. Approve the USB debugging prompt inside the headset.
4. In Unity, switch the build platform to Android.
5. Ensure OpenXR and Meta XR packages are enabled.
6. Build and run to the headset.

---

## Running the Experiment

1. Open the main scene (for example `scene1.unity`) in `Cybersickness/Assets/`.
2. Set the participant number in the Unity Inspector.
3. Press Play in the Unity Editor or deploy to the Quest 2.
4. Complete all four tunnel trials.
5. Review the generated CSV files and questionnaire responses.

---

## Experimental Conditions

1. **Control** — Static gray-to-white gradient.
2. **Low Intensity** — Blue and pink moving zebra pattern.
3. **Medium Intensity** — Black and white moving zebra pattern.
4. **High Intensity** — Purple and yellow moving zebra pattern.

---

## Data Collection

### Software Logging

The system records:

* Participant UUID
* Trial label
* Time to first `unlock` command
* Time to reach 180° of dial rotation
* Time to successful `lock` command
* Total trial duration
* Number of `unlock` attempts
* Number of `lock` attempts

### Surveys and Interviews

After each condition, participants complete:

* Virtual Reality Sickness Questionnaire (VRSQ)
* Open-ended qualitative prompts

The VRSQ was developed by Kim et al.

After all conditions, participants complete:

* A semi-structured interview focused on discomfort, adaptation, and comparative impressions

---

## Final Report

* `docs/Dungeoneers_Final_Report.pdf`

---

## Videos

### 1. Short Overview Video (3–5 minutes)
- View: https://www.youtube.com/watch?v=uIwQT-Cz3M8
- Download: https://drive.google.com/file/d/1mj8r3c2cY-k5nqMNm8poR3KapDULK95y/view?usp=sharing

### 2. Presentation Video (10–12 minutes)
- View: https://www.youtube.com/watch?v=XCb253zrlcI
- Download: https://drive.google.com/file/d/1E7lhSDX3MDurDScQVuajEH0anOTTs2y5/view?usp=drive_link)

### 3. Programming Video
- View:  https://youtu.be/hzZ44u0Qabo
- Download: https://colostate-my.sharepoint.com/:v:/g/personal/c837205363_colostate_edu/IQAwGDrBKL4oToU4wZXfqcRIASu7No8zx-YPQKJ4aGp28jY?nav=eyJyZWZlcnJhbEluZm8iOnsicmVmZXJyYWxBcHAiOiJPbmVEcml2ZUZvckJ1c2luZXNzIiwicmVmZXJyYWxBcHBQbGF0Zm9ybSI6IldlYiIsInJlZmVycmFsTW9kZSI6InZpZXciLCJyZWZlcnJhbFZpZXciOiJNeUZpbGVzTGlua0NvcHkifX0&e=xBLInJ

---

## GitHub Repository

[SP26-Dungeoneers Repository](https://github.com/csu-hci-projects/SP26-Dungeoneers?utm_source=chatgpt.com)

---

## Team Contributions

### Angel Gonzalez Portillo

* Unity development and core C# implementation
* XR integration using the Meta XR SDK
* Voice recognition integration with Wit.ai
* Scene construction and shader implementation
* Experimental testing and debugging
* Proctored and documented **Trials 5 and 6**

### Joshua Masih

* Research design and study framing
* ACM paper development and LaTeX formatting
* Participant demographics and methodological documentation
* GitHub organization, README development, and submission packaging
* Presentation slide development
* Proctored and documented **Trials 3 and 4**

### John Robert Harrison Matthews

* Prototype concept and visual design direction
* Early prototype development and task design
* Thematic analysis and interpretation of qualitative findings
* Experimental testing and usability feedback
* Presentation support and project planning
* Proctored and documented **Trials 1 and 2**

### Shared Contributions

All team members contributed to:

* Study design refinement
* Participant recruitment and scheduling
* System testing and troubleshooting
* Presentation preparation
* Final review of the report and submission materials

---

## Course Information

**CS 465 – Multimodal Interaction for 3D Interfaces**
Colorado State University
Spring 2026

---

## Additional Reproducibility Information

For detailed instructions on rebuilding and running the project, see `REPRODUCIBILITY.md`.
