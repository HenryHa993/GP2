using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GP2.ProcGen
{
    /* Class used for getting random walk directions during traversal*/
    public static class Direction2D
    {
        public static Vector2Int[] Directions =
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0)
        };

        public static Vector2Int GetRandomDirection()
        {
            return Directions[Random.Range(0, 4)];
        }
    }
}