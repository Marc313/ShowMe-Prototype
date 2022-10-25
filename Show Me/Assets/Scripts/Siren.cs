using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Siren : MonoBehaviour
{
    [SerializeField] private float singingRange = 10f;
    [SerializeField] private float attractionForce = 100f;
    [SerializeField] private float rotationSpeed = 10f;

    // Update is called once per frame
    void FixedUpdate()
    {
        OverlapSphere();
    }

    private void OverlapSphere()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, singingRange);

        foreach (Collider collider in colliders)
        {
            Boat boat = collider.GetComponent<Boat>();
            if (boat != null)
            {
                boat.AddForceFromPosition(transform.position, attractionForce, rotationSpeed);
                Debug.Log("SIREN");
            }
        }
    }
}
