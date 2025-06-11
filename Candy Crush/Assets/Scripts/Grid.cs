using UnityEngine;

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

        // change index
        int tempIndex = a.boxIndex;
        a.boxIndex = b.boxIndex;
        b.boxIndex = tempIndex;

        // change kordinates
        a.x = bx;
        a.y = by;
        b.x = ax;
        b.y = ay;

        // change pos
        a.transform.position = new Vector3(a.x * spacing, a.y * spacing, 0);
        b.transform.position = new Vector3(b.x * spacing, b.y * spacing, 0);

        // change boxIndex
        gridItems[a.x, a.y] = a;
        gridItems[b.x, b.y] = b;

        Debug.Log($"Swapped items: a({a.x},{a.y}) boxIndex={a.boxIndex}, b({b.x},{b.y}) boxIndex={b.boxIndex}");

        if (CheckThreeInARow())
            Debug.Log("Three in a row found!");
        else
            Debug.Log("No three in a row.");
    }



    public bool CheckThreeInARow()
{
    for (int y = 0; y < size; y++)
    {
        for (int x = 0; x < size - 2; x++)
        {
            int idx = gridItems[x, y].boxIndex;
            if (idx == -1) continue;

            if (gridItems[x + 1, y].boxIndex == idx &&
                gridItems[x + 2, y].boxIndex == idx)
            {
                Debug.Log($"3 in a row at ({x},{y}), ({x+1},{y}), ({x+2},{y}) with index {idx}");
                return true;
            }
        }
    }
    return false;
}


}
