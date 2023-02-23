using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void OnStartGame();
    public static event OnStartGame startGame;

    public delegate void OnEndGame();
    public static event OnEndGame endGame;


    internal void Start()
    {
        //startGame = StartGame;
       // endGame = EndGame;
        ScoreKeeper.startGame += StartGame;
    }

    internal void StartGame()
    {
        startGame.Invoke();
        Debug.Log("Event Manager GameStarted");

    }

    internal void EndGame()
    {

        Debug.Log("GameEnded");


    }


}
