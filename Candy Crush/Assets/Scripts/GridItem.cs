using System.Collections;
using UnityEngine;

public class GridItem : MonoBehaviour
{
    public int x, y;
    public int boxIndex;
    public Grid gridManager;

    private void OnMouseDown()
    {
        // get neighbors
        GridItem[] neighbors = gridManager.GetNeighbors(x, y);

        foreach (var neighbor in neighbors)
        {
            if (neighbor == null) continue;

            if (TrySwapForMatch(neighbor))
            {
                FlashHighlight();
                break; // switch with first that gives match
            }
        }
    }

    bool TrySwapForMatch(GridItem other)
    {
        int ax = x, ay = y;
        int bx = other.x, by = other.y;

        // change array
        gridManager.gridItems[ax, ay] = other;
        gridManager.gridItems[bx, by] = this;

        // change kordinates
        x = bx; y = by;
        other.x = ax; other.y = ay;

        // change pos
        transform.position = new Vector3(x * gridManager.spacing, y * gridManager.spacing, 0);
        other.transform.position = new Vector3(other.x * gridManager.spacing, other.y * gridManager.spacing, 0);

        // check three in row
        if (gridManager.CheckThreeInARow())
        {
            // remove and start anim
            var toRemove = gridManager.GetPositionsToRemove();
            gridManager.RemoveItems(toRemove);
            gridManager.DropDown();
            gridManager.StartCoroutine(gridManager.OnSwapComplete());
            Debug.Log("Match found and removed!");
            return true;
        }
        else
        {
            // switch back if no match
            gridManager.gridItems[ax, ay] = this;
            gridManager.gridItems[bx, by] = other;

            x = ax; y = ay;
            other.x = bx; other.y = by;

            transform.position = new Vector3(x * gridManager.spacing, y * gridManager.spacing, 0);
            other.transform.position = new Vector3(other.x * gridManager.spacing, other.y * gridManager.spacing, 0);

            Debug.Log("No match, swap reverted.");
            return false;
        }
    }

    void FlashHighlight()
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = Color.yellow;
            Invoke(nameof(ResetColor), 0.2f);
        }
    }

    void ResetColor()
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = Color.white;
    }
}
