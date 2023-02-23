using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using System.Runtime.InteropServices.WindowsRuntime;
//using static UnityEditor.FilePathAttribute;

public class AiPlayer : NetworkBehaviour
{
    private NetworkRunner _runner;
    [SerializeField]
    internal Transform playerbody;
    [SerializeField]
    internal AIMovement aiMovement;
    internal string playerName;

    [Networked]
    public int currentFoodPellets { get; set; }
    [Networked]
    public int currentPelletsInternal { get; set; }
    [Networked]
    public int totalFoodPelletsGathered { get; set; }
    [Networked]
    public NetworkBool hasPellets { get; set; }
    [Networked]
    public NetworkBool isSpeeding { get; set; }
    [Networked]
    public NetworkBool isSuperSight { get; set; }
    [Networked]
    public NetworkBool isWallWalking { get; set; }
    [Networked]
    public NetworkBool isInvincible { get; set; }
    [Networked]
    public NetworkBool isMagnetic { get; set; }
    [Networked]
    private TickTimer delay { get; set; }
    [Networked]
    internal float playerSpeed { get; set; }
    [SerializeField]
    internal GameObject pelletFollowRecordPoint;
    [SerializeField]
    internal int pelletGap;


    [SerializeField]
    internal GameObject playerFoodPellet;


    internal AiPlayer player;
    [SerializeField]

    internal GameObject currentFoodPelletPool;
    [SerializeField]
    private List<NetworkObject> currentFoodPelletList = new List<NetworkObject>();
    [SerializeField]
    private List<Vector3> positionHistory = new List<Vector3>();

    [Networked]
    [SerializeField]
    public bool IsRedTeam { get; set; }
    [Networked]
    [SerializeField]
    public bool IsBlueTeam { get; set; }

    [SerializeField]
    internal GameObject playerGoal;

    [SerializeField]
    internal GameObject spawnPoint;


    [SerializeField]
    internal Material redMaterial;
    [SerializeField]
    internal Material blueMaterial;

    internal Material prefabMaterial;
    [SerializeField]
    internal GameObject[] playerAdjustableBodyParts;

    internal bool stealing = false;
    [SerializeField]
    internal bool canSteal = true;
    internal bool gameRunning ;



    internal void Start()
    {
        playerName = gameObject.transform.root.name;
        player = gameObject.GetComponent<AiPlayer>();
        aiMovement= gameObject.GetComponent<AIMovement>();
        gameRunning = false;

        if (IsRedTeam)
        {
            gameObject.tag = "RedPlayer";
            playerGoal = GameObject.FindGameObjectWithTag("RedScoreBox");
            foreach (GameObject bodyPart in playerAdjustableBodyParts)
            {
                bodyPart.GetComponent<Renderer>().material = redMaterial;
                prefabMaterial = redMaterial;
            }

        }
        else if (IsBlueTeam)
        {
            gameObject.tag = "BluePlayer";
            playerGoal = GameObject.FindGameObjectWithTag("BlueScoreBox");
            foreach (GameObject bodyPart in playerAdjustableBodyParts)
            {
                bodyPart.GetComponent<Renderer>().material = blueMaterial;
                prefabMaterial = blueMaterial;
            }
        }

    }

    public override void FixedUpdateNetwork()
    {

       
        
            //gameRunning = ScoreKeeper.gameStarted;
           
        
        //if (ScoreKeeper.gameStarted)
        //{


            playerSpeed = aiMovement.speed * 50;

            if (currentFoodPellets > 0)
            {
                hasPellets = true;
                positionHistory.Insert(0, pelletFollowRecordPoint.transform.position);
                MoveCurrentPellets();

            }
            else
            {
                hasPellets = false;


            }
        //}
    }

    internal void PickUpFoodPellet(float amount)
    {

        currentFoodPellets += (int)amount;
        currentPelletsInternal += (int)amount;


        CreateFollowPellet();

    }

    private void SetPlayerColor(NetworkObject player)
    {
        GameObject adjustableParts = player.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;
        foreach (Transform child in adjustableParts.transform)
        {
            child.GetComponent<Renderer>().material = prefabMaterial;
        }


    }
    private void CreateFollowPellet()
    {
        Vector3 location = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, spawnPoint.transform.position.z);


        NetworkObject newPellet = Runner.Spawn(playerFoodPellet, location, Quaternion.identity, Object.InputAuthority);

        //newPellet.transform.position = transform.position;
        currentFoodPelletList.Add(newPellet);
        //Debug.Log("currentFoodPellet.Count: " + currentFoodPelletList.Count);
        newPellet.gameObject.name = "Player " + player.gameObject.name + " Pellet # " + currentFoodPellets.ToString();
        newPellet.transform.parent = currentFoodPelletPool.transform;
        newPellet.transform.rotation = playerbody.rotation;
        FoodPelletPlayer pelletScript = newPellet.gameObject.GetComponent<FoodPelletPlayer>();
        pelletScript.aiPlayer = player;
        if (IsBlueTeam)
        {
            pelletScript.IsBluePellet = true;
        }
        else if (IsRedTeam)
        {
            pelletScript.IsRedPellet = true;
        }
        SetPlayerColor(newPellet);



    }

    internal void MoveCurrentPellets()
    {

        int index = 0;

        foreach (NetworkObject pellet in currentFoodPelletList)
        {
            Vector3 point = positionHistory[Mathf.Min(index * pelletGap, positionHistory.Count - 1)];
            Vector3 moveDirection = point - pellet.transform.position;
            pellet.transform.position += moveDirection * playerSpeed * Runner.DeltaTime; ;
            pellet.transform.LookAt(playerbody);
            index++;

        }
    }

    
    internal void Scored()
    {
        for (int i = 0; i < currentFoodPelletList.Count; i++)
        {
            StartCoroutine(DestroyPellet(currentFoodPelletList[i]));
        }
        StartCoroutine(DisplayScore());

        totalFoodPelletsGathered = totalFoodPelletsGathered + currentFoodPellets;
    
        currentFoodPelletList.Clear();
        positionHistory.Clear();
    }

    internal void StealPellets()
    {
        if (!stealing)
        {
            stealing = true;
            PickUpFoodPellet(1);
            stealing = false;
        }
    }

    internal void LosePellet(NetworkObject pellet)
    {


        currentFoodPellets--;
        currentFoodPelletList.Remove(pellet);
        Runner.Despawn(pellet);
        //StartCoroutine(DestroyPellet(pellet));


    }

    internal void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            canSteal = false;
        }
        else
        {
            canSteal = true;
        }

    }

    internal void OnTriggerEnter(Collider other)
    {
        if (canSteal)
        {
            if (other.gameObject.CompareTag("PlayerFoodPellet"))
            {
                if (other.gameObject.transform.parent.gameObject != currentFoodPelletPool.gameObject)
                {
                    other.gameObject.GetComponent<Collider>().enabled = false;
                    FoodPelletPlayer foodPellet = other.gameObject.GetComponent<FoodPelletPlayer>();

                    if (foodPellet.IsRedPellet && IsBlueTeam && !stealing)
                    {

                        StealPellets();
                        if (foodPellet.player != null)
                        {
                            foodPellet.player.GetComponent<Player>().LosePellet(other.gameObject.GetComponent<NetworkObject>());
                        }
                        else if (foodPellet.aiPlayer != null)
                        {
                            foodPellet.aiPlayer.GetComponent<AiPlayer>().LosePellet(other.gameObject.GetComponent<NetworkObject>());
                            foodPellet.aiPlayer.aiMovement.playerTheif = other.gameObject;
                        }



                    }
                    else if (foodPellet.IsBluePellet && IsRedTeam)
                    {
                        StealPellets();
                        if (foodPellet.player != null)
                        {
                            foodPellet.player.GetComponent<Player>().LosePellet(other.gameObject.GetComponent<NetworkObject>());
                        }
                        else if (foodPellet.aiPlayer != null)
                        {
                            foodPellet.aiPlayer.GetComponent<AiPlayer>().LosePellet(other.gameObject.GetComponent<NetworkObject>());
                            foodPellet.aiPlayer.aiMovement.playerTheif = other.gameObject;
                        }
                    }
                }
            }
        }

    }
    IEnumerator DestroyPellet(NetworkObject pellet)
    {

        yield return new WaitForSeconds(.3f);
        Runner.Despawn(pellet);
        currentFoodPelletList.Remove(pellet);
        StopCoroutine(DestroyPellet(pellet));
        //Destroy(pellet);

    }

    IEnumerator DisplayScore()
    {
        yield return new WaitForSeconds(.1f);
        currentFoodPellets = 0;
        currentPelletsInternal = 0;
        StopCoroutine(DisplayScore());
    }

    IEnumerator RunStealPellet(NetworkObject pellet)
    {
        stealing = true;
        while (stealing)
        {
            Debug.Log("RunStealPellet ENum ran.");
            yield return new WaitForSeconds(.5f);
            currentFoodPelletList.Remove(pellet);
            currentFoodPellets--;
            StartCoroutine(DestroyPellet(pellet));
            stealing = false;
            StopCoroutine(RunStealPellet(pellet));
        }
    }
}





