# BoundingVolumeHierarchy

![BoundingVolumeHierarchy](https://i.imgur.com/y53Hc8h.png)

This is a port of [Erin Catto/box2d's b2_dynamic_tree from C++ to C#](https://github.com/erincatto/box2d/blob/master/src/collision/b2_dynamic_tree.cpp) ([doc here](https://box2d.org/files/ErinCatto_DynamicBVH_GDC2019.pdf)), intended for use in Unity. It's a very faithful translation with a couple of additional C# creature comforts for usability.

The code has also been adapted to work in 3D. The main difference in that regard is that I use surface area rather than perimeter as a heuristic to minimize the internal nodes.

# How to Use

The object to be stored in your tree must implement the interface IBVHClientObject: 

![IBVHClientObject](https://i.imgur.com/YROK75N.png)

The bounds should be a tight fit around your object. For previous position, I recommend just caching the object's position every frame.

Next, create an instance of BoundingVolumeHierarchy<T> with the appropriate type parameter. Your main forms of interaction with the tree are fairly self explanatory:
  
![Methods](https://i.imgur.com/3zrovkc.png)
  
Call 'update' whenever an object's position/rotation/scale/bounds changes.
  
The simplest form of querying the tree involves just enumerating all nodes:
  
![Enumeration](https://i.imgur.com/mV6qDIH.png)
