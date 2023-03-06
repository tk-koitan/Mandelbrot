using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

public class FixedPointCalculator : MonoBehaviour
{
    [SerializeField] private long a = 0;
    [SerializeField] private long b = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        KoitanDebug.Display($"a___=_0x{Convert.ToString(a, 16).PadLeft(8, '0')}\n");
        KoitanDebug.Display($"b___=_0x{Convert.ToString(b, 16).PadLeft(8, '0')}\n");
        KoitanDebug.Display($"a*b_=_0x{Convert.ToString(a * b, 16).PadLeft(8, '0')}\n");
    }
}
