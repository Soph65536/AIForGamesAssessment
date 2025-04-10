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
        GameObject healthKit = owner._agentSenses.GetObjectInViewByName(Names.HealthKit);

        //if no health kit then can't heal self
        if (!owner._agentInventory.HasItem(Names.HealthKit) || healthKit == null)
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
        GameObject healthKit = owner._agentSenses.GetObjectInViewByName(Names.HealthKit);

        //if no health kit then can't heal self
        if (!owner._agentInventory.HasItem(Names.HealthKit) || healthKit == null)
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

public class TryForBlueFlag : Node
{
    public TryForBlueFlag(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        GameObject blueFlag = owner._agentSenses.GetObjectInViewByName(Names.BlueFlag);

        if (blueFlag == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryForBlueFlag FAILURE");
            return state;
        }

        if (owner._agentSenses.IsItemInReach(blueFlag))
        {
            owner._agentActions.CollectItem(blueFlag);
            state = NodeState.SUCCESS;
            Debug.Log("TryForBlueFlag SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(blueFlag);

        state = NodeState.RUNNING;
        Debug.Log("TryForBlueFlag RUNNING");
        return state;
    }
}

public class TryForRedFlag : Node
{
    public TryForRedFlag(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        GameObject redFlag = owner._agentSenses.GetObjectInViewByName(Names.RedFlag);

        if (redFlag == null)
        {
            state = NodeState.FAILURE;
            Debug.Log("TryForRedFlag FAILURE");
            return state;
        }

        if (owner._agentSenses.IsItemInReach(redFlag))
        {
            owner._agentActions.CollectItem(redFlag);
            state = NodeState.SUCCESS;
            Debug.Log("TryForRedFlag SUCCESS");
            return state;
        }

        owner._agentActions.MoveTo(redFlag);

        state = NodeState.RUNNING;
        Debug.Log("TryForRedFlag RUNNING");
        return state;
    }
}

public class BringFlagsToBase : Node
{
    public BringFlagsToBase(AI ownerTree) : base(ownerTree) { }

    public override NodeState Evaluate()
    {
        if (!owner._agentData.HasFriendlyFlag && !owner._agentData.HasEnemyFlag)
        {
            //doesn't have any flag so failure
            state = NodeState.FAILURE;
            Debug.Log("BringFlagsToBase FAILURE");
            return state;
        }

        //go to base
        owner._agentActions.MoveTo(owner._agentData.FriendlyBase);

        //get gameobject refs for flags
        GameObject blueFlag = owner._agentSenses.GetObjectInViewByName(Names.BlueFlag);
        GameObject redFlag = owner._agentSenses.GetObjectInViewByName(Names.RedFlag);

        if(blueFlag == null && redFlag == null)
        {
            //doesn't have any flag so failure
            state = NodeState.FAILURE;
            Debug.Log("BringFlagsToBase FAILURE");
            return state;
        }

        if (blueFlag != null && 
            owner._agentInventory.HasItem(Names.BlueFlag))
        {
            //if has blue flag drop it at base
            owner._agentActions.DropItem(blueFlag);
        }

        if (redFlag != null &&
            owner._agentInventory.HasItem(Names.RedFlag))
        {
            //if has red flag drop it at base
            owner._agentActions.DropItem(redFlag);
        }

        state = NodeState.SUCCESS;
        Debug.Log("BringFlagsToBase SUCCESS");
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