using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlanZucconi.AI.BT;
using Pathfinding;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using WaitWhile = AlanZucconi.AI.BT.WaitWhile;

public class AdventurerBT : MonoBehaviour
{
    public DungeonGenerator DungeonGenerator;
    
    protected IAstarAI Pathfinding;
    
    protected Node BT;
    protected Blackboard BB;
    
    // Start is called before the first frame update
    void Start()
    {
        //DungeonGenerator = GameObject.FindGameObjectWithTag() GetComponent<DungeonGenerator>();
        Pathfinding = GetComponent<IAstarAI>();
        BT = CreateBehaviourTree();
        BB = CreateBlackboard();
    }

    // Update is called once per frame
    void Update()
    {
        BT.Evaluate();
    }

    Blackboard CreateBlackboard()
    {
        Blackboard blackboard = new Blackboard();
        blackboard.Set<float>("CurrentWaitTime", 5f);
        return blackboard;
    }

    Node CreateBehaviourTree()
    {
        return new Selector(
            new Filter(
                () => IsEnemyInRange(3f), // Attack range
                AttackEnemy()
                ),
            new Filter(
                () => IsEnemyInRange(10f), // Chasing range
                ChaseEnemy()
                ),
            Wander(5f, 5f)
            );
    }
    
    protected Node AttackEnemy()
    {
        return new Sequence();
    }
    
    protected Node ChaseEnemy()
    {
        return new Action(() =>
        {
            Vector3 position = FindClosestEnemy().transform.position;
            MoveTo(position);
        });
    }

    /* Return the closest enemy, dependant on the tag of the game object.*/
    protected GameObject FindClosestEnemy()
    {
        return GameObject.FindGameObjectsWithTag("Adventurer")
            .OrderBy(adventurer => Vector3.SqrMagnitude(adventurer.transform.position - transform.position))
            .FirstOrDefault();
    }

    /* Determine if enemies are either in attack or chase range.
       This is used for filter conditions. */
    protected bool IsEnemyInRange(float range)
    {
        return GameObject.FindGameObjectsWithTag("Adventurer")
            .Any(adventurer => Vector2.Distance(adventurer.transform.position, transform.position) <= range);
    }

    protected Node Wander(float range, float waitTime)
    {
        return new Sequence(
            WaitForSeconds(waitTime),
            new Action(() => MoveTo(GetRandomPosition(range)))
        );
    }

    protected Node WaitForSeconds(float seconds)
    {
        return new Task(() =>
        {
            print("Waiting");
            float waitTime = BB.Get<float>("CurrentWaitTime") - Time.deltaTime;
            BB.Set<float>("CurrentWaitTime", waitTime);
            if (waitTime < 0f)
            {
                BB.Set<float>("CurrentWaitTime", seconds);
                return Status.Success;
            }

            return Status.Running;
        });
    }

    protected void MoveTo(Vector3 position)
    {
        print("Moving");
        Pathfinding.destination = position;
        Pathfinding.SearchPath();
        /*if (!Pathfinding.pathPending && (Pathfinding.reachedEndOfPath || !Pathfinding.hasPath))
        {
            Pathfinding.destination = position;
            Pathfinding.SearchPath();
        }*/
    }
    
    protected Vector3 GetRandomPosition(float range)  
    {  
        Vector3 random = Random.insideUnitCircle * range;  
        Vector2 position = transform.position + random;  
        Vector2Int tilePosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));  
        
        // Todo might be worth limiting this  
        while (!DungeonGenerator.DungeonFloor.Contains(tilePosition))  
        {
            random = Random.insideUnitCircle * range;  
            position = transform.position + random;  
            tilePosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));  
        }  
        return new Vector3(tilePosition.x, tilePosition.y);  
    }
}
