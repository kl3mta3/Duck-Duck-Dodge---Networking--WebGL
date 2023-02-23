using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics.Contracts;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class LobbyPlayer
{
    public string playerName;
    public PlayerRef _player;
    public bool isReady;
    public bool isHost;
    public bool isRedTeam;
    public bool isBlueTeam;

}
public class LobbySpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    private GameObject canvas;
    private RunnerHandler runnerHandler;
    private NetworkRunner networkRunner;
    [SerializeField]
    internal NetworkObject startScreenPrefab;
    internal NetworkObject startScreen;

    [SerializeField]
    internal NetworkObject lobbyPlayerPrefab;
    internal NetworkObject lobbyPlayer;

    [SerializeField]
    internal GameObject mainScreen;
    public GameObject playerSpawner;
    [Networked]
    [SerializeField]
    public float totalPlayersInLobby { get; set; }
    [Networked]
    public float readyPlayers { get; set; }
    [Networked]
    public float blueTeamPlayers { get; set; }
    [Networked]
    public float redTeamPlayers { get; set; }
    public bool updateDisplay = false;
    public float lobbySize;
    public Lobby lobby;

    public TextMeshProUGUI[] BlueTeamNamesText /*= new TextMeshProUGUI[4]*/;
    public TextMeshProUGUI[] RedTeamNamesText /*= new TextMeshProUGUI[4]*/;
    public TextMeshProUGUI playersReadyText;
    public TextMeshProUGUI RoomCodeText;
    public TextMeshProUGUI totalPlayersText;
    public PlayerRef tempPlayerRef;

    private Vector3 lobbyPos;
    private Vector3 lobbyScale;
    private Transform lobbyparent;
    internal Button readyButton;
    //public Dictionary<PlayerRef, NetworkObject>  = new Dictionary<PlayerRef, NetworkObject>();
    public List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();
    public Dictionary<PlayerRef, string> _playerNames = new Dictionary<PlayerRef, string>();
    public void Start()
    {
        //networkRunner = runnerHandler.networkRunner;
       // mainScreen = GameObject.FindGameObjectWithTag("MainScreen");
        runnerHandler = GameObject.FindGameObjectWithTag("RunnerHandler").GetComponent<RunnerHandler>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas != null)
        {
            Debug.Log("canvas is " + canvas.gameObject.name);

        }
        else
        {

            Debug.Log("canvas is null");
        }
        //lobby = this.GetComponent<Lobby>();

    //    BlueTeamNamesText = new TextMeshProUGUI[lobby.BlueTeamNamesText.Length];
    //    RedTeamNamesText = new TextMeshProUGUI[lobby.RedTeamNamesText.Length];
    //    BlueTeamNamesText = lobby.BlueTeamNamesText;
    //    RedTeamNamesText = lobby.RedTeamNamesText;
    //    playersReadyText = lobby.playersReadyText;
    //    RoomCodeText = lobby.RoomCodeText;
    //    totalPlayersText = lobby.totalPlayersText;

    }
    public void fixedUpdateNetwork()
    {
       //if(updateDisplay)
       // {
         //UpdateDisplay();
       // }
    }

    //public void UpdateDisplay()
    //{
    //    Debug.Log("Updated Display");
    //    foreach (KeyValuePair<PlayerRef, LobbyPlayer> item in runnerHandler._blueTeam)
     
    //    {
    //        for (int i = 0; i < BlueTeamNamesText.Length; i++)
    //        {
    //            if (!BlueTeamNamesText[i].gameObject.activeInHierarchy)
    //            {
    //                BlueTeamNamesText[i].gameObject.SetActive(true);
    //                BlueTeamNamesText[i].text = item.Value.playerName;
    //                break;
    //            }
    //        }
    //    }
    //    foreach (KeyValuePair<PlayerRef, LobbyPlayer> item in runnerHandler._redTeam)

    //    {
    //        for (int i = 0; i < RedTeamNamesText.Length; i++)
    //        {
    //            if (!RedTeamNamesText[i].gameObject.activeInHierarchy)
    //            {
    //                RedTeamNamesText[i].gameObject.SetActive(true);
    //                RedTeamNamesText[i].text = item.Value.playerName;
    //                break;
    //            }
    //        }
    //    }
    //    playersReadyText.text = readyPlayers.ToString()+"/"+networkRunner.SessionInfo.PlayerCount;
    //    totalPlayersText.text = totalPlayersInLobby.ToString() + "/" + networkRunner.SessionInfo.PlayerCount;


    //}

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
       
        

            networkRunner = runner;
       
            runnerHandler = GameObject.FindGameObjectWithTag("RunnerHandler").GetComponent<RunnerHandler>();
            canvas = GameObject.FindGameObjectWithTag("Canvas");
            mainScreen = GameObject.FindGameObjectWithTag("MainScreen");
            lobbyPos = mainScreen.transform.position;
            //lobbyScale = mainScreen.transform.localScale;
            startScreen = runner.Spawn(startScreenPrefab, lobbyPos, Quaternion.identity, player);
            startScreen.gameObject.tag = "StartScreen";
            startScreen.transform.parent = canvas.transform;
            startScreen.transform.localScale = mainScreen.transform.localScale;
            lobby = startScreen.GetComponent<Lobby>();
        
            
            lobbySize = runnerHandler.lobbySize;
            tempPlayerRef = new PlayerRef();
           // BlueTeamNamesText = lobby.BlueTeamNamesText;
            //RedTeamNamesText = lobby.RedTeamNamesText;
            //playersReadyText = lobby.playersReadyText;
            //RoomCodeText = lobby.RoomCodeText;
            //totalPlayersText = lobby.totalPlayersText;
            //lobbyPlayer = runner.Spawn(lobbyPlayerPrefab, lobbyPos, Quaternion.identity, player);
            //Vector3 pos = mainScreen.transform.position;

            mainScreen.gameObject.SetActive(false);
            readyButton = lobby.readyButton;
            tempPlayerRef = player;
            readyButton.onClick.AddListener(StartGame);
            CreateLobbyPlayer(runnerHandler.playerName, player);
        updateDisplay = true;

    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        totalPlayersInLobby--;
        _playerNames.Remove(player);
        
        runner.Despawn(startScreen);
    }


    

    

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log("OnInputMissing");
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        Debug.Log("ShutdownReason: " + shutdownReason);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("Connected to server");
    }
    public void OnDisconnectedFromServer(NetworkRunner runner)
    {
        Debug.Log("Disconnected from server");
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log("OnConnectRequest");
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("OnConnectFailed" + reason);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage" + message);
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    { }
    public void OnSceneLoadDone(NetworkRunner runner)
    { }
    public void OnSceneLoadStart(NetworkRunner runner)
    { }

    public void CreateLobbyPlayer(string playerName, PlayerRef player)
    {
        LobbyPlayer temp = new LobbyPlayer();
        temp.playerName = playerName;
        Debug.Log("playerName is " + playerName);
        temp._player = player;
        Debug.Log("playerRef is " + player);
        AssignLobbyPlayerToTeam(temp);
        lobbyPlayers.Add(temp);

        totalPlayersInLobby++;
        _playerNames.Add(player, runnerHandler.playerName);

    }
    
    public void AssignLobbyPlayerToTeam(LobbyPlayer lobbyPlayer)
    {
        if (blueTeamPlayers <= lobbySize / 2)
        {
            lobbyPlayer.isBlueTeam = true;
            lobbyPlayer.isRedTeam = false;
            //runnerHandler._blueTeam.Add(lobbyPlayer._player, lobbyPlayer);
            blueTeamPlayers++;
        }
        else
        {
            lobbyPlayer.isBlueTeam = false;
            lobbyPlayer.isRedTeam = true;
            //runnerHandler._redTeam.Add(lobbyPlayer._player, lobbyPlayer);
            redTeamPlayers++;
        }


    }
   
    public void StartGame()
    {
        //runnerHandler.StartTriggered(tempPlayerRef);
        tempPlayerRef = new PlayerRef();


    }
}
