using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace TaomeeTools.AssetAuditor
{
    public class ExecuteRule
    {
        public static void Execute(RuleConfig config)
        {
            if(config.ruleType == RuleType.Folder)
            {
                ExecuteFolder(config);
            }
            else if(config.ruleType == RuleType.FileList)
            {
                ExecuteFiles(config);
            }
        }


        private static void ExecuteFolder(RuleConfig config)
        {
            string path = config.FoldPath;

            if (!Directory.Exists(path))
            {
                return;
            }
            DirectoryInfo direction = new DirectoryInfo(path);

            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            //
            //
            //List<BaseRuleConfig> tempList = new List<BaseRuleConfig>();

            var currentRuleNum = files.Length;
            for (int i = 0; i < currentRuleNum; i++)
            {
                FileInfo info = files[i];
                string tempPath = path + "/" + info.Name;
                Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(tempPath);

                if(tex != null)
                {
                    TextureImporter import = TextureImporter.GetAtPath(tempPath) as TextureImporter;

                    //Debug.Log(tex);
                    import.SetPlatformTextureSettings("Android", config.TextureMaxSizeAndroid, config.compressAndroid);
                    import.SetPlatformTextureSettings("iPhone", config.TextureMaxSizeIOS, config.compressIOS);

                    AssetDatabase.ImportAsset(import.assetPath);
                }

            }
        }




        private static void ExecuteFiles(RuleConfig config)
        {
            int n = config.FileList.Length;

            if (n == 0)
            {
                return;
            }
            //
            //
            for (int i = 0; i < n; i++)
            {
                Texture tex = config.FileList[i];

                if (tex != null)
                {
                    string tempPath = AssetDatabase.GetAssetPath(tex);
                    TextureImporter import = TextureImporter.GetAtPath(tempPath) as TextureImporter;

                    //Debug.Log(tex);
                    import.SetPlatformTextureSettings("Android", config.TextureMaxSizeAndroid, config.compressAndroid);
                    import.SetPlatformTextureSettings("iPhone", config.TextureMaxSizeIOS, config.compressIOS);

                    AssetDatabase.ImportAsset(import.assetPath);
                }

            }
        }

    }

}