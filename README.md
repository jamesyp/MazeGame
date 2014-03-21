# XNA Maze Game

This was an assignment for my [USU](http://usu.edu) CS course *Game Development*, 
a class to teach computer science principles in a video game development context.
Topics such as software architecture, algorithms and data structures, networking, 
multithreading, and 2D and 3D graphics were explored by building full PC games in 
solo and team projects.

All code is in C# using Microsoft's XNA platform. This is no longer in active 
development by Microsoft because of all the new shiny Metro stuff they're doing, 
so it might be a bit tough to compile and run this project. If you're interested
in getting it set up, I'd love to help you if you run into problems.

## Setup and Compiling

1. Install [Visual C# Express 2008](go.microsoft.com/?linkid=7729278). Make sure you get the 2008 version. This might work with newer versions of C# Express, but I haven't tested it. Also, the installer should install the .NET Framework 3.5 (required) if you don't have it.
2. Install [XNA Game Studio 4.0](http://www.microsoft.com/en-us/download/details.aspx?id=23714). This is the latest and last version of XNA Game Studio.
3. You *should* be able to open the MazeGame.sln file in Visual C# and compile it through the Project>Build menu or pressing F5 (if memory serves).
4. If you have any problems please email me and I'd be glad to help work them out.

## How To Play

The menus should be fairly self-explanatory, allowing you to start a game with a specified maze size. You can also toggle an enemy "ghost" on and off that will kill you if you touch it.

The player begins in the top-left square of a [randomly generated maze](http://en.wikipedia.org/wiki/Prim's_algorithm), and the goal is to get to the bottom-left. Points are awarded based on how closely you follow the [shortest path](http://en.wikipedia.org/wiki/Depth-first_search). 

### Controls
-   Arrow keys or WASD to move
-   Escape to pause
-   Show/hide breadcrumbs (where you've been) with B
-   Show hint by holding H
-   Show the full shortest path by holding P
