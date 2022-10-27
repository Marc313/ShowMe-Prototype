using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera cam;

    [SerializeField] private float minCameraSize = 6f;
    [SerializeField] private float maxCameraSize = 11f;
    [SerializeField] private float minZoomDistance = 5f;
    [SerializeField] private float maxZoomDistance = 20f;
    [SerializeField] private float playerCarryZoom = 9f;

    [SerializeField] private Transform[] targets;
    private Vector3 targetXZPos;
    private float targetZoom;
    private float cameraProjectionOffsetZ;

    private bool fixedZoom;
    private bool xFollow;

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
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100f, LayerMask.GetMask("Ground")))
        {
            cameraProjectionOffsetZ = Mathf.Abs(cam.transform.position.z - hit.point.z);
        }
    }

    public void SetZoom(EventName _event, object _zoomValue)
    {
        fixedZoom = true;
        cam.orthographicSize = (float) _zoomValue;
    }

    private void EnableUnfixedZoom(EventName _event, object _value)
    {
        fixedZoom = false;
    }

    private void LateUpdate()
    {
        HandleCameraMovement();
        if(!fixedZoom) HandleCameraZoom();
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
        CalculateCameraSize();
        cam.orthographicSize = targetZoom;
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
        Vector3 totalXZ = Vector3.zero;
        foreach (Transform target in targets)
        {
            totalXZ += target.transform.position.x * Vector3.right;
            totalXZ += target.transform.position.z * Vector3.forward;
        }
        Vector3 averageXZ = totalXZ / targets.Length;
        Vector3 cameraXZPos = new Vector3(averageXZ.x, 0, averageXZ.z - cameraProjectionOffsetZ);
        return cameraXZPos;
    }

    private float CalculateCameraSize()
    {
        float distance = Vector3.Distance(targets[0].position, targets[1].position);

        if (distance <= minZoomDistance)
        {
            targetZoom = minCameraSize;
        }
        else if (distance >= maxZoomDistance)
        {
            targetZoom = maxCameraSize;
        }
        else
        {
            float distanceRatio = (distance - minZoomDistance) / (maxZoomDistance - minZoomDistance);
            targetZoom = distanceRatio * (maxCameraSize - minCameraSize) + minCameraSize;
        }

        return targetZoom;
    }

    //15 = 66.7%
    // (distance - minZoomDistance) / (maxZoomDistance - minZoomDistance) = result
    //What is 66.7% of 6-11??
    // (targetZoom - minCameraSize) / (maxCameraSize - minCameraSize) = ratio
    // ratio * (maxCameraSize - minCameraSize) + minCameraSize = targetZoom

}
