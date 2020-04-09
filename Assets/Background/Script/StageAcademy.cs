using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

// Source : https://github.com/c00pala/Unity-2D-Maze-Generator
public class StageAcademy : Academy
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

    [Tooltip("If you want to disable the main sprite so the cell has no background, set to TRUE. This will create a maze with only walls.")]
    public bool disableCellSprite;

    [Header("Item generation prefab object.")]
    public GameObject bulletPackPrefab;

    [Header("Player prefab object.")]
    public GameObject playerPrefab;

    [Header("Item generation number")]
    public int bulletPackNumber = 5;

    [Header("Number of agents")]
    public int agentNumber = 2;
    public int normalAgentCount;
    public int aggresiveAgentCount;
    public int passiveAgentCount;

    [Header("Number of step to generate item")]
    public int generateStep = 1000;
    private int step = 0;

    // ------------------------------------------------------
    // System defined variables - You don't need to touch these:
    // ------------------------------------------------------

    // Variable to store size of centre room. Hard coded to be 2.
    private const int CentreSize = 2;

    // Dictionary to hold and locate all cells in maze.
    private Dictionary<Vector2, Cell> allCells = new Dictionary<Vector2, Cell>();
    // List to hold unvisited cells.
    private List<Cell> unvisited = new List<Cell>();
    // List to store 'stack' cells, cells being checked during generation.
    private List<Cell> stack = new List<Cell>();
    // List for saving dictionary key
    private List<Vector2> emptyCells = new List<Vector2>();
    private List<Vector2> emptyCellsClone = new List<Vector2>();

    // Array will hold 4 centre room cells, from 0 -> 3 these are:
    // Top left (0), top right (1), bottom left (2), bottom right (3).
    private Cell[] centreCells = new Cell[4];

    // Cell variables to hold current and checking Cells.
    private Cell currentCell;
    private Cell checkCell;

    // Array of all possible neighbour positions.
    private Vector2[] neighbourPositions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(0, -1) };

    // Size of the cells, used to determine how far apart to place cells during generation.
    private float cellSize;

    // Parent of each group
    private GameObject mazeParent;
    private GameObject itemParent;
    private GameObject playerParent;
    private List<GameObject> playersList = new List<GameObject>();

    #endregion

    public override void InitializeAcademy()
    {
        ConfigReader config = new ConfigReader();
        ConfigReader.Environment environment = config.ReadEnvironment();

        mazeRows = environment.ArenaRows;
        mazeColumns = environment.ArenaColumns;
        bulletPackNumber = environment.BulletPackNumber;
        agentNumber = environment.AgentNumber;
        generateStep = environment.GenerateStep;
        normalAgentCount = environment.NormalNumber;
        aggresiveAgentCount = environment.AggresiveNumber;
        passiveAgentCount = environment.PassiveNumber;
    }

    public override void AcademyReset()
    {
        InitializeVariable();
        GenerateMaze(mazeRows, mazeColumns);
        GenerateItem(bulletPackNumber);
        GeneratePlayers();
    }

    public override void AcademyStep()
    {
        if (AcademyValue.gameDone)
        {
            foreach (GameObject player in playersList)
            {
                player.GetComponent<Player>().Done();
            }
        }

        step++;
        if (step % generateStep == 0)
        {
            SpawnItem(bulletPackNumber);
            step = 0;
        }
    }

    private void GenerateMaze(int rows, int columns)
    {
        if (mazeParent != null) DeleteMaze();

        mazeRows = rows;
        mazeColumns = columns;
        CreateLayout();
    }

    public void InitializeVariable()
    {
        step = 0;
        AcademyValue.gameDone = false;
        playersList.Clear();
    }

    public void GenerateItem(int bulletPackNumber)
    {
        if (itemParent != null) Destroy(itemParent);

        // Create an empty parent object to hold all item in the scene.
        itemParent = new GameObject();
        itemParent.transform.position = Vector2.zero;
        itemParent.name = "Item";

        // Clone item key
        emptyCellsClone.Clear();
        foreach (Vector2 key in emptyCells)
        {
            emptyCellsClone.Add(new Vector2(key.x, key.y));
        }

        // Check if item is more than cell number
        int cellCount = emptyCells.Count;
        int itemCount = bulletPackNumber;

        if (itemCount > cellCount)
        {
            itemCount = cellCount;
            bulletPackNumber = itemCount / 2;
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
    }

    public void SpawnItem(int bulletPackNumber)
    {
        // Clone item key
        emptyCellsClone.Clear();
        foreach (Vector2 key in emptyCells)
        {
            emptyCellsClone.Add(new Vector2(key.x, key.y));
        }

        // Check if item is more than cell number
        int cellCount = emptyCells.Count;
        int itemCount = bulletPackNumber;

        if (itemCount > cellCount)
        {
            itemCount = cellCount;
            bulletPackNumber = itemCount / 2;
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
    }

    private void GeneratePlayers()
    {
        if (playerParent != null) Destroy(playerParent);

        // Create an empty parent object to hold all item in the scene.
        playerParent = new GameObject();
        playerParent.transform.position = Vector2.zero;
        playerParent.name = "Players";

        AcademyValue.playerCount = agentNumber;

        for (int i = 0; i < normalAgentCount; i++)
        {
            // Get random Vector2 value
            int index = Random.Range(0, emptyCellsClone.Count);
            Vector2 position = emptyCellsClone[index];
            emptyCellsClone.Remove(position);

            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<Player>().playerType = Player.PlayerType.Normal;
            player.GetComponent<Player>().body.color = Color.red;
            player.transform.position = position;
            player.transform.SetParent(playerParent.transform);

            playersList.Add(player);
        }

        for (int i = 0; i < aggresiveAgentCount; i++)
        {
            // Get random Vector2 value
            int index = Random.Range(0, emptyCellsClone.Count);
            Vector2 position = emptyCellsClone[index];
            emptyCellsClone.Remove(position);

            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<Player>().playerType = Player.PlayerType.Aggresive;
            player.GetComponent<Player>().body.color = Color.green;
            player.transform.position = position;
            player.transform.SetParent(playerParent.transform);

            playersList.Add(player);
        }

        for (int i = 0; i < passiveAgentCount; i++)
        {
            // Get random Vector2 value
            int index = Random.Range(0, emptyCellsClone.Count);
            Vector2 position = emptyCellsClone[index];
            emptyCellsClone.Remove(position);

            GameObject player = Instantiate(playerPrefab);
            player.GetComponent<Player>().playerType = Player.PlayerType.Passive;
            player.GetComponent<Player>().body.color = Color.cyan;
            player.transform.position = position;
            player.transform.SetParent(playerParent.transform);

            playersList.Add(player);
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

                GenerateCell(spawnPos, new Vector2(x, y));

                RemoveVerticalWall(new Vector2(x, y));

                // Increase spawnPos y.
                spawnPos.y += cellSize;
            }

            // Reset spawnPos y and increase spawnPos x.
            spawnPos.y = startPos.y;
            spawnPos.x += cellSize;
        }

        GenerateArena();
    }

    public void RemoveVerticalWall(Vector2 vector)
    {
        if (vector.x != 1)
        {
            RemoveWall(allCells[vector].cScript, 1);
        }

        if (vector.x != mazeColumns)
        {
            RemoveWall(allCells[vector].cScript, 2);
        }
    }

    public void GenerateArena()
    {
        int spawnableWallAmount = mazeColumns / 2;
        bool[] flag;

        for (int y = 1; y < mazeRows; y++)
        {
            flag = new bool[mazeColumns+1];

            for (int i = 0; i < spawnableWallAmount; i++)
            {
                int x = Random.Range(1, mazeColumns + 1);

                while(flag[x])
                {
                    x = Random.Range(1, mazeColumns + 1);
                }

                flag[x] = true;
                RemoveWall(allCells[new Vector2(x, y)].cScript, 3);
                RemoveWall(allCells[new Vector2(x, y+1)].cScript, 4);
            }
        }
    }

    // Function disables wall of your choosing, pass it the script attached to the desired cell
    // and an 'ID', where the ID = the wall. 1 = left, 2 = right, 3 = up, 4 = down.
    public void RemoveWall(CellScript cScript, int wallID)
    {
        if (wallID == 1) cScript.wallL.SetActive(false);
        else if (wallID == 2) cScript.wallR.SetActive(false);
        else if (wallID == 3) cScript.wallU.SetActive(false);
        else if (wallID == 4) cScript.wallD.SetActive(false);
    }

    public void GenerateCell(Vector2 pos, Vector2 keyPos)
    {
        // Create new Cell object.
        Cell newCell = new Cell();

        // Store reference to position in grid.
        newCell.gridPos = keyPos;
        // Set and instantiate cell GameObject.
        newCell.cellObject = Instantiate(cellPrefab, pos, cellPrefab.transform.rotation);
        // Child new cell to parent.
        if (mazeParent != null) newCell.cellObject.transform.parent = mazeParent.transform;
        // Set name of cellObject.
        newCell.cellObject.name = "Cell - X:" + keyPos.x + " Y:" + keyPos.y;
        // Get reference to attached CellScript.
        newCell.cScript = newCell.cellObject.GetComponent<CellScript>();
        // Disable Cell sprite, if applicable.
        if (disableCellSprite) newCell.cellObject.GetComponent<SpriteRenderer>().enabled = false;

        // Add to Lists.
        allCells[keyPos] = newCell;
        unvisited.Add(newCell);
    }

    public void DeleteMaze()
    {
        if (mazeParent != null) Destroy(mazeParent);
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

        // Create an empty parent object to hold the maze in the scene.
        mazeParent = new GameObject();
        mazeParent.transform.position = Vector2.zero;
        mazeParent.name = "Maze";
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