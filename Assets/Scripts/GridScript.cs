using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridScript : MonoBehaviour
{
    public Transform[,] grid;
    public int width, height;

    void Awake()
    {
        grid = new Transform[width, height];
    }

    public void UpdateGrid(Transform tetromino)
    {
        // Limpiamos la posición anterior de este tetromino en la matriz
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

        // Registramos las nuevas posiciones de cada cuadradito individual (mino)
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
        // Nota: Permitimos que y sea mayor que el límite superior para el Spawn, 
        // pero validamos el suelo y los laterales.
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
        for (int y = 0; y < height; y++)
        {
            if (LineIsFull(y))
            {
                DeleteLine(y);
                DecreaseRowsAbove(y + 1);
                y--; // Re-revisar la misma fila ya que la de arriba bajó
            }
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
}