using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotSetGenerater : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshRenderer;
    Texture2D tex;
    [SerializeField]
    int width = 16, height = 16;
    Color[] colors;
    [SerializeField]
    Vector2 offset;
    [SerializeField]
    float scale;

    private void Awake()
    {
        tex = new Texture2D(width, height);
        tex.filterMode = FilterMode.Point;
        colors = new Color[width * height];

        meshRenderer.material.SetTexture("_MainTex", tex);
    }

    // Update is called once per frame
    void Update()
    {
        for (int j = 0; j < height; j++)
        {
            float y = ((float)j / height) * scale + offset.y;
            for (int i = 0; i < width; i++)
            {
                float x = ((float)i / width) * scale + offset.x;
                colors[j * width + i] = GetMandelbrotColor(x, y);
            }
        }
        tex.SetPixels(colors);
        tex.Apply();
    }

    Color GetMandelbrotColor(float x, float y)
    {
        float a = x;
        float b = y;
        for (int i = 0; i < 50; i++)
        {
            float _a = a * a - b * b + x;
            float _b = 2 * a * b + y;
            a = _a;
            b = _b;
            if (a * a + b * b > 4)
            {
                return Color.white;
            }
        }
        return Color.black;
    }
}
