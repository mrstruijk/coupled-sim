using UnityEngine;
using UnityEngine.XR;

// Script obtained from MVN to reset the camera of the Oculus onto the head of the MVN Avatar.
// Used in combination with AnchorController.cs
public class CameraController : MonoBehaviour
{
    private Transform
        childCamera;


    private void Start()
    {
        childCamera = transform.parent;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            InputTracking.Recenter();
        }
    }


    private void LateUpdate()
    {
        var invertedPosition = -childCamera.localPosition;
        var invertedRotation = Quaternion.Inverse(childCamera.localRotation);
        transform.localPosition = invertedPosition;
        transform.localRotation = invertedRotation;
    }
}