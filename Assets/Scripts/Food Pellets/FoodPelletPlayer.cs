using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Fusion;
//using UnityEngine.Rendering.VirtualTexturing;

public class FoodPelletPlayer : NetworkBehaviour
{
    [SerializeField]
    [Networked]
    internal Player player { get; set; }

    [SerializeField]
    //[Networked]
    internal AiPlayer aiPlayer { get; set; }

    internal Player newPlayer;
    [SerializeField]
    [Networked]
    internal bool IsRedPellet { get; set; }
    [SerializeField]
    [Networked]
    internal bool IsBluePellet { get; set; }

    internal bool PlayerIsRed;
    internal bool PlayerIsBlue;




    internal void DestroyFoodPellet()
    {
        if (player != null)
        {
            player.Runner.Despawn(this.gameObject.GetComponent<NetworkObject>());
        }
        else if (aiPlayer != null)
        {
            aiPlayer.Runner.Despawn(this.gameObject.GetComponent<NetworkObject>());
        }

    }



}
