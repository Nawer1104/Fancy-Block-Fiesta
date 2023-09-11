using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TetrisBlock : MonoBehaviour
{
    public Vector3 rotationPoint;
    private float previousTime;
    public float fallTime;
    public static int height = 20;
    public static int width = 10;
    public GameObject vfx;

    private static Transform[,] grid = new Transform[width, height];

    private void OnEnable()
    {
        Left.Instance.OnClickLeft += LeftTap_OnClickLeft;
        Right.Instance.OnClickRight += RightTap_OnClickRight;
        Mid.Instance.OnClickMid += Instance_OnClickMid;
    }

    private void Instance_OnClickMid(object sender, System.EventArgs e)
    {
        transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), 90);
        if (!ValidMove())
        {
            transform.RotateAround(transform.TransformPoint(rotationPoint), new Vector3(0, 0, 1), -90);
        }
    }

    private void RightTap_OnClickRight(object sender, System.EventArgs e)
    {
        transform.position += new Vector3(1, 0, 0);
        if (!ValidMove())
        {
            transform.position -= new Vector3(1, 0, 0);
        }
    }

    private void LeftTap_OnClickLeft(object sender, System.EventArgs e)
    {
        transform.position += new Vector3(-1, 0, 0);
        if (!ValidMove())
        {
            transform.position -= new Vector3(-1, 0, 0);
        }
    }

    private void OnDisable()
    {
        Left.Instance.OnClickLeft -= LeftTap_OnClickLeft;
        Right.Instance.OnClickRight -= RightTap_OnClickRight;
        Mid.Instance.OnClickMid -= Instance_OnClickMid;
    }

    private void Update()
    {
        if (Time.time - previousTime > fallTime)
        {
            transform.position += new Vector3(0, -1, 0);
            if (!ValidMove())
            {
                transform.position -= new Vector3(0, -1, 0);
                AddToGrid();
                /*                while(!IsRowDownAllBlocks())
                                {
                                    RowDown();
                                }*/
                CheckForLine();
                this.enabled = false;
                Spawn.Instance.NewTetromino();
            }
            previousTime = Time.time;
        }
        CheckGameOver();
    }

    void CheckForLine()
    {
        for (int i = height - 1; i >= 0; i--)
        {
            if (HasLine(i))
            {
                DeleteLine(i);
                RowDown(i);
            }
        }
    }

    bool HasLine(int i)
    {
        for (int j = 0; j < width; j++ )
        {
            if (grid[j, i] == null)
            {
                return false;
            }
        }
        return true;
    }

    void DeleteLine(int i)
    {
        for (int j = 0; j < width; j++)
        {
            Destroy(grid[j, i].gameObject);
            GameObject vfxExplosion = Instantiate(vfx, grid[j, i].gameObject.transform.position, grid[j, i].gameObject.transform.rotation);
            Destroy(vfxExplosion, 1f);
            grid[j, i] = null;
        }
    }

    private void RowDown(int i)
    {
        for (int y = i; y < height; y++)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, y] != null) {

                    grid[j, y - 1] = grid[j, y];
                    grid[j, y] = null;
                    grid[j, y - 1].transform.position -= new Vector3(0, 1, 0);
                }
            }
        }
    }

    private bool IsRowDownAllBlocks()
    {
        bool isAllRow = true;

        for (int i = height - 1; i >= 1; i--)
        {
            for (int j = 0; j < width; j++)
            {
                if (grid[j, i] != null)
                {
                    if (grid[j, i - 1] == null)
                    {
                        isAllRow = false;
                    }
                }
            }
        }
        return isAllRow;
    }

    private void AddToGrid()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            grid[roundedX, roundedY] = children;
        }
    }

    private bool ValidMove()
    {
        foreach (Transform children in transform)
        {
            int roundedX = Mathf.RoundToInt(children.transform.position.x);
            int roundedY = Mathf.RoundToInt(children.transform.position.y);

            if (roundedX < 0 || roundedX >= width || roundedY < 0 || roundedY >= height)
            {
                return false;
            }

            if (grid[roundedX, roundedY] != null)
            {
                return false;
            } 
        }

        return true;
    }

    private void CheckGameOver()
    {
        for (int i = 0; i < width; i++)
        {
            if (grid[i, 17] != null) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
            
    }
}
