
using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;

[ScriptHelp(BackColor = EditorHeaderBackColor.Steel)]
public class PlayerMovement : Fusion.NetworkBehaviour
{
    protected NetworkCharacterController _ncc;
    protected NetworkRigidbody _nrb;
    protected NetworkRigidbody2D _nrb2d;
    protected NetworkTransform _nt;
    protected Player _player;

    protected Transform _pb;
    [Networked]
    public bool IsQuacking { get; set; }
    [Networked]
    public Vector3 MovementDirection { get; set; }

    public bool TransformLocal = false;

    [SerializeField]
    internal GameObject rippleFx;

    [SerializeField]
    internal bool canMove = true;
    [SerializeField]
    internal bool canWalkThroughWalls = false;
    [DrawIf(nameof(ShowSpeed), Hide = true)]
    public float Speed = 6f;
    internal Rigidbody rb;

    bool ShowSpeed => this && !TryGetComponent<NetworkCharacterController>(out _);



    public void Awake()
    {
        CacheComponents();

    }

    public override void Spawned()
    {
        CacheComponents();
    }

    private void CacheComponents()
    {
        if (!_nt) _nt = GetComponent<NetworkTransform>();
        if (!_player) _player = GetComponent<Player>();
        if (!_pb) _pb = _player.playerbody;
        if (!_ncc) _ncc = GetComponent<NetworkCharacterController>();
        if (!_nrb) _nrb = GetComponent<NetworkRigidbody>();
        if (!_nrb2d) _nrb2d = GetComponent<NetworkRigidbody2D>();
        if (!rb) rb = GetComponent<Rigidbody>();

    }

    public override void FixedUpdateNetwork()
    {




        if (Runner.Config.PhysicsEngine == NetworkProjectConfig.PhysicsEngines.None)
        {
            return;
        }

        //if (MovementDirection != Vector3.zero)
        //{
        //    rippleFx.SetActive(true);
        //}
        //else
        //{
        //    rippleFx.SetActive(false);
        //}

        //if (ScoreKeeper.gameStarted)
        //{
            Vector3 direction;
            if (GetInput(out NetworkInputPrototype input))
            {
                direction = default;

                if (input.IsDown(NetworkInputPrototype.BUTTON_FORWARD))
                {
                    if (canMove)
                    {
                        direction += Vector3.forward;
                    //Debug.Log("Forward Player Pressed");
                }


                    _pb.transform.rotation = Quaternion.Euler(0, 0, 0);
                    _pb.transform.position = transform.position;
                }

                if (input.IsDown(NetworkInputPrototype.BUTTON_BACKWARD))
                {
                    if (canMove)
                    {
                        direction -= Vector3.forward;
                    //Debug.Log("Back Player Pressed");
                }


                    _pb.transform.rotation = Quaternion.Euler(0, 180, 0);
                    _pb.transform.position = transform.position;

                }

                if (input.IsDown(NetworkInputPrototype.BUTTON_LEFT))
                {
                    if (canMove)
                    {
                        direction -= Vector3.right;
                    //Debug.Log("left Player Pressed");
                }


                    _pb.transform.rotation = Quaternion.Euler(0, -90, 0);
                    _pb.transform.position = transform.position;
                }

                if (input.IsDown(NetworkInputPrototype.BUTTON_RIGHT))
                {
                    if (canMove)
                    {
                        direction += Vector3.right;
                    //Debug.Log("Right Player Pressed");
                }

                    _pb.transform.rotation = Quaternion.Euler(0, 90, 0);
                    _pb.transform.position = transform.position;

                }

                direction = direction.normalized;

                MovementDirection = direction;

                if (input.IsDown(NetworkInputPrototype.BUTTON_QUACK))
                {

                    Quack();


                }


                if (_ncc)
                {
                    _ncc.Move(direction);
                }
                else if (_nrb && !_nrb.Rigidbody.isKinematic)
                {
                    _nrb.Rigidbody.AddForce(direction * Speed);
                }
                else if (_nrb2d && !_nrb2d.Rigidbody.isKinematic)
                {
                    Vector2 direction2d = new Vector2(direction.x, direction.y + direction.z);
                    _nrb2d.Rigidbody.AddForce(direction2d * Speed);
                }
                else
                {
                    transform.position += (direction * Speed * Runner.DeltaTime);
                }




            }
        //}



    }

    internal void OnTriggerEnter(Collider other)
    {
        if (!canWalkThroughWalls)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                canMove = false;
            }
        }
        if (other.gameObject.CompareTag("BorderWall"))
        {
            canMove = false;
        }
    }
    internal void OnTriggerExit(Collider other)
    {
        if (!canWalkThroughWalls)
        {
            if (other.gameObject.CompareTag("Wall"))
            {
                canMove = true;
            }
        }
        if (other.gameObject.CompareTag("BorderWall"))
        {
            canMove = true;
        }
    }

    public virtual void Quack()
    {

        StartCoroutine(Quacking());
        IsQuacking = true;


    }
    public virtual IEnumerator Quacking()
    {
        if (IsQuacking)
        {
            //DuckCall.Play();
            yield return new WaitForSeconds(1f);
            //DuckCall.Stop();
            IsQuacking = false;
            StopCoroutine(Quacking());
        }
    }

}
