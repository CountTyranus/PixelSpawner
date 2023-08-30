using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TapToPlace : MonoBehaviour
{
    [SerializeField] private GameObject PlacementIndicator;

    private ARRaycastManager _raycaster;
    private ARPlaneManager _planeManager;
    private List<ARRaycastHit> _hits = new();

    private Camera _camera;
    private bool _placementPoseIsValid;
    private Pose _placementPose;

    void Start()
    {
        _camera = Camera.main;
        _raycaster = GetComponent<ARRaycastManager>();
        _planeManager = GetComponent<ARPlaneManager>();
    }
    
    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
    }

    public void PlaceObject(GameObject objectToPlace)
    {
        if (!_placementPoseIsValid)
            return;

        GetWallPlacement(_hits[0], out var orientation, out var zUp);
        
        GameObject spawnedObj = Instantiate(objectToPlace, PlacementIndicator.transform.position, orientation);
        spawnedObj.transform.rotation = zUp;
        spawnedObj.AddComponent<ARAnchor>();
    }

    private void GetWallPlacement(ARRaycastHit hit, out Quaternion orientation, out Quaternion zUp)
    {
        TrackableId planeHit = hit.trackableId;
        ARPlane plane = _planeManager.GetPlane(planeHit);
        Vector3 planeNormal = plane.normal;
        orientation = Quaternion.FromToRotation(Vector3.up, planeNormal);
        Vector3 forward = hit.pose.position - (hit.pose.position + Vector3.down);
        zUp = quaternion.LookRotation(forward, planeNormal);
    }

    private void UpdatePlacementIndicator()
    {
        if (_placementPoseIsValid)
        {
            PlacementIndicator.SetActive(true);
            PlacementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
            return;
        }
        
        PlacementIndicator.SetActive(false);
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = _camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));

        _placementPoseIsValid = _raycaster.Raycast(screenCenter, _hits, TrackableType.Planes);

        if (_placementPoseIsValid)
        {
            _placementPose = _hits[0].pose;
        }
    }
}
