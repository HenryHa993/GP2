using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using CrashKonijn.Agent.Runtime;
using Pathfinding;
using UnityEngine;

namespace GP2.GOAP
{
    public class AgentMoveBehaviour : MonoBehaviour
    {
        private AgentBehaviour agent;
        private ITarget currentTarget;
        private bool shouldMove;
        private IAstarAI pathfinding;

        private void Awake()
        {
            agent = GetComponent<AgentBehaviour>();
            pathfinding = GetComponent<IAstarAI>();
        }

        private void OnEnable()
        {
            agent.Events.OnTargetInRange += OnTargetInRange;
            agent.Events.OnTargetChanged += OnTargetChanged;
            agent.Events.OnTargetNotInRange += TargetNotInRange;
            agent.Events.OnTargetLost += TargetLost;
        }

        private void OnDisable()
        {
            agent.Events.OnTargetInRange -= OnTargetInRange;
            agent.Events.OnTargetChanged -= OnTargetChanged;
            agent.Events.OnTargetNotInRange -= TargetNotInRange;
            agent.Events.OnTargetLost -= TargetLost;
        }
        
        private void TargetLost()
        {
            currentTarget = null;
            shouldMove = false;
        }

        private void OnTargetInRange(ITarget target)
        {
            shouldMove = false;
        }

        private void OnTargetChanged(ITarget target, bool inRange)
        {
            currentTarget = target;
            shouldMove = !inRange;
        }

        private void TargetNotInRange(ITarget target)
        {
            shouldMove = true;
        }

        // This is where the behaviour is written.
        public void Update()
        {
          if (agent.IsPaused)
                return;

            if (!shouldMove)
                return;
            
            if (currentTarget == null)
                return;
            
            // This is happeneing before so the path isn't even being calculated.
            if (!pathfinding.pathPending && (pathfinding.reachedEndOfPath || !pathfinding.hasPath))
            {
                pathfinding.destination = currentTarget.Position;
                pathfinding.SearchPath();
            }

            //transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentTarget.Position.x, transform.position.y, currentTarget.Position.z), Time.deltaTime);
        }

        private void OnDrawGizmos()
        {
            if (currentTarget == null)
                return;
            
            Gizmos.DrawLine(transform.position, currentTarget.Position);
        }
    }
}