using UnityEngine;

public interface IPickupable: IInteractable
{
    public bool locked { get; }
    public float halfModelHeight { get; }

    public void OnPickup(Player _carrier);
    public void OnRelease();
}
