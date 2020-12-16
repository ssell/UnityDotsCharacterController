# Unity DOTS Character Controller Example

![](Media/controller.gif)

This repository contains the source code for a basic kinematic character controller implemented using Unity DOTS (Data-Orientated Tech Stack) with the new Entities (ECS) and Physics packages.

There is an accompanying article, 

* [**Unity DOTS Character Controller**](https://www.vertexfragment.com/ramblings/unity-dots-character-controller)

which steps through how the controller was created. 

It should be noted that this project is for demonstration purposes only, and there is no guarantees of its correctness. This project _may_ see occasional future updates to support new Unity or package versions and/or bug fixes.

The code was written against:

* Unity v2020.2.0f1
* [Entities v0.16.0-preview.21](https://docs.unity3d.com/Packages/com.unity.entities@0.16/manual/index.html)
* [Physics v0.5.1-preview.2](https://docs.unity3d.com/Packages/com.unity.physics@0.5/manual/index.html)
* [Burst v1.4.3](https://docs.unity3d.com/Packages/com.unity.burst@1.4/manual/index.html)

## Web Demo

Unfortunately at this time Burst and WebGL do not get along. ([source](https://forum.unity.com/threads/burst-for-standalone-players.539820/page-5#post-5364648))

## Controls

The demo scene uses the following controls:

| Action               | Keys                  |
| -------------------- | --------------------- |
| Movement             | WASD, Arrow Keys      |
| Increase Speed       | Shift                 | 
| Jumping              | Space                 |
| Camera Zoom          | Scroll Wheel          |
| Camera Pan and Pitch | Middle Mouse + Move   |

## Contact

Any questions or comments may be directed to the contact information [found here](https://www.vertexfragment.com/about/).

<br/>
<br/>
<br/>

---

_Life Before Death, Strength Before Weakness, Journey Before Destination_