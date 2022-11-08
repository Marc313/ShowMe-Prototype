using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Siren : MonoBehaviour
{
    [SerializeField] private float singingRange = 10f;
    [SerializeField] private float deadRange = 3f;
    [SerializeField] private float attractionForce = 100f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Vignette")]
    [SerializeField] private Vector3 maxScale = new Vector3(2.5f, 3.5f, 3.5f);
    [SerializeField] private Vector3 minScale = new Vector3(3.2f, 4.5f, 4.5f);
    [SerializeField] private float minSirenDistance = 3f;
    private float minBlur = 2.5f;
    private float maxBlur = 1.5f;

    private UIManager UImanager;
    private Boat boat;
    private float distance;

    public Volume volume;
    private DepthOfField doF;

    private void Awake()
    {
        UImanager = FindObjectOfType<UIManager>();

        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet<DepthOfField>(out doF);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        OverlapSphere();
    }

    private void OverlapSphere()
    {
        //Debug.Log($"Boat: {boat}");

        if (OverlapBoat())
        {
            transform.LookAt(boat.transform.position);
            boat.AddForceFromPosition(transform.position, attractionForce, rotationSpeed);

            UImanager.ShowVignette();
            doF.active = true;

            distance = Vector3.Distance(boat.transform.position, transform.position);
            Vector3 scale = CalculateVignetteScale();
            UImanager.ScaleVignette(scale);

            doF.focusDistance.value = CalculateBlur();
        }
        else
        {
            //UImanager.HideVignette();
            boat = null;
        }
    }

    private float CalculateBlur()
    {
        float blur = 0;

        float distanceRatio = (distance - minSirenDistance) / (singingRange - minSirenDistance);
        blur = distanceRatio * (minBlur - maxBlur) + maxBlur;

        blur = Mathf.Clamp(blur, maxBlur, minBlur);

        return blur;
    }

    private Vector3 CalculateVignetteScale()
    {
        Vector3 scale = Vector3.zero;

        float distanceRatio = (distance - minSirenDistance) / (singingRange - minSirenDistance);
        scale = distanceRatio * (minScale - maxScale) + maxScale;

        scale.x = Mathf.Clamp(scale.x, maxScale.x, minScale.x);
        scale.y = Mathf.Clamp(scale.y, maxScale.y, minScale.y);

        return scale;
    }

    private bool OverlapBoat()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, singingRange);

        foreach (Collider collider in colliders)
        {
            Boat tempBoat = collider.GetComponent<Boat>();
            if (tempBoat != null)
            {
                Debug.Log("Collider: " + collider);
                boat = tempBoat;

                return true;
            }
        }

        return false;
    }
}
