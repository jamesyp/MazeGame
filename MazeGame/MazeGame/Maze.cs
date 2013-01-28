using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace MazeGame
{


    class Maze
    {
        private Cell[,] maze;
        List<Wall> wallList;
        private int size;
        Random r;

        private List<Cell> shortestPath;

        public Maze(int size)
        {
            maze = new Cell[size, size];
            wallList = new List<Wall>();
            this.size = size;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    maze[i, j] = new Cell(i, j);
                }
            }

            r = new Random();
            GenerateMaze();
            SetCellTypes();
            FindShortestPath();
        }

        public int Size
        {
            get { return size; }
        }

        public Cell[,] Cells
        {
            get { return maze; }
        }

        public Cell LastCell
        {
            get { return maze[size - 1, size - 1]; }
        }

        public List<Cell> ShortestPath
        {
            get { return shortestPath; }
        }

        private void GenerateMaze()
        {

            Cell firstCell = RandomCell();
            firstCell.inMaze = true;
            AddWallsToList(firstCell);

            while (wallList.Count != 0)
            {
                Wall wall = RandomWall();

                if (wall.B != null){
                    if (!wall.B.inMaze)
                    {
                        ConnectCells(wall);
                        wall.B.inMaze = true;

                        AddWallsToList(wall.B);
                    }
                }

                wallList.Remove(wall);
            }

        }

        private void SetCellTypes()
        {
            foreach (Cell cell in maze)
            {
                cell.SetCellType();
            }
        }

        private void FindShortestPath()
        {
            Queue<Cell> searchQueue = new Queue<Cell>();
            shortestPath = new List<Cell>();
            Cell curCell =  new Cell(0,0);

            Vector2 endPosition = new Vector2(size - 1, size - 1);

            maze[0, 0].visited = true;
            searchQueue.Enqueue(maze[0, 0]);

            while (searchQueue.Count != 0)
            {
                curCell = searchQueue.Dequeue();

                // This cell is the end
                if (curCell.position.Equals(endPosition))
                {
                    break;
                }

                // Enqueue all the successors of this node
                if (curCell.up != null)
                {
                    if (!curCell.up.visited)
                    {
                        curCell.up.visited = true;
                        curCell.up.previous = curCell;
                        searchQueue.Enqueue(curCell.up);
                    }
                }
                if (curCell.left != null)
                {
                    if (!curCell.left.visited)
                    {
                        curCell.left.visited = true;
                        curCell.left.previous = curCell;
                        searchQueue.Enqueue(curCell.left);
                    }
                } 
                if (curCell.down != null)
                {
                    if (!curCell.down.visited)
                    {
                        curCell.down.visited = true;
                        curCell.down.previous = curCell;
                        searchQueue.Enqueue(curCell.down);
                    }
                }
                if (curCell.right != null)
                {
                    if (!curCell.right.visited)
                    {
                        curCell.right.visited = true;
                        curCell.right.previous = curCell;
                        searchQueue.Enqueue(curCell.right);
                    }
                }
            }

            // After we find the end, reconstruct the path from the end back to the beginning
            // (curCell will currently be the end)
            while (curCell != null)
            {
                shortestPath.Insert(0, curCell);
                if (curCell.previous != null)
                    curCell.previous.next = curCell;
                
                curCell = curCell.previous;
            }
        }

        private void AddWallsToList(Cell cell)
        {
            // Add left wall
            if (cell.left == null)
            {
                if (cell.position.X != 0)
                    wallList.Add(new Wall(cell, maze[(int)cell.position.X - 1, (int)cell.position.Y]));
                else
                    wallList.Add(new Wall(cell, null));
            }

            // Add up wall
            if (cell.up == null)
            {
                if (cell.position.Y != 0)
                    wallList.Add(new Wall(cell, maze[(int)cell.position.X, (int)cell.position.Y - 1]));
                else
                    wallList.Add(new Wall(cell, null));
            }

            // Add right wall
            if (cell.right == null)
            {
                if (cell.position.X != size - 1)
                    wallList.Add(new Wall(cell, maze[(int)cell.position.X + 1, (int)cell.position.Y]));
                else
                    wallList.Add(new Wall(cell, null));
            }

            // Add down wall
            if (cell.down == null)
            {
                if (cell.position.Y != size - 1)
                    wallList.Add(new Wall(cell, maze[(int)cell.position.X, (int)cell.position.Y + 1]));
                else
                    wallList.Add(new Wall(cell, null));
            }
        }

        private void ConnectCells(Wall wall)
        {
            // cell A is to the left of cell B
            if (wall.A.position.X == wall.B.position.X - 1)
            {
                wall.A.right = wall.B;
                wall.B.left  = wall.A;
            }

            // Cell A is to the right of cell B
            if (wall.A.position.X - 1 == wall.B.position.X)
            {
                wall.A.left  = wall.B;
                wall.B.right = wall.A;
            }

            // Cell A is above cell B
            if (wall.A.position.Y == wall.B.position.Y - 1)
            {
                wall.A.down = wall.B;
                wall.B.up   = wall.A;
            }

            // Cell A is below cell B
            if (wall.A.position.Y - 1 == wall.B.position.Y)
            {
                wall.A.up   = wall.B;
                wall.B.down = wall.A;
            }
        }

        private Cell RandomCell()
        {
            int randX = r.Next(size - 1);
            int randY = r.Next(size - 1);

            return maze[randX, randY];
        }

        private Wall RandomWall()
        {
            int randPos = r.Next(wallList.Count - 1);

            return wallList[randPos];
        }

    }

    class Cell
    {
        public Cell up, down, left, right;              // All of this cell's adjacent neighbors, null is a wall
        public Cell previous;                             // The cell before this one in the shortest path (null means
        //  either this cell is first or its not on the shortest path)
        public Cell next;
        public bool inMaze;
        public bool visited;

        public Vector2 position;

        private CellType type;
        public CellType Type
        {
            get { return type; }
        }

        public Cell(int x, int y)
        {
            up = down = left = right = previous = null;
            inMaze = false;
            position = new Vector2(x, y);
        }

        public enum CellType
        {
            AllClosed,
            ThreeWallsOpenUp, ThreeWallsOpenRight, ThreeWallsOpenDown, ThreeWallsOpenLeft,          // A cell with 3 walls, opening facing as indicated
            TwoWallsUpRight, TwoWallsRightDown, TwoWallsDownLeft, TwoWallsLeftUp,                   // Two walls with a shared corner, all rotations
            TwoWallsUpDown, TwoWallsLeftRight,                                                      // Two walls opposite
            OneWallUp, OneWallRight, OneWallDown, OneWallLeft,                                      // One wall, all facings
            AllOpen             // This state should never occur
        }

        public void SetCellType()
        {
            if (up == null && right == null && down == null && left == null)    // All walls closed
                type = CellType.AllClosed;

            // Three walls

            if (up != null && right == null && down == null && left == null)    // Up is open
                type = CellType.ThreeWallsOpenUp;
            if (up == null && right != null && down == null && left == null)    // Right is open
                type = CellType.ThreeWallsOpenRight;
            if (up == null && right == null && down != null && left == null)    // Down is open
                type = CellType.ThreeWallsOpenDown;
            if (up == null && right == null && down == null && left != null)    // Left is open
                type = CellType.ThreeWallsOpenLeft;

            // Two walls, corners

            if (up == null && right == null && down != null && left != null)    // Up Right
                type = CellType.TwoWallsUpRight;
            if (up != null && right == null && down == null && left != null)    // Right Down
                type = CellType.TwoWallsRightDown;
            if (up != null && right != null && down == null && left == null)    // Down Left
                type = CellType.TwoWallsDownLeft;
            if (up == null && right != null && down != null && left == null)    // Left Up
                type = CellType.TwoWallsLeftUp;

            // Two walls, opposites
            if (up == null && right != null && down == null && left != null)
                type = CellType.TwoWallsUpDown;
            if (up != null && right == null && down != null && left == null)
                type = CellType.TwoWallsLeftRight;

            // One wall
            if (up == null && right != null && down != null && left != null)
                type = CellType.OneWallUp;
            if (up != null && right == null && down != null && left != null)
                type = CellType.OneWallRight;
            if (up != null && right != null && down == null && left != null)
                type = CellType.OneWallDown;
            if (up != null && right != null && down != null && left == null)
                type = CellType.OneWallLeft;

            // No walls
            if (up != null && right != null && down != null && left != null)
                type = CellType.AllOpen;
        }
    }

    class Wall
    {
        public Cell A;
        public Cell B;

        public Wall(Cell A, Cell B)
        {
            this.A = A;
            this.B = B;
        }

    }
}
