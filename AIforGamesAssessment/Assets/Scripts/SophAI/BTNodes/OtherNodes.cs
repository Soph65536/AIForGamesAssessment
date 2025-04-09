using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

public class CheckForObjectsInView : Node
{
    public CheckForObjectsInView()
    {
    }

    public override NodeState Evaluate()
    {
        if(AI._agentSenses.GetObjectsInView() == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}

public class CheckForLowHealth : Node
{
    public CheckForLowHealth()
    {
    }

    public override NodeState Evaluate()
    {
        if (AI._agentData.CurrentHitPoints > 15)
        {
            //health is too high so failure
            state = NodeState.FAILURE;
            return state;
        }

        //health is low so node succeeds
        state = NodeState.SUCCESS;
        return state;
    }
}

public class MoveToRandomLocation : Node
{
    public MoveToRandomLocation()
    {
    }

    public override NodeState Evaluate()
    {
        //this state is always the final leaf that is relayed to when nothing else is doable
        //so i made it so it always succeeds
        AI._agentActions.MoveToRandomLocation();
        state = NodeState.SUCCESS;
        return state;
    }
}

public class TryFindHealthKit : Node
{
    public TryFindHealthKit()
    {
    }

    public override NodeState Evaluate()
    {
        GameObject healthKit = AI._agentSenses.GetObjectInViewByName(Names.HealthKit);

        if (healthKit == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (AI._agentSenses.IsItemInReach(healthKit))
        {
            AI._agentActions.CollectItem(healthKit);
            state = NodeState.SUCCESS;
            return state;
        }

        AI._agentActions.MoveTo(healthKit);

        state = NodeState.RUNNING;
        return state;
    }
}

public class HealSelf : Node
{
    public HealSelf()
    {
    }

    public override NodeState Evaluate()
    {
        //get gameobject ref to healthkit in inventory
        GameObject healthKit = AI._agentSenses.GetObjectInViewByName(Names.HealthKit);

        //if no health kit then can't heal self
        if (!AI._agentInventory.HasItem(Names.HealthKit) || healthKit == null)
        {
            state = NodeState.FAILURE;
            return state;
        }


        //heal self
        AI._agentActions.UseItem(healthKit);
        state = NodeState.SUCCESS;
        return state;
    }
}

public class HealOthers : Node
{
    public HealOthers()
    {
    }

    public override NodeState Evaluate()
    {
        //get gameobject ref to healthkit in inventory
        GameObject healthKit = AI._agentSenses.GetObjectInViewByName(Names.HealthKit);

        //if no health kit then can't heal self
        if (!AI._agentInventory.HasItem(Names.HealthKit) || healthKit == null)
        {
            state = NodeState.FAILURE;
            return state;
        }


        //heal others
        List<GameObject> friendlies = AI._agentSenses.GetFriendliesInView();

        if(friendlies == null)
        {
            state = NodeState.FAILURE;
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
            return state;
        }

        if (AI._agentSenses.IsItemInReach(friendToHeal))
        {
            AgentActions friendToHealAgentActions = friendToHeal.GetComponent<AgentActions>();

            if(friendToHealAgentActions == null)
            {
                state = NodeState.FAILURE;
                return state;
            }

            //if friend object is within reach of us
            //and the ref to their agentActions script is valid
            //then we can drop our health kit for them to pickup
            AI._agentActions.DropItem(healthKit);

            //make the friend pickup and use the health kit
            friendToHealAgentActions.CollectItem(healthKit);
            friendToHealAgentActions.UseItem(healthKit);

            state = NodeState.SUCCESS;
            return state;
        }

        AI._agentActions.MoveTo(friendToHeal);

        state = NodeState.RUNNING;
        return state;
    }
}

public class TryForPowerup : Node
{
    public TryForPowerup()
    {
    }

    public override NodeState Evaluate()
    {
        GameObject powerup = AI._agentSenses.GetObjectInViewByName(Names.PowerUp);

        if (powerup == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (AI._agentSenses.IsItemInReach(powerup))
        {
            AI._agentActions.CollectItem(powerup);
            AI._agentActions.UseItem(powerup);
            state = NodeState.SUCCESS;
            return state;
        }

        AI._agentActions.MoveTo(powerup);

        state = NodeState.RUNNING;
        return state;
    }
}

public class TryForBlueFlag : Node
{
    public TryForBlueFlag()
    {
    }

    public override NodeState Evaluate()
    {
        GameObject blueFlag = AI._agentSenses.GetObjectInViewByName(Names.BlueFlag);

        if (blueFlag == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (AI._agentSenses.IsItemInReach(blueFlag))
        {
            AI._agentActions.CollectItem(blueFlag);
            state = NodeState.SUCCESS;
            return state;
        }

        AI._agentActions.MoveTo(blueFlag);

        state = NodeState.RUNNING;
        return state;
    }
}

public class TryForRedFlag : Node
{
    public TryForRedFlag()
    {
    }

    public override NodeState Evaluate()
    {
        GameObject redFlag = AI._agentSenses.GetObjectInViewByName(Names.RedFlag);

        if (redFlag == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        if (AI._agentSenses.IsItemInReach(redFlag))
        {
            AI._agentActions.CollectItem(redFlag);
            state = NodeState.SUCCESS;
            return state;
        }

        AI._agentActions.MoveTo(redFlag);

        state = NodeState.RUNNING;
        return state;
    }
}

public class BringFlagsToBase : Node
{
    public BringFlagsToBase()
    {
    }

    public override NodeState Evaluate()
    {
        if (!AI._agentData.HasFriendlyFlag && !AI._agentData.HasEnemyFlag)
        {
            //doesn't have any flag so failure
            state = NodeState.FAILURE;
            return state;
        }

        //go to base
        AI._agentActions.MoveTo(AI._agentData.FriendlyBase);

        //get gameobject refs for flags
        GameObject blueFlag = AI._agentSenses.GetObjectInViewByName(Names.BlueFlag);
        GameObject redFlag = AI._agentSenses.GetObjectInViewByName(Names.RedFlag);

        if(blueFlag == null && redFlag == null)
        {
            //doesn't have any flag so failure
            state = NodeState.FAILURE;
            return state;
        }

        if (blueFlag != null && 
            AI._agentInventory.HasItem(Names.BlueFlag))
        {
            //if has blue flag drop it at base
            AI._agentActions.DropItem(blueFlag);
        }

        if (redFlag != null &&
            AI._agentInventory.HasItem(Names.RedFlag))
        {
            //if has red flag drop it at base
            AI._agentActions.DropItem(redFlag);
        }

        state = NodeState.SUCCESS;
        return state;
    }
}

public class FleeFromEnemies : Node
{
    public FleeFromEnemies()
    {
    }

    public override NodeState Evaluate()
    {
        GameObject closestEnemy = AI._agentSenses.GetNearestEnemyInView();

        if (closestEnemy == null)
        {
            //no enemies so return fail
            state = NodeState.FAILURE;
            return state;
        }

        //there is an enemy so run from it and return success
        AI._agentActions.Flee(closestEnemy);
        state = NodeState.SUCCESS;
        return state;
    }
}

public class MoveToBase : Node
{
    public MoveToBase()
    {
    }

    public override NodeState Evaluate()
    {
        //go to base
        AI._agentActions.MoveTo(AI._agentData.FriendlyBase);
        state = NodeState.SUCCESS;
        return state;
    }
}