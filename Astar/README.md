# How to Use
- [**Note**] Prefabs have been provided. Make sure your objects' tags are set to the correct ones and have rigidbody2Ds with their desired collider2Ds.
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

# Example
```csharp
using UnityEngine;

public class AIController : MonoBehaviour
{
    public AStar.Pathfinder pathfinder;
    public Transform enemy;
    [Header("Settings")]
    public bool AIState = false;
    public float speed;

    private bool isMoving = false;

    private void Update() {
        if(AIState) {   // Check if the AI is enabled.
            if(!isMoving){  // Make sure the AI isn't already moving
                Vector3? nextPosition = pathfinder.GetNextPosition();
                // Return if no path was found.
                if(!nextPosition.HasValue) return;  
                isMoving = true;
                MoveTowards(nextPosition.Value);
            }
        }
    }

    private void MoveTowards(Vector3 position) {
        // Create travel args for the enemy.
        ObjectTravel.TravelDataArgs args = new ObjectTravel.TravelDataArgs(
            enemy, 
            // Set the movement flag to get the next position
            () => { isMoving = false; },    
            enemy.position,
            position,
            speed
        );
        // Request the movement
        ObjectTravel.ObjectTravelHandler.instance.RequestTravel(args);
    }
}
```