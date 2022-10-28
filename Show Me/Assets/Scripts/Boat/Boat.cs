using System;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour, IInteractable
{
    public enum BoatDirection { LEFT = -1, RIGHT = 1}

    [SerializeField] private float rowingForce = 200f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float boatZoom = 14f;
    [SerializeField] private List<Transform> playerSpots = new List<Transform>();

    [SerializeField] private Transform FrontLeftSpots;
    [SerializeField] private Transform FrontRightSpots;
    [SerializeField] private Transform BackLeftSpots;
    [SerializeField] private Transform BackRightSpots;

    [SerializeField] private GameObject FrontLeftPedal;
    [SerializeField] private GameObject FrontRightPedal;
    [SerializeField] private GameObject BackLeftPedal;
    [SerializeField] private GameObject BackRightPedal;

    // Stores a Player with the index of the spot and pedal
    private Dictionary<Player, int> playerSpotsDic = new Dictionary<Player, int>();
    private Rigidbody rigidBody;
    private List<Player> embarkedPlayers = new List<Player>();
    private List<GameObject> pedals;

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        pedals = new List<GameObject> 
        { 
            FrontLeftPedal, 
            FrontRightPedal, 
            BackLeftPedal,
            BackRightPedal, 
        };
        playerSpots = new List<Transform>
        {
            FrontLeftSpots,
            FrontRightSpots,
            BackLeftSpots,
            BackRightSpots
        };
    }

    private void Update()
    {
        foreach (Player player in playerSpotsDic.Keys)
        {
            int index = playerSpotsDic[player];
            player.transform.position = playerSpots[index].position;
        }
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

    public void AddRowForce(Player _player)
    {
        if (!IsFull()) return;

        int spotIndex = playerSpotsDic[_player];

        // Even spot mean Left Side, Odd means Right Side
        if (spotIndex % 2 == 0)
        {
            AddRightForce();
        }
        else if (spotIndex % 2 == 1)
        {
            AddLeftForce();
        }

        PlayPedalAnimation(spotIndex);
    }

    public void AddForceFromPosition(Vector3 _position, float _force, float _rotationSpeed)
    {
        if (!IsFull()) return;
        Vector3 dirVector = (_position - transform.position).normalized;
        Vector3 forceVector = dirVector * _force * Time.fixedDeltaTime;
        rigidBody.AddForce(forceVector);
        RotateTowardsXZ(dirVector, _rotationSpeed);
    }

    public void SwitchSide(Player _player, int _horizontal)
    {
        // Player 1: Slots 0 and 1, Player 2: Slots 2 and 3
        // When 0 of 2 (even, so left spot), _horizontal should be +1 and player should switch to right
        // When 1 and 3 (right spot), _horizontal should be -1 and player should switch to left
        int oldIndex = playerSpotsDic[_player];
        int newIndex = oldIndex + _horizontal;

        if ((oldIndex % 2 == 0 && _horizontal == 1)
            || (oldIndex % 2 == 1 && _horizontal == -1))
        {
            playerSpotsDic[_player] = newIndex;
        }
        else
        {
            Debug.Log("No switches?");
        }
    }

    public bool IsFull()
    {
        return embarkedPlayers.Count == 2;
    }

    private void Enter(Player _player)
    {
        if (embarkedPlayers.Count == 0)
        {
            playerSpotsDic.Add(_player, 0);
        }
        else if (embarkedPlayers.Count == 1)
        {
            int otherPlayerIndex = playerSpotsDic[embarkedPlayers[0]];
            int oppositeSpotIndex = 3 - otherPlayerIndex;
            playerSpotsDic.Add(_player, oppositeSpotIndex);
        }

        embarkedPlayers.Add(_player);
        _player.boat = this;

        if (IsFull())
        {
            EventSystem.RaiseEvent(EventName.BOAT_READY, boatZoom);
        }
    }

    private void Exit(Player _player)
    {
        embarkedPlayers.Remove(_player);
        playerSpotsDic.Remove(_player);
        _player.boat = null;

        EventSystem.RaiseEvent(EventName.BOAT_EXIT);
    }

    private void AddBoatForce(BoatDirection _direction)
    {
        if (rigidBody == null || !IsFull()) return;

        Vector3 dirVector = ((int)_direction * transform.right + transform.forward).normalized;
        Vector3 forceVector = dirVector * rowingForce * Time.deltaTime;
        rigidBody.AddForce(forceVector);
        rigidBody.AddTorque(transform.up * (int) _direction * rotationSpeed);
    }

    private void AddLeftForce()
    {
        AddBoatForce(BoatDirection.LEFT);
    }

    private void AddRightForce()
    {
        AddBoatForce(BoatDirection.RIGHT);
    }

    private void RotateTowardsXZ(Vector3 _dirVector, float _rotationSpeed)
    {
        Vector3 newForwardXZ = Vector3.RotateTowards(transform.forward, _dirVector, _rotationSpeed * Time.fixedDeltaTime, 0.0f);
        newForwardXZ.y = transform.forward.y;        // Ignore the difference in height
        transform.forward = newForwardXZ;
    }

    private void PlayPedalAnimation(int _pedalIndex)
    {
        GameObject pedal = pedals[_pedalIndex];
        Animator anim = pedal.GetComponent<Animator>();
        if (anim != null)
        {
            anim.CrossFade("PedalAnimation", 0);
        }
    }
}
