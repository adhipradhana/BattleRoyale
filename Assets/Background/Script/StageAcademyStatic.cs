using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

// Source : https://github.com/c00pala/Unity-2D-Maze-Generator
public class StageAcademyStatic : Academy
{
    #region Variables:
    // ------------------------------------------------------
    // User defined variables - set in editor:
    // ------------------------------------------------------
    [Header("Maze generation values:")]
    [Tooltip("How many cells tall is the maze. MUST be an even number. " +
        "If number is odd, it will be reduced by 1.\n\n" +
        "Minimum value of 4.")]
    public int mazeRows;
    [Tooltip("How many cells wide is the maze. Must be an even number. " +
        "If number is odd, it will be reduced by 1.\n\n" +
        "Minimum value of 4.")]
    public int mazeColumns;

    [Header("Maze object variables:")]
    [Tooltip("Cell prefab object.")]
    public GameObject cellPrefab;

    [Header("Item generation prefab object.")]
    public GameObject bulletPackPrefab;
    public GameObject healthPackPrefab;

    [Header("Player prefab object.")]
    public GameObject playerPrefab;

    [Header("Item generation number")]
    public int bulletPackNumber = 5;
    public int healthPackNumber = 5;

    [Header("Number of agents")]
    public int agentNumber = 2;

    // ------------------------------------------------------
    // System defined variables - You don't need to touch these:
    // ------------------------------------------------------

    // Variable to store size of centre room. Hard coded to be 2.
    private const int CentreSize = 2;

    // List for saving dictionary key
    private List<Vector2> emptyCells = new List<Vector2>();
    private List<Vector2> emptyCellsClone = new List<Vector2>();

    // Size of the cells, used to determine how far apart to place cells during generation.
    private float cellSize;

    // Parent of each group
    private GameObject itemParent;
    private GameObject playerParent;

    #endregion

    public override void InitializeAcademy()
    {
        ConfigReader config = new ConfigReader();
        ConfigReader.Environment environment = config.ReadEnvironment();

        mazeRows = environment.MazeRows;
        mazeColumns = environment.MazeColumns;
        bulletPackNumber = environment.BulletPackNumber;
        healthPackNumber = environment.HealthPackNumber;
        agentNumber = environment.AgentNumber;
    }

    public override void AcademyReset()
    {
        GenerateMaze(mazeRows, mazeColumns);
        GenerateItem(bulletPackNumber, healthPackNumber);
        GeneratePlayers();
    }

    private void GenerateMaze(int rows, int columns)
    {
        mazeRows = rows;
        mazeColumns = columns;
        CreateLayout();
    }

    public void GenerateItem(int bulletPackNumber, int healthPackNumber)
    {
        if (itemParent != null) Destroy(itemParent);

        // Create an empty parent object to hold all item in the scene.
        itemParent = new GameObject();
        itemParent.transform.position = Vector2.zero;
        itemParent.name = "Item";

        // Clone item key
        emptyCellsClone = new List<Vector2>();
        foreach (Vector2 key in emptyCells)
        {
            emptyCellsClone.Add(new Vector2(key.x, key.y));
        }

        // Check if item is more than cell number
        int cellCount = emptyCells.Count;
        int itemCount = bulletPackNumber + healthPackNumber;

        if (itemCount > cellCount)
        {
            itemCount = cellCount;
            bulletPackNumber = itemCount / 2;
            healthPackNumber = itemCount / 2;
        }

        for (int i = 0; i < bulletPackNumber; i++)
        {
            // Get random Vector2 value
            int index = Random.Range(0, emptyCellsClone.Count);
            Vector2 position = emptyCellsClone[index];
            emptyCellsClone.Remove(position);

            GameObject bulletPack = Instantiate(bulletPackPrefab);
            bulletPack.transform.position = position;
            bulletPack.transform.SetParent(itemParent.transform);
        }

        for (int i = 0; i < healthPackNumber; i++)
        {
            // Get random Vector2 value
            int index = Random.Range(0, emptyCellsClone.Count);
            Vector2 position = emptyCellsClone[index];
            emptyCellsClone.Remove(position);

            GameObject healthPack = Instantiate(healthPackPrefab);
            healthPack.transform.position = position;
            healthPack.transform.SetParent(itemParent.transform);
        }
    }

    private void GeneratePlayers()
    {
        if (playerParent != null) Destroy(playerParent);

        // Create an empty parent object to hold all item in the scene.
        playerParent = new GameObject();
        playerParent.transform.position = Vector2.zero;
        playerParent.name = "Players";

        AcademyValue.playerCount = agentNumber;
   
        for (int i = 0; i < agentNumber; i++)
        {
            // Get random Vector2 value
            int index = Random.Range(0, emptyCellsClone.Count);
            Vector2 position = emptyCellsClone[index];
            emptyCellsClone.Remove(position);

            GameObject player = Instantiate(playerPrefab);
            player.transform.position = position;
            player.transform.SetParent(playerParent.transform);
        }
    }

    // Creates the grid of cells.
    public void CreateLayout()
    {
        InitValues();

        // Set starting point, set spawn point to start.
        Vector2 startPos = new Vector2(-(cellSize * (mazeColumns / 2)) + (cellSize / 2), -(cellSize * (mazeRows / 2)) + (cellSize / 2));
        Vector2 spawnPos = startPos;

        for (int x = 1; x <= mazeColumns; x++)
        {
            for (int y = 1; y <= mazeRows; y++)
            {
                emptyCells.Add(spawnPos);
                GetBorderValue(spawnPos);

                // Increase spawnPos y.
                spawnPos.y += cellSize;
            }

            // Reset spawnPos y and increase spawnPos x.
            spawnPos.y = startPos.y;
            spawnPos.x += cellSize;
        }
    }

    public void InitValues()
    {
        // Check generation values to prevent generation failing.
        if (IsOdd(mazeRows)) mazeRows--;
        if (IsOdd(mazeColumns)) mazeColumns--;

        if (mazeRows <= 3) mazeRows = 4;
        if (mazeColumns <= 3) mazeColumns = 4;

        // Determine size of cell using localScale.
        cellSize = cellPrefab.transform.localScale.x;
    }

    public void GetBorderValue(Vector2 position)
    {
        if (position.x < AcademyValue.minimumX)
        {
            AcademyValue.minimumX = position.x - (cellSize / 2.0f);
        }

        if (position.x > AcademyValue.maximumX)
        {
            AcademyValue.maximumX = position.x + (cellSize / 2.0f);
        }

        if (position.y < AcademyValue.minimumY)
        {
            AcademyValue.minimumY = position.y - (cellSize / 2.0f);
        }
        if (position.x > AcademyValue.maximumY)
        {
            AcademyValue.maximumY = position.y + (cellSize / 2.0f);
        }
    }

    public bool IsOdd(int value)
    {
        return value % 2 != 0;
    }

    public class Cell
    {
        public Vector2 gridPos;
        public GameObject cellObject;
        public CellScript cScript;
    }
}