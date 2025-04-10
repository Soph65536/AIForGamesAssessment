using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class CheckForEnemiesInView : Node
{
    public CheckForEnemiesInView(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        if (owner._agentSenses.GetEnemiesInView() == null)
        {
            //no enemies so failure
            state = NodeState.FAILURE;
            Debug.Log("CheckForEnemiesInView FAILURE");
            return state;
        }

        //yes enemies so success
        state = NodeState.SUCCESS;
        Debug.Log("CheckForEnemiesInView SUCCESS");
        return state;
    }
}

public class CheckEnemiesForFriendlyFlag : Node
{
    public CheckEnemiesForFriendlyFlag(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        List<GameObject> enemies = owner._agentSenses.GetEnemiesInView();

        if (enemies == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("CheckEnemiesForFriendlyFlag FAILURE");
            return state;
        }

        foreach (GameObject enemy in enemies)
        {
            AgentData enemyAgentData = enemy.GetComponent<AgentData>();

            if (enemyAgentData == null)
            {
                state = NodeState.FAILURE;
                Debug.Log("CheckEnemiesForFriendlyFlag FAILURE");
                return state;
            }

            if (enemyAgentData.HasEnemyFlag)
            {
                //there is enemy with agent data saying they have the flag
                //so we shall change target enemy to this enemy and return success
                GetRootNode().SetData("targetEnemy", enemy);
                state = NodeState.SUCCESS;
                Debug.Log("CheckEnemiesForFriendlyFlag SUCCESS");
                return state;
            }
        }

        //if no enemy found with the flag then failure
        state = NodeState.FAILURE;
        Debug.Log("CheckEnemiesForFriendlyFlag FAILURE");
        return state;
    }
}

public class CheckEnemiesForEnemyFlag : Node
{
    public CheckEnemiesForEnemyFlag(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        List<GameObject> enemies = owner._agentSenses.GetEnemiesInView();

        if (enemies == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("CheckEnemiesForEnemyFlag FAILURE");
            return state;
        }

        foreach (GameObject enemy in enemies)
        {
            AgentData enemyAgentData = enemy.GetComponent<AgentData>();

            if (enemyAgentData == null)
            {
                state = NodeState.FAILURE;
                Debug.Log("CheckEnemiesForEnemyFlag FAILURE");
                return state;
            }

            if (enemyAgentData.HasFriendlyFlag)
            {
                //there is enemy with agent data saying they have the flag
                //so we shall change target enemy to this enemy and return success
                GetRootNode().SetData("targetEnemy", enemy);
                state = NodeState.SUCCESS;
                Debug.Log("CheckEnemiesForEnemyFlag SUCCESS");
                return state;
            }
        }

        //if no enemy found with the flag then failure
        state = NodeState.FAILURE;
        Debug.Log("CheckEnemiesForEnemyFlag FAILURE");
        return state;
    }
}

public class SetClosestEnemyAsTarget : Node
{
    private Transform AITransform;

    public SetClosestEnemyAsTarget(AI ownerTree, Transform _transform)
    {
        owner = ownerTree;
        AITransform = _transform;
    }

    public override NodeState Evaluate()
    {
        GameObject closestEnemy = owner._agentSenses.GetNearestEnemyInView();

        if (closestEnemy == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("SetClosestEnemyAsTarget FAILURE");
            return state;
        }

        GetRootNode().SetData("targetEnemy", closestEnemy);
        state = NodeState.SUCCESS;
        Debug.Log("SetClosestEnemyAsTarget SUCCESS");
        return state;

        //////i implemented a thing to find nearest enemy but there was already a functions for it
        //List<GameObject> enemies = AI._agentSenses.GetEnemiesInView();

        //if (enemies == null)
        //{
        //    state = NodeState.FAILURE;
        //    return state;
        //}
        //else
        //{
        //    //create list of enemies which also contains how far away they are from the player

        //    List<GameObjectWithDistance> enemiesWithDistance = new List<GameObjectWithDistance>();

        //    foreach (GameObject enemy in enemies)
        //    {
        //        enemiesWithDistance.Add(new GameObjectWithDistance(
        //            enemy, Vector3.Distance(AITransform.position, enemy.transform.position)
        //            ));
        //    }

        //    //check which enemy is closest to player and target that

        //    float smallestEnemyDistance = Mathf.Infinity;
        //    GameObject closestEnemy = null;

        //    foreach (GameObjectWithDistance enemy in enemiesWithDistance)
        //    {
        //        if (enemy.distance < smallestEnemyDistance) { closestEnemy = enemy.gameObject; }
        //    }

        //    if (closestEnemy == null)
        //    {
        //        state = NodeState.FAILURE;
        //        return state;
        //    }
        //    else
        //    {
        //        //we set closest enemy as target and return success
        //        AI.targetEnemy = closestEnemy;
        //        state = NodeState.SUCCESS;
        //        return state;
        //    }
        //}
    }
}

public class AttackAnEnemy : Node
{
    public AttackAnEnemy(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        GameObject targetEnemy = (GameObject)GetData("targetEnemy");

        if (targetEnemy == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("AttackAnEnemy FAILURE");
            return state;
        }

        if (owner._agentSenses.IsInAttackRange(targetEnemy))
        {
            owner._agentActions.AttackEnemy(targetEnemy);
            state = NodeState.SUCCESS;
            Debug.Log("AttackAnEnemy SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(targetEnemy);

        state = NodeState.RUNNING;
        Debug.Log("AttackAnEnemy RUNNING");
        return state;
    }
}