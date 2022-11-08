using UnityEngine;
using UnityEngine.VFX;

public class Siren : MonoBehaviour
{
    [SerializeField] private float singingRange = 10f;
    [SerializeField] private float attractionForce = 100f;
    [SerializeField] private float rotationSpeed = 10f;
    [Space]
    [SerializeField] private float vfxActiveRange = 30f;

    private UIManager UImanager;
    private Boat boat;
    private float distance;

    private SirenEffects effects;
    private bool isBoatInRange;
    private bool isInVFXRange;

    private VisualEffect VFX;

    private void Awake()
    {
        UImanager = FindObjectOfType<UIManager>();
        effects = FindObjectOfType<SirenEffects>();
        VFX = GetComponentInChildren<VisualEffect>();
    }

    private void Start()
    {
        VFX.SetFloat("sirenRange", singingRange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OverlapSphere();
    }

    public void RotateBoatToSiren()
    {
        transform.LookAt(boat.transform.position);
    }

    public void ApplyEffects()
    {
        effects.ScaleVignette(distance, singingRange);
        effects.SetBlurFromDistance(distance, singingRange);
    }

    private void OverlapSphere()
    {
        if (OverlapBoat())
        {
            if (isBoatInRange == false)
            {
                isBoatInRange = true;
                effects.EnableBlur(true);
                distance = Vector3.Distance(boat.transform.position, transform.position);
                effects.sirenDistances.Add(this, distance);
            }
            else if (isBoatInRange)
            {
                boat.AddForceFromPosition(transform.position, attractionForce, rotationSpeed);

                distance = Vector3.Distance(boat.transform.position, transform.position);
                effects.sirenDistances[this] = distance;
            }
        }
        else
        {
            if (isBoatInRange)
            {
                //UImanager.HideVignette();
                OnBoatOutOffRange();
            }
        }
    }

    private void OnBoatOutOffRange()
    {
        isBoatInRange = false;
        boat = null;
        effects.sirenDistances.Remove(this);
    }

    private bool OverlapBoat()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, singingRange);

        foreach (Collider collider in colliders)
        {
            Boat tempBoat = collider.GetComponent<Boat>();
            if (tempBoat != null)
            {
                // Found Boat
                boat = tempBoat;
                return true;
            }
        }

        return false;
    }


    private bool OverlapBoatVFXRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, singingRange);

        foreach (Collider collider in colliders)
        {
            Boat tempBoat = collider.GetComponent<Boat>();
            if (tempBoat != null)
            {
                // Found Boat
                boat = tempBoat;
                return true;
            }
        }

        return false;
    }
}
