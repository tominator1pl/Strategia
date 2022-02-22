using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Utils
{

    static Texture2D _whiteTexture;

    public static bool isSelecting = false;
    public static Vector3 firstMousePosition;

    public static Vector3 selectionTarget = Vector3.zero;
    public static bool targetChanged = false;

    public static int heartHealth;
    public static int allyUnits;
    public static int selectedUnits;

    public static bool lost = false;
    public static double timeSurvived;

    public static bool paused = false;
    public static Texture2D WhiteTexture
    {
        get
        {
            if(_whiteTexture == null)
            {
                _whiteTexture = new Texture2D(1, 1);
                _whiteTexture.SetPixel(0, 0, Color.white);
                _whiteTexture.Apply();
            }
            return _whiteTexture;
        }
    }

    public static void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        Utils.DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        Utils.DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;

        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static Rect GetViewportRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {

        return new Rect(screenPosition1, screenPosition2 - screenPosition1);
    }

    public static Vector2[] GetRectCorners(Rect rect)
    {
        var topLeft = rect.position;
        var topRight = new Vector2(rect.position.x + rect.width, rect.position.y);
        var bottomLeft = new Vector2(rect.position.x, rect.position.y + rect.height);
        var bottomRight = rect.position + rect.size;
        Vector2[] corners = { topLeft, topRight, bottomLeft, bottomRight };
        
        return corners;
    }

    public static Vector3[] GetScreenToWorldCorners(Vector2[] screenCorners)
    {
        RaycastHit hit;
        Vector3[] corners = new Vector3[4];

        for(int i = 0; i < screenCorners.Length; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenCorners[i]);

            if (Physics.Raycast(ray, out hit))
            {
                corners[i] = hit.point;
            }
        }
        return corners;
        
    }

    public static bool IsPointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
    {
        Vector2 d, e;
        double w1, w2;
        d = b - a;
        e = c - a;

        if (Mathf.Approximately(e.y, 0))
        {
            e.y = 0.0001f;
        }

        w1 = (e.x * (a.y - p.y) + e.y * (p.x - a.x)) / (d.x * e.y - d.y * e.x);
        w2 = (p.y - a.y - w1 * d.y) / e.y;
        return (w1 >= 0f) && (w2 >= 0.0) && ((w1 + w2) <= 1.0);
    }

    public static bool IsPointInWorldCorners(Vector2[] worldCorners, Vector2 point)
    {
        if(IsPointInTriangle(worldCorners[0], worldCorners[1], worldCorners[2], point) || IsPointInTriangle(worldCorners[1], worldCorners[2], worldCorners[3], point))
        {
            return true;
        }
        return false;
    }

    public static bool IsWorldPointInScreenRect(Rect rect, Vector2 point)
    {
        Vector2[] rectCorners = GetRectCorners(rect);
        Vector3[] worldCorners = GetScreenToWorldCorners(rectCorners);
        return IsPointInWorldCorners(worldCorners.toVector2Array(), point);
    } 

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1,Vector3 screenPosition2)
    {
        var v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        var v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    public static Vector2[] toVector2Array(this Vector3[] v3)
    {
        return System.Array.ConvertAll<Vector3, Vector2>(v3, getV2fromV3);
    }

    public static Vector2 getV2fromV3(Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    public static Vector3[] toVector3Array(this Vector2[] v2)
    {
        return System.Array.ConvertAll<Vector2, Vector3>(v2, getV3fromV2);
    }

    public static Vector3 getV3fromV2(Vector2 v2)
    {
        return new Vector3(v2.x, 0f, v2.y);
    }

    public static Vector2[] GenerateSquareGrid(int number, float scale, out Rect rect)
    {
        //scale = scale*2f;
        var sideXF = Mathf.Sqrt(number);
        int sideX = Mathf.CeilToInt(sideXF);
        float sideYF = (float)number / (float)sideX;
        int sideY = Mathf.CeilToInt(sideYF);

        float centreX = (((float)sideX / 2f) * scale) + (scale / 2);
        float centreY = (((float)sideY / 2f) * scale) + (scale / 2);

        Vector2[] grid = new Vector2[sideX*sideY];
        int i = 0;
        for(int y = 1; y <= sideY; y++)
        {
            for (int x = 1; x <= sideX; x++)
            {
                grid[i] = new Vector2((x * scale) - centreX, (y * scale) - centreY);
                i++;
            }
        }
        rect = Rect.MinMaxRect(grid[0].x, grid[0].y, grid[grid.Length-1].x, grid[grid.Length-1].y);
        return grid;
    }

}
