# SolidTurn

SolidTurn is a simulation software which simulates the cutting process of CNC (computer numerical controlled) turning machines.

##Features

- 3D panel which shows the machine, tools and rough part
- Real-time Simulation
- NC code editor
- NC code verification

## Used Technologies

The application is purely written in C# but uses the following third party libraries:
- [OpenTK] (www.opentk.com), an OpenGL wrapper for C#
- [Microsoft Enterprise Library (https://msdn.microsoft.com/en-us/library/ff648951.aspx)] is used for logging and exception handling
- [Dockpanel suite] (http://dockpanelsuite.com/) to enhance the Winforms GUI with docking support
- [This](https://github.com/govert/RobustGeometry.NET/wiki/Floating-point-on-.NET) floating point arithmetic classes , which are based on [this](http://www.cs.berkeley.edu/~jrs/papers/robustr.pdf) paper, are used to remedy floating point errors.

## Notes
- The simulation currently works just for concentric operations.
- Demo videos are available here: https://www.youtube.com/channel/UCy5K1pMjOsn1kcPI2MHDD5w
- If you like this project and want to contribute, then please don't hesitate to write me

## The Author
SolidTurn was originally designed and developed by Martin Ennemoser and Florian Ennemoser in 2014

martin.ennemoser@gmx.at
florian.ennemoser@gmx.at
