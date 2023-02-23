using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;


public class ScoreKeeper : NetworkBehaviour
{
    [SerializeField]
    [Networked]
    internal float blueTeamScore { get; set; }
    [SerializeField]
    [Networked]
    internal float redTeamScore { get; set; }
    [SerializeField]
    [Networked]
    internal int totalScore { get; set; }

    [SerializeField]
    private float gameLength;
    [SerializeField]
    private float preGameLength = 5;
    private TextMeshProUGUI timerText;
    private TextMeshProUGUI countDownTimerText;
    [Networked]
    private TickTimer Timer { get; set; }
    [Networked]
    private TickTimer PreTimer { get; set; }
    
    public delegate void OnStartGame();
    public static event OnStartGame startGame;

    public delegate void OnEndGame();
    public static event OnEndGame endGame;

    private EndScreen endScreen;
    private GameObject endScreenObject;
    private GameObject loadScreen;

    private bool preTimerRan = false;
    private bool timerRan = false;

    public static  bool gameStarted { get; set; }
    
    internal static bool gameEnded { get; set; }
    [Networked]
    internal float playerCount { get; set; }
    [SerializeField]
    internal List<GameObject> redTeamPlayers = new List<GameObject>();
    [SerializeField]
    internal List<GameObject> blueTeamPlayers = new List<GameObject>();
    //internal void Start()
    //{
         public override void Spawned()
         { 
        startGame = StartGame;
        endGame = EndGame;
        gameStarted = false;
        gameEnded = false;
        blueTeamScore = 0;
        redTeamScore = 0;
        totalScore = 0;
        Timer = TickTimer.CreateFromSeconds(Runner, gameLength);
        PreTimer = TickTimer.CreateFromSeconds(Runner, preGameLength);
        //GameObject startScreen = GameObject.FindWithTag("StartScreen");
        //startScreen.SetActive(false);
       // GameObject redGoal = GameObject.FindWithTag("RedScoreBox");
        //redGoal.GetComponent<ScoreBox>().scoreKeeper = this;
       // GameObject blueGoal = GameObject.FindWithTag("BlueScoreBox");
       // blueGoal.GetComponent<ScoreBox>().scoreKeeper = this;
        GameObject scoreBoard = GameObject.Find("ScoreBoard");
        scoreBoard.GetComponent<ScoreBoard>().scoreKeeper = this;
        //endScreenObject = GameObject.FindWithTag("EndScreen");
        //endScreen = endScreenObject.GetComponent<EndScreen>();
       // endScreenObject.SetActive(false);
        //endScreen = endScreenObject.GetComponent<EndScreen>();
        //TimeKeeper timeKeeper = GameObject.Find("TimeKeeper").GetComponent<TimeKeeper>();
        //timerText = timeKeeper.timeText;
        //countDownTimerText = timeKeeper.countDownTimeText;
        //loadScreen  = GameObject.FindWithTag("LoadScreen");
        //AssignPlayersToLists();
        //endScreen.BuildPlayerDict();
    }
    public override void FixedUpdateNetwork()
    {



            //if (!PreTimer.Expired(Runner))
            //{
            //    float minutes = Mathf.FloorToInt((float)(PreTimer.RemainingTime(Runner) / 60));
            //    float seconds = Mathf.FloorToInt((float)(PreTimer.RemainingTime(Runner) % 60));
            //    if (loadScreen == null)
            //    {
            //        loadScreen = GameObject.FindWithTag("LoadScreen");
            //    }
            //    else
            //    {
            //        loadScreen.SetActive(false);
            //    }
            //    //if (preTimer.RemainingTime(Runner) != null)
            //    //{
            //    countDownTimerText.text = Mathf.FloorToInt((float)(PreTimer.RemainingTime(Runner))).ToString();
            //    //}
            //}
            //else if (PreTimer.Expired(Runner) && !preTimerRan)
            //{

            //    preTimerRan = true;
            //    startGame?.Invoke();
            //    gameStarted = true;
            //    gameEnded = false;
            //    StartCoroutine(GoTimer());
            //}

            //if (!Timer.Expired(Runner))
            //{
            //    float minutes = Mathf.FloorToInt((float)(Timer.RemainingTime(Runner) / 60));
            //    float seconds = Mathf.FloorToInt((float)(Timer.RemainingTime(Runner) % 60));
            //    timerText.text = minutes.ToString() + ":" + seconds.ToString();

            //}
            //else if (Timer.Expired(Runner) && !timerRan)
            //{

            //    timerRan = true;
            //    gameEnded = true;
            //    gameStarted = false;
            //    endGame?.Invoke();

            //}
        
        totalScore = (int)redTeamScore + (int)blueTeamScore;
        //timerText.text = "Score: " + totalScore;
    }

  internal void StartGame()
    {
        gameStarted = true;
        Debug.Log("Score Keeper GameStarted");

    }

    internal void EndGame()
    {
        gameStarted = false;
        AssignPlayersToLists();
        CalculateFinalWinner();
        Debug.Log("GameEnded");

    }

    internal void CalculateFinalWinner()
    {
        if (endScreenObject==null)
        {
            endScreenObject = GameObject.FindWithTag("EndScreen");
        }
        //AssignPlayersToLists();

        endScreenObject.SetActive(true);
        endScreen.redTeamPlayers = redTeamPlayers;
        endScreen.blueTeamPlayers = blueTeamPlayers;
        endScreen.redTeamScore = redTeamScore;
        endScreen.blueTeamScore = blueTeamScore;
        endScreen.BuildPlayerDict();

        if (endScreen == null)
        {
            endScreen = gameObject.GetComponent<EndScreen>();
        }
        
        if ((int)redTeamScore > (int)blueTeamScore)
        {
            endScreen.redWins = true;
            endScreen.blueWins = false;
            endScreen.tie = false;
            endScreen.GameOver();
        }
        else if ((int)redTeamScore < (int)blueTeamScore)
        {
            endScreen.redWins = false;
            endScreen.blueWins = true;
            endScreen.tie = false;
            endScreen.GameOver();
        }
        else if ((int)redTeamScore == (int)blueTeamScore)
        {
            endScreen.redWins = false;
            endScreen.blueWins = false;
            endScreen.tie = true;
            endScreen.GameOver();
        }
        
    }
    
    internal void AssignPlayersToLists()
    {
        Debug.Log("Assigning Players to Array");

        redTeamPlayers.AddRange(GameObject.FindGameObjectsWithTag("RedPlayer"));
        Debug.Log("Red Team Players Found: " + redTeamPlayers.Count);
        blueTeamPlayers.AddRange(GameObject.FindGameObjectsWithTag("BluePlayer"));
        Debug.Log("Blue Team Players Found: " + blueTeamPlayers.Count);
    
    }


    
    private IEnumerator GoTimer()
    {
       
        while (true)
        {
        countDownTimerText.text = "Go!";
            yield return new WaitForSeconds(2);
            countDownTimerText.text = "";
            countDownTimerText.gameObject.SetActive(false);
            StopCoroutine(GoTimer());
        }
    }

}
