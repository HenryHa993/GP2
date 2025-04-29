using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace GP2.ProcGen
{
    public static class RandomWalk
    {
        /* Perform RandomWalkWithIterations at select locations.*/
        public static HashSet<Vector2Int> WalkAtLocations(HashSet<Vector2Int> startPositions, int iterations,
            int steps, bool randomStart)
        {
            HashSet<Vector2Int> tilePositions = new HashSet<Vector2Int>();
            foreach (var position in startPositions)
            {
                tilePositions.UnionWith(WalkWithIterations(position, iterations, steps, randomStart));
            }

            return tilePositions;
        }
        
        /* Perform multiple iterations.*/
        public static HashSet<Vector2Int> WalkWithIterations(Vector2Int startPosition, int iterations,
            int steps, bool randomStart)
        {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            Vector2Int currentPosition = startPosition;
            for (int i = 0; i < iterations; i++)
            {
                HashSet<Vector2Int> generatedPath = Walk(currentPosition, steps);
                path.UnionWith(generatedPath);
                if (randomStart)
                {
                    currentPosition = path.ElementAt(Random.Range(0, path.Count));
                }
            }

            return path;
        }
        
        /* HashSet used to track traversed positions.*/
        public static HashSet<Vector2Int> Walk(Vector2Int startPosition, int steps)
        {
            HashSet<Vector2Int> path = new HashSet<Vector2Int>();
            path.Add(startPosition);
            Vector2Int prevPosition = startPosition;
            
            for (int i = 0; i < steps; i++)
            {
                // todo: check if hash set has a position, if ignore
                Vector2Int newPosition  = prevPosition + Direction2D.GetRandomDirection();
                path.Add(newPosition);
                prevPosition = newPosition;
            }

            return path;
        }

        public static List<Vector2Int> CorridorWalkWithIterations(HashSet<Vector2Int> roomPositions, Vector2Int startPosition,
            int iterations, int steps)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int currentPosition = startPosition;
            roomPositions.Add(currentPosition);
            for (int i = 0; i < iterations; i++)
            {
                List<Vector2Int> generatedPath = CorridorWalk(currentPosition, steps);
                path.AddRange(generatedPath);
                currentPosition = path.Last();
                roomPositions.Add(currentPosition);
            }

            return path;
        }

        public static List<Vector2Int> CorridorWalkWithIterations(Vector2Int startPosition, int iterations, int steps)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int currentPosition = startPosition;

            for (int i = 0; i < iterations; i++)
            {
                List<Vector2Int> generatedPath = CorridorWalk(currentPosition, steps);
                path.AddRange(generatedPath);
                currentPosition = path.Last();
            }

            return path;
        }

        public static List<Vector2Int> CorridorWalk(Vector2Int startPosition, int steps)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            Vector2Int direction = Direction2D.GetRandomDirection();
            Vector2Int currentPosition = startPosition;
            path.Add(currentPosition);

            for (int i = 0; i < steps; i++)
            {
                currentPosition += direction;;
                path.Add(currentPosition);
            }

            return path;
        }
    }
}