using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace psdmap
{
    public class MapSetWindow : EditorWindow
    {
        private string text;
        void OnGUI()
        {
            GUILayout.Space(10);
            //输入框控件
            text = EditorGUILayout.TextField("导入的地图ID：", text);

            GUILayout.Space(10);

            if (GUILayout.Button("导入配置表"))
            {
                int id = System.Convert.ToInt32(text);
                if (id == 0)
                {
                    Debug.LogError("地图ID输入有错误！");
                }
                else
                {
                    MapPSDImporter.ImportHogSceneMenuItem(id);
                    this.Close();
                }
            }



        }

    }
}
