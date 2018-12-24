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
    /// <summary>
    /// 检查的规则文件
    /// </summary>
    /// 
    [CreateAssetMenu(fileName = "New AuditorInfo", menuName = "TaomeeTools/创建资源格式配置")]
    public class AuditorInfo : ScriptableObject
    {
       
        public AuditorInfo()
        {
        }

    }

}