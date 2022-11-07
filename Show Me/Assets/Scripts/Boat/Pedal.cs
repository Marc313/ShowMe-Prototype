using UnityEngine;

public class Pedal : Pickup, IUsable
{
    public override bool locked => true;
    public Boat.BoatDirection pedalSide;
    //public Vector3 offset;
    public float cooldown => 0.5f;
    public Transform playerPosition;

    private float timer = 0;
    private AnimatedObject anim;
    private Vector3 lockedPos;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<AnimatedObject>();
        if (anim == null) anim = GetComponent<AnimatedObject>();
    }

    private void Start()
    {
        //offset.x *= (int)pedalSide;
        timer = cooldown;
        lockedPos = transform.localPosition;
    }

    protected override void Update()
    {
        base.Update();
        if (timer > 0) timer -= Time.deltaTime;
    }

    public override void OnRelease()
    {
        base.OnRelease();
        transform.localPosition = lockedPos;
    }

    public void OnUse(Player _interacter)
    {
        if (_interacter.boat != null && _interacter.boat.IsFull() && timer <= 0)
        {
            anim.PlayAnimationCrossFade();
            Boat.BoatDirection opposite = (Boat.BoatDirection)(-(int)pedalSide);
            _interacter.boat.AddRowForce(opposite);
            //_interacter.boat.AddRowForceFromPosition(opposite, _interacter.transform.position);
            timer = cooldown;
        }
    }

    protected override void FollowCarrier()
    {
        if (isPickedUp && carrier != null)
        {
            //float angle = Vector3.Dot(Vector3.forward, transform.forward);
            float angle = Vector3.Angle(Vector3.forward, transform.forward);
            //Vector3 rotatedOffset = Quaternion.AngleAxis(angle, Vector3.up) * offset;
            carrier.transform.position = playerPosition.position;
            carrier.transform.rotation = playerPosition.rotation;
        }
    }
}
