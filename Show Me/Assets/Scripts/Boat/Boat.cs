using System;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour, IInteractable
{
    public enum BoatDirection { LEFT = -1, RIGHT = 1}

    [SerializeField] private float rowingForce = 200f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private List<Transform> playerSpots = new List<Transform>();

    [SerializeField] private Transform FrontLeftSpots;
    [SerializeField] private Transform FrontRightSpots;
    [SerializeField] private Transform BackLeftSpots;
    [SerializeField] private Transform BackRightSpots;

    [SerializeField] private GameObject FrontLeftPedal;
    [SerializeField] private GameObject FrontRightPedal;
    [SerializeField] private GameObject BackLeftPedal;
    [SerializeField] private GameObject BackRightPedal;

    // Stores a Player with the index of the spot
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

    public void SwitchSide(Player player, int vertical)
    {
        int oldIndex = playerSpotsDic[player];
        int newIndex = oldIndex + vertical;
        if (oldIndex == 0 || oldIndex == 1)
        {
            if (newIndex == 0 || newIndex == 1)
            {
                playerSpotsDic[player] = newIndex;
            }
        }
        else if (oldIndex == 2 || oldIndex == 3)
        {
            if (newIndex == 2 || newIndex == 3)
            {
                playerSpotsDic[player] = newIndex;
            }
        }
    }

    public bool IsFull()
    {
        return embarkedPlayers.Count == 2;
    }

    public void AddForce(Player _player)
    {
        int spotIndex = playerSpotsDic[_player];

        // Even spot mean Left Side, Odd means Right Side
        if (spotIndex % 2 == 0)
        {
            AddLeftForce();
        }
        else if (spotIndex % 2 == 1)
        {
            AddRightForce();
        }

        PlayPedalAnimation(spotIndex);
    }

    public void AddForceFromPosition(Vector3 _position, float _force, float _rotationSpeed)
    {
        if (!IsFull()) return;
        Vector3 dirVector = (_position - transform.position).normalized;
        Vector3 forceVector = dirVector * _force * Time.fixedDeltaTime;
        rigidBody.AddForce(forceVector);
        RotateTowards(dirVector, _rotationSpeed);
    }

    public void OnInteract(Player _interacter)
    {
        Enter(_interacter);
    }

    private void RotateTowards(Vector3 _dirVector, float _rotationSpeed)
    {
        transform.forward = Vector3.RotateTowards(transform.forward, _dirVector, _rotationSpeed/100 * Time.fixedDeltaTime, 0.0f);
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

        FindObjectOfType<CameraScript>().SetZoom(15);
    }

    private void Exit(Player _player)
    {
        embarkedPlayers.Remove(_player);
        _player.boat = null;

        FindObjectOfType<CameraScript>().EnableUnfixedZoom();
    }

    private void AddLeftForce()
    {
        AddBoatForce(BoatDirection.LEFT);
    }
    
    private void AddRightForce()
    {
        AddBoatForce(BoatDirection.RIGHT);
    }

    private void AddBoatForce(BoatDirection _direction)
    {
        if (rigidBody == null || !IsFull()) return;

        Vector3 dirVector = ((int)_direction * transform.right + transform.forward).normalized;
        Vector3 forceVector = dirVector * rowingForce * Time.deltaTime;
        rigidBody.AddForce(forceVector);
        rigidBody.AddTorque(transform.up * (int) _direction * rotationSpeed);
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
