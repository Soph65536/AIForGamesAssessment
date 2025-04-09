using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this class is used to store a gamebject along with how far away it is from the AI
public class GameObjectWithDistance
{
    public GameObject gameObject;
    public float distance;
    public GameObjectWithDistance(GameObject _gameObject, float _distance)
    {
        gameObject = _gameObject;
        distance = _distance;
    }
}
