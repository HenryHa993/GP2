using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Pathfinding;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace GP2.GOAP
{
    /* This sensor identifies a random position near the agent to navigate to.
       Since we avoid navigating outside the boundary of the map, we ensure the position
       is contained within the dungeon boundaries.*/
    public class WanderTargetSensor : LocalTargetSensorBase
    {
        private DungeonGenerator DungeonGenerator;
        
        // Created
        public override void Created()
        {
            DungeonGenerator = GameObject.FindGameObjectWithTag("DungeonGenerator").GetComponent<DungeonGenerator>();
        }

        // Every frame
        public override void Update() { }

        // Before planning a new action
        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            Vector3 position = GetRandomPosition(agent);

            return new PositionTarget(position);
        }

        public Vector3 GetRandomPosition(IActionReceiver agent)
        {
            Vector3 random = Random.insideUnitCircle * 5f;
            Vector2 position = agent.Transform.position + random;
            Vector2Int tilePosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            
            // Todo might be worth limiting this
            while (!DungeonGenerator.DungeonFloor.Contains(tilePosition))
            {
                random = Random.insideUnitCircle * 5f;
                position = agent.Transform.position + random;
                tilePosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            }

            return new Vector3(tilePosition.x, tilePosition.y);
        }
    }   
}
