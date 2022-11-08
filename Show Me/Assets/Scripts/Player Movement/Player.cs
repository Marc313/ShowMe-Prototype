using System;
using UnityEngine;

public class Player : APickupable, IStateMachineOwner
{
    public PlayerControls controls;
    public PlayerType playerType;

    public ScratchPad sharedData { get; } = new ScratchPad();
    [HideInInspector] public Player carryingPlayer;
    [HideInInspector] public Rigidbody rigidBody;
    [HideInInspector] public Boat boat;
    [HideInInspector] public MoveStateMachine moveMachine;
    [HideInInspector] public bool walkingEnabled;
    [HideInInspector] public Vector3 targetPos;

    [SerializeField] private float speed = 15f;
    [SerializeField] private float carryingObjectSpeed = 10f;
    [SerializeField] private float carryingPlayerSpeed = 30f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float pickupRange = 0.5f;
    [SerializeField] private float rowingCooldownTime = 0.5f;
    [SerializeField] private float groundCheck;
    [SerializeField] private float gravityStrength;
    [SerializeField] private float jumpHeight;

    private float startY;

    private float currentSpeed;
    private IPickupable pickedUpTarget;
    private RaycastHit groundHit;

    private bool yConstraint = true;
/*
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("PLAYER: " + collision.gameObject.name);
    }
*/
    protected override void Awake()
    {
        base.Awake();

        rigidBody = GetComponent<Rigidbody>();
        moveMachine = GetComponent<MoveStateMachine>();
    }

    private void OnEnable()
    {
        controls.OnEnable();
        controls.RowingPressed += () => HandleUseInput();
        controls.InteractPressed += () => HandleInteractInput();
        controls.JumpPressed += () => HandleJumpInput();
    }

    private void OnDisable()
    {
        controls.OnDisable();
        controls.RowingPressed -= () => HandleUseInput();
        controls.InteractPressed -= () => HandleInteractInput();
        controls.JumpPressed -= () => HandleJumpInput();

    }

    private void Start()
    {
        targetPos = transform.position;
        walkingEnabled = true;
        currentSpeed = speed;
        sharedData.Register("defaultSpeed", speed);
        sharedData.Register("carryingSpeed", carryingObjectSpeed);
        sharedData.Register("carryingPlayerSpeed", carryingPlayerSpeed);

        startY = transform.position.y;
    }

    protected override void Update()
    {
        base.Update();
        //Debug.Log($"IsGrounded: {IsGrounded()}");
        if (!IsGrounded()) ApplyGravity();
        HandleMovement();
        CheckHeight();

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (yConstraint)
            {
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
                yConstraint = false;
            }
            else
            {
                rigidBody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                yConstraint = true;
            }
        }
    }

    private void CheckHeight()
    {
        if (Math.Abs(transform.position.y - startY) > 5f)
        {
            transform.position = FindObjectOfType<Boat>().transform.position + Vector3.up * 4;
        }
    }

    public override void OnPickup(Player _carrier)
    {
        base.OnPickup(_carrier);
        carryingPlayer = _carrier;
        walkingEnabled = false;
        moveMachine.SetState(new PickedupState());
    }

    public override void OnRelease()
    {
        base.OnRelease();
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

    public void OverlapInteractBoatPriority()
    {
        // Look for closest target
        IInteractable target = null;
        float closestDistance = float.MaxValue;
        Collider[] colliders = Physics.OverlapSphere(transform.position, pickupRange);

        // Look for the closest IInteractable
        foreach (Collider collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();
            if (interactable == null) interactable = collider.GetComponentInParent<IInteractable>();
            if (interactable is Boat)
            {
                target = interactable;
                break;
            }

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

        if (target is Boat)
        {
            target.OnInteract(this);
            moveMachine.SetState(new RowingState(rowingCooldownTime));
        }
        else if (target is IPickupable)
        {
            PickupTarget((IPickupable)target);
        }
        else
        {
            target.OnInteract(this);
        }
    }

    public void OverlapInteractClosest()
    {
        //if (controls.InteractKeyPressed())
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

            if (target is Boat)
            {
                target.OnInteract(this);
                moveMachine.SetState(new RowingState(rowingCooldownTime));
            }
            else if (target is IPickupable)
            {
                PickupTarget((IPickupable) target);
            }
            else
            {
                target.OnInteract(this);
            }
        }
    }

    public void FreeTarget()
    {
        if (!pickedUpTarget.locked)
        {
            MonoBehaviour target = (MonoBehaviour)pickedUpTarget;
            target.transform.position = transform.position + transform.forward * .2f;
        }
        
        pickedUpTarget.OnRelease();

        moveMachine.SetState(new DefaultMoveState());

        pickedUpTarget = null;
        sharedData.Register("pickedUp", null);
    }

    public void FreeSelf(Player _otherPlayer)
    {
        _otherPlayer.FreeTarget();
    }

    private void ApplyGravity()
    {
        Vector3 velocity = rigidBody.velocity;
        velocity.y = -gravityStrength * Time.deltaTime;
        rigidBody.velocity = velocity;

    }

    public void Jump()
    {
        Debug.Log(IsGrounded());
        Vector3 rbVelocity = rigidBody.velocity;
        rbVelocity.y = Mathf.Sqrt(jumpHeight * 2f);
        rigidBody.velocity = rbVelocity;
    }

    public bool IsGrounded()
    {
        /*if (Physics.Raycast(transform.position, -transform.up, out groundHit, groundCheck))
        {
            if (groundHit.collider != GetComponent<Collider>()) // rip performance
                return true;
        }*/

        Collider[] colliders = Physics.OverlapSphere(transform.position, groundCheck);

        foreach (Collider collider in colliders)
        {
            if (collider != GetComponentInChildren<Collider>() && !collider.isTrigger)
            {
/*                Debug.Log(collider.name);
*/                return true;
            }
        }

        return false;
    }

    private void PickupTarget(IPickupable _target)
    {
        if (_target != null)
        {
            if (_target is Player)
            {
                if (((Player)_target).pickedUpTarget != null) return;
            }

            pickedUpTarget = _target;
            sharedData.Register("pickedUp", pickedUpTarget);
            moveMachine.SetState(new CarryingState());
            _target.OnInteract(this);
        }
    }

    private void HandleMovement()
    {
        moveMachine.GetState().HandleMovement(this, currentSpeed);
    }

    private void HandleInteractInput()
    {
        moveMachine.GetState().HandleInteractInput(this);
    }

    private void HandleUseInput()
    {
        if (boat == null || pickedUpTarget == null) return;
         
        IUsable usable = (IUsable)pickedUpTarget;
        usable.OnUse(this);

        /*MovingState currentState = moveMachine.GetState();
        if (currentState.GetType() == typeof(RowingState))
        {
            currentState.HandleRowingInput(this);
        }*/
    }

    private void HandleJumpInput()
    {
         if (IsGrounded()) Jump();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position/* - halfModelHeight/2 * Vector3.up*/, groundCheck);
    }
}
