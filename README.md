# Unity Game Development Library 2.0
Just some scripts, tools, and components to speed development up. The 2.0 version is updated and more efficent version of the previous version. The 1.0 version is deprecated and is under its own branch if needed.

# Known Working Unity Versions
- 2021.3.11f1 LTS

# Features
Every feature in its own folder should provide all that is needed in able to work right out of the box.

## Data Structures
Types of data structures that C# does not already provide.
- Minheap
    - [**Note**] Can be set in arguements to be used as a Maxheap instead.

## [Pathfinding: A*](https://github.com/JerenRaquel/Unity-GameDev-Library/blob/master/Astar/README.md) [**Prefabs Provided**]
Your standard A* pathfinder using a grid based method. This pack provides the means to establish a grid of probes that will update their states based on what is within its space, and alert the corrisponding map for A*. A* then can be called on any map to generate the path if there is one.
- [**Note**] Grid based A*.
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

## [Random Weighted Generator](https://github.com/JerenRaquel/Unity-GameDev-Library/blob/master/RWG/README.md)
A simple random generator that supports weighted values. This uses a string ballot method where the values are stringed together and evaluated by threshold.
- RandomWeightedGenerator.cs
    - Handles generating the value and returns a string of the value.
- WeightedValues.cs
    - [**Note**] Do not attach this to anything.
## [TextSystem](https://github.com/JerenRaquel/Unity-GameDev-Library/blob/master/Text%20Scroller/README.md)
This is a pack that provides auto character by character display of text. Can be used for speech bubbles, dialogue boxes, or for cutscene text.
- [**Note**] Requires TextMeshPro.
- [**Note**] Make sure your textbox is big enough to fit your entire text in.
- FontData.cs
    - Establish how you want text rendered.
- TextData.cs
    - Apply your FontData to parts of text.
    - The combined result of these parts will be the entire text displayed in the TextMeshPro text box.
- TextParser.cs
    - Helper for text system.
- TextScroller.cs
    - Controller for creating the scrolling text.
    - Allows for timed character by character display according to settings in TextData.
    - Allows for skipping the timed delay and displaying the entire text within the text box.

## [Tween](https://github.com/JerenRaquel/Unity-GameDev-Library/blob/master/Tween/README.md) [**Prefabs Provided**]
This is a file that provides basic tweening of objects needed to be moved from point A to B or rotated from X to Y.
- [**Singleton**] ObjectTravelHandler.cs
    - Lerp and Slerp are the methods for tweening.
    - Works as a tweening library for translations and rotations.
    - [**Bug**] Rotation inaccurate from 360 degrees <-> 0 degrees.

# How to use in your Unity project?
Any provided prefabs are the basics of what is required to be used out of the box. Use at your own discretion.
1. Clone this repo or download the zip file.
2. Unzip the file.
3. Drag anywhere within your assets folder in your Unity project.
4. Wait for Unity to compile.

# FAQ
- Can I use this library or some of the scripts?
    - Sure, this is using the MIT license after all.
- How often will you update this?
    - I honestly can't tell you. I will update this when I get to it and feel like it. This goes the same for pull requests. Assuming they are acceptable, I get to them once I do.
- Will I add any new features, prefabs, or scripts?
    - This is similar to the "how often will I update this" type question. I will get to it when I have the time and motivation. However, new features will role out when I feel like there is some script or prefab I find I do a lot or may need in the future.
- What does Singleton mean?
    - It's the same as the definition for the Singleton Design Pattern. 
    - However, as for how this works in Unity:
        1. Create an empty game object in any active scene.
        2. Attach the singleton script to the object.
        3. Now you can call this script from any active scene.
    - [**Note**] This script can only exist once and can be used acrossed active scenes. Also, this is using the non-lazy version of singletons (called on OnAwake).
