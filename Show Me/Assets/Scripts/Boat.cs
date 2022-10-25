using System;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour, IInteractable
{
    private enum Direction { LEFT = -1, RIGHT = 1}

    [SerializeField] private float rowingForce = 200f;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private List<Transform> playerSpots = new List<Transform>();

    private Rigidbody rigidBody;
    private List<Player> embarkedPlayers = new List<Player>();

    private void Awake()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Update player positions
        for (int i = 0; i < embarkedPlayers.Count; i++)
        {
            if (i < playerSpots.Count)
            embarkedPlayers[i].transform.position = playerSpots[i].position;
        }

        /*if (Input.GetKeyDown(KeyCode.E))
        {
            AddRightForce();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddLeftForce();
        }*/
    }

    public bool IsFull()
    {
        return embarkedPlayers.Count == 2;
    }

    public void AddForce(Player _player)
    {
        if (_player == embarkedPlayers[0])
        {
            AddLeftForce();
        }
        else if (_player == embarkedPlayers[1])
        {
            AddRightForce();
        }
    }

    public void OnInteract(Player _interacter)
    {
        Enter(_interacter);
    }

    public void AddForceFromPosition(Vector3 _position, float _force, float _rotationSpeed)
    {
        if (!IsFull()) return;
        Vector3 dirVector = (_position - transform.position).normalized;
        Vector3 forceVector = dirVector * _force * Time.fixedDeltaTime;
        rigidBody.AddForce(forceVector);
        RotateTowards(dirVector, _rotationSpeed);
    }

    private void RotateTowards(Vector3 _dirVector, float _rotationSpeed)
    {
        transform.forward = Vector3.RotateTowards(transform.forward, _dirVector, _rotationSpeed/100 * Time.fixedDeltaTime, 0.0f);
    }

    private void Enter(Player _player)
    {
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
        AddBoatForce(Direction.LEFT);
    }
    
    private void AddRightForce()
    {
        AddBoatForce(Direction.RIGHT);
    }

    private void AddBoatForce(Direction _direction)
    {
        if (rigidBody == null || !IsFull()) return;

        Vector3 dirVector = ((int)_direction * transform.right + transform.forward).normalized;
        Vector3 forceVector = dirVector * rowingForce * Time.deltaTime;
        rigidBody.AddForce(forceVector);
        rigidBody.AddTorque(transform.up * (int) _direction * rotationSpeed);
    }
}
