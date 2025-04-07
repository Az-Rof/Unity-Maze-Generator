using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
 
    public int width = 20;  // Maze width (must be odd for proper paths)
    public int height = 20; // Maze height (must be odd for proper paths)

    public GameObject wallPrefab;   // Wall Prefab
    public GameObject floorPrefab;  // Floor Prefab
    public GameObject doorPrefab;   // Exit Door Prefab
    public GameObject playerPrefab; // Player Prefab
    public GameObject enemyPrefab; // Enemy Prefab
    private Cell[,] cells;
    private int[,] grid;

    private class Cell
    {
        public bool north = true;
        public bool south = true;
        public bool east = true;
        public bool west = true;
        public bool visited = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        PrepareMap();
    }

    // Update is called once per frame
    void Update()
    {
        // Update logic if needed

    }

    void PrepareMap()
    {

        // Initialize the floor grid
        CreateFloors();

        // Initialize the walls of edge of the maze
        CreateEdgeWalls();

        // Create the Door at the random position position at the edge of the maze 
        CreateRandomDoor();

        // Generate the maze using recursive backtracking algorithm
        GenerateMaze();

        // Create the player at Random position in the maze
        CreatePlayer();

        // Create an enemy at a random position in the maze but not on the player
        CreateEnemy();

    }


    void GenerateMaze()
    {
        int cellWidth = width / 2;
        int cellHeight = height / 2;
        cells = new Cell[cellWidth, cellHeight];

        // Initialize all cells
        for (int x = 0; x < cellWidth; x++)
        {
            for (int z = 0; z < cellHeight; z++)
            {
                cells[x, z] = new Cell();
            }
        }

        // Use a stack for backtracking
        Stack<Vector2Int> stack = new Stack<Vector2Int>();
        Vector2Int start = new Vector2Int(0, 0);
        cells[start.x, start.y].visited = true;
        stack.Push(start);

        while (stack.Count > 0)
        {
            Vector2Int current = stack.Pop();
            List<Vector2Int> neighbors = GetUnvisitedNeighbors(current.x, current.y);

            if (neighbors.Count > 0)
            {
                stack.Push(current);
                Vector2Int chosen = neighbors[UnityEngine.Random.Range(0, neighbors.Count)];
                RemoveWall(current, chosen);
                cells[chosen.x, chosen.y].visited = true;
                stack.Push(chosen);
            }
        }

        // Create walls based on cell data
        GameObject internalWallsParent = new GameObject("InternalWalls");
        internalWallsParent.transform.parent = transform;

        for (int x = 0; x < cellWidth; x++)
        {
            for (int z = 0; z < cellHeight; z++)
            {
                Cell cell = cells[x, z];

                // East wall
                if (cell.east && x < cellWidth - 1)
                {
                    Vector3 wallPosition = new Vector3(x * 2 + 1, 0, z * 2);
                    GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                    wall.transform.Rotate(0, 90, 0);
                    wall.transform.parent = internalWallsParent.transform;
                }

                // North wall
                if (cell.north && z < cellHeight - 1)
                {
                    Vector3 wallPosition = new Vector3(x * 2, 0, z * 2 + 1);
                    GameObject wall = Instantiate(wallPrefab, wallPosition, Quaternion.identity);
                    wall.transform.parent = internalWallsParent.transform;
                }
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(int x, int z)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        int cellWidth = width / 2;
        int cellHeight = height / 2;

        // North
        if (z < cellHeight - 1 && !cells[x, z + 1].visited)
            neighbors.Add(new Vector2Int(x, z + 1));
        // East
        if (x < cellWidth - 1 && !cells[x + 1, z].visited)
            neighbors.Add(new Vector2Int(x + 1, z));
        // South
        if (z > 0 && !cells[x, z - 1].visited)
            neighbors.Add(new Vector2Int(x, z - 1));
        // West
        if (x > 0 && !cells[x - 1, z].visited)
            neighbors.Add(new Vector2Int(x - 1, z));

        return neighbors;
    }

    void RemoveWall(Vector2Int current, Vector2Int neighbor)
    {
        int dx = neighbor.x - current.x;
        int dz = neighbor.y - current.y;

        if (dx == 1)
        {
            cells[current.x, current.y].east = false;
            cells[neighbor.x, neighbor.y].west = false;
        }
        else if (dx == -1)
        {
            cells[current.x, current.y].west = false;
            cells[neighbor.x, neighbor.y].east = false;
        }
        else if (dz == 1)
        {
            cells[current.x, current.y].north = false;
            cells[neighbor.x, neighbor.y].south = false;
        }
        else if (dz == -1)
        {
            cells[current.x, current.y].south = false;
            cells[neighbor.x, neighbor.y].north = false;
        }
    }


    void CreateFloors()
    {
        // Create the floor parents for better organization in the hierarchy
        GameObject floorParent = new GameObject("Floors");
        floorParent.transform.parent = transform; // Set the parent to the maze generator

        // The floor grid made for 2x2 cells
        for (int x = 0; x < width; x += 2)
        {
            for (int y = 0; y < height; y += 2)
            {
                // Create a floor tile at the current position
                GameObject floorTile = Instantiate(floorPrefab, new Vector3(x, 0, y), Quaternion.identity);
                // GameObject floorTile2 = Instantiate(floorPrefab, new Vector3(x , 3f, y), Quaternion.identity);
                // floorTile2.transform.localRotation = Quaternion.Euler(180, 0, 0); // Set the rotation to zero
                
                floorTile.transform.parent = floorParent.transform; // Set the parent to the floor parent
                // floorTile2.transform.parent = floorParent.transform; // Set the parent to the floor parent
            }
        }
    }

    void CreateEdgeWalls()
    {
        // Create the wall parents for better organization in the hierarchy
        GameObject wallParent = new GameObject("EdgeWalls");
        wallParent.transform.parent = transform; // Set the parent to the maze generator

        // Create walls at the dead end of every width and height
        for (int x = -1; x < width; x++)
        {
            if (x == -1 || x == width - 1)//-2
            {
                for (int y = -1; y <= height; y += 2)//1,<
                {
                    if (y != 0 && y <= height - 1)//!=,-1
                    {
                        // Create a wall at the current position
                        GameObject wallTile = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity);

                        // Rotate the wall tile to face the correct direction
                        wallTile.transform.Rotate(0, 90, 0); // Rotate the wall tile to face the correct direction

                        // Set the wall tile's position to the correct location
                        wallTile.transform.parent = wallParent.transform; // Set the parent to the wall parent

                    }
                }
            }
        }

        for (int y = -1; y < height; y++)
        {
            if (y == -1 || y == height - 1)//-2
            {
                for (int x = -1; x <= width; x += 2)
                {
                    if (x != 0 && x <= width)
                    {
                        // Create a wall at the current position
                        GameObject wallTile = Instantiate(wallPrefab, new Vector3(x, 0, y), Quaternion.identity);
                        wallTile.transform.parent = wallParent.transform; // Set the parent to the wall parent
                    }
                }
            }
        }
    }

    void CreateRandomDoor()
    {
        // Pastikan prefab tidak kosong
        if (doorPrefab == null)
        {
            Debug.LogError("doorPrefab is not assigned!");
            return;
        }

        // Buat parent untuk pintu
        GameObject doorParent = new GameObject("Doors");
        doorParent.transform.parent = transform;

        // Randomly select an edge of the maze
        int edge = UnityEngine.Random.Range(0, 4);
        int x = 0, y = 0;

        switch (edge)
        {
            case 0: // Left
                x = -1;
                y = UnityEngine.Random.Range(1, height - 1); // Ensure not in the corner
                break;
            case 1: // Right
                x = width - 1;
                y = UnityEngine.Random.Range(1, height - 1);
                break;
            case 2: // Bottom
                x = UnityEngine.Random.Range(1, width - 1);
                y = -1;
                break;
            case 3: // Top
                x = UnityEngine.Random.Range(1, width - 1);
                y = height - 1;
                break;
        }

        // Debugging: Log posisi pintu
        Debug.Log($"Door spawned at: ({x}, {y}) on edge {edge}");

        // Cek apakah ada dinding di lokasi tersebut sebelum mengganti dengan pintu
        GameObject wallParent = GameObject.Find("EdgeWalls");
        if (wallParent != null)
        {
            foreach (Transform wall in wallParent.transform)
            {
                if (wall.position == new Vector3(x, 0, y)) // Jika ada dinding di posisi ini
                {
                    Destroy(wall.gameObject); // Hapus dinding
                    break;
                }
            }
        }

        // Buat pintu di posisi yang telah ditentukan
        GameObject doorTile = Instantiate(doorPrefab, new Vector3(x, 0, y), Quaternion.identity);
        doorTile.transform.localScale = new Vector3(2f, 2f, 2f); // Set ukuran pintu
        doorTile.transform.parent = doorParent.transform;

        // Atur rotasi pintu agar menghadap arah yang benar
        if (edge == 0)        // Kiri
            doorTile.transform.Rotate(0, 90, 0);
        else if (edge == 1)   // Kanan
            doorTile.transform.Rotate(0, -90, 0);
        else if (edge == 2)   // Bawah
            doorTile.transform.Rotate(0, 180, 0);
    }

    // Create the player at a random position in the maze
    void CreatePlayer()
    {
        // Randomly select a position in the maze
        int x = UnityEngine.Random.Range(1, width - 2);
        int y = UnityEngine.Random.Range(1, height - 2);

        // Debugging: Log posisi pemain
        Debug.Log($"Player spawned at: ({x}, {y})");

        // Create the player at the selected position
        GameObject playerTile = Instantiate(playerPrefab, new Vector3(x, 1f, y), Quaternion.identity);
        // playerTile.transform.localScale = new Vector3(1f, 1f, 1f);
    }
    void CreateEnemy()
    {
        // Randomly select a position in the maze
        int x = UnityEngine.Random.Range(1, width - 2);
        int y = UnityEngine.Random.Range(1, height - 2);

        // Debugging: Log posisi musuh
        Debug.Log($"Enemy spawned at: ({x}, {y})");

        // Create the enemy at the selected position
        GameObject enemyTile = Instantiate(enemyPrefab, new Vector3(x, 1f, y), Quaternion.identity);
    }
        // Tambahkan method ini di bagian akhir class, sebelum penutup }
    public int[,] GetGrid()
    {
        return grid;
    }
}