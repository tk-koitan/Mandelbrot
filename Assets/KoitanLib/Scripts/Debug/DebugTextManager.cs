using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System.Linq;

namespace KoitanLib
{
    public class DebugTextManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject debugCanvas;
        [SerializeField]
        private TextMeshProUGUI textMesh;
        private StringBuilder sb = new StringBuilder(1024);
        [SerializeField]
        private TextMeshProUGUI textBoxOrigin;
        private Dictionary<int, TextBox> boxDic = new Dictionary<int, TextBox>();
        TextBox a = new TextBox();

        private void Awake()
        {
            DontDestroyOnLoad(debugCanvas);
        }

        private void LateUpdate()
        {
            //標準
            textMesh.SetText(sb);
            sb.Clear();

            //ボックス
            //更新されてないのを消す
            var removeTargetLst = boxDic.Where(kv => kv.Value.aliveFrag == false).ToList();
            foreach (var item in removeTargetLst)
            {
                Destroy(item.Value.textMesh.gameObject);
                boxDic.Remove(item.Key);
            }
            //更新
            foreach (TextBox box in boxDic.Values)
            {
                if (box.aliveFrag)
                {
                    box.textMesh.SetText(box.sb);
                    box.sb.Clear();
                    box.aliveFrag = false;
                }
                else
                {
                }
            }
        }

        public void Display(string str)
        {
            sb.Append(str);
        }

        public void DisplayBox(string str, MonoBehaviour mono)
        {
            int id = mono.GetInstanceID();
            TextBox textBox;
            if (boxDic.ContainsKey(id))
            {
                textBox = boxDic[id];
            }
            else
            {
                textBox = new TextBox();
                textBox.textMesh = Instantiate(textBoxOrigin, debugCanvas.transform);

                boxDic.Add(id, textBox);
            }
            textBox.sb.Append(str);
            textBox.aliveFrag = true;
            textBox.textMesh.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, mono.transform.position);
        }

        class TextBox
        {
            public bool aliveFrag = false;
            public TextMeshProUGUI textMesh;
            public StringBuilder sb = new StringBuilder(1024);
        }
    }
}
