using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class CheckForObjectsInView : Node
{
    public CheckForObjectsInView(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        if(owner._agentSenses.GetObjectsInView() == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("CheckForObjectsInView FAILURE");
            return state;
        }

        state = NodeState.SUCCESS;
        Debug.Log("CheckForObjectsInView SUCCESS");
        return state;
    }
}

public class CheckForLowHealth : Node
{
    public CheckForLowHealth(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        if (owner._agentData.CurrentHitPoints > 15)
        {
            //health is too high so failure
            state = NodeState.FAILURE;
            Debug.Log("CheckForLowHealth FAILURE");
            return state;
        }

        //health is low so node succeeds
        state = NodeState.SUCCESS;
        Debug.Log("CheckForLowHealth SUCCESS");
        return state;
    }
}

public class MoveToRandomLocation : Node
{
    public MoveToRandomLocation(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        //this state is always the final leaf that is relayed to when nothing else is doable
        //so i made it so it always succeeds
        owner._agentActions.MoveToRandomLocation();
        state = NodeState.SUCCESS;
        Debug.Log("MoveToRandomLocation SUCCESS");
        return state;
    }
}

public class TryFindHealthKit : Node
{
    public TryFindHealthKit(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        GameObject healthKit = owner._agentSenses.GetObjectInViewByName(Names.HealthKit);

        if (healthKit == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryFindHealthKit FAILURE");
            return state;
        }

        if (owner._agentSenses.IsItemInReach(healthKit))
        {
            owner._agentActions.CollectItem(healthKit);
            state = NodeState.SUCCESS;
            Debug.Log("TryFindHealthKit SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(healthKit);

        state = NodeState.RUNNING;
        Debug.Log("TryFindHealthKit RUNNING");
        return state;
    }
}

public class HealSelf : Node
{
    public HealSelf(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        //get gameobject ref to healthkit in inventory
        GameObject healthKit = owner._agentInventory.GetItem(Names.HealthKit);

        //if no health kit or health > maxheath - healamount then can't heal self
        if (healthKit == null || owner._agentData.CurrentHitPoints > 90)
        {
            state = NodeState.FAILURE;
            Debug.Log("HealSelf FAILURE");
            return state;
        }

        //heal self
        owner._agentActions.UseItem(healthKit);
        state = NodeState.SUCCESS;
        Debug.Log("HealSelf SUCCESS");
        return state;
    }
}

public class HealOthers : Node
{
    public HealOthers(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        //get gameobject ref to healthkit in inventory
        GameObject healthKit = owner._agentInventory.GetItem(Names.HealthKit);

        //if no health kit then can't heal people
        if (healthKit == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("HealOthers FAILURE");
            return state;
        }


        //heal others
        List<GameObject> friendlies = owner._agentSenses.GetFriendliesInView();

        if(friendlies == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("HealOthers FAILURE");
            return state;
        }

        //this is set to the highest health number that a friend could have whilst still needing to be healed
        int lowestFriendHealth = 50;
        GameObject friendToHeal = null;

        foreach (GameObject friend in friendlies)
        {
            AgentData friendAgentData = friend.GetComponent<AgentData>();

            if(friendAgentData == null)
            {
                state = NodeState.FAILURE;
                Debug.Log("HealOthers FAILURE");
                return state;
            }

            //if friend health is less than lowest friend health then set them to be the friend to heal
            if (friendAgentData.CurrentHitPoints < lowestFriendHealth)
            {
                lowestFriendHealth = friendAgentData.CurrentHitPoints;
                friendToHeal = friend;
            }
        }

        if (friendToHeal == null)
        {
            //if noone needed healing then failure
            state = NodeState.FAILURE;
            Debug.Log("HealOthers FAILURE");
            return state;
        }

        if (owner._agentSenses.IsItemInReach(friendToHeal))
        {
            AgentActions friendToHealAgentActions = friendToHeal.GetComponent<AgentActions>();

            if(friendToHealAgentActions == null)
            {
                state = NodeState.FAILURE;
                Debug.Log("HealOthers FAILURE");
                return state;
            }

            //if friend object is within reach of us
            //and the ref to their agentActions script is valid
            //then we can drop our health kit for them to pickup
            owner._agentActions.DropItem(healthKit);

            //make the friend pickup and use the health kit
            friendToHealAgentActions.CollectItem(healthKit);
            friendToHealAgentActions.UseItem(healthKit);

            state = NodeState.SUCCESS;
            Debug.Log("HealOthers SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(friendToHeal);

        state = NodeState.RUNNING;
        Debug.Log("HealOthers RUNNING");
        return state;
    }
}

public class TryForPowerup : Node
{
    public TryForPowerup(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        GameObject powerup = owner._agentSenses.GetObjectInViewByName(Names.PowerUp);

        if (powerup == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryForPowerup FAILURE");
            return state;
        }

        if (owner._agentSenses.IsItemInReach(powerup))
        {
            owner._agentActions.CollectItem(powerup);
            owner._agentActions.UseItem(powerup);
            state = NodeState.SUCCESS;
            Debug.Log("TryForPowerup SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(powerup);

        state = NodeState.RUNNING;
        Debug.Log("TryForPowerup RUNNING");
        return state;
    }
}

public class TryForFriendlyFlag : Node
{
    private string friendlyFlagName; //name of whatever flag is friendly flag

    public TryForFriendlyFlag(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        //set friendly flag name based on owner tag
        friendlyFlagName = owner.tag == Tags.BlueTeam ? Names.BlueFlag : Names.RedFlag;

        GameObject friendlyFlag = owner._agentSenses.GetObjectInViewByName(friendlyFlagName);

        if (friendlyFlag == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryForFriendlyFlag FAILURE");
            return state;
        }

        //if friendly flag is within range of friendly base then return failure
        //since it's already at base and doesn't want picking up
        if (Vector3.Distance(friendlyFlag.transform.position, owner._agentData.FriendlyBase.transform.position) < 2)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryForFriendlyFlag FAILURE, already at base");
            return state;
        }

        if (owner._agentSenses.IsItemInReach(friendlyFlag))
        {
            owner._agentActions.CollectItem(friendlyFlag);
            state = NodeState.SUCCESS;
            Debug.Log("TryForFriendlyFlag SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(friendlyFlag);

        state = NodeState.RUNNING;
        Debug.Log("TryForFriendlyFlag RUNNING");
        return state;
    }
}

public class TryForEnemyFlag : Node
{
    private string enemyFlagName; //name of whatever flag is enemy flag

    public TryForEnemyFlag(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        //set enemy flag name based on AITag
        enemyFlagName = owner.tag == Tags.BlueTeam ? Names.RedFlag : Names.BlueFlag;

        GameObject enemyFlag = owner._agentSenses.GetObjectInViewByName(enemyFlagName);

        if (enemyFlag == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryForEnemyFlag FAILURE");
            return state;
        }

        //if enemy flag is within range of enemy base then return failure
        //since it's already at base and doesn't want picking up
        if (Vector3.Distance(enemyFlag.transform.position, owner._agentData.FriendlyBase.transform.position) < 2)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryForEnemyFlag FAILURE, already at base");
            return state;
        }

        if (owner._agentSenses.IsItemInReach(enemyFlag))
        {
            owner._agentActions.CollectItem(enemyFlag);
            state = NodeState.SUCCESS;
            Debug.Log("TryForEnemyFlag SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(enemyFlag);

        state = NodeState.RUNNING;
        Debug.Log("TryForEnemyFlag RUNNING");
        return state;
    }
}

public class BringFlagsToBase : Node
{
    public BringFlagsToBase(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        //get gameobject refs for flags
        GameObject blueFlag = owner._agentInventory.GetItem(Names.BlueFlag);
        GameObject redFlag = owner._agentInventory.GetItem(Names.RedFlag);

        if (blueFlag == null && redFlag == null)
        {
            //doesn't have any flag so failure
            state = NodeState.FAILURE;
            Debug.Log("BringFlagsToBase FAILURE");
            return state;
        }

        if (blueFlag != null && 
            owner._agentInventory.HasItem(Names.BlueFlag) &&
            Vector3.Distance(owner.transform.position, owner._agentData.FriendlyBase.transform.position) < 2)
        {
            //if has blue flag drop it at base
            owner._agentActions.DropItem(blueFlag);
            state = NodeState.SUCCESS;
            Debug.Log("BringFlagsToBase SUCCESS");
            return state;
        }

        if (redFlag != null &&
            owner._agentInventory.HasItem(Names.RedFlag) &&
            Vector3.Distance(owner.transform.position, owner._agentData.FriendlyBase.transform.position) < 2)
        {
            //if has red flag drop it at base
            owner._agentActions.DropItem(redFlag);
            state = NodeState.SUCCESS;
            Debug.Log("BringFlagsToBase SUCCESS");
            return state;
        }

        //go to base
        owner._agentActions.MoveTo(owner._agentData.FriendlyBase);

        state = NodeState.RUNNING;
        Debug.Log("BringFlagsToBase RUNNING");
        return state;
    }
}

public class FleeFromEnemies : Node
{
    public FleeFromEnemies(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        GameObject closestEnemy = owner._agentSenses.GetNearestEnemyInView();

        if (closestEnemy == null)
        {
            //no enemies so return fail
            state = NodeState.FAILURE;
            Debug.Log("FleeFromEnemies FAILURE");
            return state;
        }

        //there is an enemy so run from it and return success
        owner._agentActions.Flee(closestEnemy);
        state = NodeState.SUCCESS;
        Debug.Log("FleeFromEnemies SUCCESS");
        return state;
    }
}

public class MoveToBase : Node
{
    public MoveToBase(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        //go to base
        owner._agentActions.MoveTo(owner._agentData.FriendlyBase);
        state = NodeState.SUCCESS;
        Debug.Log("MoveToBase SUCCESS");
        return state;
    }
}