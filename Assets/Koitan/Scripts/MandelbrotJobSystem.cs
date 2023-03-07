using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using KoitanLib;
using System.IO;
using System;
using UnityEngine.UI;
using Unity.VisualScripting;

public class MandelbrotJobSystem : MonoBehaviour
{
    [SerializeField]
    RawImage rawImage;

    [SerializeField]
    private int size = 1024;

    int[] sizes = new int[] { 64, 128, 256, 512, 1024, 2048, 4096 };
    int sizeIndex = 3;
    TypeAccuracy typeAccuracy = TypeAccuracy.Float;

    int cnt = 1024;
    int hueScale = 256;
    int hueOffset = 0;

    [SerializeField] private Material mat;
    private decimal scale = 1m / 1.5m / 1.5m / 1.5m / 1.5m;
    private decimal offsetX;
    private decimal offsetY;
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
        UpdateTexture();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            sizeIndex = Mathf.Min(++sizeIndex, sizes.Length);
            size = sizes[sizeIndex];
            tex.Reinitialize(size, size, TextureFormat.ARGB32, false);
            UpdateTexture();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            sizeIndex = Mathf.Max(--sizeIndex, 0);
            size = sizes[sizeIndex];
            tex.Reinitialize(size, size, TextureFormat.ARGB32, false);
            UpdateTexture();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            typeAccuracy--;
            typeAccuracy = (TypeAccuracy)Mathf.Max((int)typeAccuracy, 0);
            UpdateTexture();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            typeAccuracy++;
            typeAccuracy = (TypeAccuracy)Mathf.Min((int)typeAccuracy, System.Enum.GetValues(typeof(TypeAccuracy)).Length - 1);
            UpdateTexture();
        }

        if (Input.GetKey(KeyCode.E))
        {
            cnt += Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 10 : 1;
            UpdateTexture();
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            cnt -= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 10 : 1;
            UpdateTexture();
        }

        if (Input.GetKey(KeyCode.D))
        {
            hueScale += Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 10 : 1;
            UpdateTexture();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            hueScale -= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 10 : 1;
            UpdateTexture();
        }

        if (Input.GetKey(KeyCode.C))
        {
            hueOffset += Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 10 : 1;
            UpdateTexture();
        }
        else if (Input.GetKey(KeyCode.Z))
        {
            hueOffset -= Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? 10 : 1;
            UpdateTexture();
        }

        if (Input.GetKeyDown(KeyCode.F12))
        {
#if UNITY_EDITOR
            var FilePath = Application.dataPath;//Editor上では普通にカレントディレクトリを確認            
#else
        var FilePath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');//EXEを実行したカレントディレクトリ (ショートカット等でカレントディレクトリが変わるのでこの方式で)
#endif
            if (!Directory.Exists($"{FilePath}/Screenshots"))
            {
                Directory.CreateDirectory($"{FilePath}/Screenshots");
            }
            //File.WriteAllBytes(FilePath + $"/{DateTime.Now.ToString().Replace('/', '_')}.png", tex.EncodeToPNG());
            File.WriteAllBytes($"{FilePath}/Screenshots/{offsetX}_{offsetY}_{scale}_{size}.png", tex.EncodeToPNG());
        }

        if (Input.GetMouseButtonDown(0))
        {
            prevPos = mousePos;
        }

        if (Input.GetMouseButton(0))
        {
            if (mousePos != prevPos)
            {
                //Debug.Log($"mousePos = {mousePos}");
                offsetX -= (decimal)(mousePos - prevPos).x / scale;
                offsetY -= (decimal)(mousePos - prevPos).y / scale;
                // 更新
                UpdateTexture();
            }

            prevPos = mousePos;
        }
        else
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                scale *= 1.5m;
                // 更新
                UpdateTexture();
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                scale /= 1.5m;
                // 更新
                UpdateTexture();
            }
        }

        KoitanDebug.Display($"offsetX =\n{offsetX}\n");
        KoitanDebug.Display($"offsetY =\n{offsetY}\n");
        KoitanDebug.Display($"scale =\n{scale}\n");
        KoitanDebug.Display($"size = {size}\n");
        KoitanDebug.Display($"typeAccuracy = {typeAccuracy}\n");
        KoitanDebug.Display($"cnt = {cnt}\n");
        KoitanDebug.Display($"hueScale = {hueScale}\n");
        KoitanDebug.Display($"hueOffset = {hueOffset}\n");
        KoitanDebug.Display($"↑キーで解像度を上げる\n");
        KoitanDebug.Display($"↓キーで解像度を下げる\n");
        KoitanDebug.Display($"→キーで計算精度を上げる\n");
        KoitanDebug.Display($"←キーで計算精度を下げる\n");
        KoitanDebug.Display($"QEキーで収束発散判定ループ回数変更\n");
        KoitanDebug.Display($"ADキーで色相スケール変更\n");
        KoitanDebug.Display($"ZCキーで色相オフセット変更\n");
        KoitanDebug.Display($"Shiftキーを押しながらで10倍速\n");
        KoitanDebug.Display($"F12キーで撮影\n");
    }

    void UpdateTexture()
    {
        CreateTextureByJob(size, out colors);
        tex.SetPixels32(colors);
        tex.Apply();
        rawImage.texture = tex;
        mat.mainTexture = tex;
    }

    private void CreateTextureByJob(int size, out Color32[] colors)
    {
        switch (typeAccuracy)
        {
            case TypeAccuracy.Float:
                {
                    var job = new TextureJobFloat()
                    {
                        Size = size,
                        Scale = (float)scale,
                        OffsetX = (float)offsetX,
                        OffsetY = (float)offsetY,
                        Count = cnt,
                        HueScale = hueScale,
                        HueOffset = hueOffset,
                        Pixels = new NativeArray<Color32>(size * size, Allocator.TempJob),
                    };
                    var handle = job.Schedule(size * size, 1);
                    handle.Complete();
                    colors = job.Pixels.ToArray();

                    job.Pixels.Dispose();
                }
                break;
            case TypeAccuracy.Double:
                {
                    var job = new TextureJobDouble()
                    {
                        Size = size,
                        Scale = (double)scale,
                        OffsetX = (double)offsetX,
                        OffsetY = (double)offsetY,
                        Count = cnt,
                        HueScale = hueScale,
                        HueOffset = hueOffset,
                        Pixels = new NativeArray<Color32>(size * size, Allocator.TempJob),
                    };
                    var handle = job.Schedule(size * size, 1);
                    handle.Complete();
                    colors = job.Pixels.ToArray();

                    job.Pixels.Dispose();
                }
                break;
            case TypeAccuracy.Decimal:
                {
                    var job = new TextureJobDecimal()
                    {
                        Size = size,
                        Scale = scale,
                        OffsetX = offsetX,
                        OffsetY = offsetY,
                        Count = cnt,
                        HueScale = hueScale,
                        HueOffset = hueOffset,
                        Pixels = new NativeArray<Color32>(size * size, Allocator.TempJob),
                    };
                    var handle = job.Schedule(size * size, 1);
                    handle.Complete();
                    colors = job.Pixels.ToArray();

                    job.Pixels.Dispose();
                }
                break;
            default:
                {
                    var job = new TextureJobFloat()
                    {
                        Size = size,
                        Scale = (float)scale,
                        OffsetX = (float)offsetX,
                        OffsetY = (float)offsetY,
                        Count = cnt,
                        HueScale = hueScale,
                        HueOffset = hueOffset,
                        Pixels = new NativeArray<Color32>(size * size, Allocator.TempJob),
                    };
                    var handle = job.Schedule(size * size, 1);
                    handle.Complete();
                    colors = job.Pixels.ToArray();

                    job.Pixels.Dispose();
                }
                break;
        }
    }

    [BurstCompile]
    public struct TextureJobFloat : IJobParallelFor
    {
        [ReadOnly]
        public int Size;

        [ReadOnly] public float Scale;
        [ReadOnly] public float OffsetX;
        [ReadOnly] public float OffsetY;
        [ReadOnly] public int Count;
        [ReadOnly] public int HueScale;
        [ReadOnly] public int HueOffset;


        [WriteOnly]
        public NativeArray<Color32> Pixels;

        public void Execute(int index)
        {
            float cx = (index % Size / (float)Size - 0.5f) / Scale + OffsetX;
            float cy = (index / Size / (float)Size - 0.5f) / Scale + OffsetY;
            float x = cx;
            float y = cy;
            for (int i = 0; i < Count; i++)
            {
                float _x = x * x - y * y + cx;
                float _y = 2 * x * y + cy;
                x = _x;
                y = _y;
                if (x * x + y * y > 16)
                {
                    Pixels[index] = Color.HSVToRGB((i + HueOffset) / (float)HueScale % 1.0f, 1f, 1f);
                    return;
                }
            }
            var v = (byte)0;
            Pixels[index] = new Color32(v, v, v, 255);
        }
    }

    [BurstCompile]
    public struct TextureJobDouble : IJobParallelFor
    {
        [ReadOnly]
        public int Size;

        [ReadOnly] public double Scale;
        [ReadOnly] public double OffsetX;
        [ReadOnly] public double OffsetY;
        [ReadOnly] public int Count;
        [ReadOnly] public int HueScale;
        [ReadOnly] public int HueOffset;

        [WriteOnly]
        public NativeArray<Color32> Pixels;

        public void Execute(int index)
        {
            double cx = (index % Size / (double)Size - 0.5d) / Scale + OffsetX;
            double cy = (index / Size / (double)Size - 0.5d) / Scale + OffsetY;
            double x = cx;
            double y = cy;
            for (int i = 0; i < Count; i++)
            {
                double _x = x * x - y * y + cx;
                double _y = 2 * x * y + cy;
                x = _x;
                y = _y;
                if (x * x + y * y > 16)
                {
                    Pixels[index] = Color.HSVToRGB((i + HueOffset) / (float)HueScale % 1.0f, 1f, 1f);
                    return;
                }
            }
            var v = (byte)0;
            Pixels[index] = new Color32(v, v, v, 255);
        }
    }

    [BurstCompile]
    public struct TextureJobDecimal : IJobParallelFor
    {
        [ReadOnly]
        public int Size;

        [ReadOnly] public decimal Scale;
        [ReadOnly] public decimal OffsetX;
        [ReadOnly] public decimal OffsetY;
        [ReadOnly] public int Count;
        [ReadOnly] public int HueScale;
        [ReadOnly] public int HueOffset;


        [WriteOnly]
        public NativeArray<Color32> Pixels;

        public void Execute(int index)
        {
            decimal cx = (index % Size / (decimal)Size - 0.5m) / Scale + OffsetX;
            decimal cy = (index / Size / (decimal)Size - 0.5m) / Scale + OffsetY;
            decimal x = cx;
            decimal y = cy;
            for (int i = 0; i < Count; i++)
            {
                decimal _x = x * x - y * y + cx;
                decimal _y = 2 * x * y + cy;
                x = _x;
                y = _y;
                if (x * x + y * y > 16)
                {
                    Pixels[index] = Color.HSVToRGB((i + HueOffset) / (float)HueScale % 1.0f, 1f, 1f);
                    return;
                }
            }
            var v = (byte)0;
            Pixels[index] = new Color32(v, v, v, 255);
        }
    }

    enum TypeAccuracy
    {
        Float = 0,
        Double,
        Decimal,
    }
}
