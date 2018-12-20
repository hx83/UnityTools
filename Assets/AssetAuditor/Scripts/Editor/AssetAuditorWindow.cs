using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector.Editor;
using System.Linq;
using Sirenix.Utilities.Editor;
using Sirenix.Serialization;
using UnityEditor;
using Sirenix.Utilities;


using Sirenix.OdinInspector;

namespace TaomeeTools.AssetAuditor
{
    public class AssetAuditorWindow : OdinMenuEditorWindow
    {
        //private string SettingPath = ""
        private AuditorInfo currentAuditor;

        [MenuItem("Taomee Tools/Asset Auditor")]
        private static void OpenWindow()
        {
            var window = GetWindow<AssetAuditorWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
            window.Init();
        }
        public void Init()
        {
            //AssetDatabase
            /*
            currentAuditor = AssetDatabase.LoadAssetAtPath<AuditorInfo>("Assets/AssetAuditor/Source/AuditorInfo.asset");
            currentAuditor.AddRule(new TextureRule());
            */
        }
        protected override void OnGUI()
        {
            base.OnGUI();
            
            GUILayout.BeginHorizontal();
            GUILayout.Button("New Folders Rule",GUILayoutOptions.Height(300));
            GUILayout.Button("New Files Rule");
            GUILayout.EndHorizontal();
        }
        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true)
            {
                { "Home",                           this,                           EditorIcons.House                       }, // Draws the this.someData field in this case.
                { "Odin Settings",                  null,                           EditorIcons.SettingsCog                 },
                { "Odin Settings/Color Palettes",   ColorPaletteManager.Instance,   EditorIcons.EyeDropper                  },
                { "Odin Settings/AOT Generation",   AOTGenerationConfig.Instance,   EditorIcons.SmartPhone                  },
                { "Player Settings",                Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault()       },
                { "Some Class",                     null                                                           }
            };

            tree.AddAllAssetsAtPath("Odin Settings/More Odin Settings", "Plugins/Sirenix", typeof(ScriptableObject), true)
                .AddThumbnailIcons();

            tree.AddAssetAtPath("Odin Getting Started", "Plugins/Sirenix/Getting Started With Odin.asset");

            tree.MenuItems.Insert(2, new OdinMenuItem(tree, "Menu Style", tree.DefaultMenuStyle));

            tree.Add("Menu/Items/Are/Created/As/Needed", new GUIContent());
            tree.Add("Menu/Items/Are/Created", new GUIContent("And can be overridden"));

            tree.SortMenuItemsByName();

            // As you can see, Odin provides a few ways to quickly add editors / objects to your menu tree.
            // The API also gives you full control over the selection, etc..
            // Make sure to check out the API Documentation for OdinMenuEditorWindow, OdinMenuTree and OdinMenuItem for more information on what you can do!

            return tree;
        }
    }



}
