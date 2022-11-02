using UnityEngine;

public abstract class APickupable : MonoBehaviour, IPickupable
{
    public float halfModelHeight => CalculateModelHeight();
    protected MeshRenderer meshRenderer;
    protected Collider colliderr;

    protected virtual void Awake()
    {
        InitializeColliderAndRenderer();
    }

    public virtual void OnInteract(Player _interacter)
    {
        OnPickup(_interacter);
    }

    public virtual void OnPickup(Player _carrier) { }
    public virtual void OnRelease() { }

    private float CalculateModelHeight()
    {
        if (meshRenderer != null)
        {
            return meshRenderer.bounds.extents.y;
        }
        else return 0;
    }

    private void InitializeColliderAndRenderer()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        colliderr = GetComponent<Collider>();
        if (colliderr == null)
        {
            colliderr = GetComponentInChildren<Collider>();
        }
    }
}
