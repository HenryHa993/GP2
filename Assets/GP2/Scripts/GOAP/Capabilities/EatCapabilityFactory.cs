using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GP2.GOAP
{
    public class EatCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("EatCapability");

            builder.AddGoal<EatGoal>()
                .AddCondition<Hunger>(Comparison.SmallerThanOrEqual, 0);

            builder.AddAction<EatAction>()
                .AddCondition<FoodCount>(Comparison.GreaterThanOrEqual, 1)
                .AddEffect<Hunger>(EffectType.Decrease)
                .SetRequiresTarget(false); // This just marks it for the graph.

            return builder.Build();
        }
    }
}