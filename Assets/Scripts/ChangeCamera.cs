using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ChangeCamera : MonoBehaviour
{
    [SerializeField] ARCameraManager arCameraManager;
    [SerializeField] GameObject targetTextGO;
    [SerializeField] GameObject cookieScoreTextGO;

    [SerializeField] private SphereSpawner sphereSpawner;

    private ARRaycastManager arRaycastManager;
    private ARPlaneManager arPlaneManager;
    private ARFaceManager arFaceManager;

    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPlaneManager = GetComponent<ARPlaneManager>();
        arFaceManager = GetComponent<ARFaceManager>();
        cookieScoreTextGO.SetActive(false);
    }

    public void OnSwapCameraButtonClicked()
    {
        SwapCamera();
    }

    private void SwapCamera()
    {
        if (arCameraManager.currentFacingDirection == CameraFacingDirection.World)
        {
            arRaycastManager.enabled = false;
            arPlaneManager.enabled = false;
            DestroyPlanes();
            sphereSpawner.DestroyAllSpheres();
            arFaceManager.enabled = true;

            targetTextGO.SetActive(false);
            cookieScoreTextGO.SetActive(true);


            arCameraManager.requestedFacingDirection = CameraFacingDirection.User;
        }
        else
        {
            arRaycastManager.enabled = true;
            arPlaneManager.enabled = true;
            arFaceManager.enabled = false;
            DestroyFaces();

            cookieScoreTextGO.SetActive(false);
            targetTextGO.SetActive(true);

            arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
        }
    }

    private void DestroyPlanes()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            Destroy(plane.gameObject);
        }
    }

    private void DestroyFaces()
    {
        foreach(var face in arFaceManager.trackables)
        {
            Destroy(face.gameObject);
        }
    }
}
