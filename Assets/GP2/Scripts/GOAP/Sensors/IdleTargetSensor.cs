using CrashKonijn.Agent.Core;
using CrashKonijn.Goap.Runtime;
using Pathfinding;
using UnityEngine;

namespace GP2.GOAP
{
    public class IdleTargetSensor : LocalTargetSensorBase
    {
        private static readonly Bounds Bounds = new(Vector3.zero, new Vector2(15, 8));
        
        // Called when intialised
        public override void Created() { }

        // Caching data to be processed in Sense()
        public override void Update() { }

        public override ITarget Sense(IActionReceiver agent, IComponentReference references, ITarget existingTarget)
        {
            Vector2 random = GetRandomPosition(agent);
            if (existingTarget is PositionTarget positionTarget)
            {
                return positionTarget.SetPosition(random);
            }

            return new PositionTarget(random);
        }

        private Vector2 GetRandomPosition(IActionReceiver agent)
        {
            IAstarAI pathfinding = agent.Transform.gameObject.GetComponent<IAstarAI>();
            //pathfinding.
            if (AstarPath.active.isScanning)
            {
                return agent.Transform.position;
            }
            
            Vector3 random = Random.insideUnitCircle * 5f;
            Vector2 position = agent.Transform.position + random;
            
            NNInfoInternal node = AstarPath.active.graphs[0].GetNearest(position);
            
            while (!node.node.Walkable)
            {
                random = Random.insideUnitCircle * 5f;
                position = agent.Transform.position + random;
                node = AstarPath.active.graphs[0].GetNearest(position);
            }

            return node.clampedPosition;
            
            /*var random = Random.insideUnitCircle * 3f;
            var position = agent.Transform.position + new Vector3(random.x, 0f, random.y);

            // Check if the position is within the bounds of the world.
            if (Bounds.Contains(position))
                return position;

            return Bounds.ClosestPoint(position);*/
        }
    }
   
}