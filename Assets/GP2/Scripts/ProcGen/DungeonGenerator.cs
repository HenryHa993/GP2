using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GP2.ProcGen;
using Pathfinding;
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

    [Header("A Star Pathfinding")]
    public LayerMask LayerMask;

    private DungeonTiler DungeonTiler;
    private DungeonRoomGenerator DungeonRoomGenerator;
    
    private List<DungeonRoom> DungeonRooms;
    public HashSet<Vector2Int> DungeonFloor;

    private void Start()
    {
        DungeonTiler = GetComponent<DungeonTiler>();
        DungeonRoomGenerator = GetComponent<DungeonRoomGenerator>();
        
        // Initialise dungeon room
        DungeonRooms = new List<DungeonRoom>();
        
        Generate();
    }

    /* Determines generation type depending on settings. */
    public void Generate()
    {
        GenerateRoomsWithCorridors();
        //qGenerateRoomsOnly();
        // todo
        // Place this in dungeon tiler
        
        StartCoroutine(GenerateGridGraph(DungeonTiler.WallTilemap.cellBounds));
    }

    public void GenerateRoomsOnly()
    {
        HashSet<Vector2Int> positions =
            RandomWalk.WalkWithIterations(StartPosition, Iterations, StepsPerIteration, true);
        DungeonTiler.TileFloor(positions);

        DungeonFloor = positions;
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(positions);
        DungeonTiler.TileWalls(wallPositions);
        
        DungeonTiler.GenerateBackground(positions);
    }

    public void GenerateCorridorsOnly()
    {
        List<Vector2Int> corridorPositions =
            RandomWalk.CorridorWalkWithIterations(StartPosition, CorridorIterations, CorridorLength);
        DungeonTiler.TileFloor(corridorPositions);
        DungeonFloor = new HashSet<Vector2Int>();
        DungeonFloor.UnionWith(corridorPositions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(corridorPositions);
        DungeonTiler.TileWalls(wallPositions);
        
        DungeonTiler.GenerateBackground(corridorPositions);
    }
    
    public void GenerateRoomsWithCorridors()
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        
        // Generate corridors and populate roomPositions
        List<Vector2Int> corridorPositions =
            RandomWalk.CorridorWalkWithIterations(roomPositions, StartPosition, CorridorIterations, CorridorLength);
        
        // Filter out a random subset of room positions from corridor generations
        HashSet<Vector2Int> floorPositions = GetRandomRoomSubset(roomPositions);
        
        // Generate rooms for dead-ends if false
        if (!DeadEnds)
        {
            HashSet<Vector2Int> deadEnds = GetDeadEnds(corridorPositions);
            floorPositions.UnionWith(deadEnds);
        }
        
        // Generate rooms here
        floorPositions.UnionWith(GenerateAndRegisterRooms(floorPositions));
        
        // Union with corridor positions
        floorPositions.UnionWith(corridorPositions);
        
        // Scale positions
        floorPositions = ScaleTilePositions(floorPositions);
        
        // Tile map
        DungeonTiler.TileFloor(floorPositions);
        
        DungeonFloor = floorPositions;
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(floorPositions);
        DungeonTiler.TileWalls(wallPositions);
        
        DungeonRoomGenerator.GenerateRooms(DungeonRooms, wallPositions);
        
        DungeonTiler.GenerateBackground(floorPositions);
    }

    public IEnumerator GenerateGridGraph(BoundsInt bounds)
    {
        // This holds all graph data
        AstarData data = AstarPath.active.data;

        // This creates a Grid Graph
        GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
        
        // Set grid
        gg.SetGridShape(InspectorGridMode.Grid);
        gg.is2D = true;

        // Setup a grid graph with some values
        int width = bounds.size.x * 2;
        int depth = bounds.size.y * 2;
        float nodeSize = 0.5f;

        gg.center = bounds.center;
        gg.center.z = 0;

        // Updates internal size from the above values
        gg.SetDimensions(width, depth, nodeSize);
        
        gg.collision.use2D = true;
        gg.collision.collisionCheck = true;
        
        gg.collision.diameter = 0.4f;
        gg.collision.type = ColliderType.Sphere;
        
        gg.showMeshOutline = true;
        gg.showMeshSurface = true;

        gg.collision.mask = LayerMask;
        
        AstarPath.active.Scan();
        
        yield return null;
        
        // Scans all graphs
        AstarPath.active.Scan();
    }

    /* Generate and register room to RoomMap*/
    // todo you need to generate wall positions
    public HashSet<Vector2Int> GenerateAndRegisterRooms(IEnumerable<Vector2Int> roomPositions)
    {
        HashSet<Vector2Int> allRoomPositions = new HashSet<Vector2Int>();
        
        foreach (Vector2Int position in roomPositions)
        {
            // Generate room based on generator parameters
            HashSet<Vector2Int> floorPositions = RandomWalk.WalkWithIterations(position, Iterations, StepsPerIteration, RandomStart);
            
            allRoomPositions.UnionWith(floorPositions);

            floorPositions = ScaleTilePositions(floorPositions);

            // Process wall positions
            HashSet<Vector2Int> wallPositions = GetWallPositions(floorPositions);
            
            // Register room
            DungeonRooms.Add(new DungeonRoom(floorPositions, wallPositions));
        }

        return allRoomPositions;
    }

    #region Utilities
    public HashSet<Vector2Int> GetRandomRoomSubset(HashSet<Vector2Int> positions)
    {
        HashSet<Vector2Int> randomPositions = new HashSet<Vector2Int>();
        
        // Filter out a random subset of room positions from corridor generations
        int num = Mathf.RoundToInt(RoomPercent * positions.Count);
        for (int i = 0; i < num; i++)
        {
            Vector2Int roomPosition = positions.ElementAt(Random.Range(0, positions.Count));
            randomPositions.Add(roomPosition);
            positions.Remove(roomPosition);
        }

        return randomPositions;
    }

    /* Get all dead-ends from a collection of positions.*/
    public HashSet<Vector2Int> GetDeadEnds(IEnumerable<Vector2Int> positions)
    {
        HashSet<Vector2Int> deadEnds = new HashSet<Vector2Int>();
        
        foreach(var position in positions)
        {
            if (IsDeadEnd(positions, position))
            {
                deadEnds.Add(position);
            }
        }

        return deadEnds;
    }

    /* Check if a position is a dead-end*/
    public bool IsDeadEnd(IEnumerable<Vector2Int> positions, Vector2Int position)
    {
        int neighbourCount = 0;
        foreach (var direction in Direction2D.Directions)
        {
            // If contains neighbour, increase count
            if (positions.Contains(position + direction))
            {
                // If neighbour count larger than 1, iterate next position
                if (++neighbourCount > 1)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /* Scale tile placements positions depending on the walker size.*/
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

    /* Return tilemap positions with only a single neighbour.*/
    public HashSet<Vector2Int> GetWallPositions(IEnumerable<Vector2Int> positions)
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
    #endregion
}
