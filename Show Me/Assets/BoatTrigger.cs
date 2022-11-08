using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTrigger : MonoBehaviour
{
    public Transform boatPos;

    private void OnTriggerEnter(Collider other)
    {
        Boat boat = other.GetComponent<Boat>();

        if (boat != null)
        {
            boat.rigidBody.isKinematic = true;
            boat.transform.position = boatPos.position;
            boat.transform.forward = Vector3.forward;
        }
    }

}
