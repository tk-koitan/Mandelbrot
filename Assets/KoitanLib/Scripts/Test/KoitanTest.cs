using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KoitanLib;

public class KoitanTest : MonoBehaviour
{
    public int n = 2;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        KoitanDebug.Display("デバッグ表示です\n");
        KoitanDebug.Display((1f / Time.unscaledDeltaTime).ToString() + "\n");
        string s = "pos : " + transform.position.ToString() + "\nrot : " + transform.rotation.eulerAngles.ToString() + "\nscale : " + transform.lossyScale.ToString();
        KoitanDebug.DisplayBox(s, this);
    }
}
