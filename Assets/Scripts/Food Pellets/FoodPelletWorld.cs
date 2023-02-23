using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


[System.Serializable]
public class WorldPeletAnimation
{
    [SerializeField]
    internal float minAmplitude = 1.0f;
    [SerializeField]
    internal float maxAmplitude = 1.0f;
    [SerializeField]
    internal float minFrequency = 1.0f;
    [SerializeField]
    internal float maxFrequency = 1.0f;
    [SerializeField]
    internal float offset = 1f;
    internal Vector3 posOffset = new Vector3();
    internal Vector3 tempPos = new Vector3();
}
public class FoodPelletWorld : NetworkBehaviour
{
    internal Player player;
    internal AiPlayer aiPlayer;
    [SerializeField]
    internal bool isBigPellet = false;
    //[SerializeField]
    //internal bool isEaten;
    [SerializeField]
    internal GameObject foodPellet;
    [SerializeField]
    private float pelletResetTime = 10;
    [SerializeField]
    internal WorldPeletAnimation _animation;

    [SerializeField]
    [Networked]
    internal bool IsVisable { get; set; }
    [SerializeField]
    
    internal bool IsEaten { get; set; }



    internal float amplitude = 0;
   
    internal float frequency = 0;

    internal float defaultResetTime;
    internal bool playerNear;
    internal Collider _collider;
    
    internal void Start()
    {
        _collider = GetComponent<SphereCollider>();
        defaultResetTime = pelletResetTime;
        
        _animation.posOffset = transform.position;
        amplitude = Random.Range(_animation.minAmplitude, _animation.maxAmplitude);
        frequency = Random.Range(_animation.minFrequency, _animation.maxFrequency);
    }
    public  override void FixedUpdateNetwork()
    {
        //ResetFoodPellet();
        if (IsEaten)
        {
            foodPellet.SetActive(false);
        }
        else
        {
            foodPellet.SetActive(true);
        }

    }
    internal void Update()
    {
        ResetFoodPellet();


    }
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log("Player has collided with food pellet");
        //    player = other.gameObject.GetComponent<Player>();
        //    isEaten = true;
        //    if (player != null)
        //    {
        //        player.PickUpFoodPellet();
        //    }
        //}


        if (other.gameObject.CompareTag("RedPlayer") || other.gameObject.CompareTag("BluePlayer"))
        {
            //Debug.Log(other.gameObject.transform.root.name + "Hit Pellet" );
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
            
            IsEaten = true;
            if (player != null)
            {
                if (isBigPellet)
                {
                    player.PickUpFoodPellet(10);
                }
                else
                {
                    player.PickUpFoodPellet(1);
                }
            }
            else if (aiPlayer != null)
            {
                if (isBigPellet)
                {
                    aiPlayer.PickUpFoodPellet(10);
                    aiPlayer.aiMovement.RemoveTargetPelletFromList(gameObject);
                    //aiPlayer.aiMovement.ArrivedAtPellet();
                }
                else
                {
                    aiPlayer.PickUpFoodPellet(1);
                    aiPlayer.aiMovement.RemoveTargetPelletFromList(gameObject);
                    //aiPlayer.aiMovement.ArrivedAtPellet();
                }
            }
        }

    }


    internal void ResetFoodPellet()
    {
        if (IsEaten)
        {
            //foodPellet.SetActive(false);
            _collider.enabled = false;
            pelletResetTime -=  Time.deltaTime;
            if (pelletResetTime <= 0)
            {
                //foodPellet.SetActive(true);
                pelletResetTime = defaultResetTime;
                IsEaten = false;
                player = null;
                aiPlayer = null;
                playerNear = false;
                _collider.enabled = true;
            }
        }
        else if(!IsEaten)
        {

            Animate();


        }
    }

    internal void Animate()
    {
        //transform.Rotate(new Vector3(15 * _animation.offset, 30 * _animation.offset, 45 * _animation.offset) * Time.deltaTime);

        _animation.tempPos = _animation.posOffset;
        _animation.tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;

        transform.position = _animation.tempPos;

    }
}
