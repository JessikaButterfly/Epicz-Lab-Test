using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject box;
    public int size;
    public float spacing = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
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
                Vector3 spawnPos = new Vector3(x * spacing, y * spacing, 0f);
                Instantiate(box, spawnPos, Quaternion.identity, transform);
            }
        }
    }
}
