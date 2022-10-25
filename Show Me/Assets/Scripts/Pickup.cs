using UnityEngine;

public class Pickup : MonoBehaviour, IPickupable
{
    public float modelHeight { get; private set; }

    private Transform carrier;
    private bool isPickedUp;

    private void Start()
    {
        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            modelHeight = renderer.bounds.extents.y;
        }
    }

    public void OnInteract(Player _interacter)
    {
        OnPickup(_interacter);
    }

    public void OnPickup(Player _carrier)
    {
        isPickedUp = true;
        carrier = _carrier.transform;
    }

    public void OnRelease()
    {
        isPickedUp = false;
        carrier = null;
    }

    private void Update()
    {
        if (isPickedUp && carrier != null)
        {
            transform.position = carrier.position;
        }
    }
}
