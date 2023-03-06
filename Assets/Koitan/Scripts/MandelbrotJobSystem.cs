using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class MandelbrotJobSystem : MonoBehaviour
{
    [SerializeField]
    private int size = 1024;

    [SerializeField] private Material mat;
    private double scale = 1d;
    private double offsetX;
    private double offsetY;
    private Color32[] colors;

    private Texture2D tex;

    private Vector3 mousePos;

    private Vector3 prevPos;
    // Start is called before the first frame update
    void Start()
    {
        colors = new Color32[size * size];
        tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
        tex.filterMode = FilterMode.Point;
        CreateTextureByJob(size, out colors);
        tex.SetPixels32(colors);
        tex.Apply();
        mat.mainTexture = tex;
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        if (Input.GetMouseButtonDown(0))
        {
            prevPos = mousePos;
        }

        if (Input.GetMouseButton(0))
        {
            if (mousePos != prevPos)
            {
                //Debug.Log($"mousePos = {mousePos}");
                offsetX -= (double)(mousePos - prevPos).x / scale;
                offsetY -= (double)(mousePos - prevPos).y / scale;
                // 更新
                CreateTextureByJob(size, out colors);
                tex.SetPixels32(colors);
                tex.Apply();
                mat.mainTexture = tex;
            }

            prevPos = mousePos;
        }
        else
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                scale *= 1.5d;
                // 更新
                CreateTextureByJob(size, out colors);
                tex.SetPixels32(colors);
                tex.Apply();
                mat.mainTexture = tex;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                scale /= 1.5d;
                // 更新
                CreateTextureByJob(size, out colors);
                tex.SetPixels32(colors);
                tex.Apply();
                mat.mainTexture = tex;
            }
        }
    }

    private void CreateTextureByJob(int size, out Color32[] colors)
    {

        var job = new TextureJob()
        {
            Size = size,
            Scale = (double)scale,
            OffsetX = (double)offsetX,
            OffsetY = (double)offsetY,
            Pixels = new NativeArray<Color32>(size * size, Allocator.TempJob),
        };
        var handle = job.Schedule(size * size, 1);
        handle.Complete();
        colors = job.Pixels.ToArray();

        job.Pixels.Dispose();
    }

    [BurstCompile]
    public struct TextureJob : IJobParallelFor
    {
        [ReadOnly]
        public int Size;

        [ReadOnly] public double Scale;
        [ReadOnly] public double OffsetX;
        [ReadOnly] public double OffsetY;

        [WriteOnly]
        public NativeArray<Color32> Pixels;

        public void Execute(int index)
        {
            double cx = (index % Size / (double)Size - 0.5d) / Scale + OffsetX;
            double cy = (index / Size / (double)Size - 0.5d) / Scale + OffsetY;
            double x = cx;
            double y = cy;
            for (int i = 0; i < 1000; i++)
            {
                double _x = x * x - y * y + cx;
                double _y = 2 * x * y + cy;
                x = _x;
                y = _y;
                if (x * x + y * y > 16)
                {
                    Pixels[index] = Color.HSVToRGB((i / 256f) % 1.0f, 1f, 1f);
                    return;
                }
            }
            var v = (byte)0;
            Pixels[index] = new Color32(v, v, v, 255);
        }
    }
}
