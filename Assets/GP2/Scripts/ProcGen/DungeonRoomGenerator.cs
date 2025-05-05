using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class DungeonRoomGenerator : MonoBehaviour
{
    public DungeonRoomData[] PossibleRooms;

    public DungeonTiler DungeonTiler;
    
    private System.Random random = new System.Random();
    
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
        
        // todo find the intersect with between the levels wall positions and the rooms and then tile them.
        room.WallPositions.IntersectWith(wallPositions);
        
        // Paint the walls with a new rule tile
        DungeonTiler.TileWalls(room.WallPositions, roomType.WallRuleTile);
        
        // Decorate the floor
        DungeonTiler.TileFloor(room.FloorPositions, roomType.FloorRuleTile);
        
        // Populate the floor with enemies
        SpawnUnits(room, roomType);
    }

    public void SpawnUnits(DungeonRoom room, DungeonRoomData roomType)
    {
        if (roomType.MaxUnitsPerRoom <= 0 || roomType.Units.Length == 0 || roomType.SpawnProbability == 0f)
        {
            return;
        }
        
        int unitsSpawned = 0;
        float randomNum = Random.Range(0f, 1f);

        foreach (Vector2Int position in room.FloorPositions.OrderBy(_ => random.Next()))
        {
            if (unitsSpawned > roomType.MaxUnitsPerRoom)
            {
                return;
            }

            if (randomNum < roomType.SpawnProbability)
            {
                Instantiate(roomType.Units[Random.Range(0, roomType.Units.Length)], new Vector3(position.x, position.y),
                    Quaternion.identity);

                unitsSpawned++;
            }
        }
    }
}
