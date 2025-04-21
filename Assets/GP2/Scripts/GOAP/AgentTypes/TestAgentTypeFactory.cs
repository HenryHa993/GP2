using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Core;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GP2.GOAP
{
    public class TestAgentTypeFactory : AgentTypeFactoryBase
    {
        public override IAgentTypeConfig Create()
        {
            var builder = new AgentTypeBuilder("TestAgent");
            builder.AddCapability<IdleCapabilityFactory>();
            //builder.AddCapability<FoodCapability>();
            return builder.Build();
        }
    }
}