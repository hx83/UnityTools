﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using Sirenix.Utilities.Editor;
using Sirenix.Serialization;
using UnityEditor;
using Sirenix.Utilities;
using System.IO;


using Sirenix.OdinInspector;

namespace TaomeeTools.AssetAuditor
{
    public class AssetAuditorWindow : OdinMenuEditorWindow
    {
        private AssetAuditorSetting setting;
        private AuditorInfo currentAuditor;
        //
        private List<string> nameList;

        [MenuItem("Taomee Tools/Asset Auditor")]
        private static void OpenWindow()
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }
        private void Init()
        {
            nameList = new List<string>();
            //
            string[] list = AssetDatabase.FindAssets("t:AssetAuditorSetting");
            var path = AssetDatabase.GUIDToAssetPath(list[0]);
            setting = AssetDatabase.LoadAssetAtPath<AssetAuditorSetting>(path);
            currentAuditor = setting.infoFile;

            /*
            int n = currentAuditor.ConfigList.Count;
            Debug.Log("config num:" + n);

            if(n == 0)
            {
                Debug.Log("create new");
                var asset = CreateNewRule("rule");

                currentAuditor.AddRule(asset);
                AssetDatabase.SaveAssets();
            }
            */
        }
        protected override void OnGUI()
        {
            if(currentAuditor == null)
            {
                this.Init();
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            string ruleName = EditorGUILayout.TextField(new GUIContent("Rule Name:"), "", GUILayoutOptions.Height(20));
            GUI.color = Color.green;
            if(GUILayout.Button("+ New Rule", GUILayoutOptions.MaxWidth(100).Height(20)))
            {
                Debug.Log("??" + ruleName + "!");
                
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();
            if (ruleName.Trim() == "")
            {
                Debug.Log("errr");
                GUILayout.Box("Rule Name is Empty");
            }
            GUILayout.Space(10);

            base.OnGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Button("New Folders Rule",GUILayoutOptions.Height(100));
            GUILayout.Button("New Files Rule");
            GUILayout.EndHorizontal();
            //
        }

        /// <summary>
        /// Creates the new rule asset file.
        /// </summary>
        /// <param name="str">Name.</param>
        private BaseRuleConfig CreateNewRule(string str)
        {
            var tempPath = setting.configPath + "/" +
                          Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(currentAuditor)) + "Res";


            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            var asset = new TextureRule();
            AssetDatabase.CreateAsset(asset, tempPath + "/" + str + ".asset");
            AssetDatabase.SaveAssets();
            return asset;
        }








        protected override OdinMenuTree BuildMenuTree()
        {

            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            { };
            //
            //
            var tempPath = setting.configPath + "/" +
                          Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(currentAuditor)) + "Res";

            if (!Directory.Exists(tempPath))
            {
                return tree;
            }
            DirectoryInfo direction = new DirectoryInfo(tempPath);  

            FileInfo[] files = direction.GetFiles("*",SearchOption.TopDirectoryOnly);
            var currentRuleNum = files.Length;
            for (int i = 0;i < currentRuleNum; i++)
            {
                FileInfo info = files[i];
                if(info.Name.EndsWith(".asset"))
                {
                    BaseRuleConfig config = AssetDatabase.LoadAssetAtPath<BaseRuleConfig>(tempPath + "/" + info.Name);
                    var name = Path.GetFileNameWithoutExtension(info.Name);
                    tree.Add(name, config);
                    nameList.Add(name);
                }
            } 
            
            /*

            tree.Add("Menu/Items/Are/Created/As/Needed", new GUIContent());
            //tree.Add("Menu/Items/Are/Created", new GUIContent("And can be overridden"));

            tree.SortMenuItemsByName();
            */

            return tree;
        }
    }



}
