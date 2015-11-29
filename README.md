# SolidTurn

SolidTurn is a simulation software which simulates the cutting process of CNC (computer numerical controlled) turning machines.

##Features

- 3D panel which shows the machine, tools and rough part
- Real-time Simulation
- NC code editor
- NC code verification

## Used Technologies

The application is purely written in C# but uses the following third party libraries:
- OpenTK (www.opentk.com) is an OpenGL wrapper for C#
- Microsoft Enterprise Library (https://msdn.microsoft.com/en-us/library/ff648951.aspx) is used for logging and exception handling
- Dockpanel suite (http://dockpanelsuite.com/) to enhance the Winforms GUI with docking support
- This floating point arithmetic classes (https://github.com/govert/RobustGeometry.NET/wiki/Floating-point-on-.NET), which are based on this paper, are used to remedy floating point errors.

## Notes
- The simulation currently works just for concentric operations.
- Demo videos are available here: https://www.youtube.com/channel/UCy5K1pMjOsn1kcPI2MHDD5w
- If you like this project and want to contribute, then please don't hesitate to write me

# The Author
SolidTurn was originally designed and developed by Martin Ennemoser in 2014

martin.ennemoser@gmx.at
