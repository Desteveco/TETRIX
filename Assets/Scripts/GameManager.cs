using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] Tetrominos;
    public float baseFrequency = 0.8f; 
    private float movementFrequency; 
    private float passedTime = 0;
    private GameObject currentTetromino;

    public GameObject gameOverText; 
    public GameObject restartButton; // Referencia como GameObject para el OnClick

    private bool isGameOver = false;
    
    public GameObject grayBlockPrefab; // Arrastra aquí tu prefab de bloque gris
    public float garbageInterval = 5f; // Cada cuántos segundos sale la fila
    private float garbageTimer = 0f;
    public Transform previewPoint; 
    private int nextIndex;
    private GameObject nextTetrominoPreview;
    private GameObject ghostTetromino; 
    private GameObject currentGhost;
    
    
    void UpdateGhost()
{
    if (currentTetromino == null || isGameOver)
    {
        if (currentGhost != null) currentGhost.SetActive(false);
        return;
    }

    if (currentGhost == null)
    {
        currentGhost = Instantiate(currentTetromino);
        currentGhost.name = "Ghost";
        
        Destroy(currentGhost.GetComponent<GameManager>());
        
        foreach (SpriteRenderer sr in currentGhost.GetComponentsInChildren<SpriteRenderer>())
    {
        sr.sortingLayerName = "Default"; 

        sr.sortingOrder = 2; 

        sr.color = new Color(1f, 1f, 1f, 0.2f); 
        
        sr.maskInteraction = SpriteMaskInteraction.None;
    }
    }

    currentGhost.SetActive(true);
    currentGhost.transform.position = currentTetromino.transform.position;
    currentGhost.transform.rotation = currentTetromino.transform.rotation;

    DropGhost();
}
void DropGhost()
{
    // Bajamos la sombra mientras la posición sea válida en el grid
    while (GetComponent<GridScript>().IsValidPosition(currentGhost.transform))
    {
        currentGhost.transform.position += Vector3.down;
    }
    
    // Cuando choca, la subimos un espacio (porque el while se pasó por uno)
    currentGhost.transform.position -= Vector3.down;
}

bool IsValidGhostPosition()
{
    // Usamos la misma lógica del Grid pero aplicada al objeto sombra
    return GetComponent<GridScript>().IsValidPosition(currentGhost.transform);
}
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void Start()
    {
        movementFrequency = baseFrequency;
        gameOverText.SetActive(false);
        if(restartButton != null) restartButton.SetActive(false);
        nextIndex = Random.Range(0, Tetrominos.Length);
        SpawnTetromino();
    }

    void Update()
    {
        if (isGameOver) return;

        UpdateSpeed();
        HandleGarbageTimer();

        passedTime += Time.deltaTime;
        if (passedTime >= movementFrequency)
        {
            passedTime -= movementFrequency;
            MoveTetromino(Vector3.down);
        }
        UserInput();
        UpdateGhost();
    }
    void UpdateSpeed()
{
    int score = GetComponent<GridScript>().currentScore;
    
    float level = Mathf.Floor(score / 1000f);
    baseFrequency = Mathf.Max(0.8f - (level * 0.1f), 0.2f);
}
    [Header("Input Settings")]
public float continuousMoveDelay = 0.2f; // Retraso inicial
public float continuousMoveInterval = 0.05f; // Velocidad de repetición
private float moveTimer = 0;
private float horizontalTimer = 0;
    void UserInput()
{
    // --- MOVIMIENTO HORIZONTAL (Izquierda/Derecha) ---
    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
        MoveTetromino(Vector3.left);
        horizontalTimer = Time.time + continuousMoveDelay;
    }
    else if (Input.GetKey(KeyCode.LeftArrow) && Time.time > horizontalTimer)
    {
        MoveTetromino(Vector3.left);
        horizontalTimer = Time.time + continuousMoveInterval;
    }

    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
        MoveTetromino(Vector3.right);
        horizontalTimer = Time.time + continuousMoveDelay;
    }
    else if (Input.GetKey(KeyCode.RightArrow) && Time.time > horizontalTimer)
    {
        MoveTetromino(Vector3.right);
        horizontalTimer = Time.time + continuousMoveInterval;
    }

    // --- ROTACIÓN Y OTROS (Se mantienen con GetKeyDown) ---
    if (Input.GetKeyDown(KeyCode.UpArrow))
    {
        currentTetromino.transform.Rotate(0, 0, 90);
        if (!IsValidPosition())
        {
            currentTetromino.transform.Rotate(0, 0, -90);
        }
    }
    else if (Input.GetKeyDown(KeyCode.Space))
    {
        HardDrop();
    }

    // --- CAÍDA RÁPIDA ---
    if (Input.GetKey(KeyCode.DownArrow))
    {
        movementFrequency = 0.05f; // Un poco más rápido que antes
    }
    else
    {
        movementFrequency = baseFrequency;
    }
}
    void SpawnTetromino()
    {
        currentTetromino = Instantiate(Tetrominos[nextIndex], new Vector3(5, 18, 0), Quaternion.identity);

        if (nextTetrominoPreview != null)
        {
            Destroy(nextTetrominoPreview);
        }
        if (currentGhost != null) Destroy(currentGhost);

        nextIndex = Random.Range(0, Tetrominos.Length);

        ShowNextPiece();

        if (!IsValidPosition())
        {
            GameOver();
        }
    }

    void MoveTetromino(Vector3 direction)
    {
        currentTetromino.transform.position += direction;
        if (!IsValidPosition())
        {
            currentTetromino.transform.position -= direction;

            if (direction == Vector3.down)
            {
                LockTetromino();
            }
        }
    }

    void HardDrop()
    {
        while (IsValidPosition())
        {
            currentTetromino.transform.position += Vector3.down;
        }

        currentTetromino.transform.position -= Vector3.down;
        LockTetromino();
    }

    void LockTetromino()
    {
        GetComponent<GridScript>().UpdateGrid(currentTetromino.transform);
        CheckForLines();
        SpawnTetromino();
    }

    bool IsValidPosition()
    {
        return GetComponent<GridScript>().IsValidPosition(currentTetromino.transform);
    }

    void CheckForLines()
    {
        GetComponent<GridScript>().CheckForLines();
    }

    void GameOver()
{
    if (isGameOver) return;
    isGameOver = true;

    if (currentTetromino != null) Destroy(currentTetromino);

    if(gameOverText != null) gameOverText.SetActive(true);
    if(restartButton != null) restartButton.SetActive(true);

    ChangeAllBlocksToGray();
    Time.timeScale = 0f;
    this.enabled = false;
}
    void ShowNextPiece()
    {
        nextTetrominoPreview = Instantiate(Tetrominos[nextIndex], previewPoint.position, Quaternion.identity);
        
    }

public Material grayMaterial; 

void ChangeAllBlocksToGray()
{
    GridScript gridScript = GetComponent<GridScript>();
    
    for (int y = 0; y < gridScript.height; y++)
    {
        for (int x = 0; x < gridScript.width; x++)
        {
            if (gridScript.grid[x, y] != null)
            {
                SpriteRenderer sr = gridScript.grid[x, y].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.material = grayMaterial;
                    sr.color = Color.white; 
                }
            }
        }
    }
}
void HandleGarbageTimer()
{
    if (isGameOver) return;

    garbageTimer += Time.deltaTime;
    if (garbageTimer >= garbageInterval)
    {
        AddGarbageRow();
        garbageTimer = 0f;
    }
}

void AddGarbageRow()
{
    GridScript gridScript = GetComponent<GridScript>();

    // 1. Subir el grid (tu lógica actual)
    for (int y = gridScript.height - 1; y >= 0; y--)
    {
        for (int x = 0; x < gridScript.width; x++)
        {
            if (gridScript.grid[x, y] != null)
            {
                if (y + 1 >= gridScript.height) { GameOver(); return; }
                gridScript.grid[x, y].position += Vector3.up;
                gridScript.grid[x, y + 1] = gridScript.grid[x, y];
                gridScript.grid[x, y] = null;
            }
        }
    }

    // 2. Crear la fila de basura (tu lógica actual)
    int gapIndex = Random.Range(0, gridScript.width); 
    for (int x = 0; x < gridScript.width; x++)
    {
        if (x != gapIndex)
        {
            GameObject block = Instantiate(grayBlockPrefab, new Vector3(x, 0, 0), Quaternion.identity);
            gridScript.grid[x, 0] = block.transform;
        }
    }

    // --- SOLUCIÓN POR COLISIÓN ---
    // Si la pieza del jugador ahora está solapada con el nuevo suelo...
    if (currentTetromino != null && !IsValidPosition())
    {
        // Opción A: Intentar "desatascar" la pieza subiéndola solo si colisiona
        currentTetromino.transform.position += Vector3.up;
        
        // Si después de subir sigue colisionando (ej. contra el techo u otra pieza), es GameOver
        if (!IsValidPosition()) 
        {
            GameOver();
        }
    }
}

}
