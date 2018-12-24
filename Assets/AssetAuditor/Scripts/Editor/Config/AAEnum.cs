using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaomeeTools.AssetAuditor
{
    /// <summary>
    /// 规则类型-针对文件夹，还是针对单个文件
    /// </summary>
    public enum RuleType
    {
        Folder,FileList,
    }
    public enum AssetType
    {
        Texture,
        Material,
        Model,
    }

}