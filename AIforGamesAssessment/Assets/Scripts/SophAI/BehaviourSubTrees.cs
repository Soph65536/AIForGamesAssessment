using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourSubTrees : MonoBehaviour
{
    //new classes used in behaviour tree functions

    private class GameObjectWithDistance
    {
        public GameObject gameObject;
        public float distance;
        public GameObjectWithDistance(GameObject _gameObject, float _distance)
        {
            gameObject = _gameObject;
            distance = _distance;
        }
    }

    //enemy functions

    public bool CheckForEnemies()
    {
        return false;
    }

    public GameObject CheckEnemiesForEnemyFlag()
    {
        return null;
    }

    public GameObject CheckEnemiesForFriendlyFlag()
    {
        return null;
    }

    public void AttackAnEnemy(AgentActions _agentActions, GameObject targetEnemy)
    {

    }

    public void AttackEnemies(AgentActions _agentActions, List<GameObject> EnemiesInView)
    {
        GameObject targetEnemy = EnemiesInView[0];
        AttackAnEnemy(_agentActions, targetEnemy);
    }

    //other functions

    public void HealOthers(AgentActions _agentActions, List<GameObject> FriendliesInView)
    {

    }

    public void TryForHealthKit(AgentActions _agentActions, List<GameObject> CollectablesInView, Vector3 CurrentPosition)
    {
        List<GameObjectWithDistance> HealthKitsInView = new List<GameObjectWithDistance>();

        foreach (GameObject collectable in CollectablesInView)
        {
            if (collectable.CompareTag(Tags.Collectable))
            {
                HealthKitsInView.Add(new GameObjectWithDistance(
                    collectable, Vector3.Distance(CurrentPosition, collectable.transform.position)
                    ));
            }
        }


        //if no powerups in view then return
        if (HealthKitsInView.Count <= 0) { return; }


        float SmallestHealthKitDistance = Mathf.Infinity;
        GameObject ClosestHealthKit = null;

        foreach (GameObjectWithDistance powerup in HealthKitsInView)
        {
            if (powerup.distance < SmallestHealthKitDistance) { ClosestHealthKit = powerup.gameObject; }
        }


    }

    public void TryForPowerup(AgentActions _agentActions, List<GameObject> CollectablesInView, Vector3 CurrentPosition)
    {
        List<GameObjectWithDistance> PowerupsInView = new List<GameObjectWithDistance>();

        foreach (GameObject collectable in CollectablesInView)
        {
            if (collectable.CompareTag(Tags.Collectable))
            {
                PowerupsInView.Add(new GameObjectWithDistance(
                    collectable, Vector3.Distance(CurrentPosition, collectable.transform.position)
                    ));
            }
        }


        //if no powerups in view then return
        if (PowerupsInView.Count <= 0) { return; }


        float SmallestPowerupDistance = Mathf.Infinity;
        GameObject ClosestPowerup = null;

        foreach (GameObjectWithDistance powerup in PowerupsInView)
        {
            if (powerup.distance < SmallestPowerupDistance) { ClosestPowerup = powerup.gameObject; }
        }


    }

    public void TryForEnemyFlag(AgentActions _agentActions, List<GameObject> ObjectsInView)
    {
        foreach(GameObject _object in ObjectsInView)
        {
            if (_object.CompareTag(Tags.Flag))
            {

            }
        }
    }

    public void RunAway(AgentActions _agentActions, List<GameObject> EnemiesInView, GameObject FriendlyBase)
    {
        if (EnemiesInView.Count > 0)
        {
            _agentActions.Flee(EnemiesInView[0]);
        }
        else
        {
            _agentActions.MoveTo(FriendlyBase);
        }
    }
}
