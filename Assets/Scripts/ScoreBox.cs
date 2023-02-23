using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Polarith.UnityUtils;
//using static ScoreKeeper;

public class ScoreBox : NetworkBehaviour
{
    internal Player player;
    internal AiPlayer aiPlayer;
    [SerializeField]
    internal ScoreBoard scoreBoard;
    [SerializeField]
   internal ScoreKeeper scoreKeeper;
    [SerializeField]
    internal bool isRedTeam=false;
    [SerializeField]
    internal bool isBlueTeam=false;

    private void OnEnable()
    {
        GameObject Go = GameObject.FindWithTag("ScoreKeeper");
        scoreKeeper = Go.GetComponent<ScoreKeeper>();
        ScoreKeeper.startGame += StartGame;

        
    }
    private void Start()
    {
        //GameObject Go = GameObject.FindWithTag("ScoreKeeper");
       //scoreKeeper = Go.GetComponent<ScoreKeeper>();
        //player = GetComponent<Player>();
        //if (scoreBoard == null)
        //{
        //    scoreBoard = GameObject.Find("ScoreBoard").GetComponent<ScoreBoard>();
        //}

        if (isBlueTeam)
            {
            gameObject.tag = "BlueScoreBox";
          
            }
        else if (isRedTeam)
            {
            gameObject.tag = "RedScoreBox";
             }
    }

    internal void StartGame()
    {
       
        Debug.Log("Game Started for Goal");




    }
    internal void OnTriggerEnter(Collider other)
    {

        if (isRedTeam && other.gameObject.CompareTag("RedPlayer"))
        {
            //Debug.Log("Red Team Score Added");

            if (other.gameObject.TryGetComponent(out Player _player))
            {
                player = _player;
                //Debug.Log("Player has collided with food pellet");
            }
            else if (other.gameObject.TryGetComponent(out AiPlayer _aiPlayer))
            {
                aiPlayer = _aiPlayer;
                //Debug.Log("Ai has collided with food pellet");
            }

            if (player != null)
            {
                if (player.currentFoodPellets > 0)
                {
                    scoreKeeper.redTeamScore = scoreKeeper.redTeamScore + (Mathf.Ceil(player.currentFoodPellets));
                    //player.currentFoodPellets = 0;
                    player.Scored();
                }

            }
            else if (aiPlayer != null)
            {

                if (aiPlayer.currentFoodPellets > 0)
                {
                    scoreKeeper.redTeamScore = scoreKeeper.redTeamScore + (Mathf.Ceil(aiPlayer.currentFoodPellets));
                    //player.currentFoodPellets = 0;
                    aiPlayer.Scored();
                }


            }
        }
        else if (isBlueTeam && other.gameObject.CompareTag("BluePlayer"))

        {
            if (other.gameObject.TryGetComponent(out Player _player))
            {
                player = _player;
                //Debug.Log("Player has collided with food pellet");
            }
            else if (other.gameObject.TryGetComponent(out AiPlayer _aiPlayer))
            {
                aiPlayer = _aiPlayer;
                //Debug.Log("Ai has collided with food pellet");
            }


            if (player != null)
            {
                if (player.currentFoodPellets > 0)
                {
                    scoreKeeper.blueTeamScore = scoreKeeper.blueTeamScore + (Mathf.Ceil(player.currentFoodPellets)/2);
                    //player.currentFoodPellets = 0;
                    player.Scored();
                }

            }
            else if (aiPlayer != null)
            {

                if (aiPlayer.currentFoodPellets > 0)
                {
                    scoreKeeper.blueTeamScore = scoreKeeper.blueTeamScore + (Mathf.Ceil(aiPlayer.currentFoodPellets)/2);
                    //player.currentFoodPellets = 0;
                    aiPlayer.Scored();
                }


            }
        }
        }


   internal void OnTriggerExit()
    {
        if (player != null)
        {
            player = null;

        }
        else if (aiPlayer != null)
        {

            aiPlayer = null;


        }



    }

}
  

