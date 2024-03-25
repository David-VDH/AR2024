using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

// Thanks to Jeremy for the help
public class Face : MonoBehaviour
{
    [SerializeField] private TMP_Text cookieScoreText;
    [SerializeField] private GameObject cookiePrefab;
    [SerializeField] private ARCameraManager arCameraManager;
    [SerializeField] private ARFaceManager arFaceManager;

    private List<ARFace> arFaces = new List<ARFace>();

    private void OnEnable() => arFaceManager.facesChanged += OnFaceChanged;
    private void OnDisable() => arFaceManager.facesChanged -= OnFaceChanged;

    private int mouthUpperIndex = 13;
    private int mouthLowerIndex = 16;

    private bool isMouthOpen = false;

    private float mouthOpenThreshold = 0.02f;

    private int cookieCount = 0;

    private Dictionary<ARFace, GameObject> faceToCookieMap = new Dictionary<ARFace, GameObject>();

    private void Start()
    {
        cookieScoreText.text = "Open mouth to eat cookies ...";
    }

    private void Update()
    {
        if (arCameraManager.currentFacingDirection == CameraFacingDirection.User)
        {
            foreach (var face in arFaces)
            {
                float mouthDistance = Vector3.Distance(
                    face.vertices[mouthUpperIndex],
                    face.vertices[mouthLowerIndex]
                );

                GameObject cookieInstance;
                if (faceToCookieMap.TryGetValue(face, out cookieInstance))
                {
                    // Assumes the cookie model is the first child of the instantiated prefab
                    MeshRenderer cookieRenderer = cookieInstance.transform.GetChild(0).GetComponent<MeshRenderer>();

                    if (mouthDistance > mouthOpenThreshold && !isMouthOpen)
                    {
                        isMouthOpen = true;
                        cookieRenderer.enabled = true;
                    }
                    else if (mouthDistance <= mouthOpenThreshold && isMouthOpen)
                    {
                        isMouthOpen = false;
                        cookieRenderer.enabled = false;
                        cookieCount++;
                        if (cookieCount > 0 && cookieCount < 10)
                        {
                            cookieScoreText.text = "You ate " + cookieCount.ToString() + " cookies!";
                        }
                        else if (cookieCount >= 10)
                        {
                            cookieScoreText.text = "Cheat day?";
                        }
                    }
                }
            }
        }
    }

    internal void OnFaceChanged(ARFacesChangedEventArgs eventArgs)
    {
        foreach (var newFace in eventArgs.added)
        {
            arFaces.Add(newFace);
            AttachCookieToMouth(newFace);
        }

        foreach (var removedFace in eventArgs.removed)
        {
            arFaces.Remove(removedFace);
            if (faceToCookieMap.TryGetValue(removedFace, out var cookieInstance))
            {
                Destroy(cookieInstance);
                faceToCookieMap.Remove(removedFace);
            }
        }
    }

    private void AttachCookieToMouth(ARFace face)
    {
        // Calculate the midpoint between the upper and lower mouth vertices
        Vector3 mouthPos = face.transform.TransformPoint(face.vertices[mouthLowerIndex]);

        GameObject cookieInstance = Instantiate(cookiePrefab, mouthPos, Quaternion.identity);
        cookieInstance.transform.parent = face.transform;

        // Initially hide the cookie by accessing the MeshRenderer of the first child
        cookieInstance.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;

        // Map the face to the cookie instance for tracking
        faceToCookieMap[face] = cookieInstance;
    }
}