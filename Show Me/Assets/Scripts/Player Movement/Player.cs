using System;
using UnityEngine;

public class Player : MonoBehaviour, IPickupable, IStateMachineOwner
{
    public ScratchPad sharedData { get; } = new ScratchPad();
    public float modelHeight { get; private set; }

    [SerializeField] public DirectionControls controls;
    [SerializeField] public KeyCode interactKey = KeyCode.E;
    [SerializeField] public KeyCode rowingKey = KeyCode.Q;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float carryingObjectSpeed = 10f;
    [SerializeField] private float carryingPlayerSpeed = 30f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float pickupRange = 0.5f;

    public Player carryingPlayer;

    public Boat boat;
    //private Transform boatSpot;

    private MoveStateMachine moveMachine;
    private float currentSpeed;
    private bool walkingEnabled;
    private IPickupable pickedUpTarget;

    private void Awake()
    {
        moveMachine = new MoveStateMachine(this);
    }

    private void Start()
    {
        walkingEnabled = true;
        currentSpeed = speed;
        sharedData.Register("defaultSpeed", speed);
        sharedData.Register("carryingSpeed", carryingObjectSpeed);
        sharedData.Register("carryingPlayerSpeed", carryingPlayerSpeed);

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            modelHeight = renderer.bounds.extents.y;
        }
    }

    private void Update()
    {
        HandleMovement();
        HandlePickupInput();
        HandleRowingInput();
    }

    public void OnInteract(Player _interacter)
    {
        OnPickup(_interacter);
    }

    public void OnPickup(Player _carrier)
    {
        carryingPlayer = _carrier;
        transform.position = _carrier.transform.position + Vector3.up * 2f;
        walkingEnabled = false;
        moveMachine.SetState(new PickedupState());
    }

    public void OnRelease()
    {
        carryingPlayer = null;
        moveMachine.SetState(new DefaultMoveState());
    }

    public void SetSpeed(float _speed)
    {
        currentSpeed = _speed;
    }

    public void EnableMovement()
    {
        walkingEnabled = true;
    }

    public void DisableMovement()
    {
        walkingEnabled = false;
    }

    public void OverlapInteract()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // Look for closest target
            IInteractable target = null;
            float closestDistance = float.MaxValue;
            Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

            // Look for the closest IInteractable
            foreach (Collider collider in colliders)
            {
                IInteractable interactable = collider.GetComponent<IInteractable>();
                if (interactable != null && interactable != (IInteractable)this)
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        target = interactable;
                    }
                }
            }

            if (target == null) return;

            if (target is IPickupable)
            {
                PickupTarget((IPickupable) target);
            }
            else if (target is Boat)
            {
                target.OnInteract(this);
                moveMachine.SetState(new RowingState());
            }
            else
            {
                target.OnInteract(this);
            }
        }
    }

    private void PickupTarget(IPickupable _target)
    {
        if (_target != null)
        {
            pickedUpTarget = _target;
            sharedData.Register("pickedUp", pickedUpTarget);
            moveMachine.SetState(new CarryingState());
            _target.OnPickup(this);
        }
    }

    public void FreeTarget()
    {
        MonoBehaviour target = (MonoBehaviour)pickedUpTarget;
        target.transform.position = transform.position + transform.forward * 2f;
        pickedUpTarget.OnRelease();

        moveMachine.SetState(new DefaultMoveState());

        pickedUpTarget = null;
        sharedData.Register("pickedUp", null);
    }

    public void FreeSelf(Player _otherPlayer)
    {
        _otherPlayer.FreeTarget();
    }

    private void HandleMovement()
    {
        if (walkingEnabled)
        {
            moveMachine.GetState().HandleMovement(this, currentSpeed);
        }
    }

    private void HandlePickupInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            moveMachine.GetState().HandlePickupInput(this);
        }
    }

    private void HandleRowingInput()
    {
        if (Input.GetKeyDown(rowingKey))
        {
            if (boat == null) return;

            MovingState currentState = moveMachine.GetState();
            if (currentState.GetType() == typeof(RowingState))
            {
                currentState.HandleRowingInput(this);
            }
        }
    }
}
