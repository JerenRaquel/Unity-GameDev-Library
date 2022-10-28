# Unity-GameDev-Library 2.0
Just some scripts, tools, and components to speed development up.

## Features
- Data Structures
    - Minheap
        - [**Note**] Can be set in arguements to be used as a Maxheap instead.
- [A*](https://github.com/JerenRaquel/Unity-GameDev-Library/blob/master/Astar/README.md)
    - [**Singleton**] AStar.cs
        - Main controller for handling global calculations and debugging settings.
    - Node.cs
        - Handles knowing if a certain collision tag is in it.
            - i.e. If the an enemy or player is standing on the node, alert the pathfinder to its state.
        - Automatically created by the pathfinder.
        - [**Note**] Do not attach this to anything, it should be automatic.
    - Pathfinder.cs
        - Handles node creation.
        - Calls the global A* script to find its path.
- Miscellaneous
    - ObjectTravelHandler.cs
        - Works as a tweening library for translations and rotations.
        - [**Bug**] Rotation inaccurate from 360 degrees <-> 0 degrees.
