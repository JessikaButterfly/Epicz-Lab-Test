using UnityEngine;

public class GridItem : MonoBehaviour
{
    public int x, y;
    public int boxIndex;
    public Grid gridManager;

    private static GridItem selected = null;

    private void OnMouseDown()
    {
        if (selected == null)
        {
            selected = this;
            Highlight(true);
        }
        else
        {
            if (IsNeighbor(selected))
            {
                // If the new selection is a neighbor, swap items
                gridManager.SwapItems(selected, this);
                selected.Highlight(false);
                selected = null; // Clear selection after swap
            }
            else
            {
                // select new if not neighbor
                selected.Highlight(false);
                selected = this;
                Highlight(true);
            }
        }
    }

    // Check if is neihgbor
    bool IsNeighbor(GridItem other)
    {
        int dx = Mathf.Abs(x - other.x);
        int dy = Mathf.Abs(y - other.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    // Highlight or unhighlight this item by changing its color
    void Highlight(bool on)
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = on ? Color.yellow : Color.white;
    }
}
