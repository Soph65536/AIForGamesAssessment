﻿using System.Collections.Generic;
using UnityEngine;

using BehaviourTree;

/*****************************************************************************************************************************
 * Write your core AI code in this file here. The private variable 'agentScript' contains all the agents actions which are listed
 * below. Ensure your code it clear and organised and commented.
 *
 * Unity Tags
 * public static class Tags
 * public const string BlueTeam = "Blue Team";	The tag assigned to blue team members.
 * public const string RedTeam = "Red Team";	The tag assigned to red team members.
 * public const string Collectable = "Collectable";	The tag assigned to collectable items (health kit and power up).
 * public const string Flag = "Flag";	The tag assigned to the flags, blue or red.
 * 
 * Unity GameObject names
 * public static class Names
 * public const string PowerUp = "Power Up";	Power up name
 * public const string HealthKit = "Health Kit";	Health kit name.
 * public const string BlueFlag = "Blue Flag";	The blue teams flag name.
 * public const string RedFlag = "Red Flag";	The red teams flag name.
 * public const string RedBase = "Red Base";    The red teams base name.
 * public const string BlueBase = "Blue Base";  The blue teams base name.
 * public const string BlueTeamMember1 = "Blue Team Member 1";	Blue team member 1 name.
 * public const string BlueTeamMember2 = "Blue Team Member 2";	Blue team member 2 name.
 * public const string BlueTeamMember3 = "Blue Team Member 3";	Blue team member 3 name.
 * public const string RedTeamMember1 = "Red Team Member 1";	Red team member 1 name.
 * public const string RedTeamMember2 = "Red Team Member 2";	Red team member 2 name.
 * public const string RedTeamMember3 = "Red Team Member 3";	Red team member 3 name.
 * 
 * _agentData properties and public variables
 * public bool IsPoweredUp;	Have we powered up, true if we’re powered up, false otherwise.
 * public int CurrentHitPoints;	Our current hit points as an integer
 * public bool HasFriendlyFlag;	True if we have collected our own flag
 * public bool HasEnemyFlag;	True if we have collected the enemy flag
 * public GameObject FriendlyBase; The friendly base GameObject
 * public GameObject EnemyBase;    The enemy base GameObject
 * public int FriendlyScore; The friendly teams score
 * public int EnemyScore;       The enemy teams score
 * 
 * _agentActions methods
 * public bool MoveTo(GameObject target)	Move towards a target object. Takes a GameObject representing the target object as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveTo(Vector3 target)	Move towards a target location. Takes a Vector3 representing the destination as a parameter. Returns true if the location is on the navmesh, false otherwise.
 * public bool MoveToRandomLocation()	Move to a random location on the level, returns true if the location is on the navmesh, false otherwise.
 * public void CollectItem(GameObject item)	Pick up an item from the level which is within reach of the agent and put it in the inventory. Takes a GameObject representing the item as a parameter.
 * public void DropItem(GameObject item)	Drop an inventory item or the flag at the agents’ location. Takes a GameObject representing the item as a parameter.
 * public void UseItem(GameObject item)	Use an item in the inventory (currently only health kit or power up). Takes a GameObject representing the item to use as a parameter.
 * public void AttackEnemy(GameObject enemy)	Attack the enemy if they are close enough. ). Takes a GameObject representing the enemy as a parameter.
 * public void Flee(GameObject enemy)	Move in the opposite direction to the enemy. Takes a GameObject representing the enemy as a parameter.
 * 
 * _agentSenses properties and methods
 * public List<GameObject> GetObjectsInViewByTag(string tag)	Return a list of objects with the same tag. Takes a string representing the Unity tag. Returns null if no objects with the specified tag are in view.
 * public GameObject GetObjectInViewByName(string name)	Returns a specific GameObject by name in view range. Takes a string representing the objects Unity name as a parameter. Returns null if named object is not in view.
 * public List<GameObject> GetObjectsInView()	Returns a list of objects within view range. Returns null if no objects are in view.
 * public List<GameObject> GetCollectablesInView()	Returns a list of objects with the tag Collectable within view range. Returns null if no collectable objects are in view.
 * public List<GameObject> GetFriendliesInView()	Returns a list of friendly team AI agents within view range. Returns null if no friendlies are in view.
 * public List<GameObject> GetEnemiesInView()	Returns a list of enemy team AI agents within view range. Returns null if no enemies are in view.
 * public GameObject GetNearestEnemyInView()   Returns the nearest enemy AI in view to the agent. Returns null if no enemies are in view.
 * public bool IsItemInReach(GameObject item)	Checks if we are close enough to a specific collectible item to pick it up), returns true if the object is close enough, false otherwise.
 * public bool IsInAttackRange(GameObject target)	Check if we're with attacking range of the target), returns true if the target is in range, false otherwise.
 * public Vector3 GetVectorToTarget(GameObject target)  Return a normalised vector pointing to the target GameObject
 * public Vector3 GetVectorToTarget(Vector3 target)     Return a normalised vector pointing to the target vector
 * public Vector3 GetFleeVectorFromTarget(GameObject target)    Return a normalised vector pointing away from the target GameObject
 * public Vector3 GetFleeVectorFromTarget(Vector3 target)   Return a normalised vector pointing away from the target vector
 * 
 * _agentInventory properties and methods
 * public bool AddItem(GameObject item)	Adds an item to the inventory if there's enough room (max capacity is 'Constants.InventorySize'), returns true if the item has been successfully added to the inventory, false otherwise.
 * public GameObject GetItem(string itemName)	Retrieves an item from the inventory as a GameObject, returns null if the item is not in the inventory.
 * public bool HasItem(string itemName)	Checks if an item is stored in the inventory, returns true if the item is in the inventory, false otherwise.
 * 
 * You can use the game objects name to access a GameObject from the sensing system. Thereafter all methods require the GameObject as a parameter.
 * 
*****************************************************************************************************************************/

/// <summary>
/// Implement your AI script here, you can put everything in this file, or better still, break your code up into multiple files
/// and call your script here in the Update method. This class includes all the data members you need to control your AI agent
/// get information about the world, manage the AI inventory and access essential information about your AI.
///
/// You may use any AI algorithm you like, but please try to write your code properly and professionaly and don't use code obtained from
/// other sources, including the labs.
///
/// See the assessment brief for more details
/// </summary>
public class AI : BehaviourTree.Tree
{
    // Gives access to important data about the AI agent (see above)
    public AgentData _agentData;
    // Gives access to the agent senses
    public Sensing _agentSenses;
    // gives access to the agents inventory
    public InventoryController _agentInventory;
    // This is the script containing the AI agents actions
    // e.g. agentScript.MoveTo(enemy);
    public AgentActions _agentActions;

    // Use this for initialization
    protected override void Start()
    {
        // Initialise the accessable script components
        _agentData = GetComponent<AgentData>();
        _agentActions = GetComponent<AgentActions>();
        _agentSenses = GetComponentInChildren<Sensing>();
        _agentInventory = GetComponentInChildren<InventoryController>();

        base.Start();
    }

    protected override Node SetupTree()
    {
        Node root = null;


        if(gameObject.name == Names.BlueTeamMember1 ||
            gameObject.name == Names.RedTeamMember1)
        {
            //Member 1 of team is Offense AI
            root = new Selector(this, new List<Node>
            {
                //if has a flag try returning it to base
                new BringFlagsToBase(this),

                //if has health kit try healing others, then self
                new HealOthers(this),
                new HealSelf(this),

                //if objects in view
                new Sequence(this, new List<Node>
                {
                    new CheckForObjectsInView(this),
                    new Selector(this, new List<Node>
                    {
                        //if low health
                        new Sequence(this, new List<Node>
                        {
                            new CheckForLowHealth(this),
                            new Selector(this, new List<Node>
                            {
                                //try find health kit
                                new Sequence(this, new List<Node>
                                {
                                    new TryFindHealthKit(this),
                                    new HealSelf(this)
                                }),

                                //otherwise try find flags and try bring to base
                                //try find both flags and bring to base
                                new Sequence(this, new List<Node>
                                {
                                    new TryForFriendlyFlag(this),
                                    new TryForEnemyFlag(this),
                                    new BringFlagsToBase(this),
                                }),
                                //try find friendly flag and bring to base
                                new Sequence(this, new List<Node>
                                {
                                    new TryForFriendlyFlag(this),
                                    new BringFlagsToBase(this),
                                }),
                                //try find enemy flag and bring to base
                                new Sequence(this, new List<Node>
                                {
                                    new TryForEnemyFlag(this),
                                    new BringFlagsToBase(this),
                                }),

                                //otherwise flee from enemies
                                new FleeFromEnemies(this),

                                //otherwise if no enemies then just run to base
                                new MoveToBase(this)
                            })
                        }),

                        //else try for flags
                        //try find both flags and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForFriendlyFlag(this),
                            new TryForEnemyFlag(this),
                            new BringFlagsToBase(this),
                        }),
                        //try find friendly flag and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForFriendlyFlag(this),
                            new BringFlagsToBase(this),
                        }),
                        //try find enemy flag and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForEnemyFlag(this),
                            new BringFlagsToBase(this),
                        }),

                        //else if enemies
                        new Sequence(this, new List<Node>
                        {
                            new CheckForEnemiesInView(this),
                            
                            //set enemy target
                            new Selector(this, new List<Node>
                            {
                                new CheckEnemiesForFriendlyFlag(this),
                                new CheckEnemiesForEnemyFlag(this),
                                new SetClosestEnemyAsTarget(this, transform)
                            }),

                            //attack target enemy
                            new AttackAnEnemy(this)
                        }),

                        //else try for powerup
                        new TryForPowerup(this),
                    })
                }),

                //else move to random location
                new MoveToRandomLocation(this)
            });
        }

        else if(gameObject.name == Names.BlueTeamMember2 ||
            gameObject.name == Names.RedTeamMember2)
        {
            //Member 2 of team is Defense/Support AI
            root = new Selector(this, new List<Node>
            {
                //if has a flag try returning it to base
                new BringFlagsToBase(this),

                //if has health kit try healing others, then self
                new HealOthers(this),
                new HealSelf(this),

                //if objects in view
                new Sequence(this, new List<Node>
                {
                    new CheckForObjectsInView(this),
                    new Selector(this, new List<Node>
                    {
                        //if low health
                        new Sequence(this, new List<Node>
                        {
                            new CheckForLowHealth(this),
                            new Selector(this, new List<Node>
                            {
                                //try find health kit
                                new Sequence(this, new List<Node>
                                {
                                    new TryFindHealthKit(this),
                                    new HealSelf(this)
                                }),

                                //otherwise flee from enemies
                                new FleeFromEnemies(this),

                                //otherwise if no enemies then just run to base
                                new MoveToBase(this)
                            })
                        }),

                        //else try for flags
                        //try find both flags and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForFriendlyFlag(this),
                            new TryForEnemyFlag(this),
                            new BringFlagsToBase(this),
                        }),
                        //try find friendly flag and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForFriendlyFlag(this),
                            new BringFlagsToBase(this),
                        }),
                        //try find enemy flag and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForEnemyFlag(this),
                            new BringFlagsToBase(this),
                        }),

                        //else try heal others
                        new Sequence(this, new List<Node>
                        {
                            new TryFindHealthKit(this),
                            new HealOthers(this)
                        }),

                        //else if enemies
                        new Sequence(this, new List<Node>
                        {
                            new CheckForEnemiesInView(this),
                            
                            //set enemy target (defense only attacks enemy if they have a flag)
                            new Selector(this, new List<Node>
                            {
                                new CheckEnemiesForFriendlyFlag(this),
                                new CheckEnemiesForEnemyFlag(this),
                            }),

                            //attack target enemy
                            new AttackAnEnemy(this)
                        })
                    })
                }),

                //else move to random location
                new MoveToRandomLocation(this)
            });
        }

        else
        {
            //Member 3 of team is All Rounder AI
            root = new Selector(this, new List<Node>
            {
                //if has a flag try returning it to base
                new BringFlagsToBase(this),

                //if has health kit try healing others, then self
                new HealOthers(this),
                new HealSelf(this),

                //if objects in view
                new Sequence(this, new List<Node>
                {
                    new CheckForObjectsInView(this),
                    new Selector(this, new List<Node>
                    {
                        //if low health
                        new Sequence(this, new List<Node>
                        {
                            new CheckForLowHealth(this),
                            new Selector(this, new List<Node>
                            {
                                //try find health kit
                                new Sequence(this, new List<Node>
                                {
                                    new TryFindHealthKit(this),
                                    new HealSelf(this)
                                }),

                                //otherwise flee from enemies
                                new FleeFromEnemies(this),

                                //otherwise if no enemies then just run to base
                                new MoveToBase(this)
                            })
                        }),

                        //else try for flags
                        //try find both flags and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForFriendlyFlag(this),
                            new TryForEnemyFlag(this),
                            new BringFlagsToBase(this),
                        }),
                        //try find friendly flag and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForFriendlyFlag(this),
                            new BringFlagsToBase(this),
                        }),
                        //try find enemy flag and bring to base
                        new Sequence(this, new List<Node>
                        {
                            new TryForEnemyFlag(this),
                            new BringFlagsToBase(this),
                        }),

                        //else if enemies
                        new Sequence(this, new List<Node>
                        {
                            new CheckForEnemiesInView(this),
                            
                            //set enemy target (all rouner just attacks any enemy it sees)
                            new Selector(this, new List<Node>
                            {
                                new SetClosestEnemyAsTarget(this, transform)
                            }),

                            //attack target enemy
                            new AttackAnEnemy(this)
                        }),

                        //else try for powerup
                        new TryForPowerup(this),
                    })
                }),

                //else move to random location
                new MoveToRandomLocation(this)
            });
        }


        return root;
    }
}