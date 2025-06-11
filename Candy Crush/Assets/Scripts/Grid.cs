using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject box;
    public GameObject[] boxOptions;
    public int size;
    public float spacing = 1.1f;

    private int[,] gridIndex;
    private GridItem[,] gridItems;

    // Start is called before the first frame update
    void Start()
    {
        gridIndex = new int[size, size];
        gridItems = new GridItem[size, size];
        createGrid();
    }

    void createGrid()
    {
        //Instantiate for each coluumn
        for (int x = 0; x < size; x++)
        {
            //Instantiate fr each row
            for (int y = 0; y < size; y++)
            {
                //list with allowed index
                List<int> allowedIndex = new List<int>();

                //check not four in row
                for (int i = 0; i < boxOptions.Length; i++)
                {
                    bool canUse = true;

                    // three in row
                    if (x > 1 && gridIndex[x - 1, y] == i && gridIndex[x - 2, y] == i)
                        canUse = false;

                    // three in collumn
                    if (y > 1 && gridIndex[x, y - 1] == i && gridIndex[x, y - 2] == i)
                        canUse = false;

                    if (canUse)
                        allowedIndex.Add(i);

                }

                //no allowed index
                if (allowedIndex.Count == 0)
                    allowedIndex.AddRange(System.Linq.Enumerable.Range(0, boxOptions.Length));
                
                //random index and save
                int chosenIndex = allowedIndex[Random.Range(0, allowedIndex.Count)];
                gridIndex[x, y] = chosenIndex;

                //spawn objects
                Vector3 spawnPos = new Vector3(x * spacing, y * spacing, 0f);
                Instantiate(box, spawnPos, Quaternion.identity, transform);
                GameObject go = Instantiate(boxOptions[chosenIndex], spawnPos, Quaternion.identity, transform);

                //add to griditem
                GridItem item = go.AddComponent<GridItem>();
                item.x = x;
                item.y = y;
                item.gridManager = this;
                gridItems[x, y] = item;
            }
        }

    }

    public GridItem GetNeighborIfClose(GridItem from)
    {
        float maxDistance = spacing * 0.75f;

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0), //right
            new Vector2Int(-1, 0), //left
            new Vector2Int(0, 1), //up
            new Vector2Int(0, -1) //down
        };

        //check neighbor
        foreach (Vector2Int dir in directions)
        {
            int nx = from.x + dir.x;
            int ny = from.y + dir.y;

            if (nx >= 0 && nx < size && ny >= 0 && ny < size)
            {
                GridItem neighbor = gridItems[nx, ny];
                //calculate dist between neighbors
                float dist = Vector3.Distance(from.transform.position, neighbor.transform.position);

                //not neighbors return
                if (dist <= maxDistance)
                    return neighbor;
            }
        }

        return null;
    }

    public void SwapItems(GridItem a, GridItem b)
    {
        // switch
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
    }

}
