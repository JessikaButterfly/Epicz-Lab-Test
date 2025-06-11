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

    // Start is called before the first frame update
    void Start()
    {
        gridIndex = new int[size, size];
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
                    if (x > 2 && gridIndex[x - 1, y] == i && gridIndex[x - 2, y] == i && gridIndex[x - 3, y] == i)
                        canUse = false;

                    // three in collumn
                    if (y > 2 && gridIndex[x, y - 1] == i && gridIndex[x, y - 2] == i && gridIndex[x, y - 3] == i)
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

                //spawn object
                Vector3 spawnPos = new Vector3(x * spacing, y * spacing, 0f);
                Instantiate(box, spawnPos, Quaternion.identity, transform);
                Instantiate(boxOptions[chosenIndex], spawnPos, Quaternion.identity, transform);
            }
        }

    }
}
