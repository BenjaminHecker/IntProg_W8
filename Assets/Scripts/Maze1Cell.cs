using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze1Cell : MonoBehaviour
{
    [HideInInspector] public bool visited = false;

    public float mazeSize = 5;

    public GameObject[] walls;

    // index in int[,] of generated map
    [HideInInspector] public int locX;
    [HideInInspector] public int locY;

    public void Init(int x, int y)
    {
        locX = x;
        locY = y;
    }
}
