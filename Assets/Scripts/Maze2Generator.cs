using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Maze2Solver;

public class Maze2Generator : MonoBehaviour
{
    public static Maze2Generator instance;

    [SerializeField] private PrefabDatabase prefabDB;

    [SerializeField] private int mazeX = 59;
    [SerializeField] private int mazeY = 59;

    public static int mazeWidth { get { return instance.mazeX; } }
    public static int mazeHeight { get { return instance.mazeY; } }

    [SerializeField] [Range(0f, 1f)] private float wallRemovalProportion = 0f;

    [SerializeField] private Transform mazeGroup;

    public static Maze2Cell[,] mazeCellMap;

    private List<Maze2Cell> unvisitedCells = new List<Maze2Cell>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateMaze();

        GeneratePath(new Cell(1, 1));
    }

    private void GenerateMaze()
    {
        mazeCellMap = new Maze2Cell[mazeX, mazeY];

        for (int x = 0; x < mazeX; x++)
        {
            for (int y = 0; y < mazeY; y++)
            {
                Maze2Cell cell = Instantiate(prefabDB.prefabList[1], mazeGroup).GetComponent<Maze2Cell>();
                cell.transform.position = new Vector3(cell.mazeSize * x, 0, cell.mazeSize * y);

                mazeCellMap[x, y] = cell;
                cell.Init(x, y);
            }
        }

        Maze2Cell startCell = mazeCellMap[1, 1];
        unvisitedCells.Add(startCell);
        RecursiveRandomPrim(startCell);

        HashSet<Maze2Cell> removedCells = new HashSet<Maze2Cell>();
        for (int i = 0; i < mazeX * mazeY * wallRemovalProportion; i++)
        {
            Maze2Cell cell;

            do
            {
                cell = mazeCellMap[Random.Range(0, mazeX), Random.Range(0, mazeY)];
            }
            while (removedCells.Contains(cell));

            removedCells.Add(cell);
            cell.wall.SetActive(false);
        }
    }

    private void RecursiveRandomPrim(Maze2Cell startCell)
    {
        unvisitedCells.Remove(startCell);

        if (!startCell.visited)
        {
            startCell.visited = true;
            startCell.wall.SetActive(false);

            if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Right)
            {
                mazeCellMap[startCell.locX + 1, startCell.locY].wall.SetActive(false);
            }
            if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Left)
            {
                mazeCellMap[startCell.locX - 1, startCell.locY].wall.SetActive(false);
            }
            if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Up)
            {
                mazeCellMap[startCell.locX, startCell.locY + 1].wall.SetActive(false);
            }
            if (startCell.tunnelDirection == Maze2TunnelDirectionIndicator.Down)
            {
                mazeCellMap[startCell.locX, startCell.locY - 1].wall.SetActive(false);
            }

            List<Maze2Cell> neighborUnvisitedCells = NeighborChecker(startCell);

            if (neighborUnvisitedCells.Count > 0)
            {
                Maze2Cell endCell = neighborUnvisitedCells[Random.Range(0, neighborUnvisitedCells.Count)];
                endCell.visited = true;
                endCell.wall.SetActive(false);

                if (endCell.locX < startCell.locX)
                {
                    mazeCellMap[startCell.locX - 1, startCell.locY].wall.SetActive(false);
                }
                if (endCell.locX > startCell.locX)
                {
                    mazeCellMap[startCell.locX + 1, startCell.locY].wall.SetActive(false);
                }
                if (endCell.locY < startCell.locY)
                {
                    mazeCellMap[startCell.locX, startCell.locY - 1].wall.SetActive(false);
                }
                if (endCell.locY > startCell.locY)
                {
                    mazeCellMap[startCell.locX, startCell.locY + 1].wall.SetActive(false);
                }

                neighborUnvisitedCells.Remove(endCell);

                unvisitedCells.AddRange(neighborUnvisitedCells);
                unvisitedCells.AddRange(NeighborChecker(endCell));
            }
        }

        if (unvisitedCells.Count > 0)
        {
            RecursiveRandomPrim(unvisitedCells[Random.Range(0, unvisitedCells.Count)]);
        }
        else
        {
            Debug.Log("Generation finished");
        }
    }

    private List<Maze2Cell> NeighborChecker(Maze2Cell cell)
    {
        List<Maze2Cell> neighborUnvisitedCells = new List<Maze2Cell>();

        if (cell.locX - 2 > 0)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX - 2, cell.locY];
            if (!checkingNeighborCell.visited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Right;
            }
        }
        if (cell.locX + 2 < mazeX - 1)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX + 2, cell.locY];
            if (!checkingNeighborCell.visited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Left;
            }
        }
        if (cell.locY - 2 > 0)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY - 2];
            if (!checkingNeighborCell.visited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Up;
            }
        }
        if (cell.locY + 2 < mazeY - 1)
        {
            Maze2Cell checkingNeighborCell = mazeCellMap[cell.locX, cell.locY + 2];
            if (!checkingNeighborCell.visited)
            {
                neighborUnvisitedCells.Add(checkingNeighborCell);
                checkingNeighborCell.tunnelDirection = Maze2TunnelDirectionIndicator.Down;
            }
        }

        return neighborUnvisitedCells;
    }
}
