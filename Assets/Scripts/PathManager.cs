using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PathManager : MonoBehaviour
{
    // map variables 
    public MapManager mapManager;
    private int[][] _map; // 1's -> object, 0's -> free 
    public int[][] Map {  get { return _map; } }
    private int[] xRange = new int[2];
    private int[] yRange = new int[2];
    public bool inflateObstacleLayer = true;

    // A* variables
    private Dictionary<Point, bool> _closedList;
    private int _numberOfCells;
    private int _numberFreeCells;
    private List<Vector3> _path;
    private int numberOfOpenListItems;
    private Node[] _openList;
    private Dictionary<Point, bool> _insideOpenList;

    // path smoothing variables 
    public float minDistance = 0.01f;
    public float turningThreshold = 0.25f;


    void Start()
    {
        InitEnvironment();
    }

    void InitEnvironment()
    {
        // get level number and assign correct values to storage data 
        if (mapManager.levelNumber == 1)
        {
            _map = mapManager.Map1;
            _numberOfCells = mapManager.Map1Cells;
        }
        else if (mapManager.levelNumber == 2)
        {
            _map = mapManager.Map2;
            _numberOfCells = mapManager.Map2Cells;
        }
        else
        {
            Debug.LogError("Level Number Incomplete");
        }

        // inflate obstacles to avoid travelling too close to obstacles 
        if (inflateObstacleLayer)
            mapManager.InflateObstacleLayer(ref _map);

        // assign correct range of values for x and y dimensions 
        xRange[0] = 0;
        xRange[1] = _map[0].Length - 1;
        yRange[0] = 0;
        yRange[1] = _map.Length - 1;

        // initialize lists for A star with number of free cells in environment 
        // to avoid data structure expansion during finding path operations 
        _numberFreeCells = mapManager.GetFreeCells(_map);
        _closedList = new Dictionary<Point, bool>(_numberFreeCells);
        numberOfOpenListItems = 0;
        _openList = new Node[_map[0].Length * _map.Length + 1];
        _insideOpenList = new Dictionary<Point, bool>(_numberFreeCells);

        // populate dict with free spaces with a false boolean value 
        for (int i = 0; i < _map.Length; i++)
        {
            for (int j = 0; j < _map[0].Length; j++)
            {
                if (_map[i][j] == 0)
                {
                    _closedList.Add(new Point(j, i), false);
                    _insideOpenList.Add(new Point(j, i), false);
                }

            }
        }
}


    public void findPath(Vector3 from, Vector3 to, out List<Vector3> path, out int pathSize)
    {
        // initialize starting and finish nodes
        int startX, startZ, finishX, finishZ;
        mapManager.ConvertCartesianToCell(from.x, from.z, out startX, out startZ);
        mapManager.ConvertCartesianToCell(to.x, to.z, out finishX, out finishZ);

        if (_map[startZ][startX] == 1)
        {
            Debug.LogError("Agent is outside map, inside inflated obstacle area or inside obstacle.");
        }

        Node start = new Node(startX, startZ);
        Node finish = new Node(finishX, finishZ);

        // initialize path
        path = new List<Vector3>(_numberFreeCells);
        pathSize = 0;

        // add initial position to open list -> locations to be checked out
        openListAdd(start);
        _insideOpenList[start.Point] = true;

        // stop when target square added to closed list or failed to find target square
        bool targetAdded = false;

        while (numberOfOpenListItems > 0)
        {
            // remove from openList the node with lowest F and add it to closed list
            Node current = _openList[1];
            openListDeleteFirst();
            _insideOpenList[current.Point] = false;
            _closedList[current.Point] = true;

            // check if target location was found
            if (finish.Equals(current))
            {
                targetAdded = true;
                // add nodes to path 
                do
                {
                    float x, z;
                    mapManager.ConvertCellToCartesian(current.Point.X, current.Point.Y, out x, out z);
                    path.Insert(0, new Vector3(x, 0.0f, z));
                    pathSize++;

                    current = current.Parent;
                } while (current != null);
                Debug.Log("Path was found.");
                break;
            }

            // add reachable locations from initial to open list too 
            int[,] neighbours = new int[,] { { 0, -1 }, { 0, 1 }, { -1, 0 }, { 1, 0 }, { -1, -1 }, { -1, 1 }, { 1, -1 }, { 1, 1 } };
            for (int k = 0; k < neighbours.GetLength(0); k++)
            {
                int xCurrent = current.Point.X + neighbours[k, 0];
                int yCurrent = current.Point.Y + neighbours[k, 1];

                if (xCurrent < xRange[0] || xCurrent > xRange[1] ||
                        yCurrent < yRange[0] || yCurrent > yRange[1])
                    continue;

                // check if diagonal or side square
                bool diagonal = true;
                if (neighbours[k, 0] == 0 || neighbours[k, 1] == 0)
                    diagonal = false;

                // create node from values 
                Node adjacent = new Node(xCurrent, yCurrent);

                // ignore non walkable squares 
                if (_map[adjacent.Point.Y][adjacent.Point.X] == 1)
                    continue;

                // if node is inside closed, ignore
                if (_closedList[adjacent.Point] == true)
                    continue;

                // if is not in open list 
                if (_insideOpenList[adjacent.Point] == false)
                {
                    // make current node the parent of this adjacent square
                    adjacent.Parent = current;

                    // record f, g, h
                    adjacent.H = adjacent.CalculateH(finish.Point.X, finish.Point.Y);
                    adjacent.G = adjacent.Parent.G + adjacent.CalculateG(diagonal);
                    adjacent.F = adjacent.CalculateF();

                    // add it to open list
                    openListAdd(adjacent);
                    _insideOpenList[adjacent.Point] = true;
                }

                // if already in open list
                else
                {
                    // check if G is lower than the current one
                    if (adjacent.G < current.G)
                    {
                        // change parent of the square to the current square 
                        adjacent.Parent = current;

                        // recalculate G and F of the square 
                        adjacent.G = adjacent.Parent.G + adjacent.CalculateG(diagonal);
                        adjacent.F = adjacent.CalculateF();
                        // if list sorted by F, account for this change -> binary tree
                    }
                }
            }
        }

        if (!targetAdded)
            Debug.LogError("Path was not found.");

        cleanUp();
    }

    public void openListAdd(Node node)
    {
        // binary heap add 

        numberOfOpenListItems++;
        _openList[numberOfOpenListItems] = node;

        // move value into correct position
        int m = numberOfOpenListItems;
        while (m > 1)
        {
            // if child is smaller than parent, swap them
            int n = (int) Mathf.Round(m / 2.0f);
            if (_openList[m].F <= _openList[n].F)
            {
                Node temp = _openList[n];
                _openList[n] = _openList[m];
                _openList[m] = temp;
                m = n;
            }
            else
                break;
        }
    }

    public void openListDeleteFirst()
    {
        // binary heap remove first 
        _openList[1] = _openList[numberOfOpenListItems];
        _openList[numberOfOpenListItems] = (Node)null;
        numberOfOpenListItems--;

        int v = 1; int u = 1;
        while (true)
        {
            u = v;

            // if parent has both children
            if ((2 * u + 1) <= numberOfOpenListItems)
            {
                // select lowest of two children if bigger than parent
                if (_openList[u].F >= _openList[2 * u].F)
                    v = 2 * u;
                if (_openList[v].F >= _openList[2 * u + 1].F)
                    v = 2 * u + 1;
            }

            // if parent has only one children
            else if ((2 * u) <= numberOfOpenListItems)
            {
                // select child if bigger than parent 
                if (_openList[u].F >= _openList[2 * u].F)
                    v = 2 * u;
            }            

            if (v > u)
            {
                Node temp = _openList[u];
                _openList[u] = _openList[v];
                _openList[v] = temp;
            }
            else
                break;
        }
    }
    
    void cleanUp()
    {
        // clean up variables before next path planning call
        numberOfOpenListItems = 0;
        _openList = new Node[_map[0].Length * _map.Length + 1];
        

        // reset to falses 
        for (int i = 0; i < _map.Length; i++)
        {
            for (int j = 0; j < _map[0].Length; j++)
            {
                if (_map[i][j] == 0)
                {
                    Point p = new Point(j, i);
                    _closedList[p] = false;

                    _insideOpenList[p] = false;
                }

            }
        }
    }

    public void FilterPath(List<Vector3> path, int sizeOfPath, out List<Vector3> filteredPath)
    {
        // filter path by removing redundant waypoints
        filteredPath = new List<Vector3>();
        for (int i = 0; i < sizeOfPath; i++)
        {
            if ((i == 0) || (i == (sizeOfPath - 1)))
                filteredPath.Add(path[i]);
            else
            {
                Vector3 previousDir = (path[i] - path[i - 1]).normalized;
                Vector3 nextDir = (path[i + 1] - path[i]).normalized;
                Vector3 difference = nextDir - previousDir;

                if (difference.sqrMagnitude < minDistance)
                    continue;
                else
                    filteredPath.Add(path[i]);
            }
        }
    }

    public bool PassedTurningPoint(List<Vector3> path, int index, Vector2 currentPosition)
    {
        // checks if agent has passed current waypoint
        Vector3 previousWaypoint3D = path[index - 1];
        Vector3 currentWaypoint3D = path[index];
        Vector2 previousWaypoint2D = new Vector2(previousWaypoint3D.x, previousWaypoint3D.z);
        Vector2 currentWaypoint2D = new Vector2(currentWaypoint3D.x, currentWaypoint3D.z);

        Vector2 differenceWaypoints = currentWaypoint2D - previousWaypoint2D;

        // if last waypoint 
        float modifiedThresh = turningThreshold;
        if (index == path.Count - 1)
            modifiedThresh = 0.0f;

        Vector2 turningPoint = differenceWaypoints - differenceWaypoints.normalized * modifiedThresh;
        Vector2 toCurrentPositon = currentPosition - previousWaypoint2D;

        if (toCurrentPositon.sqrMagnitude > turningPoint.sqrMagnitude)
        {
            return true;
        }
        else
            return false;
    }

    public class Node
    {
        // Node contains current point, G value, H heuristic, F value, reference to parent 
        private Point _point;
        
        private int _g;
        private int _h;
        private int _f;

        private Node _parent;

        // constructor 
        public Node(int x, int y)
        {
            _point = new Point(x, y);

            _g = 0;
            _h = 0;
            _f = 0;

            _parent = null;
        }

        public Point Point { get { return _point; } set { _point = value;  } }
        public Node Parent { get { return _parent; } set { _parent = value; } }
        public int G { get { return _g; } set { _g = value; } }
        public int H { get { return _h; } set { _h = value; } }
        public int F { get { return _f; } set { _f = value; } }
        
        public int CalculateH(int xEnd, int yEnd)
        {
            return Mathf.Abs(xEnd - _point.X) + Mathf.Abs(yEnd - _point.Y);
        }

        public int CalculateG(bool diagonal)
        {
            int value = 0;
            if (diagonal)
                value = 14;
            else
                value = 10;
            return value;
        }

        public int CalculateF()
        {
            return _h + _g;
        }

        // overrides Equal based on point 
        public override bool Equals(object obj)
        {
            var item = obj as Node;
            
            if (item == null)
            {
                return false;
            }
            Node comparingNode = (Node) item;
            return this._point.X == comparingNode._point.X && 
                this._point.Y == comparingNode._point.Y;
        }
    }

    public class Point
    {
        private int _x;
        private int _y;

        public Point(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public int X { get { return _x; } set { _x = value; } }
        public int Y { get { return _y; } set { _y = value; } }

        // override hash code based on x and y values
        public override int GetHashCode()
        {
            const int prime = 31;

            int result = 1;
            result = prime * result + (int)(_x);
            result = prime * result + (int)(_y);

            return result;
        }

        // overrides equals based on x and y 
        public override bool Equals(object obj)
        {
            var item = obj as Point;

            if (item == null)
            {
                return false;
            }
            Point comparingPoint = (Point)item;
            return this._x == comparingPoint._x && this._y == comparingPoint._y;
        }
    }

}
