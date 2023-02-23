using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;


public class Lobby : MonoBehaviour
{

    LobbySpawner lobbySpawner;
    public TextMeshProUGUI[] BlueTeamNamesText;
    public TextMeshProUGUI[] RedTeamNamesText;
    public TextMeshProUGUI playersReadyText;
    public TextMeshProUGUI RoomCodeText;
    public TextMeshProUGUI totalPlayersText;
    public TextMeshProUGUI readyButtonText;
    
    public Button readyButton;


    public void Start()
    {
        GameObject Go = GameObject.Find("Network Runner");
        lobbySpawner = Go.GetComponent<LobbySpawner>();
        lobbySpawner.RedTeamNamesText = new TextMeshProUGUI[RedTeamNamesText.Length];
        for (int i = 0; i < RedTeamNamesText.Length; i++)
        {
            lobbySpawner.RedTeamNamesText[i] = RedTeamNamesText[i];
        }
     
        lobbySpawner.BlueTeamNamesText = new TextMeshProUGUI[BlueTeamNamesText.Length];
        for (int i = 0; i < BlueTeamNamesText.Length; i++)
        {
            lobbySpawner.BlueTeamNamesText[i] = BlueTeamNamesText[i];
        }
        lobbySpawner.BlueTeamNamesText = BlueTeamNamesText;
        lobbySpawner.RedTeamNamesText = RedTeamNamesText;
        lobbySpawner.playersReadyText = playersReadyText;
        lobbySpawner.RoomCodeText = RoomCodeText;
        lobbySpawner.totalPlayersText = totalPlayersText;
        lobbySpawner.readyButton = readyButton;

        Debug.Log("LobbySpawener Text filled");
    }
}
