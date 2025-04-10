using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    //base node class
    public class Node
    {
        //attributes

        protected NodeState state;

        public AI owner; //the tree that owns this node

        public Node parent; //the parent node of this node
        protected List<Node> children = new List<Node>(); //the children nodes of this node

        //shared data between all nodes
        private Dictionary<string, object> sharedData = new Dictionary<string, object>();


        //constructors

        //creates the node without a parent or owner tree
        public Node()
        {
            owner = null;
            parent = null;
        }

        //creates the node without being attached to any parent or children
        public Node(AI ownerTree)
        {
            owner = ownerTree;

            parent = null;
        }

        //creates the node with the nodes in children attached to it as its children
        public Node(AI ownerTree, List<Node> children)
        {
            owner = ownerTree;

            foreach (Node childNode in children)
            {
                AttachChild(childNode);
            }
        }


        //methods

        //sets a node to be a child node of this node
        private void AttachChild(Node childNode)
        {
            childNode.parent = this;
            children.Add(childNode);
        }

        //evaluate function, this will contain different code per node
        //we will return failure for it here since this is the base class
        public virtual NodeState Evaluate() => NodeState.FAILURE;


        //shared data methods

        //get root node
        public Node GetRootNode()
        {
            Node parentNode = parent;

            while (parentNode.parent != null)
            {
                parentNode = parentNode.parent;
            }

            return parentNode;
        }

        //set shared data to new value
        public void SetData(string key, object value)
        {
            sharedData[key] = value;
        }

        //get value from shared data
        public object GetData(string key)
        {
            //try find key's value within this node's shared data
            object value = null;

            if(sharedData.TryGetValue(key, out value)) {  return value; }

            //if not found
            //then run this function from parent and trace back
            //until there is either no parent or key's value has been found
            Node parentNode = parent;

            while (parentNode != null)
            {
                value = parentNode.GetData(key);
                if(value != null) {  return value; }
                parentNode = parentNode.parent;
            }

            //return null if not found key's value
            return null;
        }

        //remove a piece of data from shared data
        //returns true if managed to remove the data
        //returns false if could not find the data to remove
        public bool ClearData(string key)
        {
            //try remove data from this node's shared data
            if (sharedData.ContainsKey(key))
            {
                sharedData.Remove(key);
                return true;
            }

            //otherwise run from parent and trace back
            //until there is either no parent or key hasn't been found
            Node parentNode = parent;

            while (parentNode != null)
            {
                bool clearedData = parentNode.ClearData(key);
                if (clearedData) { return true; }
                parentNode = parentNode.parent;
            }

            //return false if not found key
            return false;
        }
    }

    //sequence node class
    //equivalent to AND
    public class Sequence : Node
    {
        //set constructors to same as parent
        public Sequence() : base() { }
        public Sequence(AI ownerTree) : base(ownerTree) { }
        public Sequence(AI ownerTree, List<Node> children) : base(ownerTree, children) { }

        public override NodeState Evaluate()
        {
            //bool to indicate a child node is still running
            bool anyChildIsRunning = false;

            //go through each of the child nodes
            //if one child fails then this node fails
            //if all children succeed then this node succeeds
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE: //if one child fails then this node fails
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS: //if one child succeeds then just go to the next
                        continue;
                    case NodeState.RUNNING: //if a child node is running anyChildIsRunning is true
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }

            //if a child node is still running then this node is still running,
            //otherwise all child nodes have succeeded so this node succeeds
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }

    //selector node class
    //equivalent to OR
    public class Selector : Node
    {
        //set constructors to same as parent
        public Selector() : base() { }
        public Selector(AI ownerTree) : base(ownerTree) { }
        public Selector(AI ownerTree, List<Node> children) : base(ownerTree, children) { }

        public override NodeState Evaluate()
        {
            //go through each of the child nodes
            //if one child fails then just go to the next
            //if one child succeeds then this node succeeds
            foreach (Node node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE: //if one child fails then just go to the next
                        continue;
                    case NodeState.SUCCESS: //if one child succeeds then this node succeeds
                        state = NodeState.SUCCESS; 
                        return state;
                    case NodeState.RUNNING: //if a child node is running then this node is still running
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }

            //if no children have returned success then this node fails
            state = NodeState.FAILURE;
            return state;
        }
    }
}
