using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Demos.Complex.Goals;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GP2.GOAP
{
    public class FoodCapability : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("FoodCapability");
            
            builder.AddGoal<PickupFoodGoal>()
                .AddCondition<FoodCount>(Comparison.GreaterThanOrEqual, 3);

            builder.AddAction<PickupFoodAction>()
                .AddEffect<FoodCount>(EffectType.Increase)
                .SetTarget<ClosestFood>();

            builder.AddMultiSensor<FoodSensor>();

            return builder.Build();
        }
    }
   
}