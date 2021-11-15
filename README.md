# BoundingVolumeHierarchy

![BoundingVolumeHierarchy](https://i.imgur.com/y53Hc8h.png)

This is a port of [Erin Catto/box2d's b2_dynamic_tree from C++ to C#](https://github.com/erincatto/box2d/blob/master/src/collision/b2_dynamic_tree.cpp) ([doc here](https://box2d.org/files/ErinCatto_DynamicBVH_GDC2019.pdf)), intended for use in Unity. It's a very faithful translation with a couple of additional C# creature comforts for usability.

The code has also been adapted to work in 3D. The main difference in that regard is that I use surface area rather than perimeter as a heuristic to minimize the internal nodes.
