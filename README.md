# CISC486
https://youtu.be/I1UwzwejKOI

https://youtu.be/I1UwzwejKOI


Cat AI FSM
![alt text](Cat-FSM.png)

This finite state machine (FSM) models the behavior of a cat interacting with
a mouse. The system consists of four states: Patrolling, Seeking, Chase, and
Attack. The transitions between these states depend on visibility, distance, and
timers.
1. Patrolling
Behavior: The cat follows its patrol route.
Transitions:
• If the mouse is noticed at the edge of vision → Seeking.
• If the mouse is within attack range → Attack.
1
2. Seeking
Behavior: The cat actively searches for the mouse.
Transitions:
• If the mouse is found but not in range → Chase.
• If the mouse is within attack range → Attack.
• If no mouse is found for 10 seconds → Patrolling.
3. Chase
Behavior: The cat aggressively pursues the mouse.
Transitions:
• If the mouse is within attack range → Attack.
• If the mouse escapes line of sight for 5 seconds → Seeking (with a 10-
second timer).
4. Attack
Behavior: The cat attacks the mouse, dealing 50 damage (half of the mouse’s
total health) while it is in range.
Transitions:
• If the mouse moves out of attack range → Chase (if visible) or Seeking
(if lost).
