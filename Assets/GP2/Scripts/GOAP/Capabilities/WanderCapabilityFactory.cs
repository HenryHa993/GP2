using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace  GP2.GOAP
{
    public class WanderCapabilityFactory : CapabilityFactoryBase
    {
        public override ICapabilityConfig Create()
        {
            var builder = new CapabilityBuilder("WanderCapability");

            builder.AddGoal<WanderGoal>()
                .AddCondition<IsWandering>(Comparison.GreaterThanOrEqual, 1)
                .SetBaseCost(2);

            builder.AddAction<WanderAction>()
                .AddEffect<IsWandering>(EffectType.Increase)
                .SetTarget<WanderTargetKey>();

            builder.AddTargetSensor<WanderTargetSensor>()
                .SetTarget<WanderTargetKey>();

            return builder.Build();
        }
    }
}