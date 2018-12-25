using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace TaomeeTools.AssetAuditor
{
    [System.Serializable]
    public partial class RuleConfig : ScriptableObject
    {
        [ReadOnly]
        public string RuleName;

        [Title("Priority")]
        [InfoBox("如果在不同规则中存在相同的重复资源，则以优先级高的规则设置为准！")]
        [InlineButton("SubPriority", " - ")]
        [InlineButton("AddPriority", " + ")]
        public int Priority = 0;

        /// <summary>
        /// 增加优先级
        /// </summary>
        private void AddPriority()
        {
            Priority++;

            AssetAuditorWindow.ReSort();
        }
        /// <summary>
        /// 减小优先级
        /// </summary>
        private void SubPriority()
        {
            Priority--;

            AssetAuditorWindow.ReSort();
        }


        
        //
        //
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
        [PropertySpace(20)]
        [Title("Rule Type:")]
        [InfoBox("    选择一个规则类型\n    Folder : 针对文件夹设置\n    FileList : 针对特定文件设置")]
        [EnumPaging]
        public RuleType ruleType = RuleType.Folder;
        //
        [PropertySpace(20)]
        [Title("File List: (5 per page)")]

        [ShowIf("ruleType", RuleType.FileList)]
        [ListDrawerSettings(NumberOfItemsPerPage = 5, Expanded = true)]
        public Texture[] FileList;

        //
        //
        [PropertySpace(20)]
        [Title("Folder Path:")]

        [ShowIf("ruleType", RuleType.Folder)]
        [FolderPath]
        public string FoldPath;


        [PropertySpace(20)]
        [Title("Rule Settings:")]

        [EnumPaging, OnValueChanged("OnSelectTypeChange")]
        [InfoBox("选择要设置的资源类型")]
        public AssetType AssetType;

        private void OnSelectTypeChange()
        {
            //new TextureFormat
        }
        //

        //
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
        [Button("注意！", ButtonHeight = 20)]
        [ShowIf("showDeleteWarning")]
        private void Empty() { }


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
    }
}