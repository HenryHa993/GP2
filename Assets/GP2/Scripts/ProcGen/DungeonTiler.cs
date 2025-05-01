using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonTiler : MonoBehaviour
{
    [Header("Floor Tiles")]
    public Tilemap FloorTilemap;
    public RuleTile DefaultFloorRuleTile;
    
    [Header("Wall Tiles")]
    public Tilemap WallTilemap;
    public RuleTile DefaultWallRuleTile;
    
    [Header("Background Tiles")]
    public Tilemap BackgroundTilemap;
    public RuleTile DefaultBackgroundRuleTile;
    
    public void TileFloor(IEnumerable<Vector2Int> positions, RuleTile ruleTile)
    {
        //FloorTilemap.ClearAllTiles();

        foreach (var position in positions)
        {
            FloorTilemap.SetTile(new Vector3Int(position.x, position.y), ruleTile);
        }
    }
    
    public void TileFloor(IEnumerable<Vector2Int> positions)
    {
        FloorTilemap.ClearAllTiles();

        foreach (var position in positions)
        {
            FloorTilemap.SetTile(new Vector3Int(position.x, position.y), DefaultFloorRuleTile);
        }
    }
    
    public void TileWalls(HashSet<Vector2Int> positions, RuleTile ruleTile)
    {
        //WallTilemap.ClearAllTiles();
        
        foreach (var position in positions)
        {
            WallTilemap.SetTile(new Vector3Int(position.x, position.y), ruleTile);
        }
    }
    
    public void TileWalls(HashSet<Vector2Int> positions)
    {
        WallTilemap.ClearAllTiles();
        
        foreach (var position in positions)
        {
            WallTilemap.SetTile(new Vector3Int(position.x, position.y), DefaultWallRuleTile);
        }
    }

    public void TileBackground(IEnumerable<Vector2Int> positions)
    {
        BackgroundTilemap.ClearAllTiles();

        foreach (var position in positions)
        {
            BackgroundTilemap.SetTile(new Vector3Int(position.x, position.y), DefaultBackgroundRuleTile);
        }
    }
    
    // todo maybe move this into the dungeon gen
    /* Generate a background based off bounds.*/
    public void GenerateBackground(IEnumerable<Vector2Int> dungeonFloor)
    {
        HashSet<Vector2Int> backgroundPositions = new HashSet<Vector2Int>();

        BoundsInt bounds = WallTilemap.cellBounds;
        
        Vector3Int topBound = new Vector3Int(bounds.max.x, bounds.max.y);
        Vector3Int bottomBound = new Vector3Int(bounds.min.x, bounds.min.y);

        for (int x = bottomBound.x; x < topBound.x; x++)
        {
            for (int y = bottomBound.y; y < topBound.y; y++)
            {
                backgroundPositions.Add(new Vector2Int(x, y));
                //BackgroundTilemap.SetTile(new Vector3Int(x, y), DefaultBackgroundRuleTile);
            }
        }
        
        backgroundPositions.ExceptWith(dungeonFloor);
        
        TileBackground(backgroundPositions);
    }
}
