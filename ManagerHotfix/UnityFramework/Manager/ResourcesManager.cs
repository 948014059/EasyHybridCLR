using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;



/// <summary>
/// 资源加载 区分编辑器模式  还是打包模式
/// </summary>
public static class  ResourcesManager 
{
#if ASSETBUNDLE
    private static ABManager aBManager = ABManager.GetInstance();
#endif



    public static GameObject GetPrefab(string path)
    {

#if !ASSETBUNDLE
        return GetGameObjectFromEditorPath(path);
#else
        return  aBManager.GetGameObject(path);
#endif
    }



    public static void GetGameObjectAsync(string path, Action<GameObject> callback)
    {
#if !ASSETBUNDLE
        GetGameObjectAsyncFromEditor(path, callback);
#else
        aBManager.GetGameObjectAsync(path,callback);
#endif
    }


    //    public static void GetPrefabsGameObjectAsync(List<string> paths, Action<Dictionary<string,GameObject>> callback)
    //    {
    //#if !ASSETBUNDLE
    //        return GetGameObjectFromEditorPath(path);
    //#else
    //        aBManager.GetGameObjectAsycn(paths, callback);
    //#endif
    //    }







    public static Sprite GetSprite(string spritePath)
    {

#if !ASSETBUNDLE
        return GetSpriteFromEditor(spritePath);
#else
        return aBManager.GetSprite(spritePath);
#endif
    }

    public static Sprite GetSpriteFromAtlas(string atlasPath , string spriteName)
    {

#if !ASSETBUNDLE
        return GetSpriteFromEditorPath(atlasPath, spriteName);
#else
        return aBManager.GetSpriterFromAtlas(atlasPath, spriteName);

#endif
    }


    public static string GetTextAsset(string path)
    {
#if !ASSETBUNDLE
        return GetTxtDataFromEditorPath(path);
#else
        return  aBManager.GetTextAssets(path);
#endif
    }

    public static AudioClip GetAudioClip()
    {
        return null;
    }



#if !ASSETBUNDLE

    public static void GetGameObjectAsyncFromEditor(string path, Action<GameObject> callback)
    {
        string GoPath = Config.EditorPath + path + ".prefab";
        if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
        {
            callback.Invoke(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(GoPath));
        }
        else
        {
            Debug.Log("未找到资源：" + path);
        }
        //callback.Invoke(null);
    }
    private static GameObject GetGameObjectFromEditorPath(string path)
    {
        string GoPath = Config.EditorPath + path + ".prefab";
        if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(GoPath);
        }
        else
        {
            Debug.Log("未找到资源：" + path);
        }
        return null;
    }


    private static Sprite GetSpriteFromEditor(string spritePath)
    {
        string GoPath = Config.EditorPath + spritePath + ".png";
        if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        }
        else
        {
            Debug.Log("未找到资源：" + spritePath);
        }
        return null;
    }

    private static Sprite GetSpriteFromEditorPath(string atlasPath, string spriteName)
    {
        string GoPath = Config.EditorPath + atlasPath + ".spriteatlas";
        if (File.Exists(Application.dataPath.Replace("Assets", "") + GoPath))
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<SpriteAtlas>(GoPath).GetSprite(spriteName);
        }
        else
        {
            Debug.Log("未找到资源：" + atlasPath);
        }
        return null;
    }

    public static  string  GetTxtDataFromEditorPath(string path)
    {
        string Txtpath = Config.EditorPath + path + ".txt";
        if (File.Exists(Application.dataPath.Replace("Assets", "") + Txtpath))
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(Txtpath).text;
        }
        else
        {
            Debug.Log("未找到资源：" + Txtpath);
        }
        return "";
    }


#endif
}

