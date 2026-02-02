using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridScript : MonoBehaviour
{
    public Transform[,] grid;
    public int width, height;
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;
    public GameObject combo1;
    public GameObject combo2;
    public GameObject combo3;
    public GameObject combo4;
    private int comboCount = 0;

    void Awake()
    {
        grid = new Transform[width, height];
    }

    public void UpdateGrid(Transform tetromino)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null && grid[x, y].parent == tetromino)
                {
                    grid[x, y] = null;
                }
            }
        }

        foreach (Transform mino in tetromino)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < height)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public static Vector2 Round(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    public bool IsInsideBorder(Vector2 pos)
    {
        return (int)pos.x >= 0 && (int)pos.x < width && (int)pos.y >= 0; 
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
{
    int x = (int)pos.x;
    int y = (int)pos.y;

    if (x < 0 || x >= width || y < 0 || y >= height)
    {
        return null;
    }

    return grid[x, y];
}

    public bool IsValidPosition(Transform tetromino)
{
    foreach (Transform mino in tetromino)
    {
        Vector2 pos = Round(mino.position);

        if (pos.x < 0 || pos.x >= width || pos.y < 0)
        {
            return false;
        }

        if (pos.y < height)
        {
            Transform t = GetTransformAtGridPosition(pos);
            if (t != null && t.parent != tetromino)
            {
                return false;
            }
        }
    }
    return true;
}

    public void CheckForLines()
    {
        int linesCleared = 0;

        for (int y = 0; y < height; y++)
        {
            if (LineIsFull(y))
            {
                DeleteLine(y);
                DecreaseRowsAbove(y + 1);
                y--;
                linesCleared++;
            }
        }

        if (linesCleared > 0)
        {
            comboCount++; 
            AddScore(linesCleared, comboCount); 
            ShowCombo(comboCount);
        }
        else
        {
            comboCount = 0;
            HideAllCombos();
        }
    }
    void AddScore(int lines, int combo)
    {
        int basePoints = 0;

        switch (lines)
        {
            case 1: basePoints = 100; break;
            case 2: basePoints = 300; break;
            case 3: basePoints = 500; break;
            case 4: basePoints = 800; break; 
        }

        currentScore += basePoints * combo;

        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore.ToString();
        }
    }


    bool LineIsFull(int y)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, y] == null) return false;
        }
        return true;
    }

    void DeleteLine(int y)
    {
        for (int x = 0; x < width; x++)
        {
            // Guardamos referencia al padre antes de destruir el hijo
            Transform parentObj = grid[x, y].parent;

            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;

            // Opcional: Si el Tetromino padre se queda sin hijos, lo borramos de la escena
            if (parentObj != null && parentObj.childCount <= 1) 
            {
                Destroy(parentObj.gameObject, 0.1f); 
            }
        }
    }

    void DecreaseRowsAbove(int startRow)
    {
        for (int y = startRow; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null)
                {
                    // Movemos la referencia en la matriz hacia abajo
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;

                    // Movemos el objeto visualmente
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }
    void ShowCombo(int currentCombo)
    {
        HideAllCombos(); // Limpiamos el texto anterior

        GameObject textToShow = null;

        // Decidimos qué objeto mostrar según el contador acumulado
        if (currentCombo == 1) textToShow = combo1;
        else if (currentCombo == 2) textToShow = combo2;
        else if (currentCombo == 3) textToShow = combo3;
        else if (currentCombo >= 4) textToShow = combo4;

        if (textToShow != null)
        {
            textToShow.SetActive(true);
            Animator anim = textToShow.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("ComboAppear", 0, 0f);
            }
        }
    }
    void HideAllCombos()
    {
        if (combo1) combo1.SetActive(false);
        if (combo2) combo2.SetActive(false);
        if (combo3) combo3.SetActive(false);
        if (combo4) combo4.SetActive(false);
    }
}