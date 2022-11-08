using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Boat : MonoBehaviour { 

    public enum BoatDirection { LEFT = -1, RIGHT = 1}

    [Header("Variables")]
    [SerializeField] private float constantBoatForce;
    [SerializeField] private float rowingForce = 200f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float collisionDistance = 0.2f;
    [SerializeField] private float bounceForce = 100f;
    [SerializeField] private float boatZoom = 14f;

    [Header("References")]
    [SerializeField] private Collider downWall;
    [SerializeField] private Collider upWall;
    [SerializeField] private Transform CollisionChecker;

    [Space]
    [SerializeField] private GameObject FrontLeftPedal;
    [SerializeField] private GameObject FrontRightPedal;
    [SerializeField] private GameObject BackLeftPedal;
    [SerializeField] private GameObject BackRightPedal;

    private bool useConstantForce;
    private Vector3 lastBoatPos;
    public Rigidbody rigidBody;
    private List<Player> embarkedPlayers = new List<Player>();
    private List<GameObject> pedals;
    private RaycastHit hit;
    private Quaternion lastBoatRotation;

    public VisualEffect vfx1;
    public VisualEffect vfx2;

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");

        if (!IsFull()) rigidBody.isKinematic = true;

        lastBoatPos = transform.position;
        lastBoatRotation = transform.rotation;

        pedals = new List<GameObject> 
        { 
            FrontLeftPedal, 
            FrontRightPedal, 
            BackLeftPedal,
            BackRightPedal, 
        };

        downWall.gameObject.SetActive(false);

        vfx1.enabled = false;
        vfx2.enabled = false;
    }
    
    private void Update()
    {
        if (IsFull())
        {
            rigidBody.AddForce(Vector3.forward * constantBoatForce * Time.deltaTime);
        }

        MovePlayers();

        CheckContact();
        CheckLand();

        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }

    private void CheckLand()
    {
        if (GroundInRangeOfWall(upWall.transform.position, 3f))
        {
            upWall.gameObject.SetActive(false);
            Debug.Log("Upwall Off");
            useConstantForce = false;    // enable constant force when away from land
        }
        else if (!upWall.gameObject.activeSelf)
        {
            upWall.gameObject.SetActive(true);
        }

        if (GroundInRangeOfWall(downWall.transform.position, 3f))
        {
            downWall.gameObject.SetActive(false);
            Debug.Log("Downwall Off");
        }
        else if (!downWall.gameObject.activeSelf)
        {
            downWall.gameObject.SetActive(true);
        }

        if (upWall.gameObject.activeSelf)
        {
            useConstantForce = true;    // enable constant force when away from land
        }
    }

    private bool GroundInRangeOfWall(Vector3 _wallPos, float _range)
    {
        Collider[] colliders = Physics.OverlapSphere(_wallPos, _range);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                return true;
            }
        }

        return false;
    }

    public void OnInteract(Player _interacter)
    {
        if (embarkedPlayers.Contains(_interacter))
        {
            Exit(_interacter);
        }
        else
        {
            Enter(_interacter);
        }
    }

    public void AddForceFromPosition(Vector3 _position, float _force, float _rotationSpeed)
    {
        Vector3 dirVector = (_position - transform.position).normalized;
        AddBoatForce(dirVector, _force);
        RotateTowardsXZ(dirVector, _rotationSpeed);
    }

    public bool IsFull()
    {
        return embarkedPlayers.Count == 2;
    }

    public void Enter(Player _player)
    {
        /*if (embarkedPlayers.Count == 0)
        {
            int leftSpot = _player.playerType == PlayerType.Player1 ? 0 : 2;
            playerSpotsDic.Add(_player, leftSpot);
        }
        else if (embarkedPlayers.Count == 1)
        {
            int otherPlayerIndex = playerSpotsDic[embarkedPlayers[0]];
            int oppositeSpotIndex = 3 - otherPlayerIndex;
            playerSpotsDic.Add(_player, oppositeSpotIndex);
        }*/

        //if (embarkedPlayers.Contains(_player)) return;

        int index = (int)_player.playerType;
        embarkedPlayers.Add(_player);
        _player.boat = this;

        if (IsFull())
        {
            downWall.gameObject.SetActive(true);
            rigidBody.isKinematic = false;
            EventSystem.RaiseEvent(EventName.BOAT_READY, boatZoom);
            gameObject.layer = LayerMask.NameToLayer("Boat");
            vfx1.enabled = true;
            vfx2.enabled = true;
        }
    }

    public void Exit(Player _player)
    {
        embarkedPlayers.Remove(_player);
        //playerSpotsDic.Remove(_player);
        _player.boat = null;

        EventSystem.RaiseEvent(EventName.BOAT_EXIT);
    }

    public void AddRowForce(BoatDirection _direction)
    {
        if (rigidBody == null || !IsFull()) return;

        Vector3 dirVector = ((int)_direction * transform.right + transform.forward).normalized;
        Vector3 forceVector = dirVector * rowingForce * Time.deltaTime;
        rigidBody.AddForce(forceVector);
        rigidBody.AddTorque(transform.up * (int) _direction * rotationSpeed);
    }

    public void AddRowForceFromPosition(BoatDirection _direction, Vector3 _position)
    {
        if (rigidBody == null || !IsFull()) return;

        Vector3 dirVector = ((int)_direction * transform.right + transform.forward).normalized;
        Vector3 forceVector = dirVector * rowingForce * Time.deltaTime;
        rigidBody.AddForceAtPosition(forceVector, _position);
        rigidBody.AddTorque(transform.up * (int)_direction * rotationSpeed);
    }

    private void AddBoatForce(Vector3 _direction, float _force)
    {
        if (rigidBody == null || !IsFull()) return;

        Vector3 forceVector = _direction * _force * Time.deltaTime;
        rigidBody.AddForce(forceVector);
    }

    private void AddLeftForce()
    {
        AddRowForce(BoatDirection.LEFT);
    }

    private void AddRightForce()
    {
        AddRowForce(BoatDirection.RIGHT);
    }

    private void RotateTowardsXZ(Vector3 _dirVector, float _rotationSpeed)
    {
        Vector3 newForwardXZ = Vector3.RotateTowards(transform.forward, _dirVector, _rotationSpeed * Time.fixedDeltaTime, 0.0f);
        newForwardXZ.y = transform.forward.y;        // Ignore the difference in height
        transform.forward = newForwardXZ;
    }

    private void MovePlayers()
    {
        foreach (Player player in embarkedPlayers)
        {
            Vector3 deltaPosition = transform.position - lastBoatPos;
            player.rigidBody.position += deltaPosition;

            /*            float deltaRotationY = transform.rotation.eulerAngles.y - lastBoatRotation.eulerAngles.y;
                        player.rigidBody.rotation = Quaternion.Euler(player.rigidBody.rotation.eulerAngles.x, 
                                                                        player.rigidBody.rotation.eulerAngles.y + deltaRotationY,
                                                                        player.rigidBody.rotation.eulerAngles.z);*/

            player.rigidBody.rotation = transform.rotation;
        }

        lastBoatPos = transform.position;
        lastBoatRotation = transform.rotation;
    }

    private void CheckContact()
    {
        if (CollisionChecker == null) return;

        float boatWidth = (GetComponent<Collider>().bounds.extents.x + .3f);
        Vector3 boxExtends = new Vector3(boatWidth, 1, collisionDistance);
        Collider[] colliders = Physics.OverlapBox(CollisionChecker.position, boxExtends, transform.rotation);

        foreach (Collider collider in colliders)
        {
            Player player = collider.GetComponent<Player>();
            if (player == null && (collider.name.Contains("Stalagmite") || collider.name.Contains("Rock") || collider.name.Contains("Siren") || collider.name.Contains("WaterWall")))
            {
                Vector3 forceDirection = (collider.transform.position - transform.position).normalized;
                AddBoatForce(-forceDirection, bounceForce);
                break;
            }
        }
    }
}
