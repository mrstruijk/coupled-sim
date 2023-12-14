using UnityEngine;
using UnityEngine.XR;


public class CameraCounter : MonoBehaviour
{
    private Transform
        childCamera;


    // Use this for initialization
    private void Start()
    {
        childCamera = transform.GetChild(0);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            InputTracking.Recenter();
        }
    }


    // Update is called once per frame
    private void LateUpdate()
    {
        var invertedPosition = -childCamera.localPosition;
        transform.localPosition = invertedPosition;
    }
}