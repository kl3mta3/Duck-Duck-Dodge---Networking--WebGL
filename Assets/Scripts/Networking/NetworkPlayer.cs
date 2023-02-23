using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            Debug.Log("Local Player Spawned");
        }
        else
        {
            Debug.Log("Local Player Spawned");
        }

    }
    public void PlayerLeft(PlayerRef player)
    {

        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
            Debug.Log("Local Player DeSpawned");
        }

    }
}
