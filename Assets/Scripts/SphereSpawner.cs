using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SphereSpawner : MonoBehaviour
{
    [SerializeField] GameObject targetSpherePrefab;
    private List<GameObject> spawnedSpheres = new List<GameObject>();

    private ARRaycastManager raycastManager;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private TrackableType trackableType = TrackableType.PlaneWithinPolygon;

    private void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
    }

    public void SingleTap(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed)
        {
            return;
        }

        var touchPos = context.ReadValue<Vector2>();

        if (raycastManager.Raycast(touchPos, hits, trackableType))
        {
            Pose hitPose = hits[0].pose;
            var spawnedSphere = Instantiate(targetSpherePrefab, hitPose.position, Quaternion.identity);
            spawnedSpheres.Add(spawnedSphere);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit hitObject;

            if (Physics.Raycast(ray, out hitObject))
            {
                if (hitObject.transform.CompareTag("TargetSphere"))
                {
                    Destroy(hitObject.transform.gameObject);
                }
            }
        }
    }

    public void DestroyAllSpheres()
    {
        foreach(var sphere in spawnedSpheres)
        {
            if (sphere != null)
            {
                Destroy(sphere);
            }
        }
        spawnedSpheres.Clear();
    }
}
