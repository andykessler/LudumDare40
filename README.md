# Ludum Dare 40 : _The more you have, the worse it is._

https://akessler.itch.io/ld40

Thanks for playing my first Ludum Dare game! I look forward to hearing your feedback.

*Full screen recommended*

# About

- You are the **`yellow capsule`**--you can carry a ball

- Opponents are the **`teal capsules`**--they can carry balls too

- Hunters are the **`purple capsules`**--they chase down and kill those who have a ball

- As you hold a ball longer the purple capsule will **`chase you faster`**

- **`Throw the ball`** to your opponents to try get them killed

- Everyone has 3 lives by default

- Last capsule standing wins!

# Controls

- **`Space`** bar to reset the game and cancel the active round (if any)

- **`Space`** again to begin a the round

- **`Right Mouse Button`** hold or click to move player

- **`Left Mouse Button`** click on opponent to throw the ball

# Configuration Changes

_**If you find any values that work particularly well please share them!**_

Control different properties of the game through the interactive UI sliders. This includes values like `maxSpeed`, `acceleration`, `lives`, etc. Currently some values require a restart of the game (through `Space` commmand), while others will reflect immiediately. Hopefully the UI is clear enough to show this distinction. 

# Bugs / Issues / Suggestions

Sometimes the purple capsules go on high velocity off the map, but don't worry they always seem to return :P

**If you see any bugs or have any suggestions**, please create a Git issue in this repo. If possible, try and determine conditions for the scenario to occur.

# Code

Towards the end of Ludum Dare 40, when submission deadlines started to approach, I started to lose quality in my overall game design and binding flow. I started noticing myself put in more hacks that I'll "resolve later"--sounds like a typical hackathon to me. Because of this there are a few places in my code that I want to revisit to remove coupling and non-optimal design patterns.

# Future

Following this update will come some of the more "nice-to-have" features that were thought of but never got time to implement. One of these includes a healthy dose of refactoring in order to prepare for network behavior scripts. The full list of these future features are listed on the project's Trello board. 
