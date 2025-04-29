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

    [Header("Background")]
    public int BackgroundScale;
    
    [Header("Tilemaps")]
    public Tilemap FloorTilemap;
    public Tilemap WallTilemap;
    public Tilemap BackgroundTilemap;
    public Tile FloorTile;
    public Tile WallTile;
    public RuleTile BackgroundRuleTile;

    private void Start()
    {
        Generate();
    }

    /* Determines generation type depending on settings. */
    public void Generate()
    {
        GenerateRoomsWithCorridors();
        BoundsInt bounds = WallTilemap.cellBounds;
        GenerateBackground(bounds);
    }

    public void GenerateRoomsOnly()
    {
        HashSet<Vector2Int> positions =
            RandomWalk.WalkWithIterations(StartPosition, Iterations, StepsPerIteration, true);
        TileFloor(positions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(positions);
        TileWalls(wallPositions);
    }

    public void GenerateCorridorsOnly()
    {
        List<Vector2Int> corridorPositions =
            RandomWalk.CorridorWalkWithIterations(StartPosition, CorridorIterations, CorridorLength);
        TileFloor(corridorPositions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(corridorPositions);
        TileWalls(wallPositions);
    }
    
    public void GenerateRoomsWithCorridors()
    {
        // Generate corridors to get positions for potential room placement
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        List<Vector2Int> corridorPositions =
            RandomWalk.CorridorWalkWithIterations(roomPositions, StartPosition, CorridorIterations, CorridorLength);
        
        // Filter out a random subset of room positions
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
        TileFloor(floorPositions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(floorPositions);
        TileWalls(wallPositions);
    }

    /* Generate a background based off bounds.*/
    public void GenerateBackground(BoundsInt bounds)
    {
        Vector3Int topBound = new Vector3Int(bounds.max.x, bounds.max.y);
        Vector3Int bottomBound = new Vector3Int(bounds.min.x, bounds.min.y);

        for (int x = bottomBound.x; x < topBound.x; x++)
        {
            for (int y = bottomBound.y; y < topBound.y; y++)
            {
                BackgroundTilemap.SetTile(new Vector3Int(x, y), BackgroundRuleTile);
            }
        }
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

    private void TileWalls(HashSet<Vector2Int> positions)
    {
        WallTilemap.ClearAllTiles();
        
        foreach (var position in positions)
        {
            WallTilemap.SetTile(new Vector3Int(position.x, position.y), WallTile);
        }
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

    private void TileFloor(IEnumerable<Vector2Int> positions)
    {
        FloorTilemap.ClearAllTiles();

        foreach (var position in positions)
        {
            FloorTilemap.SetTile(new Vector3Int(position.x, position.y), FloorTile);
        }
    }
}
