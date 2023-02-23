using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private PlayerMovement player; 
    internal void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.GetComponent<PlayerMovement>();
            
        }
    }
    internal void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.canMove = true;
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.canMove = true;
            player = null;

        }
    }
}
