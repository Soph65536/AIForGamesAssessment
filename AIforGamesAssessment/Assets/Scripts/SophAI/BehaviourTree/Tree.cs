using BehaviourTree;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public abstract class Tree : MonoBehaviour
    {
        private Node rootNode = null;

        // Start is called before the first frame update
        protected void Start()
        {
            rootNode = SetupTree();
        }

        // Update is called once per frame
        private void Update()
        {
            //if tree contains root then evaluate it continuously
            if (rootNode != null) { rootNode.Evaluate(); }
        }

        protected abstract Node SetupTree();
    }
}
