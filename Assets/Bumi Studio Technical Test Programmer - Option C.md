# Bumi Studio — Game Programmer Technical Test 
**Duration:** 5–7 Days
**Engine:** Unity

---

## Table of Contents

1. [Overview & Purpose](#1-overview--purpose)
2. [General Rules & Submission Guidelines](#2-general-rules--submission-guidelines)
3. [Project Requirement and Overview](#3-project-requirement-and-overview)
4. [Frequently Asked Questions](#4-frequently-asked-questions)

---

## 1. Overview & Purpose

This technical test is designed to evaluate your **fundamentals, logic, code architecture, and planning ability** as a game programmer. It is scoped to be completable within **5–7 days**.

We are not looking for a polished, shippable product. We want to see **how you think, how you plan, and how you build**. The game idea, its mechanics, and its structure are yours to define — within the genre scope of the option you choose.

The three options below all fall within the **2D PC game** space, each with a different emphasis. Choose the one that best matches your strengths and interests.

> **Note:** We have our own games in production. The purpose of these options is to evaluate your skills — not to gather free ideas. None of your work will be used by the company.

---

## 2. General Rules & Submission Guidelines

### Engine
- Only Use Unity Engine with editor version 6000.0.75f1
- Document your engine and language choice in your README.
- Do **not** use paid third-party plugins or assets that the reviewer cannot freely access.
- Free assets (Unity Asset Store free tier, Kenney.nl, itch.io free assets, etc.) are allowed for **visuals and audio only** — all gameplay code must be your own.
- Do **not** use high-level gameplay frameworks that implement core systems for you (e.g., a pre-built FSM library, a pre-built card game framework). If you use a tool, understand it and be prepared to explain it.

### Build Output
- All three options target **PC**. Submit a Windows `.exe` build.

### Submission
- Submit your project as a **Git repository link** (GitHub, GitLab, or Bitbucket — set to public or share access with the reviewer).
- Your repository **must include**:
  - Full project source code.
  - A `README.md` at the root — use the template below as your structure.
  - The build file (`.exe`) in a `/Build` folder, or linked via Google Drive / itch.io.
- **Commit history matters.** Do not squash all work into a single commit. We want to see your progression and how you approached the work over time.

### README Template

Your `README.md` must follow this structure. Fill in each section honestly — the README is evaluated as part of your submission, not as a formality.

```markdown
# [Your Game Title]

## Game Overview
Describe your game in a few sentences. What is the genre? What is the core mechanic?
What does the player do, and what is the win/lose condition?
Write this as if explaining to someone who has never seen your game.

## How to Run
Step-by-step instructions to run your build.

- Engine & version used:
- Build location: /Build/ or [link]

## Technical Decisions
Explain the significant technical choices you made and why.
Examples: how you structured game state, what architecture patterns you applied,
why you made specific system design decisions, what you deliberately chose NOT to do.

## What I Would Do With More Time
Be specific. List the features, systems, or fixes you would prioritize
if you had an extra 2–3 days. This tells us how you think even for unfinished work.

## Known Issues
List any bugs or incomplete features you are aware of.
An honest list here is valued over leaving the reviewer to discover them silently.
```

### Scope Discipline
You have 5–7 days. **Timebox deliberately.** A working, well-structured prototype that does one thing well is worth far more than an ambitious scope that is broken or incomplete. If you run out of time, document what is missing and describe how you would have approached it.

---

## 3. Project Requirement and Overview

Read the full brief before deciding.

This section will explain how you will work on the project, a **minimum deliverable**, and optional **stretch goals**. The game concept, mechanics, visual style, and feature set within that genre are entirely **your decision**. We want to see how you plan and what you choose to prioritize.

---

## 2D Roguelike with Resource Management

**Platform:** PC
**Build Output:** Windows `.exe`
**Genre Scope:** A 2D roguelike where the player must manage one or more resources across a run — not just HP, but meaningful secondary resources (e.g., food, morale, fuel, population, sanity) that constrain decisions and create tension beyond direct combat or action. Each run must be procedurally varied in some way.

### What We're Looking For
Design and build a **2D roguelike with resource management prototype** of your own concept. The game must have a run-based structure, at least one form of procedural variation per run (random events, procedural maps, shuffled encounter order, etc.), and at least **two tracked resources** that the player must balance — where letting either drop to zero (or below a threshold) is a meaningful threat.

You decide the theme, the core action loop, and what the resources represent. The focus of this option is on **resource system design and state management** — how resources are tracked and updated across a run, how the player reads their current state at a glance, how decisions create resource tension, and how the game handles the end of a run cleanly (win or loss) and resets for a new one.

### Minimum Deliverable
A playable prototype where the player progresses through at least **four procedurally varied stages or events** per run, actively managing at least two resources throughout. The run must have clear win and loss conditions tied to resource or progression state, and the player must be able to start a new run after finishing.

### Stretch Goals *(optional)*
- Random events or encounters that present the player with a choice affecting resources.
- A meta-progression layer that carries something small across runs (e.g., an unlocked starting bonus).
- A log or journal that records what happened during the last run.
- Visual feedback that communicates resource urgency (e.g., a resource approaching zero triggers a warning state).

---

## 4. Frequently Asked Questions

**Q: Can I use any engine I want?**
No, you can only use Unity engine in this test and make sure you use Unity Editor version 6000.0.75f1

**Q: The genre scope is broad — how much creative freedom do I actually have?**
A lot. The genre descriptions are intentionally wide. If your idea fits within the spirit of the option and you can make a case for it in your README, go ahead. When genuinely in doubt, contact your recruiting point of contact and ask.

**Q: Can I use third-party assets?**
Free visual and audio assets are fine. All gameplay code must be your own. Do not use third-party frameworks that replace the systems we are evaluating.

**Q: What if my engine has a built-in feature that covers a rubric point (e.g., Unity's built-in event system)?**
Using engine-provided tools is fine, but understand what you are using and be prepared to discuss it. The rubric rewards understanding, not reinvention.

**Q: What if I can't finish everything in 5–7 days?**
Document what is incomplete in your README and explain how you would have approached the missing parts. An honest, well-scoped, partially complete submission is better than a bloated, broken one.

**Q: Is there a language requirement for the README?**
English is preferred. Bahasa Indonesia is also acceptable.

---

*Good luck. We look forward to seeing how you think.*

**— Bumi Studio Lead Programmers**
