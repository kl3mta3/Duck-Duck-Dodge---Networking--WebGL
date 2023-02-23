using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Fusion;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine.Events;
//using static UnityEditor.FilePathAttribute;
//using static UnityEditor.PlayerSettings;

public class Player : NetworkBehaviour
{

    public static Player Local { get; set; }
   
    //private NetworkRunner _runner;
    [SerializeField]
    internal Transform playerbody;
    [Networked]
    public int currentFoodPellets { get; set; }
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
    internal GameObject baseFoodPelletObject;


    [SerializeField]
    internal PlayerMovement controller;
    [SerializeField]
    internal FoodPelletPlayer baseFoodPellet;

    internal FoodPelletPlayer lastFoodPelletCreated;

    [SerializeField]
    internal GameObject playerFoodPellet;
    [SerializeField]
    internal AudioSource pickupSound;
    [SerializeField]
    internal AudioSource scoreSound;
    [SerializeField]
    internal AudioSource DuckCall;
    [SerializeField]
    internal AudioSource bgMusic;
    [SerializeField]
    internal TextMeshProUGUI currentScoreText;
    internal Player player;
    [SerializeField]

    internal GameObject currentFoodPelletPool;
    [SerializeField]
    private List<NetworkObject> currentFoodPelletList = new List<NetworkObject>();
    [SerializeField]
    private List<Vector3> positionHistory = new List<Vector3>();

    //[SerializeField]
    //public bool isRedTeam = false;

    //[SerializeField]
    //public bool isBlueTeam = false;



    [Networked]
    [SerializeField]
    public bool IsRedTeam { get; set; }
    [Networked]
    [SerializeField]
    public bool IsBlueTeam { get; set; }

    [SerializeField]
    internal GameObject playerGoal;
    [SerializeField]
    internal GameObject goalDirectionArrow;
    [SerializeField]
    internal GameObject spawnPoint;


    [SerializeField]
    internal Material redMaterial;
    [SerializeField]
    internal Material blueMaterial;

    internal Material prefabMaterial;
    [SerializeField]
    internal GameObject[] playerAdjustableBodyParts;
    internal bool isQuacking;
    internal bool muteMusic = false;
    [SerializeField]
    internal bool isAIDuck = false;
    internal bool stealing = false;
    [SerializeField]
    internal bool canSteal = true;

    private bool gameRunning;

    internal string playerName { get; set; }

    //for testing
    [SerializeField]
    internal NetworkObject enemyPlayer;
    internal bool enemyPlayerSpawned = false;
    [SerializeField]
    internal bool createAi = false;

    [SerializeField]
    internal GameObject scoreKeeperPrefab;
    [Networked]
    internal bool scorekeeperSpawned { get; set; }
    internal ScoreKeeper scoreKeeper;

    //public void OnEnable()
    //{
    //    Debug.Log("Player OnEnable Triggered");
    //    ScoreKeeper.startGame += StartGame;
    //}

    //private void OnDisable()
    //{
    //    ScoreKeeper.startGame -= StartGame;

    //}
   
    //public void PlayerLeft(PlayerRef player)
    //{

    //    if (player == Object.InputAuthority)
    //    {
    //        Runner.Despawn(Object);
    //        Debug.Log("Local Player DeSpawned");
    //    }

    //}
    internal void Start()
    {
        playerName = gameObject.transform.root.name;
        //ScoreKeeper.startGame += StartGame;
        //if (GameObject.Find("ScoreKeeper") == null)
        //{

        //    if (scorekeeperSpawned != true)
        //    {
        //        Vector3 pos = new Vector3(0, 0, 0);
        //        NetworkObject scoreBoard = Runner.Spawn(scoreKeeperPrefab, pos, Quaternion.identity, Object.InputAuthority);
        //        scoreKeeper = scoreBoard.GetComponent<ScoreKeeper>();
        //        scorekeeperSpawned = true;
        //        EventManager.startGame += StartGame;
        //        gameRunning = ScoreKeeper.gameStarted;
        //    }
        //    else
        //    {
        //        scorekeeperSpawned = true;
        //        EventManager.startGame += StartGame;
        //        gameRunning = ScoreKeeper.gameStarted;
        //    }
        //}
        //else
        //{
        //    scorekeeperSpawned = true;
        //    EventManager.startGame += StartGame;
        //    gameRunning = ScoreKeeper.gameStarted;
        //}

        player = gameObject.GetComponent<Player>();
        lastFoodPelletCreated = baseFoodPellet;
        //lastFoodPelletCreated = baseFoodPellet;
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

        //MuteMusic();

    }





    internal void StartGame()
    {
        gameRunning = ScoreKeeper.gameStarted;
        Debug.Log("Game Started for Player");




    }
    private void MuteMusic()
    {
        muteMusic = !muteMusic;
        if (muteMusic)
        {
            bgMusic.gameObject.SetActive(false);
        }
        else
        {

            bgMusic.gameObject.SetActive(true);
        }

    }
    public override void FixedUpdateNetwork()
    {


        //if (ScoreKeeper.gameStarted)
        //{
           // Debug.Log("Game Started for Player");


            if (currentFoodPellets > 0)
            {
                hasPellets = true;
                positionHistory.Insert(0, pelletFollowRecordPoint.transform.position);
                MoveCurrentPellets();
                if (!isAIDuck)
                {
                    goalDirectionArrow.SetActive(true);
                    AdjustGoalDirectionArrow();
                }
            }
            else
            {
                hasPellets = false;

                if (!isAIDuck)
                {
                    goalDirectionArrow.SetActive(false);
                }
            }



            if (!isAIDuck)
            {
                playerSpeed = controller.Speed;

                if (controller.IsQuacking && !isQuacking)
                {
                    isQuacking = true;
                    DuckCall.Play();
                }
                else if (!controller.IsQuacking)
                {
                    isQuacking = false;
                    DuckCall.Stop();
                }
            }
        //}
    }

    internal void PickUpFoodPellet(float amount)
    {

        currentFoodPellets += (int)amount;
        pickupSound.Play();
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
        pelletScript.player = player;
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

    internal void AdjustGoalDirectionArrow()
    {
        if (!isAIDuck)
        {
            Vector3 directionToMove = playerGoal.transform.position - goalDirectionArrow.transform.position;
            goalDirectionArrow.transform.LookAt(directionToMove);

            //goalDirectionArrow.transform.LookAt(playerGoal.transform);
        }

    }
    internal void Scored()
    {
        for (int i = 0; i < currentFoodPelletList.Count; i++)
        {
            StartCoroutine(DestroyPellet(currentFoodPelletList[i]));
        }
        StartCoroutine(DisplayScore());
        scoreSound.Play();
        totalFoodPelletsGathered = totalFoodPelletsGathered + currentFoodPellets;
        //currentFoodPellets = 0;
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

    internal void Magnetisim()
    {





    }
    internal void SuperSight()
    {





    }

    internal void Speeding()
    {





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
                            foodPellet.player.LosePellet(foodPellet.gameObject.GetComponent<NetworkObject>());
                        }
                        else if (foodPellet.aiPlayer != null)
                        {
                            foodPellet.aiPlayer.LosePellet(other.gameObject.GetComponent<NetworkObject>());
                            foodPellet.aiPlayer.aiMovement.playerTheif = gameObject;

                        }

                    }
                    else if (foodPellet.IsBluePellet && IsRedTeam)
                    {
                        StealPellets();
                        if (foodPellet.player != null)
                        {
                            foodPellet.player.LosePellet(foodPellet.gameObject.GetComponent<NetworkObject>());
                        }
                        else if (foodPellet.aiPlayer != null)
                        {
                            foodPellet.aiPlayer.LosePellet(other.gameObject.GetComponent<NetworkObject>());
                            foodPellet.aiPlayer.aiMovement.playerTheif = gameObject;

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
            currentScoreText.gameObject.SetActive(true);
            currentScoreText.text = /*"Current Pellets: " +*/ currentFoodPellets.ToString();
            Vector3 originalScale = currentScoreText.transform.localScale;
            int waitTime = 40;
            while (waitTime > 0)
            {
                currentScoreText.transform.localScale = Vector3.Lerp(currentScoreText.transform.localScale, new Vector3(4f, 4f, 4f), Time.deltaTime * 5);
                waitTime--;
                yield return new WaitForSeconds(.1f);
            }
            // yield return new WaitForSeconds(5);
            currentScoreText.gameObject.SetActive(false);
            currentFoodPellets = 0;
            StopCoroutine(DisplayScore());
        }
        IEnumerator Quack()
        {

            DuckCall.Play();
            yield return new WaitForSeconds(1f);
            DuckCall.Stop();
            StopCoroutine(Quack());


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


       

