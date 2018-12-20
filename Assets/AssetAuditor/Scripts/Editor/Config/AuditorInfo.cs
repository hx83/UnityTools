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
/// <summary>
/// 检查的规则文件
/// </summary>
/// 
namespace TaomeeTools.AssetAuditor
{
    [CreateAssetMenu(fileName = "New AuditorInfo", menuName = "TaomeeTools/创建资源格式配置")]
    public class AuditorInfo : ScriptableObject
    {
        [SerializeField]
        [ReadOnly]
        private List<BaseRuleConfig> list;
        //
        //

        public AuditorInfo()
        {
            //list = new List<IRuleConfig>();
        }
        public void AddRule(BaseRuleConfig rule)
        {
            list.Add(rule);
        }

        public void DeleteRule(BaseRuleConfig rule)
        {
            if(!list.Contains(rule))
            {
                list.Remove(rule);
            }
        }
    }

}