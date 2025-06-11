using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    public GameObject box;
    public GameObject[] boxOptions;
    public int size = 8;
    public float spacing = 1.1f;

    public GridItem[,] gridItems;

    void Start()
    {
        gridItems = new GridItem[size, size];
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                int index;

                do
                {
                    index = Random.Range(0, boxOptions.Length);
                    // no three in row
                } while (x >= 2 &&
                    gridItems[x - 1, y] != null &&
                    gridItems[x - 2, y] != null &&
                    gridItems[x - 1, y].boxIndex == index &&
                    gridItems[x - 2, y].boxIndex == index);

                Vector3 pos = new Vector3(x * spacing, y * spacing, 0);
                Instantiate(box, pos, Quaternion.identity, transform);
                GameObject go = Instantiate(boxOptions[index], pos, Quaternion.identity, transform);

                GridItem item = go.AddComponent<GridItem>();
                item.x = x;
                item.y = y;
                item.boxIndex = index;
                item.gridManager = this;

                gridItems[x, y] = item;
            }
        }
    }


    public void SwapItems(GridItem a, GridItem b)
    {
        // old kordinates
        int ax = a.x;
        int ay = a.y;
        int bx = b.x;
        int by = b.y;

        // change index in array
        gridItems[ax, ay] = b;
        gridItems[bx, by] = a;

        // change kordinates
        a.x = bx;
        a.y = by;
        b.x = ax;
        b.y = ay;

        // change pos
        a.transform.position = new Vector3(a.x * spacing, a.y * spacing, 0);
        b.transform.position = new Vector3(b.x * spacing, b.y * spacing, 0);

        Debug.Log($"Swapped items: a({a.x},{a.y}) boxIndex={a.boxIndex}, b({b.x},{b.y}) boxIndex={b.boxIndex}");

        if (CheckThreeInARow())
        {
            List<Vector2Int> toRemove = GetPositionsToRemove();
            RemoveItems(toRemove);
            DropDown();
            OnSwapComplete();
            Debug.Log("Removed items around three in a row!");
        }

        else
        {
            Debug.Log("No three in a row, swapping back.");

            // back to old pos
            gridItems[ax, ay] = a;
            gridItems[bx, by] = b;

            a.x = ax;
            a.y = ay;
            b.x = bx;
            b.y = by;

            a.transform.position = new Vector3(a.x * spacing, a.y * spacing, 0);
            b.transform.position = new Vector3(b.x * spacing, b.y * spacing, 0);
        }
    }



    public bool CheckThreeInARow()
    {
        bool found = false;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int idx = gridItems[x, y].boxIndex;
                Debug.Log($"Checking position ({x},{y}) with boxIndex {idx}");

                if (idx == -1) continue;

                // left start
                if (x + 2 < size &&
                    gridItems[x + 1, y].boxIndex == idx &&
                    gridItems[x + 2, y].boxIndex == idx)
                {
                    Debug.Log($"3 in a row at ({x},{y}), ({x + 1},{y}), ({x + 2},{y}) with index {idx}");
                    found = true;
                }

                // middle start
                if (x - 1 >= 0 && x + 1 < size &&
                    gridItems[x - 1, y].boxIndex == idx &&
                    gridItems[x + 1, y].boxIndex == idx)
                {
                    Debug.Log($"3 in a row at ({x - 1},{y}), ({x},{y}), ({x + 1},{y}) with index {idx}");
                    found = true;
                }

                // right start
                if (x - 2 >= 0 &&
                    gridItems[x - 1, y].boxIndex == idx &&
                    gridItems[x - 2, y].boxIndex == idx)
                {
                    Debug.Log($"3 in a row at ({x - 2},{y}), ({x - 1},{y}), ({x},{y}) with index {idx}");
                    found = true;
                }
            }
        }

        return found;
    }

    public List<Vector2Int> GetPositionsToRemove()
    {
        List<Vector2Int> positionsToRemove = new List<Vector2Int>();

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size - 2; x++)
            {
                int idx = gridItems[x, y].boxIndex;
                if (idx == -1) continue;

                // check if three in row has the same
                if (gridItems[x + 1, y].boxIndex == idx &&
                    gridItems[x + 2, y].boxIndex == idx)
                {
                    // add those
                    positionsToRemove.Add(new Vector2Int(x, y));
                    positionsToRemove.Add(new Vector2Int(x + 1, y));
                    positionsToRemove.Add(new Vector2Int(x + 2, y));

                    // check if left has the same symbol
                    if (x - 1 >= 0 && gridItems[x - 1, y].boxIndex == idx)
                        positionsToRemove.Add(new Vector2Int(x - 1, y));

                    // check if right has the same symbol
                    if (x + 3 < size && gridItems[x + 3, y].boxIndex == idx)
                        positionsToRemove.Add(new Vector2Int(x + 3, y));

                    // check above
                    if (y + 1 < size)
                    {
                        if (gridItems[x, y + 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(x, y + 1));
                        if (gridItems[x + 1, y + 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(x + 1, y + 1));
                        if (gridItems[x + 2, y + 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(x + 2, y + 1));
                    }

                    // checkt bellow
                    if (y - 1 >= 0)
                    {
                        if (gridItems[x, y - 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(x, y - 1));
                        if (gridItems[x + 1, y - 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(x + 1, y - 1));
                        if (gridItems[x + 2, y - 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(x + 2, y - 1));
                    }
                }
            }
        }

        // remove dublicates
        HashSet<Vector2Int> uniquePositions = new HashSet<Vector2Int>(positionsToRemove);
        return new List<Vector2Int>(uniquePositions);
    }

    public void RemoveItems(List<Vector2Int> positions)
    {
        foreach (var pos in positions)
        {
            GridItem item = gridItems[pos.x, pos.y];
            if (item != null)
            {
                Destroy(item.gameObject);
                gridItems[pos.x, pos.y] = null;
            }
        }
    }

    public void DropDown()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 1; y < size; y++)
            {
                if (gridItems[x, y] != null && gridItems[x, y - 1] == null)
                {
                    // Move down one step
                    GridItem item = gridItems[x, y];

                    // Change array
                    gridItems[x, y - 1] = item;
                    gridItems[x, y] = null;

                    // update kordinates
                    item.y = y - 1;

                    // update pos
                    item.transform.position = new Vector3(x * spacing, (y - 1) * spacing, 0);

                    y = 0;
                }
            }
        }
    }

    public void OnSwapComplete()
    {
        bool foundMatch = CheckThreeInARow();

        while (foundMatch)
        {
            List<Vector2Int> toRemove = GetPositionsToRemove();
            RemoveItems(toRemove);
            DropDown();
            foundMatch = CheckThreeInARow();
        }
    }


}
