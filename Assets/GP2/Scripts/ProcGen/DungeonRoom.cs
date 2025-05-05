using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Information which encapsulates the details required to generate a room
   in the level. Taken positions would have been used to place prefabs if
   I had enough time.*/
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
