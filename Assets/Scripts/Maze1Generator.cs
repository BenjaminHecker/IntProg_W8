using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze1Generator : MonoBehaviour
{
    [SerializeField] private PrefabDatabase prefabDB;

    [SerializeField] private int mazeX = 20;
    [SerializeField] private int mazeY = 20;

    [SerializeField] private Transform mazeGroup;

    private Maze1Cell[,] mazeCellMap;

    private Stack<Maze1Cell> pathFindingCells = new Stack<Maze1Cell>();

    private void Start()
    {
        GenerateMaze();
    }

    private void GenerateMaze()
    {
        mazeCellMap = new Maze1Cell[mazeX, mazeY];

        for (int x = 0; x < mazeX; x++)
        {
            for (int y = 0; y < mazeY; y++)
            {
                Maze1Cell cell = Instantiate(prefabDB.prefabList[0], mazeGroup).GetComponent<Maze1Cell>();
                cell.transform.position = new Vector3(cell.mazeSize * x, 0, cell.mazeSize * y);

                mazeCellMap[x, y] = cell;
                cell.Init(x, y);
            }
        }

        RecursiveBackTracking(mazeCellMap[Random.Range(0, mazeX), Random.Range(0, mazeY)]);
    }

    private void RecursiveBackTracking(Maze1Cell selectedCell)
    {
        selectedCell.visited = true;
        List<Maze1Cell> neighborUnvisitedCells = new List<Maze1Cell>();

        NeighborChecker(ref neighborUnvisitedCells, selectedCell.locX - 1, selectedCell.locY);      // left
        NeighborChecker(ref neighborUnvisitedCells, selectedCell.locX + 1, selectedCell.locY);      // right
        NeighborChecker(ref neighborUnvisitedCells, selectedCell.locX, selectedCell.locY - 1);      // down
        NeighborChecker(ref neighborUnvisitedCells, selectedCell.locX, selectedCell.locY + 1);      // up

        if (neighborUnvisitedCells.Count > 0)
        {
            Maze1Cell nextSelectedCell = neighborUnvisitedCells[Random.Range(0, neighborUnvisitedCells.Count)];

            if (nextSelectedCell.locX < selectedCell.locX)
            {
                nextSelectedCell.walls[0].SetActive(false);
                selectedCell.walls[1].SetActive(false);
            }
            else if (nextSelectedCell.locX > selectedCell.locX)
            {
                nextSelectedCell.walls[1].SetActive(false);
                selectedCell.walls[0].SetActive(false);
            }
            else if (nextSelectedCell.locY > selectedCell.locY)
            {
                nextSelectedCell.walls[2].SetActive(false);
                selectedCell.walls[3].SetActive(false);
            }
            else if (nextSelectedCell.locY < selectedCell.locY)
            {
                nextSelectedCell.walls[3].SetActive(false);
                selectedCell.walls[2].SetActive(false);
            }

            pathFindingCells.Push(selectedCell);
            RecursiveBackTracking(nextSelectedCell);
        }
        else if (pathFindingCells.Count > 0)
        {
            Maze1Cell nextSelectedCell = pathFindingCells.Pop();
            RecursiveBackTracking(nextSelectedCell);
        }
        else
        {
            Debug.Log("Generation finished");
        }
    }

    private void NeighborChecker(ref List<Maze1Cell> unvisited, int x, int y)
    {
        if (x >= 0 && x < mazeX && y >= 0 && y < mazeY)
        {
            Maze1Cell checkingNeighborCell = mazeCellMap[x, y];

            if (!checkingNeighborCell.visited)
            {
                unvisited.Add(checkingNeighborCell);
            }
        }
    }
}
