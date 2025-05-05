using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AlanZucconi.AI.BT;
using Pathfinding;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using WaitWhile = AlanZucconi.AI.BT.WaitWhile;

public class BehaviourTreeBase : MonoBehaviour
{
    // todo: blackboard or here?
    public int Damage;
    public LayerMask AttackLayerMask;
    public string EnemyTag;
        
    protected DungeonGenerator DungeonGenerator;
    protected IAstarAI Pathfinding;
    
    protected Node BT;
    protected Blackboard BB;
    
    // Start is called before the first frame update
    void Start()
    {
        DungeonGenerator = GameObject.FindGameObjectWithTag("DungeonGenerator").GetComponent<DungeonGenerator>();
        Pathfinding = GetComponent<IAstarAI>();
        BT = CreateBehaviourTree();
        BB = CreateBlackboard();
    }

    // Update is called once per frame
    void Update()
    {
        BT.Evaluate();
    }

    protected virtual Blackboard CreateBlackboard()
    {
        Blackboard blackboard = new Blackboard();
        blackboard.Set<float>("CurrentWaitTime", 5f);
        return blackboard;
    }

    protected virtual Node CreateBehaviourTree()
    {
        return Action.Nothing;
    }

    /* An area attack which hits multiple enemies in a radius.*/
    protected Node AttackNearbyEnemies(string enemyTag, float radius)
    {
        return new Action(() =>
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, AttackLayerMask);
            foreach (Collider2D collider in colliders)
            {
                GameObject enemy = collider.gameObject;
                HealthSystem enemyHealthSystem = enemy.GetComponent<HealthSystem>();
                if (enemyHealthSystem && !CompareTag(enemy.tag))
                {
                    enemyHealthSystem.ApplyDamage(Damage, (enemy.transform.position - transform.position).normalized);
                }
            }
        });
    }
    
    /* Singular attack on the nearest enemy.*/
    protected Node AttackNearestEnemy(string enemyTag)
    {
        return new Action(() =>
        {
            GameObject enemy = FindClosestEnemy(enemyTag);
            HealthSystem enemyHealthSystem = enemy.GetComponent<HealthSystem>();
            Vector2 direction = (enemy.transform.position - transform.position).normalized;
            enemyHealthSystem.ApplyDamage(Damage, direction);
        });
    }
    
    protected Node ChaseNearestEnemy(string enemyTag)
    {
        return new Task(() =>
        {
            GameObject enemy = FindClosestEnemy(enemyTag);

            if (enemy == null)
            {
                return Status.Failure;
            }
            
            Vector3 position = enemy.transform.position;
            Vector3 random = Random.insideUnitCircle * 2f;
            position += random;
            MoveTo(position);
            return Status.Success;
        });
        /*return new Action(() =>
        {
            Vector3 position = FindClosestEnemy(enemyTag).transform.position;
            Vector3 random = Random.insideUnitCircle * 2f;
            position += random;
            MoveTo(position);
        });*/
    }

    /* Return the closest enemy, dependant on the tag of the game object.*/
    protected GameObject FindClosestEnemy(string enemyTag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        
        if (enemies.Length == 0)
        {
            return null;
        }
        
        return enemies
            .OrderBy(adventurer => Vector3.SqrMagnitude(adventurer.transform.position - transform.position))
            .FirstOrDefault();
    }

    /* Determine if enemies are either in attack or chase range.
       This is used for filter conditions. */
    protected bool IsEnemyInRange(string enemyTag, float range)
    {
        return GameObject.FindGameObjectsWithTag(enemyTag)
            .Any(adventurer => Vector2.Distance(adventurer.transform.position, transform.position) <= range);
    }

    /* Wandering behaviour. Periodically move to a random location and wait
       a random amount of time.*/
    protected Node Wander(float range, float minWait, float maxWait)
    {
        return new Sequence(
            WaitForSeconds(Random.Range(minWait, maxWait)),
            new Action(() => MoveTo(GetRandomPosition(range)))
        );
    }

    /* Task node which returns running until a timer is out.*/
    protected Node WaitForSeconds(float seconds)
    {
        return new Task(() =>
        {
            //print("Waiting");
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

    /* Move to destination given.*/
    protected void MoveTo(Vector3 position)
    {
        //print("Moving");
        Pathfinding.destination = position;
        Pathfinding.SearchPath();
        /*if (!Pathfinding.pathPending && (Pathfinding.reachedEndOfPath || !Pathfinding.hasPath))
        {
            Pathfinding.destination = position;
            Pathfinding.SearchPath();
        }*/
    }
    
    /* Get a random position based on range given.*/
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
