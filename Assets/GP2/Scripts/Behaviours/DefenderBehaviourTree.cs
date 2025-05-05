using System.Collections;
using System.Collections.Generic;
using AlanZucconi.AI.BT;
using UnityEngine;

public class DefenderBehaviourTree : BehaviourTreeBase
{
    protected override Node CreateBehaviourTree()
    {
        return new Selector(
            new Filter(
                () => IsEnemyInRange(EnemyTag, 3f), // Attack range
                new Sequence(
                    WaitForSeconds(1f),
                    AttackNearbyEnemies(EnemyTag, 3f)
                )),
            new Filter(
                () => IsEnemyInRange(EnemyTag, 10f), // Chasing range
                ChaseNearestEnemy(EnemyTag)
            ),
            Wander(5f, 2f, 5f)
        );
    }
}
