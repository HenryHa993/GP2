using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomGenerator : MonoBehaviour
{
    public DungeonRoomData[] PossibleRooms;

    private DungeonGenerator DungeonGenerator;
    private DungeonTiler DungeonTiler;
    
    // Start is called before the first frame update
    void Start()
    {
        DungeonTiler = GetComponent<DungeonTiler>();
    }

    public void GenerateRoom(DungeonRoom room, HashSet<Vector2Int> wallPositions)
    {
        // Randomly assign a room
        DungeonRoomData roomType = PossibleRooms[Random.Range(0, PossibleRooms.Length)];
        
        // todo find the intersect with between the levels wallpositions and the rooms and then tile them.
        
        // Paint the walls with a new rule tile
        DungeonTiler.TileWalls(room.WallPositions, roomType.WallRuleTile);
        
        // Decorate the floor
        DungeonTiler.TileFloor(room.FloorPositions, roomType.FloorRuleTile);
        
        // Populate the floor with prefabs
        
    }
}
