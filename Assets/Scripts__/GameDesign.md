# Game Design Document

## Concept

A high-tension horror game where **progress is the invitation for death**.

---

## Target Emotions (Core Experiences)

### 1. Conflicting Urgency
A constant mental tug-of-war between the drive to *"finish the task as quickly as possible"* and the paralyzing fear that *"putting on the mask will bring the enemy closer."*

### 2. Deep Paranoia ("Is it actually moving?")
Even when it should be safe (mask off), the player begins to doubt their own eyes, feeling as though the mannequins have shifted positions slightly when they weren't looking.

### 3. Uncanny Intrusiveness
The repulsive "presence" of an inanimate, emotionless object standing right behind or beside you. An unbearable feeling of having one's personal space invaded by something non-human.

### 4. Absolute Helplessness
A sense of despair arising from having no weapons or means to fight back. The player's only agency is the binary choice to *"look"* or *"not look,"* emphasizing their inability to change the situation.

### 5. Profound Relief (Catharsis)
The overwhelming sense of security and "fresh air" when finally escaping the silent standoff and the pounding heartbeat of the encounter.

---

## Core Gameplay Systems

### Risk-Reward Mask System
- Players can only progress with clear-condition tasks **while wearing the mask**.
- However, wearing the mask **triggers the mannequins to move toward the player**, forcing them to weigh progress against immediate danger.

### Fixed-View Tasking
- While performing a task, the **camera is locked**, preventing the player from checking their surroundings.
- This heightens the urgency to finish quickly before something reaches them.

### Procedural Threat Assignment
- Each playthrough **randomly determines** which mannequins are "active" (mobile) and which are "static" (props).
- This ensures the player can **never trust any specific mannequin**.

### Auditory Stress (Hyperventilation)
- When the mask is equipped, the sound of the player's **heavy, panicked breathing** is amplified.
- This heightens the sense of claustrophobia and anxiety.

### Proximity-Based Sensory Feedback
- As a mannequin approaches:
  - **Creaking floorboards** increase in volume.
  - **Visual distortions** are applied to the screen.
- This reinforces the terrifying sensation that *"something is right behind me."*

### Uncanny Static Poses
- Active mannequins **freeze in unnatural, distorted poses** the moment they are observed.
- This visual "evidence" confirms to the player that *"this thing was definitely moving a second ago."*

---

## Proposed Task Design Specifications

### Core Principles for Tasks

| Principle | Description |
|-----------|-------------|
| **Intuitive Mastery** | Must be understandable at a glance without a tutorial. |
| **Anxiety-Driven Difficulty** | Should be effortless to complete when calm, but easy to fail when panicking. |
| **Temporal Friction** | Mechanics must require a fixed amount of time to complete, preventing players from rushing through instantly. |

---

## Proposed Task Candidates

### 1. Power Charge (Hold-to-Charge)
- **Mechanism:** Players hold a specific key to charge a battery or power generator.
- **Gameplay:** Simple "Hold" interaction. If the key is released (e.g., to check for mannequins), the charge may slowly deplete.

### 2. Data Download (Pop-up Management)
- **Mechanism:** Click a "DOWNLOAD" button to start. During the process, random "Error" pop-ups appear.
- **Gameplay:** The progress bar stops whenever an error appears. Players must click the pop-ups to clear them.

### 3. Manual Clockwork (Precision Rotation)
- **Mechanism:** Hold a button (or drag the mouse) to wind a clock hand to a specific target time.
- **Gameplay:** If the player winds too fast and overshoots the target, the hand must complete another full rotation to reset.
