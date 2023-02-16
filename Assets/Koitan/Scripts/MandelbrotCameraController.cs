using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MandelbrotCameraController : MonoBehaviour
{
    [SerializeField] private Material mat;

    private Vector3 oldMousePos;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float scale = mat.GetFloat("_Scale");
        if (Input.mouseScrollDelta.y > 0)
        {
            scale *= 1.1f;
            mat.SetFloat("_Scale", scale);
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            scale /= 1.1f;
            mat.SetFloat("_Scale", scale);
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        if (Input.GetMouseButtonDown(0))
        {
            oldMousePos = mousePos;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 deltaMousePos = mousePos - oldMousePos;
            Vector2 offset = mat.GetVector("_Offset");
            mat.SetVector("_Offset", offset - deltaMousePos / scale);
        }
        oldMousePos = mousePos;
    }
}
