using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDungeonRoom", menuName = "Dungeon/Room")]
public class DungeonRoomData : ScriptableObject
{
    public RuleTile WallRuleTile;
    public RuleTile FloorRuleTile;
    public DungeonPrefabData[] Prefabs;
    public GameObject[] Enemies;
}
