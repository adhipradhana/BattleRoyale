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
    public GameObject healthPackPrefab;

    [Header("Player prefab object.")]
    public GameObject playerPrefab;
    public GameObject aggresivePlayerPrefab;

    [Header("Item generation number")]
    public int bulletPackNumber = 5;
    public int healthPackNumber = 5;

    [Header("Number of agents")]
    public int agentNumber = 2;
    public int normalAgentCount;
    public int aggresiveAgentCount;

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

        mazeRows = environment.MazeRows;
        mazeColumns = environment.MazeColumns;
        bulletPackNumber = environment.BulletPackNumber;
        healthPackNumber = environment.HealthPackNumber;
        agentNumber = environment.AgentNumber;
        generateStep = environment.GenerateStep;
        normalAgentCount = environment.NormalCount;
        aggresiveAgentCount = environment.AggresiveCount;
    }

    public override void AcademyReset()
    {
        InitializeVariable();
        GenerateMaze(mazeRows, mazeColumns);
        GenerateItem(bulletPackNumber, healthPackNumber);
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
            SpawnItem(bulletPackNumber, healthPackNumber);
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

    public void GenerateItem(int bulletPackNumber, int healthPackNumber)
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

    public void SpawnItem(int bulletPackNumber, int healthPackNumber)
    {
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

        for (int i = 0; i < normalAgentCount; i++)
        {
            // Get random Vector2 value
            int index = Random.Range(0, emptyCellsClone.Count);
            Vector2 position = emptyCellsClone[index];
            emptyCellsClone.Remove(position);

            GameObject player = Instantiate(playerPrefab);
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

            GameObject player = Instantiate(aggresivePlayerPrefab);
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

                // Increase spawnPos y.
                spawnPos.y += cellSize;
            }

            // Reset spawnPos y and increase spawnPos x.
            spawnPos.y = startPos.y;
            spawnPos.x += cellSize;
        }

        CreateCentre();
        RunAlgorithm();
    }

    // This is where the fun stuff happens.
    public void RunAlgorithm()
    {
        // Get start cell, make it visited (i.e. remove from unvisited list).
        unvisited.Remove(currentCell);

        // While we have unvisited cells.
        while (unvisited.Count > 0)
        {
            List<Cell> unvisitedNeighbours = GetUnvisitedNeighbours(currentCell);
            if (unvisitedNeighbours.Count > 0)
            {
                // Get a random unvisited neighbour.
                checkCell = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
                // Add current cell to stack.
                stack.Add(currentCell);
                // Compare and remove walls.
                CompareWalls(currentCell, checkCell);
                // Make currentCell the neighbour cell.
                currentCell = checkCell;
                // Mark new current cell as visited.
                unvisited.Remove(currentCell);
            }
            else if (stack.Count > 0)
            {
                // Make current cell the most recently added Cell from the stack.
                currentCell = stack[stack.Count - 1];
                // Remove it from stack.
                stack.Remove(currentCell);
            }
        }
    }

    public List<Cell> GetUnvisitedNeighbours(Cell curCell)
    {
        // Create a list to return.
        List<Cell> neighbours = new List<Cell>();
        // Create a Cell object.
        Cell nCell = curCell;
        // Store current cell grid pos.
        Vector2 cPos = curCell.gridPos;

        foreach (Vector2 p in neighbourPositions)
        {
            // Find position of neighbour on grid, relative to current.
            Vector2 nPos = cPos + p;
            // If cell exists.
            if (allCells.ContainsKey(nPos)) nCell = allCells[nPos];
            // If cell is unvisited.
            if (unvisited.Contains(nCell)) neighbours.Add(nCell);
        }

        return neighbours;
    }

    // Compare neighbour with current and remove appropriate walls.
    public void CompareWalls(Cell cCell, Cell nCell)
    {
        // If neighbour is left of current.
        if (nCell.gridPos.x < cCell.gridPos.x)
        {
            RemoveWall(nCell.cScript, 2);
            RemoveWall(cCell.cScript, 1);
        }
        // Else if neighbour is right of current.
        else if (nCell.gridPos.x > cCell.gridPos.x)
        {
            RemoveWall(nCell.cScript, 1);
            RemoveWall(cCell.cScript, 2);
        }
        // Else if neighbour is above current.
        else if (nCell.gridPos.y > cCell.gridPos.y)
        {
            RemoveWall(nCell.cScript, 4);
            RemoveWall(cCell.cScript, 3);
        }
        // Else if neighbour is below current.
        else if (nCell.gridPos.y < cCell.gridPos.y)
        {
            RemoveWall(nCell.cScript, 3);
            RemoveWall(cCell.cScript, 4);
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

    public void CreateCentre()
    {
        // Get the 4 centre cells using the rows and columns variables.
        // Remove the required walls for each.
        centreCells[0] = allCells[new Vector2((mazeColumns / 2), (mazeRows / 2) + 1)];
        RemoveWall(centreCells[0].cScript, 4);
        RemoveWall(centreCells[0].cScript, 2);
        centreCells[1] = allCells[new Vector2((mazeColumns / 2) + 1, (mazeRows / 2) + 1)];
        RemoveWall(centreCells[1].cScript, 4);
        RemoveWall(centreCells[1].cScript, 1);
        centreCells[2] = allCells[new Vector2((mazeColumns / 2), (mazeRows / 2))];
        RemoveWall(centreCells[2].cScript, 3);
        RemoveWall(centreCells[2].cScript, 2);
        centreCells[3] = allCells[new Vector2((mazeColumns / 2) + 1, (mazeRows / 2))];
        RemoveWall(centreCells[3].cScript, 3);
        RemoveWall(centreCells[3].cScript, 1);

        // Create a List of ints, using this, select one at random and remove it.
        // We then use the remaining 3 ints to remove 3 of the centre cells from the 'unvisited' list.
        // This ensures that one of the centre cells will connect to the maze but the other three won't.
        // This way, the centre room will only have 1 entry / exit point.
        List<int> rndList = new List<int> { 0, 1, 2, 3 };
        int startCell = rndList[Random.Range(0, rndList.Count)];
        rndList.Remove(startCell);
        currentCell = centreCells[startCell];
        foreach (int c in rndList)
        {
            unvisited.Remove(centreCells[c]);
        }
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