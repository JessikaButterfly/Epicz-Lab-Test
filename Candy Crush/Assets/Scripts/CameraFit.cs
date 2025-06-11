using UnityEngine;

public class CameraFitAllObjects2D : MonoBehaviour
{
    public Camera cam;

    void Start()
    {
        FitCameraToObjects();
    }

    public void FitCameraToObjects()
    {
        // find all rendered objects
        Renderer[] renderers = FindObjectsOfType<Renderer>();
        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }

        Vector3 newPos = bounds.center;
        newPos.z = cam.transform.position.z;
        cam.transform.position = newPos;

        float verticalSize = bounds.size.y / 2f;
        float horizontalSize = bounds.size.x / 2f / cam.aspect;

        cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize);

        cam.orthographicSize *= 1.1f;
    }

    void Update()
    {
        FitCameraToObjects();
    }

}
