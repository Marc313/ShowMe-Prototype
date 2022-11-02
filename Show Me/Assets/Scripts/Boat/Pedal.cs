using UnityEngine;

public class Pedal : Pickup, IUsable
{
    private AnimatedObject anim;

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<AnimatedObject>();
    }

    public void OnUse(Player _interacter)
    {
        anim.PlayAnimationCrossFade();
    }
}
