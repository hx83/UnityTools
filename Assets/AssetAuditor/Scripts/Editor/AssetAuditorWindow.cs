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
        private AssetAuditorWindow window;
        private AssetAuditorSetting setting;
        private AuditorInfo currentAuditor;
        //
        private List<string> nameList;

        private bool isDeleting = false;

        private bool isNameOK = true;
        private string boxTip = "";
        private string ruleName = "";

        [MenuItem("Taomee Tools/Asset Auditor")]
        private static void OpenWindow()
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }
        public static void DeleteRule(string path)
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.Delete(path);
        }

        private void Delete(string path)
        {
            isDeleting = true;
            this.BuildMenuTree();
            this.ForceMenuTreeRebuild();
            //????
            AssetDatabase.DeleteAsset(path);
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


            ruleName = EditorGUILayout.TextField(new GUIContent("Rule Name:"), ruleName, GUILayoutOptions.Height(20));

            GUI.color = Color.green;

            if(GUILayout.Button("+ New Rule", GUILayoutOptions.MaxWidth(100).Height(20)))
            {
                ruleName = ruleName.Trim();
                //Debug.Log("??" + tempName + "!");
                if(ruleName == "")
                {
                    isNameOK = false;
                    boxTip = "Rule name cannot be empty! 规则名称不能为空";
                }
                else if(nameList.Contains(ruleName))
                {
                    isNameOK = false;
                    boxTip = "Rule name has already exist! 规则名称已经存在";
                }
                else
                {
                    isNameOK = true;
                }

                if (isNameOK == true)
                {
                    Debug.Log("create new rule");
                    CreateNewRule(ruleName);
                    ruleName = "";
                }
            }
            GUI.color = Color.white;

            GUILayout.EndHorizontal();

            if (isNameOK == false && boxTip != "")
            {
                GUI.color = Color.red;
                GUILayout.Box(boxTip, GUILayoutOptions.ExpandWidth());
                GUI.color = Color.white;
            }


            GUILayout.Space(10);

            base.OnGUI();
            /*
            GUILayout.BeginHorizontal();
            GUILayout.Button("New Folders Rule",GUILayoutOptions.Height(100));
            GUILayout.Button("New Files Rule");
            GUILayout.EndHorizontal();
            */
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
            
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: false)
            { };
            //
            //
            if(isDeleting)
            {
                return tree;
            }
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
