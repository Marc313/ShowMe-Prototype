using UnityEngine;

public abstract class APickupable : MonoBehaviour, IPickupable
{
    public virtual bool locked => false;
    public float halfModelHeight => CalculateModelHeight();

    protected MeshRenderer meshRenderer;
    protected Collider colliderr;

    protected Transform carrier;
    protected bool isPickedUp;

    protected virtual void Awake()
    {
        InitializeColliderAndRenderer();
    }

    protected virtual void Update()
    {
        FollowCarrier();
    }

    public virtual void OnInteract(Player _interacter)
    {
        OnPickup(_interacter);
    }

    public virtual void OnPickup(Player _carrier)
    {
        isPickedUp = true;
        colliderr.enabled = false;
        carrier = _carrier.transform;
    }

    public virtual void OnRelease()
    {
        isPickedUp = false;
        colliderr.enabled = true;
        carrier = null;
    }

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

    protected virtual void FollowCarrier()
    {
        if (isPickedUp && carrier != null)
        {
            transform.position = carrier.position + carrier.up * (halfModelHeight + ((IPickupable)carrier.GetComponent<Player>()).halfModelHeight);
        }
    }
}
