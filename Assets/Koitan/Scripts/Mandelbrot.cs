using UnityEngine;

public class Mandelbrot : MonoBehaviour
{
    public int maxIterations = 100;
    public float zoom = 1;
    public Vector2 offset = Vector2.zero;

    private Texture2D texture;
    private Material material;

    void Start()
    {
        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        GetComponent<Renderer>().material.mainTexture = texture;
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        for (int x = 0; x < Screen.width; x++)
        {
            for (int y = 0; y < Screen.height; y++)
            {
                float a = (float)x / Screen.width * 4f / zoom - 2f / zoom + offset.x;
                float b = (float)y / Screen.height * 4f / zoom - 2f / zoom + offset.y;
                float ca = a;
                float cb = b;
                int n = 0;
                while (n < maxIterations)
                {
                    float aa = a * a - b * b;
                    float bb = 2f * a * b;
                    a = aa + ca;
                    b = bb + cb;
                    if (a * a + b * b > 4)
                    {
                        break;
                    }
                    n++;
                }
                Color color = Color.black;
                if (n == maxIterations)
                {
                    color = Color.white;
                }
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        material.mainTexture = texture;
    }
}
