using UnityEngine;

public class Pickup : APickupable
{
    private Transform carrier;
    private bool isPickedUp;

    public override void OnPickup(Player _carrier)
    {
        isPickedUp = true;
        colliderr.enabled = false;   
        carrier = _carrier.transform;
    }

    public override void OnRelease()
    {
        isPickedUp = false;
        colliderr.enabled = true;
        carrier = null;
    }

    private void Update()
    {
        if (isPickedUp && carrier != null)
        {
            transform.position = carrier.position + carrier.up * (halfModelHeight + ((IPickupable) carrier.GetComponent<Player>()).halfModelHeight);
        }
    }
}
