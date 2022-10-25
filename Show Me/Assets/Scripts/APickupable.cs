using UnityEngine;

public abstract class APickupable : MonoBehaviour, IPickupable
{
    public float halfModelHeight => CalculateModelHeight();

    public virtual void OnInteract(Player _interacter)
    {
        OnPickup(_interacter);
    }

    public virtual void OnPickup(Player _carrier) { }
    public virtual void OnRelease() { }

    private float CalculateModelHeight()
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            return renderer.bounds.extents.y;
        }
        else return 0;
    }
}
