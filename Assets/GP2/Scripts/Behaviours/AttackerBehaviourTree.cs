using System.Collections;
using System.Collections.Generic;
using AlanZucconi.AI.BT;
using UnityEngine;

public class AttackerBehaviourTree : BehaviourTreeBase
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
            new Sequence(
                WaitForSeconds(0.5f),
                ChaseNearestEnemy(EnemyTag)));
    }
}
