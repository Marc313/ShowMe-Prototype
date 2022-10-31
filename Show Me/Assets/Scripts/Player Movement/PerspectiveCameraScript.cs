using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveCameraScript : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private float minCameraDistance = 6f;
    [SerializeField] private float maxCameraDistance = 11f;
    [SerializeField] private float minPlayerDistance = 5f;
    [SerializeField] private float maxPlayerDistance = 20f;
    [SerializeField] private float playerCarryZoom = 9f;

    [SerializeField] private Transform[] targets;
    private Vector3 targetXZPos;
    private Vector3 cameraGroundHit;
    private float targetZoom;
    private float cameraProjectionOffsetZ;

    private bool fixedZoom;
    private bool xFollow = true;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        EventSystem.Subscribe(EventName.BOAT_READY, SetZoom);
        EventSystem.Subscribe(EventName.BOAT_EXIT, EnableUnfixedZoom);
    }

    private void OnDisable()
    {
        EventSystem.Unsubscribe(EventName.BOAT_READY, SetZoom);
        EventSystem.Unsubscribe(EventName.BOAT_EXIT, EnableUnfixedZoom);
    }

    private void Start()
    {
        // Calculate offset with raycast (also possible with pythagoras)
        RecalculateGroundHit();
    }

    public void SetZoom(EventName _event, object _zoomValue)
    {
        fixedZoom = true;
        targetZoom = (float )_zoomValue;
        cam.transform.position = cameraGroundHit + (float) targetZoom * -cam.transform.forward;
    }

    private void EnableUnfixedZoom(EventName _event, object _value)
    {
        fixedZoom = false;
    }

    private void LateUpdate()
    {
        RecalculateGroundHit();
        HandleCameraMovement();
        HandleCameraZoom();
    }

    private void RecalculateGroundHit()
    {
        Vector3 averageXZ = CalculateAverageXZ();
        averageXZ.y = (targets[0].position.y + targets[1].position.y) / 2;
        cameraGroundHit = averageXZ;
        cameraProjectionOffsetZ = Mathf.Abs(cam.transform.position.z - averageXZ.z);
    }

    private void HandleCameraMovement()
    {
        if (xFollow)
        {
            targetXZPos = CalculateCameraXZPos();
            cam.transform.position = new Vector3(targetXZPos.x, transform.position.y, targetXZPos.z);
        }
        else
        {
            float targetZPos = CalculateCameraZPos();
            cam.transform.position = new Vector3(transform.position.x, transform.position.y, targetZPos);
        }
    }

    private void HandleCameraZoom()
    {
        if (!fixedZoom) CalculateCameraSize();
        cam.transform.position = cameraGroundHit + (float)targetZoom * -cam.transform.forward;
    }

    private float CalculateCameraZPos()
    {
        float totalZ = 0;
        foreach (Transform target in targets)
        {
            totalZ += target.transform.position.z;
        }
        float averageZ = totalZ / targets.Length;
        float cameraZPos = averageZ - cameraProjectionOffsetZ;
        return cameraZPos;
    }

    private Vector3 CalculateCameraXZPos()
    {
        Vector3 averageXZ = CalculateAverageXZ();
        Vector3 cameraXZPos = new Vector3(averageXZ.x, 0, averageXZ.z - cameraProjectionOffsetZ);
        return cameraXZPos;
    }

    private Vector3 CalculateAverageXZ()
    {
        Vector3 totalXZ = Vector3.zero;
        foreach (Transform target in targets)
        {
            totalXZ += target.transform.position.x * Vector3.right;
            totalXZ += target.transform.position.z * Vector3.forward;
        }
        Vector3 averageXZ = totalXZ / targets.Length;
        return averageXZ;
    }

    private float CalculateCameraSize()
    {
        float distance = Vector3.Distance(targets[0].position, targets[1].position);

        if (distance <= minPlayerDistance)
        {
            targetZoom = minCameraDistance;
        }
        else if (distance >= maxPlayerDistance)
        {
            targetZoom = maxCameraDistance;
        }
        else
        {
            float distanceRatio = (distance - minPlayerDistance) / (maxPlayerDistance - minPlayerDistance);
            targetZoom = distanceRatio * (maxCameraDistance - minCameraDistance) + minCameraDistance;
        }

        return targetZoom;
    }
}
