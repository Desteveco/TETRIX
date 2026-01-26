using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] Tetrominos;
    public float baseFrequency = 0.8f; 
    private float movementFrequency; 
    private float passedTime = 0;
    private GameObject currentTetromino;

    public GameObject gameOverText; 

    private bool isGameOver = false;
    [Header("Next Piece Settings")]
    public Transform previewPoint; 
    private int nextIndex;
    private GameObject nextTetrominoPreview;

    void Start()
    {
        movementFrequency = baseFrequency;
        gameOverText.SetActive(false);
        nextIndex = Random.Range(0, Tetrominos.Length);
        SpawnTetromino();
    }

    void Update()
    {
        if (isGameOver) return;

        UpdateSpeed();

        passedTime += Time.deltaTime;
        if (passedTime >= movementFrequency)
        {
            passedTime -= movementFrequency;
            MoveTetromino(Vector3.down);
        }
        UserInput();
    }
    void UpdateSpeed()
{
    int score = GetComponent<GridScript>().currentScore;
    
    float level = Mathf.Floor(score / 1000f);
    baseFrequency = Mathf.Max(0.8f - (level * 0.1f), 0.2f);
}

    void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveTetromino(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveTetromino(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
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

        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementFrequency = 0.1f;
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

        Debug.Log("Game Over! The stack reached the top.");

        Destroy(currentTetromino);

        gameOverText.SetActive(true);

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

}
