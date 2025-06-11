using UnityEngine;

public class GridItem : MonoBehaviour
{
    public int x;
    public int y;
    public Grid gridManager;

    private Vector3 startPos;
    private Vector3 mouseOffset;

    private void OnMouseDown()
    {
        startPos = transform.position;
        mouseOffset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        Vector3 mouseWorld = GetMouseWorldPos() + mouseOffset;
        transform.position = new Vector3(mouseWorld.x, mouseWorld.y, 0f);
    }

    private void OnMouseUp()
    {
        GridItem neighbor = gridManager.GetNeighborIfClose(this);

        if (neighbor != null)
        {
            //swap pos with neighbor
            gridManager.SwapItems(this, neighbor);
        }
        else
        {
            // no neigbor back to startpos
            transform.position = startPos;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Plane plane = new Plane(Vector3.forward, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }
}
