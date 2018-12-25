using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TaomeeTools.AssetAuditor
{

    public partial class RuleConfig : ScriptableObject
    {
        private static int[] TextureSizes = new int[] { 32, 64, 128, 256, 512, 1024, 2048 };
        private static TextureImporterFormat[] TextureFormatAndroid = new TextureImporterFormat[] {
                                                                                                    TextureImporterFormat.ETC2_RGB4,
                                                                                                    TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA,
                                                                                                    TextureImporterFormat.ETC2_RGBA8,
                                                                                                    TextureImporterFormat.ASTC_RGBA_4x4,
                                                                                                    TextureImporterFormat.ASTC_RGB_4x4,
                                                                                                    TextureImporterFormat.RGBA32,
                                                                                                    TextureImporterFormat.RGBA16,
                                                                                                    TextureImporterFormat.RGB24,
                                                                                                    TextureImporterFormat.RGB16,
                                                                                                    };

        private static TextureImporterFormat[] TextureFormatIOS = new TextureImporterFormat[] {
                                                                                                    TextureImporterFormat.PVRTC_RGB2,
                                                                                                    TextureImporterFormat.PVRTC_RGB4,
                                                                                                    TextureImporterFormat.PVRTC_RGBA2,
                                                                                                    TextureImporterFormat.PVRTC_RGBA4,
                                                                                                    TextureImporterFormat.ASTC_RGBA_4x4,
                                                                                                    TextureImporterFormat.ASTC_RGB_4x4,
                                                                                                    TextureImporterFormat.RGBA32,
                                                                                                    TextureImporterFormat.RGBA16,
                                                                                                    TextureImporterFormat.RGB24,
                                                                                                    TextureImporterFormat.RGB16,
                                                                                                    };



        //
        private bool showDeleteWarning;
        //
        [ShowIf("AssetType", AssetType.Texture)]
        public bool ReadWriteEnabled;
        [ShowIf("AssetType", AssetType.Texture)]
        public bool MipMaps;


        //


        [FoldoutGroup("Android Setting", true)]
        [ShowIf("AssetType", AssetType.Texture)]
        [ValueDropdown("TextureSizes")]
        [LabelText("Max Size")]
        public int TextureMaxSizeAndroid = 512;

        [FoldoutGroup("Android Setting")]
        [ShowIf("AssetType", AssetType.Texture)]
        [ValueDropdown("TextureFormatAndroid")]
        [LabelText("Compress Format")]
        public TextureImporterFormat compressAndroid = TextureImporterFormat.ETC2_RGBA8;




        [FoldoutGroup("IOS Setting", true)]
        [ShowIf("AssetType", AssetType.Texture)]
        [ValueDropdown("TextureSizes")]
        [LabelText("Max Size")]
        public int TextureMaxSizeIOS = 512;

        [FoldoutGroup("IOS Setting")]
        [ShowIf("AssetType", AssetType.Texture)]
        [ValueDropdown("TextureFormatIOS")]
        [LabelText("Compress Format")]
        public TextureImporterFormat compressIOS = TextureImporterFormat.PVRTC_RGBA4;

        //
        //

        [PropertySpace(10)]
        [InfoBox("删除规则将无法还原，确定要删除吗？", InfoMessageType.Error, "showDeleteWarning")]
        [Button("注意！",ButtonHeight = 20)]
        [ShowIf("showDeleteWarning")]
        private void Empty(){}


        [PropertySpace(20)]
        [Button(ButtonSizes.Large), GUIColor(1, 0, 0)]
        [HideIf("showDeleteWarning")]
        private void DeleteRule()
        {
            showDeleteWarning = true;
        }

        

        [PropertySpace(20)]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        [ShowIf("showDeleteWarning")]
        [HorizontalGroup("Split", 0.5f)]
        private void Cancel()
        {
            showDeleteWarning = false;
        }

        [PropertySpace(20)]
        [Button(ButtonSizes.Large), GUIColor(1, 0, 0)]
        [ShowIf("showDeleteWarning")]
        [VerticalGroup("Split/right")]
        private void Apply()
        {
            showDeleteWarning = false;
            string path = AssetDatabase.GetAssetPath(this);
            //Debug.Log(path);
            //AssetDatabase.DeleteAsset(path);
            AssetAuditorWindow.DeleteRule(path);
        }
        //
        /*
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("On Inspector GUI can also be used on both methods and properties", UnityEditor.MessageType.Info);
        }
        */
    }
}