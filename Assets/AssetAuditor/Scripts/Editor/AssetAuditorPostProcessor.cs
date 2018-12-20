using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetAuditorPostProcessor : AssetPostprocessor
{

    void OnPreprocessAsset()
    {
        Debug.Log(assetImporter.assetBundleName);
        if (assetImporter.assetBundleName == "Asset Auditor Setting")
        {

            ModelImporter modelImporter = assetImporter as ModelImporter;
            if (modelImporter != null)
            {
                
            }
        }
    }
}
