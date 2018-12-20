using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaomeeTools.AssetAuditor
{
    /// <summary>
    /// 检查工具的设置，记录工具的一些全局设置
    /// </summary>
    /// 
    [CreateAssetMenu]
    public class AssetAuditorSetting : ScriptableObject
    {
        /// <summary>
        /// 当前在使用的配置文件路径
        /// </summary>
        /// 
        [SerializeField]
        public AuditorInfo infoFile;
    }
}