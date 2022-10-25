using UnityEngine;

public abstract class APickupable : MonoBehaviour, IPickupable
{
    public float modelHeight { get; protected set; }

    public virtual void OnInteract(Player _interacter)
    {
        OnPickup(_interacter);
    }

    public virtual void OnPickup(Player _carrier) { }
    public virtual void OnRelease() { }

    private void Start()
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            modelHeight = renderer.bounds.extents.y;
        }
    }
}
