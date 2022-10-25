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
    private float targetZPos;
    private float targetZoom;
    private float cameraProjectionOffsetZ;

    private bool fixedZoom;

    private void Awake()
    {
        cam = GetComponent<Camera>();
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

    public void SetZoom(float _zoom)
    {
        fixedZoom = true;
        cam.orthographicSize = _zoom;
    }

    public void EnableUnfixedZoom()
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
        targetZPos = CalculateCameraZPos();
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, targetZPos);
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
