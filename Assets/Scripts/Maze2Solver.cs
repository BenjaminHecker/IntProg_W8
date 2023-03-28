using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maze2Generator;

public class Maze2Solver : MonoBehaviour
{
    public class Cell
    {
        public int x;
        public int y;

        public Cell(int x, int y) { this.x = x; this.y = y; }
    }

    public static Maze2Solver instance;

    [SerializeField] private Transform solverGroup;
    public static Transform SolverGroup { get { return instance.solverGroup; } }

    [SerializeField] private GameObject markerPrefab;
    public static GameObject MarkerPrefab { get { return instance.markerPrefab; } }

    [SerializeField] private float markerTick = 0.02f;
    public static float MarkerTick { get { return instance.markerTick; } }

    private void Awake()
    {
        instance = this;
    }

    public static void GeneratePath(Cell src)
    {
        instance.StartCoroutine(Pathfinder(src));
    }

    private static IEnumerator Pathfinder(Cell src)
    {
        Stack<Cell> s = new Stack<Cell>();                        // stores stack of cells to search

        bool[,] visited = new bool[mazeWidth, mazeHeight];        // tracks visited cells

        // resets array before pathfinding
        for (int i = 0; i < mazeWidth; i++)
            for (int j = 0; j < mazeHeight; j++)
                visited[i, j] = false;

        // starts at src
        visited[src.x, src.y] = true;
        s.Push(src);

        // runs until the entire maze has been searched
        while (s.Count != 0)
        {
            Cell u = s.Pop();                       // stores the first cell in the stack

            Instantiate(MarkerPrefab, mazeCellMap[u.x, u.y].transform.position, Quaternion.identity, SolverGroup);

            // iterates through adjacent open cells
            List<Cell> adjacent = Adj(u);
            foreach (Cell c in adjacent)
            {
                int x = c.x;
                int y = c.y;

                // if unvisited, mark it as visited, store its predecessor, and add it to the stack
                if (visited[x, y] == false)
                {
                    visited[x, y] = true;
                    s.Push(c);
                }

                yield return new WaitForSeconds(MarkerTick);
            }
        }
    }

    // returns a list of the adjacent open cells
    private static List<Cell> Adj(Cell c)
    {
        List<Cell> neighbors = new List<Cell>();
        int x = c.x;
        int y = c.y;

        if (x > 0 && mazeCellMap[x - 1, y].IsWalkable())                   // left
            neighbors.Add(new Cell(x - 1, y));

        if (y > 0 && mazeCellMap[x, y - 1].IsWalkable())                   // down
            neighbors.Add(new Cell(x, y - 1));

        if (x < mazeWidth - 1 && mazeCellMap[x + 1, y].IsWalkable())      // right
            neighbors.Add(new Cell(x + 1, y));

        if (y < mazeHeight - 1 && mazeCellMap[x, y + 1].IsWalkable())     // up
            neighbors.Add(new Cell(x, y + 1));

        return neighbors;
    }
}
