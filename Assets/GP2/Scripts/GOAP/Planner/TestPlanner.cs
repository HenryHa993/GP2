using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using CrashKonijn.Goap.Runtime;
using UnityEngine;

namespace GP2.GOAP
{
    public class TestPlanner : MonoBehaviour
    {
        private AgentBehaviour agent;
        private GoapActionProvider provider;
        private GoapBehaviour goap;
        private DataBehaviour Data;
        
        private void Awake()
        {
            this.goap = FindObjectOfType<GoapBehaviour>();
            this.agent = this.GetComponent<AgentBehaviour>();
            this.provider = this.GetComponent<GoapActionProvider>();
            this.Data = this.GetComponent<DataBehaviour>();
            
            // This only applies sto the code demo
            if (this.provider.AgentTypeBehaviour == null)
                this.provider.AgentType = this.goap.GetAgentType("TestAgent");
        }

        private void Start()
        {
            //this.provider.RequestGoal<IdleGoal, PickupFoodGoal>();
            provider.RequestGoal<WanderGoal>();
        }

        private void OnEnable()
        {
            this.agent.Events.OnActionEnd += this.OnActionEnd;
        }

        private void OnDisable()
        {
            this.agent.Events.OnActionEnd -= this.OnActionEnd;
        }

        private void OnActionEnd(IAction action)
        {
            /*if (this.Data.Hunger > 50)
            {
                this.provider.RequestGoal<EatGoal>();
                return;
            }
            
            this.provider.RequestGoal<IdleGoal, PickupFoodGoal>();*/
            
            provider.RequestGoal<WanderGoal>();
        }
    }

}