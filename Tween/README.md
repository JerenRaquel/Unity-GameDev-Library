# How to Use
- [**Note**] A prefab has been provided with no changes necessary.
1. Create an empty gameobject and attach this script to it.
    - [**Note**] This script is a singleton, so there can only be one of it.
2. Create the travel args and set its parameters.
3. Request travel.

# Example
```csharp
public void MoveTowards(GameObject go, Vector3 goalPosition, float speed) {
  // Create the arguement instructions
  ObjectTravel.TravelDataArgs args = ObjectTravel.TravelDataArgs(
    go.transform,
    () => { Debug.log("Travel Complete!")},
    go.transform.position,
    goalPosition,
    speed,
    true // Request the travel to be done in parallel with other requests 
  );
  // Request the travel
  ObjectTravel.ObjectTravelHandler.instance.RequestTravel(args);
}
```