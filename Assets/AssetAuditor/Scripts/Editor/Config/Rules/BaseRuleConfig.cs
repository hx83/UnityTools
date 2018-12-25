using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TaomeeTools.AssetAuditor
{
    public partial class RuleConfig : ScriptableObject
    {
        private string _name;

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

        public string RuleName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        
        //
        //


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
        public Texture[] textureList;

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
    }
}