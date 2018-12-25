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
using System.Timers;

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

        private bool isNameOK = true;
        private string boxTip = "";
        private string ruleName = "";

        private List<string> DeleteList;
        private List<RuleConfig> ruleConfigList;


        [MenuItem("Taomee Tools/Asset Auditor")]
        private static void OpenWindow()
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 650);
        }
        /// <summary>
        /// 删除一个规则配置
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteRule(string path)
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.Delete(path);
        }

        private void Delete(string path)
        {
            DeleteList.Add(path);
            
            this.BuildMenuTree();
            this.ForceMenuTreeRebuild();
            //
            this.OnEndGUI += DelayDelete;
        }
        
        private void DelayDelete()
        {
            this.OnEndGUI -= DelayDelete;
            
            foreach (var item in DeleteList)
            {
                AssetDatabase.DeleteAsset(item);
            }
            DeleteList.Clear();
        }
        

        public static void ReSort()
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.ReSortMenu();
        }
        private void ReSortMenu()
        {
            this.BuildMenuTree();
            this.ForceMenuTreeRebuild();
        }


        private void Init()
        {
            nameList = new List<string>();
            DeleteList = new List<string>();
            ruleConfigList = new List<RuleConfig>();
            //
            string[] list = AssetDatabase.FindAssets("t:AssetAuditorSetting");
            var path = AssetDatabase.GUIDToAssetPath(list[0]);
            setting = AssetDatabase.LoadAssetAtPath<AssetAuditorSetting>(path);
            currentAuditor = setting.infoFile;
            
        }
        protected override void OnGUI()
        {
            if (currentAuditor == null)
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
                    //Debug.Log("create new rule");
                    CreateNewRule(ruleName);
                    ruleName = "";
                    
                    this.BuildMenuTree();
                    this.ForceMenuTreeRebuild();
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
            
            GUILayout.BeginHorizontal();
            GUI.color = Color.cyan;
            if(GUILayout.Button("应用全部规则",GUILayoutOptions.Height(40)))
            {
                ExecuteAllRule();
            }
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            
            //
        }
        /// <summary>
        /// 执行所有规则
        /// </summary>
        private void ExecuteAllRule()
        {
            int n = ruleConfigList.Count;
            /*
            for (int i = 0; i < n; i++)
            {
                RuleConfig config = ruleConfigList[i];
                ExecuteRule.Execute(config);
            }
            */
            for (int i = n - 1; i >= 0; i--)
            {
                RuleConfig config = ruleConfigList[i];
                ExecuteRule.Execute(config);
            }
        }


        /// <summary>
        /// Creates the new rule asset file.
        /// </summary>
        /// <param name="str">Name.</param>
        private RuleConfig CreateNewRule(string str)
        {
            var tempPath = setting.configPath + "/" +
                          Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(currentAuditor)) + "Res";


            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            var asset = new RuleConfig();
            asset.RuleName = str;
            AssetDatabase.CreateAsset(asset, tempPath + "/" + str + ".asset");
            AssetDatabase.SaveAssets();
            return asset;
        }

        
        protected override OdinMenuTree BuildMenuTree()
        {
            nameList.Clear();
            ruleConfigList.Clear();

            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: false)
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
            //
            //
            //List<BaseRuleConfig> tempList = new List<BaseRuleConfig>();
            
            var currentRuleNum = files.Length;
            for (int i = 0;i < currentRuleNum; i++)
            {
                FileInfo info = files[i];
                if(info.Name.EndsWith(".asset"))
                {
                    var path = tempPath + "/" + info.Name;
                    if(DeleteList.Contains(path) == false)
                    {
                        //Debug.Log(path);

                        RuleConfig config = AssetDatabase.LoadAssetAtPath<RuleConfig>(path);
                        //Debug.Log(config);
                        var name = Path.GetFileNameWithoutExtension(config.RuleName);
                        nameList.Add(name);
                        //tree.Add(name, config);
                        ruleConfigList.Add(config);
                    }
                }
            }
            //
            ruleConfigList.Sort(SortList);
            for (int j = 0; j < ruleConfigList.Count; j++)
            {
                RuleConfig conf = ruleConfigList[j];
                tree.Add(conf.RuleName, conf);
            }

            return tree;
        }
        //
        //
        private int SortList(RuleConfig a, RuleConfig b)

        {

            if (a.Priority > b.Priority)

            {
                return -1;

            }

            else if (a.Priority < b.Priority)

            {

                return 1;

            }

            return 0;

        }
    }



}
