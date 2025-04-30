using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DungeonRoom
{
    public HashSet<Vector2Int> FloorPositions;
    public HashSet<Vector2Int> WallPositions;
    public HashSet<Vector2Int> TakenPositions;
    
    public DungeonRoom(HashSet<Vector2Int> floorPositions, HashSet<Vector2Int> wallPositions)
    {
        FloorPositions = floorPositions;
        WallPositions = wallPositions;
        
        TakenPositions = new HashSet<Vector2Int>();
    }
}
