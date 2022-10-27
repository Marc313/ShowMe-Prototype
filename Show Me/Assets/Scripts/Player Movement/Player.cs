using UnityEngine;

public class Player : APickupable, IStateMachineOwner
{
    public PlayerControls controls;

    public ScratchPad sharedData { get; } = new ScratchPad();
    [HideInInspector] public Player carryingPlayer;
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Boat boat;

    [SerializeField] private float speed = 15f;
    [SerializeField] private float carryingObjectSpeed = 10f;
    [SerializeField] private float carryingPlayerSpeed = 30f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float pickupRange = 0.5f;
    [SerializeField] private float rowingCooldownTime = 0.5f;

    public MoveStateMachine moveMachine;
    public bool walkingEnabled;
    public Vector3 targetPos;
    private float currentSpeed;
    private IPickupable pickedUpTarget;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        moveMachine = GetComponent<MoveStateMachine>();
    }

    private void Start()
    {
        targetPos = transform.position;
        walkingEnabled = true;
        currentSpeed = speed;
        sharedData.Register("defaultSpeed", speed);
        sharedData.Register("carryingSpeed", carryingObjectSpeed);
        sharedData.Register("carryingPlayerSpeed", carryingPlayerSpeed);
    }

    private void Update()
    {
        HandleMovement();

        HandleInteractInput();
        HandleRowingInput();
    }

    private void FixedUpdate()
    {
    }

    public override void OnPickup(Player _carrier)
    {
        carryingPlayer = _carrier;
        walkingEnabled = false;
        moveMachine.SetState(new PickedupState());
    }

    public override void OnRelease()
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
        if (controls.InteractKeyPressed())
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

            Debug.Log(target);

            if (target == null) return;

            if (target is IPickupable)
            {
                PickupTarget((IPickupable) target);
            }
            else if (target is Boat)
            {
                target.OnInteract(this);
                moveMachine.SetState(new RowingState(rowingCooldownTime));
            }
            else
            {
                target.OnInteract(this);
            }
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

    private void HandleMovement()
    {
        moveMachine.GetState().HandleMovement(this, currentSpeed);
    }

    private void HandleInteractInput()
    {
        if (controls.InteractKeyPressed())
        {
            moveMachine.GetState().HandleInteractInput(this);
        }
    }

    private void HandleRowingInput()
    {
        if (controls.RowKeyPressed())
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
