using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoBehaviour
{
    public GameObject box;
    public GameObject[] boxOptions;

    [SerializeField, Range(10, 50)]
    private int _x = 20;
    public int x
    {
        get => _x;
        set => _x = Mathf.Clamp(value, 10, 50);
    }

    [SerializeField, Range(10, 50)]
    private int _y = 20;
    public int y
    {
        get => _y;
        set => _y = Mathf.Clamp(value, 10, 50);
    }

    public float spacing = 1.1f;

    public GridItem[,] gridItems;

    void Start()
    {
        gridItems = new GridItem[x, y];
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int index;

                do
                {
                    index = Random.Range(0, boxOptions.Length);
                    // no three in row
                } while (i >= 2 &&
                    gridItems[i - 1, j] != null &&
                    gridItems[i - 2, j] != null &&
                    gridItems[i - 1, j].boxIndex == index &&
                    gridItems[i - 2, j].boxIndex == index);

                Vector3 pos = new Vector3(i * spacing, j * spacing, 0);
                Instantiate(box, pos, Quaternion.identity, transform);
                GameObject go = Instantiate(boxOptions[index], pos, Quaternion.identity, transform);

                GridItem item = go.AddComponent<GridItem>();
                item.x = i;
                item.y = j;
                item.boxIndex = index;
                item.gridManager = this;

                gridItems[i, j] = item;
            }
        }
    }

    public GridItem GetItemAt(int x, int y)
    {
        if (x >= 0 && x < this.x && y >= 0 && y < this.y)
            return gridItems[x, y];
        return null;
    }

    public GridItem[] GetNeighbors(int x, int y)
    {
        return new GridItem[]
        {
        GetItemAt(x + 1, y),
        GetItemAt(x - 1, y),
        GetItemAt(x, y + 1),
        GetItemAt(x, y - 1)
        };
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
            StartCoroutine(OnSwapComplete());
            Debug.Log("Removed items around three in a row!");
        }

        else
        {
            Debug.Log("No three in a row, swapping back.");

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

        for (int j = 0; j < y; j++)
        {
            for (int i = 0; i < x; i++)
            {
                if (gridItems[i, j] == null) continue;

                int idx = gridItems[i, j].boxIndex;
                Debug.Log($"Checking position ({i},{j}) with boxIndex {idx}");

                if (idx == -1) continue;

                // left start
                if (i + 2 < x &&
                    gridItems[i + 1, j] != null &&
                    gridItems[i + 2, j] != null &&
                    gridItems[i + 1, j].boxIndex == idx &&
                    gridItems[i + 2, j].boxIndex == idx)
                {
                    Debug.Log($"3 in a row at ({i},{j}), ({i + 1},{j}), ({i + 2},{j}) with index {idx}");
                    found = true;
                }

                // middle start
                if (i - 1 >= 0 && i + 1 < x &&
                    gridItems[i - 1, j] != null &&
                    gridItems[i + 1, j] != null &&
                    gridItems[i - 1, j].boxIndex == idx &&
                    gridItems[i + 1, j].boxIndex == idx)
                {
                    Debug.Log($"3 in a row at ({i - 1},{j}), ({i},{j}), ({i + 1},{j}) with index {idx}");
                    found = true;
                }

                // right start
                if (i - 2 >= 0 &&
                    gridItems[i - 1, j] != null &&
                    gridItems[i - 2, j] != null &&
                    gridItems[i - 1, j].boxIndex == idx &&
                    gridItems[i - 2, j].boxIndex == idx)
                {
                    Debug.Log($"3 in a row at ({i - 2},{j}), ({i - 1},{j}), ({i},{j}) with index {idx}");
                    found = true;
                }
            }
        }

        return found;
    }

    public List<Vector2Int> GetPositionsToRemove()
    {
        List<Vector2Int> positionsToRemove = new List<Vector2Int>();

        for (int j = 0; j < y; j++)
        {
            for (int i = 0; i < x - 2; i++)
            {
                if (gridItems[i, j] == null ||
                    gridItems[i + 1, j] == null ||
                    gridItems[i + 2, j] == null)
                    continue;

                int idx = gridItems[i, j].boxIndex;
                if (idx == -1) continue;

                // check if three in row has the same
                if (gridItems[i + 1, j].boxIndex == idx &&
                    gridItems[i + 2, j].boxIndex == idx)
                {
                    // add those
                    positionsToRemove.Add(new Vector2Int(i, j));
                    positionsToRemove.Add(new Vector2Int(i + 1, j));
                    positionsToRemove.Add(new Vector2Int(i + 2, j));

                    // check if left has the same symbol
                    if (i - 1 >= 0 && gridItems[i - 1, j] != null &&
                        gridItems[i - 1, j].boxIndex == idx)
                        positionsToRemove.Add(new Vector2Int(i - 1, j));

                    // check if right has the same symbol
                    if (i + 3 < x && gridItems[i + 3, j] != null &&
                        gridItems[i + 3, j].boxIndex == idx)
                        positionsToRemove.Add(new Vector2Int(i + 3, j));

                    // check above
                    if (j + 1 < y)
                    {
                        if (gridItems[i, j + 1] != null && gridItems[i, j + 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(i, j + 1));
                        if (gridItems[i + 1, j + 1] != null && gridItems[i + 1, j + 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(i + 1, j + 1));
                        if (gridItems[i + 2, j + 1] != null && gridItems[i + 2, j + 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(i + 2, j + 1));
                    }

                    // checkt bellow
                    if (j - 1 >= 0)
                    {
                        if (gridItems[i, j - 1] != null && gridItems[i, j - 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(i, j - 1));
                        if (gridItems[i + 1, j - 1] != null && gridItems[i + 1, j - 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(i + 1, j - 1));
                        if (gridItems[i + 2, j - 1] != null && gridItems[i + 2, j - 1].boxIndex == idx)
                            positionsToRemove.Add(new Vector2Int(i + 2, j - 1));
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
        for (int i = 0; i < x; i++)
        {
            for (int j = 1; j < y; j++)
            {
                if (gridItems[i, j] != null && gridItems[i, j - 1] == null)
                {
                    // Move down one step
                    GridItem item = gridItems[i, j];

                    // Change array
                    gridItems[i, j - 1] = item;
                    gridItems[i, j] = null;

                    // update kordinates
                    item.y = j - 1;

                    // update pos
                    Vector3 targetPos = new Vector3(i * spacing, (j - 1) * spacing, 0);

                    // anim
                    StartCoroutine(MoveOverTime(item.transform, targetPos, 0.5f));

                    j = 0; // restart checking this column from bottom
                }
            }
        }
    }

    private IEnumerator MoveOverTime(Transform item, Vector3 targetPosition, float duration)
    {
        Vector3 start = item.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            item.position = Vector3.Lerp(start, targetPosition, t);
            yield return null;
        }

        item.position = targetPosition;
    }

    public IEnumerator OnSwapComplete()
    {
        yield return null;

        bool foundMatch = CheckThreeInARow();

        while (foundMatch)
        {
            List<Vector2Int> toRemove = GetPositionsToRemove();

            yield return new WaitForSeconds(0.9f);

            RemoveItems(toRemove);

            DropDown();

            foundMatch = CheckThreeInARow();
        }
    }

}
