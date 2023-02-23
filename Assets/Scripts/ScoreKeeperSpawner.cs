using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
//using static UnityEditor.PlayerSettings;



[SimulationBehaviour(Stages = SimulationStages.Forward, Modes = SimulationModes.Server | SimulationModes.Host)]
public class ScoreKeeperSpawner : SimulationBehaviour/*, ISceneLoadDone */
{
    [SerializeField]
    private NetworkObject scoreKeeperPrefab;
    void Start()
    {
        //Vector3 pos = new Vector3(0, 0, 0);
        //NetworkObject scoreKeeper = Runner.Spawn(scoreKeeperPrefab, pos, Quaternion.identity, Object.InputAuthority);
        //// Spawn a score keeper

        //scoreKeeper.name = "Level1";
    }




    //public void SceneLoadDone()
    //{
    //    SpawnScoreKeeper();

    //}

    //public void SpawnScoreKeeper()
    //{
    //    Vector3 pos = new Vector3(0, 0, 0);
    //    NetworkObject scoreKeeper = Runner.Spawn(scoreKeeperPrefab, pos, Quaternion.identity, Object.InputAuthority);
    //    // Spawn a score keeper

    //    scoreKeeper.name = "Level1";
    //}
}

