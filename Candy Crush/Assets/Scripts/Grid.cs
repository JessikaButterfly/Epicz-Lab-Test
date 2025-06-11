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
                //spawn grid
                int index = Random.Range(0, boxOptions.Length);
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
        // change index
        gridItems[a.x, a.y] = b;
        gridItems[b.x, b.y] = a;

        // change kordinates
        int tempX = a.x;
        int tempY = a.y;
        a.x = b.x;
        a.y = b.y;
        b.x = tempX;
        b.y = tempY;

        // change pos
        a.transform.position = new Vector3(a.x * spacing, a.y * spacing, 0);
        b.transform.position = new Vector3(b.x * spacing, b.y * spacing, 0);

        // change boxIndex
        int tempIndex = a.boxIndex;
        a.boxIndex = b.boxIndex;
        b.boxIndex = tempIndex;
    }
}
