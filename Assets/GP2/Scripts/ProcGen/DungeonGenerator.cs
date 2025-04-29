using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GP2.ProcGen;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation")]
    public Vector2Int StartPosition;
    public int WalkerSize;
    
    [Header("Random Walk Parameters")]
    public int Iterations;
    public int StepsPerIteration;
    public float RoomPercent;
    public bool RandomStart;

    [Header("Corridor Walk Parameters")]
    public int CorridorIterations;
    public int CorridorLength;
    public bool DeadEnds;

    private DungeonTiler DungeonTiler;

    private void Start()
    {
        DungeonTiler = GetComponent<DungeonTiler>();
        Generate();
    }

    /* Determines generation type depending on settings. */
    public void Generate()
    {
        GenerateRoomsWithCorridors();
        // Place this in dungeon tiler
        BoundsInt bounds = DungeonTiler.WallTilemap.cellBounds;
        DungeonTiler.GenerateBackground(bounds);
    }

    public void GenerateRoomsOnly()
    {
        HashSet<Vector2Int> positions =
            RandomWalk.WalkWithIterations(StartPosition, Iterations, StepsPerIteration, true);
        DungeonTiler.TileFloor(positions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(positions);
        DungeonTiler.TileWalls(wallPositions);
    }

    public void GenerateCorridorsOnly()
    {
        List<Vector2Int> corridorPositions =
            RandomWalk.CorridorWalkWithIterations(StartPosition, CorridorIterations, CorridorLength);
        DungeonTiler.TileFloor(corridorPositions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(corridorPositions);
        DungeonTiler.TileWalls(wallPositions);
    }
    
    public void GenerateRoomsWithCorridors()
    {
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        
        // Generate corridors to get positions for potential room placement
        List<Vector2Int> corridorPositions =
            RandomWalk.CorridorWalkWithIterations(roomPositions, StartPosition, CorridorIterations, CorridorLength);
        
        // Filter out a random subset of room positions from corridor generations
        int num = Mathf.RoundToInt(RoomPercent * roomPositions.Count);
        for (int i = 0; i < roomPositions.Count; i++)
        {
            Vector2Int roomPosition = roomPositions.ElementAt(Random.Range(0, roomPositions.Count));
            floorPositions.Add(roomPosition);
            roomPositions.Remove(roomPosition);
        }
        
        // Generate rooms for dead-ends if false
        // I might be able to return dead ends when generating the corridors themselves
        // If direction taken in the next iteration is the inverse of the last, that start position is a dead-end.
        if (!DeadEnds)
        {
            HashSet<Vector2Int> deadEnds = new HashSet<Vector2Int>();
            
            // Todo put this in its own method
            // Identify dead-ends by checking if they have just one neighbour
            foreach(var position in corridorPositions)
            {
                // Avoid checking positions already added
                if (floorPositions.Contains(position))
                {
                    continue;
                }
                    
                int neighbourCount = 0;
                foreach (var direction in Direction2D.Directions)
                {
                    // If contains neighbour, increase count
                    if (corridorPositions.Contains(position + direction))
                    {
                        // If neighbour count larger than 1, iterate next position
                        if (++neighbourCount > 1)
                        {
                            break;
                        }
                    }
                }

                if (neighbourCount == 1)
                {
                    deadEnds.Add(position);
                }
            }
            
            floorPositions.UnionWith(deadEnds);
        }
        
        // Generate rooms based on positions
        floorPositions.UnionWith(RandomWalk.WalkAtLocations(floorPositions, Iterations, StepsPerIteration, RandomStart));
        
        // Union with corridor positions
        floorPositions.UnionWith(corridorPositions);
        
        // Scale positions
        floorPositions = ScaleTilePositions(floorPositions);
        
        // Tile map
        DungeonTiler.TileFloor(floorPositions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(floorPositions);
        DungeonTiler.TileWalls(wallPositions);
    }

    /* Scale tiles to adjust for walker size.*/
    public HashSet<Vector2Int> ScaleTilePositions(IEnumerable<Vector2Int> positions)
    {
        HashSet<Vector2Int> scaledPositions = new HashSet<Vector2Int>();
        
        foreach (var position in positions)
        {
            Vector2Int currentPosition = new Vector2Int(position.x, position.y) * WalkerSize;
            scaledPositions.Add(currentPosition);
            for (int x = 0; x < WalkerSize; x++)
            {
                for (int y = 0; y < WalkerSize; y++)
                {
                    scaledPositions.Add(currentPosition + new Vector2Int(x, y));
                }
            }
        }

        return scaledPositions;
    }

    private HashSet<Vector2Int> GetWallPositions(IEnumerable<Vector2Int> positions)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        
        foreach (var position in positions)
        {
            foreach (var direction in Direction2D.Directions)
            {
                Vector2Int neighbour = position + direction;
                if (!positions.Contains(neighbour))
                {
                    wallPositions.Add(neighbour);
                }
            }
        }
        
        return wallPositions;
    }
}
