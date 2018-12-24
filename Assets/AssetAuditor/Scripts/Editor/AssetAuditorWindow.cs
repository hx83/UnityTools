using System.Collections;
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

        [MenuItem("Taomee Tools/Asset Auditor")]
        private static void OpenWindow()
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }
        private void Init()
        {
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
            base.OnGUI();

            GUILayout.BeginHorizontal();
            GUILayout.Button("New Folders Rule",GUILayoutOptions.Height(300));
            GUILayout.Button("New Files Rule");
            GUILayout.EndHorizontal();

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
  
            for(int i=0;i<files.Length;i++)
            {
                FileInfo info = files[i];
                if(info.Name.EndsWith(".asset"))
                {
                    BaseRuleConfig config = AssetDatabase.LoadAssetAtPath<BaseRuleConfig>(tempPath + "/" + info.Name);
                    tree.Add(Path.GetFileNameWithoutExtension(info.Name), config);
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
