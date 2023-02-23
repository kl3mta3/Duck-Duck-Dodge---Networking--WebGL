using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEditor;

public class PlayerSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
   private NetworkRunner runner;

    [SerializeField] 
    public NetworkPrefabRef _playerPrefab;
    [SerializeField]
    public NetworkPrefabRef _aiPrefab;
    [SerializeField]
    internal Vector3 aiSpawnPoint;
    //[SerializeField]
    //public NetworkPrefabRef _ScoreKeeper;
    //[SerializeField]
    //public NetworkPrefabRef _LevelPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();
    private RunnerHandler runnerHandler;
    //[SerializeField]
    //public GameObject[] _redTeamspawnPoints = new GameObject[1];
    [SerializeField]
    public GameObject[] _blueTeamspawnPoints = new GameObject[1];
    void Start()
    {
        GameObject runnerHandlerObject = GameObject.FindGameObjectWithTag("RunnerHandler");
        runnerHandler = runnerHandlerObject.GetComponent<RunnerHandler>();
        
      
    }


    //player spawn
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {

            // Spawn the ScoreKeeper

            Vector3 spawnPosition = _blueTeamspawnPoints[0].transform.position;
            Vector3 levelSpawnpoint = new Vector3(0, 0, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);


            Debug.Log("Player Spawned");
            //Player newPlayer = networkPlayerObject.gameObject.GetComponentInChildren<Player>();
            if (networkPlayerObject.gameObject.GetComponentInChildren<Player>().createAi)
            {
                Vector3 aISpawnPosition = aiSpawnPoint;
               
                NetworkObject networkAIObject = runner.Spawn(_aiPrefab, aISpawnPosition, Quaternion.identity, player);


            }
            // Keep track of the player avatars so we can remove it when they disconnect
            _spawnedCharacters.Add(player, networkPlayerObject);
            GameObject menu = GameObject.FindGameObjectWithTag("MainScreen");
            menu.SetActive(false);
        }
        else if (runner.IsClient)
        {
            GameObject menu = GameObject.FindGameObjectWithTag("MainScreen");
            menu.SetActive(false);
        }

    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

        var data = new NetworkInputPrototype();

        if (Input.GetKey(KeyCode.W))
        {
            data.Buttons.Set(NetworkInputPrototype.BUTTON_FORWARD, true); ;
            //Debug.Log("Forward Network Pressed");
        }

        if (Input.GetKey(KeyCode.S))
        {
            data.Buttons.Set(NetworkInputPrototype.BUTTON_BACKWARD, true);
            //Debug.Log("Back Network Pressed");
        }

        if (Input.GetKey(KeyCode.A))
        {
            data.Buttons.Set(NetworkInputPrototype.BUTTON_LEFT, true);
            //Debug.Log("Left Network Pressed");
        }

        if (Input.GetKey(KeyCode.D))
        {
            data.Buttons.Set(NetworkInputPrototype.BUTTON_RIGHT, true);
            //Debug.Log("Right Network Pressed");
        }

        if (Input.GetKey(KeyCode.Space))
        {
            data.Buttons.Set(NetworkInputPrototype.BUTTON_QUACK, true);
           // Debug.Log("Quack Network Pressed");
        }

        input.Set(data);


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
        Debug.Log("OnConnectFailed"+ reason);
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log("OnUserSimulationMessage"+message);
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





 
}
