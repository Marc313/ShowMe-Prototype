using UnityEngine;

public interface IPickupable : IInteractable
{
    public float modelHeight { get; }

    public void OnPickup(Player _carrier);
    public void OnRelease();
}
