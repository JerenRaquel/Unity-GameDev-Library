# How to Use
1. Create an empty object and attach AStar.cs to it.
    - There can only be one of these objects at a time.
    - This object is a singleton which can be called from anywhere even across active scenes.
    - [**Optional**] Set the debugging settings here.
        - These are global.
2. Create an empty object and attach Pathfinding.cs to it.
    - Create as many as you want for different "areas".
        - These areas are seperate maps for A* to navigate.
    - Set the settings for this.
        - This is how far you want A* for this seperate map to search through.
        - The nodes for the map will auto generate on start up.
    - [**Debug**] Enable debug line to show how the pathfinder will move.
3. Create and assign some tag for the starting node and ending nodes.
    - These can be called whatever, just make sure you assign them with colliders so A* can notice it.
4. [**Optional**] Create a tag and assign it for obstacles.
5. [**Recommended**] Create an empty object and attach ObjectTravelHandler.cs to it.
    - This will allow for object movement by lerping at some speed between the two distances.
    - This should provide an easy way for moving objects by the pathfinder to some location.