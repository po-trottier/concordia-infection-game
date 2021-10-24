# Infection

The goal of the game is to give masks, vaccinate, clean surfaces and disperse crowds as much as possible without allowing any infected person to leave the shopping center. You have 3 lives and every time an infected person leaves the building you lose a life. The rest of the rules are as detailed in "Intructions.pdf".

## Code Structures

Any script that is independent of another module is located in the GameObject which it modifies. When a script requires another component of GameObject to work they are almost always linked using SerializeFields.

The main exception to that would be the NPC scripts. Since NPCs are instantiated at runtime it is not possible to use the Unity Inspector to link components from the scene. The scripts therefore look for the components they need and assume there is only 1 instance of the component in the scene (Example: ScoreManager).

When scripts are timing-sensitive, UnityEvents and Listeners are used. Such examples can most often be found in the UI scripts since they usually depend on User Input or on timers.

Timers or countdowns are mostly all implemented using Coroutines. Such coroutines are always private and require access to the OnGamePaused event so that the coroutines can be stopped when the game is paused. This

When parts of the code are less obvious, comments are usually added to explain what is going on. Otherwise, the method names and variable names should describe the behavior of a scripts.

## Compiling & Running

Compiling and running the code should be as simple as opening the project in Unity 2021.1.22f1 or newer and clicking the play button. If you wish to play a built version of the game, you can simply press the `CTRL+B` shortcut and the Editor will build the game and start the built project.

## Keybinds

All the controls for the game are located on the keyboard. The default keybinds are as follows:

- **Pause**: Escape
- **Move**: WASD
- **Jump**: Space
- **Give Mask**: Q
- **Vaccinate**: E
- **Disperse Crowd**: R
- **Clean Surface**: F

Special Mode keybinds:

- **Slow Down Time *(Special)***: C

## Attributions

- **A\* Pathfinding Algorithm** was used for the NPC pathfinding logic.
- **Google Images** were used as assets for the Storefront sprites.
- **mixkit.co** was used to download Sound Effects.
- **freesound.org** was used to download Sound Effects.

*Every other visual asset was produced by me.*