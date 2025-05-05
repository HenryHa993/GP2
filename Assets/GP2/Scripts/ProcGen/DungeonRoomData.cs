using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Data used to determine the look and feel of each room. Each room
   has a unique wall and floor tileset, as well as units which inhabit them.*/
[CreateAssetMenu(fileName = "NewDungeonRoom", menuName = "Dungeon/Room")]
public class DungeonRoomData : ScriptableObject
{
    public RuleTile WallRuleTile;
    public RuleTile FloorRuleTile;
    public DungeonPrefabData[] Prefabs;
    public GameObject[] Units;
    public float SpawnProbability;
    public int MaxUnitsPerRoom;
}
