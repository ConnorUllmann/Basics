using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace Basics
{
    public interface ISolidTile
    {
        bool Solid { get; }
    }

    public enum DistanceEstimationAlgorithm
    {
        Manhattan,
        Euclidean
    }
    
    public static class Path
    {
        private class PathTile<T> : IComparable where T : ISolidTile
        {
            private readonly Grid<T> grid;
            public int x;
            public int y;
            public T Object => grid.Get(x, y);
            public List<T> NeighborsCardinal => grid.GetNeighborsCardinal(x, y);
            public List<T> NeighborsSquare => grid.GetNeighborsSquare(x, y);

            public float Steps => steps.HasValue ? steps.Value : int.MaxValue;
            public float? steps;
            public float TargetDistance => targetDistance.HasValue ? targetDistance.Value : float.MaxValue;
            public float? targetDistance;
            public float heuristic => Heuristic(steps, targetDistance);

            public PathTile(Grid<T> _grid, int _x, int _y)
            {
                grid = _grid;
                x = _x;
                y = _y;
                Reset();
            }

            public int CompareTo(object other)
            {
                var o = (PathTile<T>)other;
                if (o == null)
                    return 0;
                return Math.Sign(heuristic - o.heuristic);
            }
            
            public void SetHeuristicProperties(float? steps, float? targetDistance)
            {
                this.steps = steps;
                this.targetDistance = targetDistance;
            }
            public void Reset() => SetHeuristicProperties(null, null);
            public static float Heuristic(float? steps, float? targetDistance) => steps.HasValue && targetDistance.HasValue ? steps.Value + targetDistance.Value : int.MaxValue;

        }

        public class PathMap<T> where T : ISolidTile
        {
            private DistanceEstimationAlgorithm distanceType;
            private bool canMoveDiagonally;

            private Grid<T> grid;
            private Grid<PathTile<T>> gridPath;

            private delegate List<PathTile<T>> GetNeighbors(int x, int y);
            private GetNeighbors neighborsCallback;

            public PathMap(Grid<T> _grid, bool _canMoveDiagonally = true, DistanceEstimationAlgorithm _distanceType = DistanceEstimationAlgorithm.Euclidean)
            {
                grid = _grid;
                canMoveDiagonally = _canMoveDiagonally;
                distanceType = _distanceType;
                InitializeGrid();
            }

            private void InitializeGrid()
            {
                gridPath = new Grid<PathTile<T>>(grid.Width, grid.Height);
                neighborsCallback = GetNeighborsCallback(gridPath);
                for (var x = 0; x < gridPath.Width; x++)
                    for (var y = 0; y < gridPath.Height; y++)
                        gridPath.Set(new PathTile<T>(grid, x, y), x, y);
            }

            private void Reset() => gridPath.ForEachYX(x => x.Reset());

            private GetNeighbors GetNeighborsCallback(Grid<PathTile<T>> gridPath)
            {
                return canMoveDiagonally ? (GetNeighbors)gridPath.GetNeighborsSquare : gridPath.GetNeighborsCardinal;
            }

            private float Distance(int _x1, int _y1, int _x2, int _y2)
            {
                switch (distanceType)
                {
                    case DistanceEstimationAlgorithm.Manhattan: return Utils.ManhattanDistance(_x1, _y1, _x2, _y2);
                    case DistanceEstimationAlgorithm.Euclidean: return Utils.EuclideanDistance(_x1, _y1, _x2, _y2);
                }
                return -1;
            }
            
            private float Distance(PathTile<T> a, PathTile<T> b) => Distance(a.x, a.y, b.x, b.y);

            /// <summary>
            /// Finds the shortest path from the start point to the target point on the grid.
            /// </summary>
            /// <param name="xstart">x-position of start point on grid</param>
            /// <param name="ystart">y-position of start point on grid</param>
            /// <param name="xtarget">x-position of target point on grid</param>
            /// <param name="ytarget">y-position of target point on grid</param>
            /// <returns>List of tiles in the grid making up the shortest path from the start point to the end point</returns>
            public List<T> FindPath(int xstart, int ystart, int xtarget, int ytarget)
            {
                Reset();

                var open = new Heap<PathTile<T>>();
                var closed = new List<PathTile<T>>();
                var path = new List<PathTile<T>>();

                //Start at the end since we add tiles to the list in reverse order
                var first = gridPath.Get(xtarget, ytarget);
                if (first.Object.Solid)
                    return new List<T>();

                var last = gridPath.Get(xstart, ystart);
                if (last.Object.Solid)
                    return new List<T>();

                first.SetHeuristicProperties(0, Distance(first, last));
                open.Add(first);

                PathTile<T> current;
                while (!open.Empty())
                {
                    current = open.Pop();
                    closed.Add(current);

                    if (current == last)
                    {
                        current.SetHeuristicProperties(null, null);
                        while (true)
                        {
                            path.Add(current);
                            var neighborsBacktrack = neighborsCallback(current.x, current.y);
                            foreach (var neighbor in neighborsBacktrack)
                                if (neighbor.Steps < current.Steps)
                                    current = neighbor;
                            if (current == first)
                            {
                                path.Add(current);
                                return path.Select(x => x.Object).ToList();
                            }
                        }
                    }

                    var neighbors = neighborsCallback(current.x, current.y);
                    foreach (var neighbor in neighbors)
                    {
                        if (neighbor.Object.Solid || closed.Contains(neighbor))
                            continue;
                        
                        var neighborSteps = current.steps.Value + Distance(current, neighbor);
                        var neighborTargetDistance = Distance(neighbor, last);
                        if (open.Contains(neighbor))
                        {
                            var neighborHeuristic = PathTile<T>.Heuristic(neighborSteps, neighborTargetDistance);
                            if (neighborHeuristic < neighbor.heuristic)
                                neighbor.SetHeuristicProperties(neighborSteps, neighborTargetDistance);
                        }
                        else
                        {
                            neighbor.SetHeuristicProperties(neighborSteps, neighborTargetDistance);
                            open.Add(neighbor);
                        }
                    }
                }
                return new List<T>();
            }
        }

        /// <summary>
        /// Finds the shortest path from the start point to the target point on the grid
        /// </summary>
        /// <param name="_grid">grid to explore</param>
        /// <param name="xstart">x-position of start point on grid</param>
        /// <param name="ystart">y-position of start point on grid</param>
        /// <param name="xtarget">x-position of target point on grid</param>
        /// <param name="ytarget">y-position of target point on grid</param>
        /// <param name="_canMoveDiagonally">whether the pathfinding can move diagonally</param>
        /// <param name="_distanceType">what type of distance to use (euclidean/manhattan)</param>
        /// <returns>List of tiles in the grid making up the shortest path from the start point to the end point</returns>
        public static List<T> FindPath<T>(Grid<T> _grid, float xstart, float ystart, float xtarget, float ytarget, bool _canMoveDiagonally = true, DistanceEstimationAlgorithm _distanceType = DistanceEstimationAlgorithm.Euclidean) where T : ISolidTile =>
            FindPath(_grid, (int)xstart, (int)ystart, (int)xtarget, (int)ytarget, _canMoveDiagonally, _distanceType);

        public static List<T> FindPath<T>(Grid<T> _grid, int xstart, int ystart, int xtarget, int ytarget, bool _canMoveDiagonally = true, DistanceEstimationAlgorithm _distanceType = DistanceEstimationAlgorithm.Euclidean) where T : ISolidTile =>
            new PathMap<T>(_grid, _canMoveDiagonally, _distanceType).FindPath(xstart, ystart, xtarget, ytarget);
    }

}
