using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

using UnityEngine.UI;
using UnityEditor.SceneManagement;
namespace psdmap
{
    //------------------------------------------------------------------------------
    // class definition
    //------------------------------------------------------------------------------
    public class MapPSDImporter : Editor
    {
        //--------------------------------------------------------------------------
        // static private properties
        //--------------------------------------------------------------------------
        private static string baseFilename;
        private static string baseDirectory;

        private static int MapID;
        private static int maxIndex;
        private static GameObject activeObj;
        private static GameObject topObj;
        private static GameObject mapTopObj;
        private static GameObject mapObj;
        //
        private static string activeNameConst = "active_";
        private static string mapNameConst = "map_";
        private static string mapTopNameConst = "maptop_";
        private static string topNameConst = "top_";
        //
        private static Dictionary<string, Sprite> dict;

        [MenuItem("MapTool/Import PSD ...")]

        public static void OpenMapSetWindow()
        {
            EditorWindow.GetWindow(typeof(MapSetWindow));
        }

        static public void ImportHogSceneMenuItem(int mapID)
        {

            MapID = mapID;

            string inputFile = EditorUtility.OpenFilePanel("Choose PSDUI File to Import", Application.dataPath, "xml");
            if ((inputFile != null) && (inputFile != "") && (inputFile.StartsWith(Application.dataPath)))
            {
                ImportPSDUI(inputFile);
            }
        }

        static private void ImportPSDUI(string assetPath)
        {
            // before we do anything else, try to deserialize the input file and be sure it's actually the right kind of file
            PSDMAP psdUI = (PSDMAP)DeserializeXml(assetPath, typeof(PSDMAP));

            //Debug.Log(psdUI.width + "=====psdSize======" + psdUI.height);

            if (psdUI == null)
            {
                Debug.Log("The file " + assetPath + " wasn't able to generate a PSDUI.");
                return;
            }

            // next, we're going to be creating scenes, allow the user to save if they want
            // see if user wants to save current scene, bail if they don't
            if (EditorApplication.SaveCurrentSceneIfUserWantsTo() == false)
            {
                return;
            }
            //
            maxIndex = psdUI.maxIndex;
            dict = new Dictionary<string, Sprite>();

            // cache some useful variables
            baseFilename = Path.GetFileNameWithoutExtension(assetPath);
            baseDirectory = "Assets/" + Path.GetDirectoryName(assetPath.Remove(0, Application.dataPath.Length + 1)) + "/";

            Debug.Log("baseFilename " + baseFilename);
            Debug.Log("baseDirectory " + baseDirectory);
            //
            foreach (var item in psdUI.items.itemList)
            {
                /*
                Texture2D tex = new Texture2D(item.width, item.height);
                WWW www = new WWW(baseDirectory + item.name + ".png");
                www.LoadImageIntoTexture(tex);
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, item.width, item.height), Vector2.zero);

                dict.Add(item.name, sprite);
                */
                string texturePathName = baseDirectory + item.name + ".png";

                // modify the importer settings
                TextureImporter textureImporter = AssetImporter.GetAtPath(texturePathName) as TextureImporter;
                try
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                }
                catch (Exception e)
                {
                    Debug.Log("aa");
                }

                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.mipmapEnabled = false;
                AssetDatabase.WriteImportSettingsIfDirty(texturePathName);
                AssetDatabase.ImportAsset(texturePathName);
                //
                string targetPathName = "Assets/Textures/Map/";
                if (item.name.StartsWith(activeNameConst))
                {
                    targetPathName += "Map" + MapID + "_ActiveLayer/";
                }
                else if (item.name.StartsWith(mapNameConst))
                {
                    targetPathName += "Map" + MapID + "_MapLayer/";
                }
                else if (item.name.StartsWith(mapTopNameConst))
                {
                    targetPathName += "Map" + MapID + "_MapTopLayer/";
                }
                else if (item.name.StartsWith(topNameConst))
                {
                    targetPathName += "Map" + MapID + "_TopLayer/";
                }
                else
                {
                    Debug.LogError("图片名称规则不对！");
                    return;
                }
                if (!Directory.Exists(targetPathName))
                {
                    Directory.CreateDirectory(targetPathName);
                    AssetDatabase.Refresh();
                }

                string fileName = targetPathName + item.name + ".png";
                AssetDatabase.MoveAsset(texturePathName, fileName);

                //
                //string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                Sprite sprite = AssetDatabase.LoadAssetAtPath(fileName, typeof(Sprite)) as Sprite;
                dict.Add(item.name, sprite);
            }

            AssetDatabase.Refresh();

            // if the scene already exists, delete it
            /*
            string scenePath = baseDirectory + baseFilename + " Scene.unity";
            if (File.Exists (scenePath) == true)
            {
                File.Delete (scenePath);
                AssetDatabase.Refresh ();
            }
            */
            // now create a new scene
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);

            GameObject root = new GameObject();
            root.name = "MapRoot";

            mapObj = new GameObject();
            mapObj.name = "MapLayer";
            mapObj.transform.SetParent(root.transform, false);

            mapTopObj = new GameObject();
            mapTopObj.name = "MapTopLayer";
            mapTopObj.transform.SetParent(root.transform, false);

            activeObj = new GameObject();
            activeObj.name = "ActiveLayer";
            activeObj.transform.SetParent(root.transform, false);

            topObj = new GameObject();
            topObj.name = "TopLayer";
            topObj.transform.SetParent(root.transform, false);

            GenerateLayer(psdUI.layers.map.layerList, mapObj);
            GenerateLayer(psdUI.layers.mapTop.layerList, mapTopObj);
            GenerateLayer(psdUI.layers.active.layerList, activeObj);
            GenerateLayer(psdUI.layers.top.layerList, topObj);


            /*
            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                ImportLayer (psdUI.layers [layerIndex], baseDirectory);
            }

            Canvas temp = Resources.Load (PSDImporterConst.PREFAB_PATH_CANVAS, typeof(Canvas)) as Canvas;
            Canvas canvas = GameObject.Instantiate (temp) as Canvas;

            GameObject obj = new GameObject (baseFilename);
            obj.transform.SetParent(canvas.transform, false);

            for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
            {
                DrawLayer (psdUI.layers [layerIndex], obj);
            }

            AssetDatabase.Refresh ();
            EditorApplication.SaveScene (scenePath);

            if (baseFilename.Contains("Common"))
            {
                for (int layerIndex = 0; layerIndex < psdUI.layers.Length; layerIndex++)
                {
                    MoveAsset(psdUI.layers[layerIndex], baseDirectory);
                }

                AssetDatabase.Refresh();
            }
            */
        }


        private static void GenerateLayer(List<Layer> layerList, GameObject parent)
        {
            int length = layerList.Count;

            for (int i = 0; i < length; i++)
            {
                Layer layer = layerList[i];
                GameObject obj = new GameObject();
                obj.name = layer.item;

                var x = float.Parse(layer.x.Replace("px", ""));
                var y = float.Parse(layer.y.Replace("px", ""));

                var width = float.Parse(layer.width.Replace("px", ""));
                var height = float.Parse(layer.height.Replace("px", ""));

                var ox = x + (width - x) / 2f;
                var oy = y + (height - y) / 2f;
                
                obj.transform.position = new Vector3(ox / 100f, -oy / 100f);
                obj.transform.SetParent(parent.transform, false);
                //
                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = dict[layer.item];
                sr.sortingOrder = layer.index - maxIndex;
                if(layer.flip.IndexOf("x") != -1)
                {
                    sr.flipX = true;
                }
                if (layer.flip.IndexOf("y") != -1)
                {
                    sr.flipY = true;
                }
            }
        }
        /*
        //--------------------------------------------------------------------------
        // private methods
        //-------------------------------------------------------------------------

        static private void ImportLayer (PSDUI.Layer layer, string baseDirectory)
        {
            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    // we need to fixup all images that were exported from PS
                    PSDUI.Image image = layer.images [imageIndex];

                    if (image.imageSource == PSDUI.ImageSource.Custom)
                    {
                        string texturePathName = baseDirectory + layer.images [imageIndex].name + PSDImporterConst.PNG_SUFFIX;

                        // modify the importer settings
                        TextureImporter textureImporter = AssetImporter.GetAtPath (texturePathName) as TextureImporter;
                        try
                        {
                            textureImporter.textureType = TextureImporterType.Sprite;
                        }
                        catch(Exception e)
                        {
                            Debug.Log("aa");
                        }
                        textureImporter.spriteImportMode = SpriteImportMode.Single;
                        textureImporter.spritePackingTag = baseFilename;
                        textureImporter.mipmapEnabled = false;
                        textureImporter.maxTextureSize = 2048;

                        if (baseFilename.Contains ("Common") && layer.name == PSDImporterConst.NINE_SLICE)  //If Psd's name contains "Common", then it's Common type;
                        {
                            textureImporter.spriteBorder = new Vector4 (3, 3, 3, 3);   // Set Default Slice type Image's border to Vector4 (3, 3, 3, 3)
                        }

                        AssetDatabase.WriteImportSettingsIfDirty (texturePathName);
                        AssetDatabase.ImportAsset (texturePathName);                
                    }
                }
            }

            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    ImportLayer (layer.layers [layerIndex], baseDirectory);
                }
            }
        }

        //------------------------------------------------------------------
        //when it's a common psd, then move the asset to special folder
        //------------------------------------------------------------------
        static private void MoveAsset (PSDUI.Layer layer, string baseDirectory)
        {
            if (layer.images != null)
            {
                string newPath = baseDirectory;
                //if (layer.name == PSDImporterConst.IMAGE)
                //{
                //    newPath = baseDirectory + PSDImporterConst.IMAGE + "/";
                //    System.IO.Directory.CreateDirectory (newPath);
                //}
                //else
                if (layer.name == PSDImporterConst.NINE_SLICE)
                {
                    newPath = baseDirectory + PSDImporterConst.NINE_SLICE + "/";
                    System.IO.Directory.CreateDirectory (newPath);
                    Debug.Log("creating new folder : " + newPath);

                    AssetDatabase.Refresh();

                    for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                    {
                        // we need to fixup all images that were exported from PS
                        PSDUI.Image image = layer.images[imageIndex];

                        if (image.imageSource == PSDUI.ImageSource.Custom)
                        {
                            string texturePathName = baseDirectory + layer.images[imageIndex].name + PSDImporterConst.PNG_SUFFIX;
                            string targetPathName = newPath + layer.images[imageIndex].name + PSDImporterConst.PNG_SUFFIX;

                            Debug.Log(texturePathName);
                            Debug.Log(targetPathName);
                            AssetDatabase.MoveAsset(texturePathName, targetPathName);
                        }
                    }
                }
            }

            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    MoveAsset (layer.layers [layerIndex], baseDirectory);
                }
            }
        }

        static private void DrawNormalLayer (PSDUI.Layer layer, GameObject parent)
        {
            GameObject obj = new GameObject (layer.name);
            obj.transform.SetParent(parent.transform, false);

            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    PSDUI.Image image = layer.images [imageIndex];

                    if (image.imageSource == PSDUI.ImageSource.Custom)
                    {
                        Image pic = Resources.Load (PSDImporterConst.PREFAB_PATH_IMAGE, typeof(Image)) as Image;

                        string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                        Sprite sprite = AssetDatabase.LoadAssetAtPath (assetPath, typeof(Sprite)) as Sprite;

                        if (sprite == null)
                        {
                            Debug.Log ("loading asset at path: " + baseDirectory + image.name);
                        }

                        pic.sprite = sprite;

                        Image myImage = GameObject.Instantiate (pic) as Image;
                        myImage.name = image.name;
                        myImage.transform.SetParent(obj.transform, false);

                        RectTransform rectTransform = myImage.GetComponent<RectTransform> ();
                        rectTransform.sizeDelta = new Vector2 (image.size.width, image.size.height);
                        rectTransform.anchoredPosition = new Vector2 (image.position.x, image.position.y);
                    }
                    else
                    if (image.imageSource == PSDUI.ImageSource.Common)
                    {
                        if (image.imageType == PSDUI.ImageType.Label)
                        {
                            Text text = Resources.Load (PSDImporterConst.PREFAB_PATH_TEXT, typeof(Text)) as Text;

                            Text myText = GameObject.Instantiate (text) as Text;
                            myText.gameObject.name = image.name;
                            Debug.Log ("Label Color : " + image.arguments [0]);
                            //myText.color = ColorUtil.HexToColor(image.arguments[0]);
                            //myText.font = image.arguments[1];
                            Debug.Log ("fontSize : " + image.arguments [2]);

                            myText.fontSize = System.Convert.ToInt32 (System.Convert.ToDouble(image.arguments [2]));
                            myText.text = image.arguments [3];
                            myText.transform.SetParent(obj.transform, false);

                            RectTransform rectTransform = myText.GetComponent<RectTransform> ();
                            rectTransform.sizeDelta = new Vector2 (image.size.width, image.size.height);
                            rectTransform.anchoredPosition = new Vector2 (image.position.x, image.position.y);
                        }
                        else if (image.imageType == PSDUI.ImageType.Texture)
                        {
                            Image pic = Resources.Load (PSDImporterConst.PREFAB_PATH_IMAGE, typeof(Image)) as Image;
                            pic.sprite = null;
                            Image myImage = GameObject.Instantiate (pic) as Image;
                            myImage.name = image.name;
                            myImage.transform.SetParent(obj.transform, false);

                            RectTransform rectTransform = myImage.GetComponent<RectTransform> ();
                            rectTransform.sizeDelta = new Vector2 (image.size.width, image.size.height);
                            rectTransform.anchoredPosition = new Vector2 (image.position.x, image.position.y);
                        }
                        else
                        {
                            Image pic = Resources.Load (PSDImporterConst.PREFAB_PATH_IMAGE, typeof(Image)) as Image;
                            string[] imageNameArr = image.name.Split('@');
                            string imageName = imageNameArr[0];
                            string imageRelativePath = imageNameArr[1];
                            string commonImagePath = PSDImporterConst.COMMON_BASE_FOLDER + imageRelativePath.Replace (".", "/") + PSDImporterConst.PNG_SUFFIX;
                            Debug.Log ("==  CommonImagePath  ====" + commonImagePath);
                            Sprite sprite = AssetDatabase.LoadAssetAtPath (commonImagePath, typeof(Sprite)) as Sprite;                     
                            pic.sprite = sprite;

                            Image myImage = GameObject.Instantiate (pic) as Image;
                            myImage.name = imageName;
                            myImage.transform.SetParent(obj.transform, false);

                            if (image.imageType == PSDUI.ImageType.SliceImage)
                            {
                                myImage.type = Image.Type.Sliced;
                            }

                            RectTransform rectTransform = myImage.GetComponent<RectTransform> ();
                            rectTransform.sizeDelta = new Vector2 (image.size.width, image.size.height);
                            rectTransform.anchoredPosition = new Vector2 (image.position.x, image.position.y);

                        }
                    }
                }
            }

            if (layer.layers != null)
            {
                for (int layerIndex = 0; layerIndex < layer.layers.Length; layerIndex++)
                {
                    DrawLayer (layer.layers [layerIndex], obj);
                }
            }
        }

        static private void DrawButton (PSDUI.Layer layer, GameObject parent)
        {
            Button temp = Resources.Load (PSDImporterConst.PREFAB_PATH_BUTTON, typeof(Button)) as Button;
            Button button = GameObject.Instantiate (temp) as Button;
            button.gameObject.name = layer.name;
            button.transform.SetParent(parent.transform, false);


            if (layer.images != null)
            {
                for (int imageIndex = 0; imageIndex < layer.images.Length; imageIndex++)
                {
                    PSDUI.Image image = layer.images [imageIndex];

                    if (image.imageType == PSDUI.ImageType.Label)
                    {
                        Text text = button.GetComponentInChildren<Text>();
                        text.gameObject.name = image.name;
                        Debug.Log("Label Color : " + image.arguments[0]);
                        //text.color = ColorUtil.HexToColor(image.arguments[0]);
                        //myText.font = image.arguments[1];
                        Debug.Log("fontSize : " + image.arguments[2]);

                        text.fontSize = (int)System.Convert.ToDouble(image.arguments[2]);
                        text.text = image.arguments[3];
                    }
                    else if (image.imageSource == PSDUI.ImageSource.Custom)
                    {                       
                        string assetPath = baseDirectory + image.name + PSDImporterConst.PNG_SUFFIX;
                        Sprite sprite = AssetDatabase.LoadAssetAtPath (assetPath, typeof(Sprite)) as Sprite;
                        button.image.sprite = sprite;

                        RectTransform rectTransform = button.GetComponent<RectTransform> ();
                        rectTransform.sizeDelta = new Vector2 (image.size.width, image.size.height);
                        rectTransform.anchoredPosition = new Vector2 (image.position.x, image.position.y);
                    }
                    else if(image.imageSource == PSDUI.ImageSource.Common)
                    {
                        string[] imageNameArr = image.name.Split('@');
                        string imageRelativePath;
                        if (imageNameArr.Length == 1)
                            imageRelativePath = imageNameArr[0];
                        else
                            imageRelativePath = imageNameArr[1];
                        string commonImagePath = PSDImporterConst.COMMON_BASE_FOLDER + imageRelativePath.Replace(".", "/") + PSDImporterConst.PNG_SUFFIX;
                        Debug.Log("==  CommonImagePath  ====" + commonImagePath);
                        Sprite sprite = AssetDatabase.LoadAssetAtPath(commonImagePath, typeof(Sprite)) as Sprite;
                        button.image.sprite = sprite;

                        RectTransform rectTransform = button.GetComponent<RectTransform>();
                        rectTransform.sizeDelta = new Vector2(image.size.width, image.size.height);
                        rectTransform.anchoredPosition = new Vector2(image.position.x, image.position.y);
                    }
                }
            }
        }

        static private GridLayoutGroup DrawGrid (PSDUI.Layer layer, GameObject parent)
        {
            GridLayoutGroup temp = Resources.Load (PSDImporterConst.PREFAB_PATH_GRID, typeof(GridLayoutGroup)) as GridLayoutGroup;
            GridLayoutGroup gridLayoutGroup = GameObject.Instantiate (temp) as GridLayoutGroup;
            gridLayoutGroup.gameObject.name = layer.name;
            gridLayoutGroup.transform.SetParent(parent.transform, false);

            gridLayoutGroup.padding = new RectOffset ();
            gridLayoutGroup.cellSize = new Vector2 (System.Convert.ToInt32 (layer.arguments [2]), System.Convert.ToInt32 (layer.arguments [3]));

            RectTransform rectTransform = gridLayoutGroup.GetComponent<RectTransform> ();
            rectTransform.sizeDelta = new Vector2 (layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2 (layer.position.x, layer.position.y);

            int cellCount = System.Convert.ToInt32 (layer.arguments [0]) * System.Convert.ToInt32 (layer.arguments [1]);
            for (int cell = 0; cell < cellCount; cell++)
            {
                Image pic = Resources.Load (PSDImporterConst.PREFAB_PATH_IMAGE, typeof(Image)) as Image;
                pic.sprite = null;
    //            Sprite sprite = Resources.Load (relativeResoucesDirectory + "normal_13", typeof(Sprite)) as Sprite;
    //            pic.sprite = sprite;

                Image myImage = GameObject.Instantiate (pic) as Image;
                myImage.transform.SetParent(rectTransform, false);
            }

            return gridLayoutGroup;
        }

        static private void DrawScrollView (PSDUI.Layer layer, GameObject parent)
        {
            ScrollRect temp = Resources.Load (PSDImporterConst.PREFAB_PATH_SCROLLVIEW, typeof(ScrollRect)) as ScrollRect;
            ScrollRect scrollRect = GameObject.Instantiate (temp) as ScrollRect;
            scrollRect.name = layer.name;
            scrollRect.transform.SetParent(parent.transform, false);

            RectTransform rectTransform = scrollRect.GetComponent<RectTransform> ();
            rectTransform.sizeDelta = new Vector2 (layer.size.width, layer.size.height);
            rectTransform.anchoredPosition = new Vector2 (layer.position.x, layer.position.y);

            GridLayoutGroup grid = DrawGrid (layer, parent);
            scrollRect.content = grid.GetComponent<RectTransform> ();
            grid.transform.SetParent(scrollRect.transform);
        }

        static private void DrawLayer (PSDUI.Layer layer, GameObject parent)
        {
            switch (layer.type)
            {
            case PSDUI.LayerType.Normal:
                DrawNormalLayer (layer, parent);
                break;
            case PSDUI.LayerType.Button:
                DrawButton (layer, parent);
                break;
            case PSDUI.LayerType.Grid:
                DrawGrid (layer, parent);
                break;
            case PSDUI.LayerType.ScrollView:
                DrawScrollView (layer, parent);
                break;
            }
        }


        */


        static private object DeserializeXml(string filePath, System.Type type)
        {
            object instance = null;
            StreamReader xmlFile = File.OpenText(filePath);
            if (xmlFile != null)
            {
                string xml = xmlFile.ReadToEnd();
                if ((xml != null) && (xml.ToString() != ""))
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] byteArray = encoding.GetBytes(xml);
                    MemoryStream memoryStream = new MemoryStream(byteArray);
                    XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                    if (xmlTextWriter != null)
                    {
                        instance = xs.Deserialize(memoryStream);
                    }
                }
            }
            xmlFile.Close();
            return instance;
        }
    }
}