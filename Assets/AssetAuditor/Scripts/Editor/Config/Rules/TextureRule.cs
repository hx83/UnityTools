using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaomeeTools.AssetAuditor
{
    
    public class TextureRule : BaseRuleConfig 
    {
        [SerializeField]
        public string label;
        [SerializeField]
        public Texture texture;

    }
}