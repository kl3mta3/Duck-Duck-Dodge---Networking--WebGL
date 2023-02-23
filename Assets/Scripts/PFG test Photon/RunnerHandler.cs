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
using Unity.VisualScripting;

public class RunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    public NetworkRunner networkRunner;

    [SerializeField]
    public TMP_InputField playerNameInput;
    [SerializeField]
    public TMP_InputField roomNameInput;

    [SerializeField]
    public float lobbySize=8;

    [SerializeField]
    public GameObject PlayerSpawner;

    [SerializeField]
    public NetworkPrefabRef _playerPrefab;


    [SerializeField]
    public GameObject[] _blueTeamspawnPoints = new GameObject[1];

    public Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public string playerName;
    public string RoomName;

    void Start()
    {
        
    }
    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        //if (playerNameInput != null)
        //{
        //    playerName = playerNameInput.text;
        //}
        //else
        //{
        //    playerName = "";
        //}
        //if (roomNameInput != null)
        //{
        //    RoomName = roomNameInput.text;
        //}
        //else
        //{
        //    RoomName = null;
        //}

        if (sceneManager == null)
        {
            //handle networked objects that already exist in the scene
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        //runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = RoomName,
            CustomLobbyName = "(Lobby)"+RoomName,
            Initialized = initialized,
            SceneManager = sceneManager,
            PlayerCount = (int)lobbySize,
        });


    }


    public void AutoHostorClient()
    {


        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network Runner";
        networkRunner.ProvideInput = true;
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
 
        Debug.Log("clientTask: " + clientTask);
        Debug.Log($"Server NetworkRunner Started");

    }

    public void Host()
    {

        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network Runner";
        networkRunner.ProvideInput = true;
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);
     
        Debug.Log("clientTask: " + clientTask);
        Debug.Log($"Server NetworkRunner Started");

    }
    public void JoinAsClient()
    {


        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network Runner";
        networkRunner.ProvideInput = true;
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log("clientTask: " + clientTask);
        Debug.Log($"Server NetworkRunner Started");
    }

    public void SinglePlayer()
    {       networkRunner = Instantiate(networkRunnerPrefab);
            networkRunner.name = "Network Runner";
            networkRunner.ProvideInput = true;
            lobbySize = 1;
            var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Single, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log("clientTask: " + clientTask);
        Debug.Log($"Server NetworkRunner Started");

    }

    


}   
