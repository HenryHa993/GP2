using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GP2.ProcGen;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation")]
    public Vector2Int StartPosition;
    
    [Header("Random Walk Parameters")]
    public int Iterations;
    public int StepsPerIteration;

    [Header("Corridor Walk Parameters")]
    public int CorridorIterations;
    public int CorridorLength;
    
    [Header("Tilemaps")]
    public Tilemap FloorTilemap;
    public Tilemap WallTilemap;
    public Tilemap DecorationTilemap;
    public Tile FloorTile;
    public Tile WallTile;

    private void Start()
    {
        Generate();
    }

    /* Determines generation type depending on settings. */
    public void Generate()
    {
        GenerateRoomsWithCorridors();
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
            RandomWalk.CorridorWalkWithIterations(StartPosition,CorridorIterations , CorridorLength);
        TileFloor(corridorPositions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(corridorPositions);
        TileWalls(wallPositions);
    }

    public void GenerateRoomsWithCorridors()
    {
        HashSet<Vector2Int> positions =
            RandomWalk.WalkWithIterations(StartPosition, Iterations, StepsPerIteration, true);
        List<Vector2Int> corridorPositions =
            RandomWalk.CorridorWalkWithIterations(positions.ElementAt(Random.Range(0, positions.Count)),CorridorIterations , CorridorLength);
        positions.UnionWith(corridorPositions);
        TileFloor(positions);
        
        HashSet<Vector2Int> wallPositions = GetWallPositions(positions);
        TileWalls(wallPositions);
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
