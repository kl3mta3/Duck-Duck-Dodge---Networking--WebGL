using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class AIMovement : NetworkBehaviour
{
    [SerializeField]
    internal AiPlayer aiPlayer;
    [SerializeField]
    internal GameObject playerTheif;
    internal GameObject playerStealTarget;
    [SerializeField]
    internal GameObject goal;
    [SerializeField]
    internal Transform body;
   


    [SerializeField]
    internal GameObject nextPellet;
    [SerializeField]
    internal GameObject lastPellet;
    //[Networked]
    internal Vector3 nextPelletLocation { get; set; }
    //[Networked]
    internal Vector3 goalLocation { get; set; }
   // [Networked]
    internal Vector3 theftTargetLocation { get; set; }

    [SerializeField]
    public  GameObject[] worldPellets;
    [SerializeField]
    internal float pelletSearchRange = 10f;

    internal List<GameObject> targetPellets = new List<GameObject>();

    [SerializeField]
    private bool moveToGoal;
    [SerializeField]
    internal bool isMovingToPlayer;
    [SerializeField]
    internal bool isMovingToPellet;
    [SerializeField]
    internal bool isMovingtoGoal;
    [SerializeField]
    internal bool findingPellet;
    [SerializeField]
    internal bool findingGoal;
   
    [SerializeField]
    internal bool nextPelletLocationFound;
    [SerializeField]
    internal  bool arrivedAtPellet;

    [SerializeField]
    internal float currentPellets;
    [SerializeField]
    internal float currentPelletsInternal;
    [SerializeField]
    private bool arrivedAtGoal;
    [SerializeField]
    internal float waitTime = 5f;
    [SerializeField]
    private bool startWaited = false;
    [SerializeField]
    internal bool tryAndSteal;
    [SerializeField]
    internal float speed = .3f;
    [SerializeField]
    internal float goScoreMin = 5f;
    [SerializeField]
    internal float goScoreMax = 20f;
    [SerializeField]
    internal float goScore ;
    [SerializeField]
    internal float minChaseDist;
    [SerializeField]
    internal float maxChaseDist;
    [SerializeField]
    internal GameObject rippleFx;
    internal NavMeshAgent agent;
    internal Rigidbody rb;

    [SerializeField]
    internal LayerMask foodPellet;
    internal bool stolenFrom;
    internal bool stealing;
    internal void Awake()
    {
        //foodPellet =  LayerMask.GetMask("FoodPellet");
        rb = GetComponent<Rigidbody>();
        aiPlayer = GetComponent<AiPlayer>();
        goal = GameObject.FindGameObjectWithTag("RedScoreBox");
        goalLocation = goal.transform.position;
        worldPellets = GameObject.FindGameObjectsWithTag("FoodPellet");
        agent = GetComponent<NavMeshAgent>();
        Setpellets();
        isMovingToPellet = false;
        //if (goal == null)
        //{
        //    goal = aiPlayer.playerGoal;
        //}
        
        StartCoroutine(StartWait());
        goScore = Mathf.Round(UnityEngine.Random.Range(goScoreMin, (targetPellets.Count/2)));
    }
    
    internal void Setpellets()
    {
        targetPellets = worldPellets.ToList();
        Debug.Log(worldPellets.Length + " World Pellets Found");
        Debug.Log(targetPellets.Count + " Target Pellets Added");

    }
    public  override void FixedUpdateNetwork()
    {

       // if (ScoreKeeper.gameStarted)
        //{
            if (Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.None)
            {
                return;
            }


            aiPlayer.playerSpeed = speed;

           if (startWaited)
           {

                currentPellets = aiPlayer.currentFoodPellets;
                currentPelletsInternal = aiPlayer.currentPelletsInternal;
                if (!isMovingToPellet && !isMovingtoGoal && !isMovingToPlayer)
                {
                    Debug.Log("Enemey Current Pellets: " + currentPellets);

                    if (currentPellets < goScore && !isMovingToPellet)
                    {
                        Debug.Log(" move to pellet" );

                        isMovingToPellet = true;
                        arrivedAtPellet = false;
                        MoveToNext();

                    }
                    else if (currentPellets >= goScore && !moveToGoal)
                    {
                        StopCoroutine(MoveToNextPellet());
                        isMovingToPellet = false;
                        //Debug.Log(" move to goal");
                        isMovingtoGoal = true;
                        arrivedAtGoal = false;
                        MoveToGoal();

                    }
                    else
                    {
                        Debug.Log("Error in AI move to pellet");
                    }
                }
                else if (stolenFrom && !isMovingToPlayer)
                {
                    Debug.Log("Stolen From");
                    StopCoroutine(MoveToNextPellet());
                    StopCoroutine(MoveToNextGoal());
                    isMovingToPlayer = true;
                    isMovingToPellet = false;
                    isMovingtoGoal = false;
                    moveToGoal = false;
                    TryAndSteal(playerTheif);


                }
                if (tryAndSteal)
                {
                    StolenFrom();
                }
                //Debug.Log(targetPellets.Count + " Target Pellets update");
                if (targetPellets.Count <= 0)
                {


                    Setpellets();
                }

           // }
        }
    } 

    internal void MoveToNext()
    {

        Debug.Log("Moving to next pellet");
        FindNextPellet();

        //agent.SetDestination(nextPelletLocation);
        StartCoroutine(MoveToNextPellet());
    }
    internal void MoveToGoal()
    {
        Debug.Log("Moving to goal");
        if (goal == null)
        {
        goal = aiPlayer.playerGoal;
        }

        Vector3 location = new Vector3(goal.transform.position.x, 0, goal.transform.position.z);
        goalLocation = location;
        //agent.SetDestination(goalLocation);
        moveToGoal = true;

        StartCoroutine(MoveToNextGoal());
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, pelletSearchRange);
    }
    internal void FindNextPellet()
    {
        Debug.Log("Finding next pellet");

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = new Vector3(transform.position.x, 0, transform.position.z);
        GameObject bestTarget = null;

        ///uses the target pellets list to find the closest pellet

        //foreach (GameObject pellet in targetPellets)
        //{
        //    if (pellet.activeInHierarchy && pellet != lastPellet)
        //    {
        //        Vector3 pelletPosition = new Vector3(pellet.transform.position.x, 0, pellet.transform.position.z);
        //        var thisDistance = (pelletPosition - currentPosition).sqrMagnitude;
        //        if (thisDistance < closestDistanceSqr)
        //        {
        //            closestDistanceSqr = thisDistance;
        //            bestTarget = pellet;
        //        }
        //    }
        //}


        ///uses OverLap Sphere to find the closest pellet
        Collider[] possibleTargetPellets = Physics.OverlapSphere(transform.position, pelletSearchRange, foodPellet);
        if (possibleTargetPellets.Length >= 0)
        {
            Debug.Log(" No Pellets Found " + "Doubling Search Range");
            Collider[] _possibleTargetPellets = Physics.OverlapSphere(transform.position, pelletSearchRange * 2, foodPellet);
            Debug.Log(_possibleTargetPellets.Length + " _Possible Pellets Found in Expanded Search");
            foreach (Collider _pellet in _possibleTargetPellets)
            {
                if (_pellet.gameObject.CompareTag("FoodPellet"))
                {
                    if (_pellet.gameObject.activeInHierarchy && _pellet.gameObject != lastPellet)
                    {
                        Vector3 pelletPosition = new Vector3(_pellet.gameObject.transform.position.x, 0, _pellet.gameObject.transform.position.z);
                        var thisDistance = (pelletPosition - currentPosition).sqrMagnitude;
                        if (thisDistance < closestDistanceSqr)
                        {
                            closestDistanceSqr = thisDistance;
                            bestTarget = _pellet.gameObject;
                        }
                    }
                }
            }

        }
        else
        {
            Debug.Log(possibleTargetPellets.Length + " Possible Pellets Found");
            foreach (Collider pellet in possibleTargetPellets)
            {
                if (pellet.CompareTag("FoodPellet"))
                {
                    if (pellet.gameObject.activeInHierarchy && pellet != lastPellet)
                    {
                        Vector3 pelletPosition = new Vector3(pellet.transform.position.x, 0, pellet.transform.position.z);
                        var thisDistance = (pelletPosition - currentPosition).sqrMagnitude;
                        if (thisDistance < closestDistanceSqr)
                        {
                            closestDistanceSqr = thisDistance;
                            bestTarget = pellet.gameObject;
                        }
                    }
                }
            }

        }


        nextPellet = bestTarget;
        if(nextPellet==null)
        {
            Debug.Log("Sphere found No Pellets Defaulted to list ");
            foreach (GameObject pellet in targetPellets)
            {
                if (pellet.activeInHierarchy && pellet != lastPellet)
                {
                    Vector3 pelletPosition = new Vector3(pellet.transform.position.x, 0, pellet.transform.position.z);
                    var thisDistance = (pelletPosition - currentPosition).sqrMagnitude;
                    if (thisDistance < closestDistanceSqr)
                    {
                        closestDistanceSqr = thisDistance;
                        bestTarget = pellet;
                    }
                }
            }
            nextPellet = bestTarget;

            //nextPellet = transform.gameObject;
        }
        nextPelletLocation = new Vector3(nextPellet.transform.position.x, 0, nextPellet.transform.position.z);
        nextPelletLocationFound = true;

        
    }

    internal void ArrivedAtPellet()
    {
        //Debug.Log("Arrived at pellet");
        isMovingToPellet = false;
        nextPelletLocationFound = false;
        nextPelletLocation = Vector3.zero;
        //RemoveTargetPelletFromList(nextPellet);
        arrivedAtPellet = true;
        lastPellet = nextPellet;
        nextPellet = null;
        StopCoroutine(MoveToNextPellet());
        

    }
    

    internal bool StolenFrom()
    {
        if (currentPelletsInternal > currentPellets && !arrivedAtGoal)
        {
            stolenFrom = true;
            return true;
        }
        else
        {
            stolenFrom = false;
            return false;
        }
 
    }
    
    internal void TryAndSteal(GameObject mark)
    {
        if (mark != null)
        {
            nextPelletLocationFound = false;
            isMovingToPellet = false;
            isMovingtoGoal = false;
            arrivedAtPellet = false;
           // isMovingToPlayer = true;
            stealing = true;
            //Debug.Log("Trying to steal");
            Vector3 location = new Vector3(mark.transform.position.x, 0, mark.transform.position.z);
            theftTargetLocation = location;
            StartCoroutine(MoveToNextGoal());
        }
        else
        {
            stealing = false;
        }




    }
    internal void ArrivedAtGoal()
    {
        //Debug.Log("Arrived at goal");
        isMovingtoGoal = false;
        isMovingToPellet = false;
        isMovingToPlayer = false;
        moveToGoal = false;
        arrivedAtGoal = true;
        goScore = Mathf.Round(UnityEngine.Random.Range(goScoreMin, (targetPellets.Count / 2)));
        StopCoroutine(MoveToNextGoal());
    }
    internal void ArrivedAtStealTarget()
    {
        //Debug.Log("Arrived at StealTarget");
        isMovingtoGoal = false;
        isMovingToPlayer = false;
        isMovingToPellet = false;
        arrivedAtGoal = false;
        stealing = false;
        stolenFrom = false;
        playerTheif = null;
        theftTargetLocation = Vector3.zero;
        StopCoroutine(MoveToStealTarget());
        //Debug.Log(" move to goal");
        isMovingtoGoal = true;
        arrivedAtGoal = false;
        MoveToGoal();
    }


    private IEnumerator StartWait()
    {
        if (!startWaited)
        {
            //Debug.Log("StartWaited Enum Ran");
            yield return new WaitForSeconds(waitTime);
            startWaited = true;
            StopCoroutine(StartWait());
        }

    }


    private IEnumerator MoveToNextPellet()
    {

       
        while(nextPelletLocationFound)
        {
            if (nextPelletLocation == Vector3.zero)
            {
                FindNextPellet();
            }
            if (Vector3.Distance(transform.position, nextPelletLocation) > 0.1f)
            {
             Debug.Log("Moving... to " + nextPellet.gameObject.transform.parent.name + "/" + nextPellet.gameObject.name);
                isMovingToPellet = true;

                agent.destination =nextPelletLocation;
                yield return new WaitForSeconds(0.1f);
                agent.velocity = Vector3.zero;
              
         

            }
                if (!agent.pathPending)
                {
                    if (agent.hasPath)
                    {

                            if (agent.remainingDistance <=  .9)
                            {
                            //Debug.Log("boop");
                            isMovingToPellet = false;
                            ArrivedAtPellet();
                            }
                        
                    }

                }
        }


    }
    internal void RemoveTargetPelletFromList(GameObject pellet)
    {
        if (targetPellets.Contains(pellet))
        {
            //Debug.Log("Removing " + pellet.gameObject.name + " from target list");
            targetPellets.Remove(pellet);
        }
    }

   private IEnumerator MoveToNextGoal()
    {

        while (moveToGoal)
        {

            if (Vector3.Distance(transform.position, goalLocation) > 0.1f)
            {
                isMovingtoGoal = true;
                
                agent.destination = goalLocation;
       
                yield return new WaitForSeconds(0.1f);
                agent.velocity = Vector3.zero;
         
            }
            if (!agent.pathPending)
            {
                if (agent.hasPath)
                {

                    if (agent.remainingDistance <= .9)
                    {
                       
                       
                        ArrivedAtGoal();
                    }

                }

            }

        }


    }

    private IEnumerator MoveToStealTarget()
    {

        while (stealing)
        {

            if (Vector3.Distance(transform.position, theftTargetLocation /*playerTheif.transform.position*/) > 0.1f)
            {
                isMovingToPlayer = true;

                agent.destination = theftTargetLocation/* playerTheif.transform.position*/;

                yield return new WaitForSeconds(0.1f);
                agent.velocity = Vector3.zero;

            }
            if (!agent.pathPending)
            {
                if (agent.hasPath)
                {

                    if (agent.remainingDistance <= .9)
                    {

                        ArrivedAtStealTarget();
                    }
                }
            }
        }

        //yield return new WaitForSeconds(0.5f);
        //if (stealing)
        //{
        //    //Debug.Log("Moving... to " + playerTheif.gameObject.transform.parent.name + "/" + playerTheif.gameObject.name);
        //    //Debug.Log("At Location (" + theftTargetLocation.x + "," + 0 + "," + theftTargetLocation.z + ")");
        //    while (Vector3.Distance(transform.position, playerTheif.transform.position) > 0.1f /*&& Vector3.Distance(transform.position, playerTheif.transform.position)<2*/)

        //    {
        //        isMovingToPlayer = true;
        //        transform.position = Vector3.MoveTowards(transform.position, playerTheif.transform.position, speed);
        //        body.position = transform.position;
        //        transform.LookAt(playerTheif.transform.position);
        //        yield return new WaitForSeconds(0.01f);
        //    }
        //    ArrivedAtStealTarget();

        //}


    }
}
