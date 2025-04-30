using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomGenerator : MonoBehaviour
{
    public DungeonRoomData[] PossibleRooms;

    public DungeonTiler DungeonTiler;
    
    // Start is called before the first frame update
    void Start()
    {
        //DungeonTiler = GetComponent<DungeonTiler>();
    }

    public void GenerateRooms(List<DungeonRoom> rooms, HashSet<Vector2Int> wallPositions)
    {
        foreach (DungeonRoom room in rooms)
        {
            GenerateRoom(room, wallPositions);
        }
    }

    public void GenerateRoom(DungeonRoom room, HashSet<Vector2Int> wallPositions)
    {
        // Randomly assign a room
        DungeonRoomData roomType = PossibleRooms[Random.Range(0, PossibleRooms.Length)];
        
        // todo find the intersect with between the levels wallpositions and the rooms and then tile them.
        room.WallPositions.IntersectWith(wallPositions);
        
        // Paint the walls with a new rule tile
        DungeonTiler.TileWalls(room.WallPositions, roomType.WallRuleTile);
        
        // Decorate the floor
        DungeonTiler.TileFloor(room.FloorPositions, roomType.FloorRuleTile);
        
        // Populate the floor with prefabs
        
    }
}
