using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GP2.GOAP
{
    public class FoodSensor : MultiSensorBase
    {
        private FoodBehaviour[] Food;

        // Use constructor to register all sensors
        public FoodSensor()
        {
            AddLocalWorldSensor<FoodCount>((agent, references) =>
            {
                // Get data behaviour from agent
                var data = references.GetCachedComponent<DataBehaviour>();
                return data.FoodCount;
            });
            
            this.AddLocalWorldSensor<Hunger>((agent, references) =>
            {
                var data = references.GetCachedComponent<DataBehaviour>();
                return (int) data.Hunger;
            });

            this.AddLocalTargetSensor<ClosestFood>((agent, references, target) =>
            {
                // Find the closest food to the agent
                var closestFood = Closest(Food, agent.Transform.position);

                if (closestFood == null)
                    return null;

                // If the target is a transform target, set the target to the closest pear
                if (target is TransformTarget transformTarget)
                    return transformTarget.SetTransform(closestFood.transform);

                return new TransformTarget(closestFood.transform);
            });
        }
        
        public override void Created()
        {
            
        }

        public override void Update()
        {
            Food = Object.FindObjectsOfType<FoodBehaviour>();
        }
        
        private T Closest<T>(IEnumerable<T> list, Vector3 position)
            where T : MonoBehaviour
        {
            T closest = null;
            var closestDistance = float.MaxValue; // Start with the largest possible distance

            foreach (var item in list)
            {
                var distance = Vector3.Distance(item.gameObject.transform.position, position);

                if (!(distance < closestDistance))
                    continue;

                closest = item;
                closestDistance = distance;
            }

            return closest;
        }
    }
}