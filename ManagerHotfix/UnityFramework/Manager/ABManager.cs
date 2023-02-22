using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// AB 包管理
/// </summary>
public class ABManager : BaseSingleTon<ABManager>
{

    private AssetBundle AbMain;
    private AssetBundleManifest ABManifest;
    private Dictionary<string, AssetBundle> ABDict = new Dictionary<string, AssetBundle>();

    private void Awake()
    {
        InitData();
    }

    private void OnDestroy()
    {
        DestoryAllAB();
    }

    /// <summary>
    /// 初始化AB包
    /// </summary>
    public void InitData()
    {
        LoadMainABWithManifest();
        //Instantiate(GetGameObject("perfabs/panel/startpanel"));
    }

    /// <summary>
    /// 卸载所有Ab包
    /// </summary>
    public void DestoryAllAB()
    {
        AbMain.Unload(false);
        foreach (var item in ABDict)
        {
            item.Value.Unload(false);
        }
        ABDict.Clear();
    }

    /// <summary>
    /// 重新加载新的AB包
    /// </summary>
    public void ReLoadAssetBundle()
    {
        DestoryAllAB();
        InitData();
    }

    
    /// <summary>
    /// 加载GameObject
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public GameObject GetGameObject(string _name)
    {
        //Debug.Log("使用AssetBundle加载资源");
        string name = _name.ToLower();
        GetAssetBundelWithAllDepend(name);
        if (ABDict.ContainsKey(name))
        {
           string[] nameSplit = name.Split("/");
           return   ABDict[name].LoadAsset<GameObject>(nameSplit[nameSplit.Length-1]);
        }
        Debug.Log("未加载ab包");
        return null;
    }
    
    public Sprite GetSprite(string _name)
    {
        string name = _name.ToLower();
        GetAssetBundelWithAllDepend(name);
        if (ABDict.ContainsKey(name))
        {
            string[] nameSplit = name.Split("/");
            return ABDict[name].LoadAsset<Sprite>(nameSplit[nameSplit.Length - 1]);
        }
        Debug.Log("未加载ab包");
        return null;
    }

    public Sprite GetSpriterFromAtlas(string _atlasName, string string_name)
    {
        string atlasName = _atlasName.ToLower();
        GetAssetBundelWithAllDepend(atlasName);
        if (ABDict.ContainsKey(atlasName))
        {
            string[] nameSplit = atlasName.Split("/");
            SpriteAtlas spa = ABDict[atlasName].LoadAsset<SpriteAtlas>(nameSplit[nameSplit.Length - 1]);
            if (spa != null) { 
                return spa.GetSprite(string_name);
            }           
        }

        return null;
    }






    public void UnloadABundle(string _name)
    {
        string name = _name.ToLower();
        UnloadABundleWithAllDepend(name);
    }


    /// <summary>
    /// 卸载AB包
    /// </summary>
    /// <param name="path"></param>
    private void UnloadABundleWithAllDepend(string path)
    {
        string[] dependName = ABManifest.GetAllDependencies(path);
        foreach (var item in dependName)
        {
            UnloadABundleWithAllDepend(item);
        }
        ABDict[path].Unload(false);
        ABDict.Remove(path);
    }

    /// <summary>
    /// 加载AB包 以及其对应的资源
    /// </summary>
    /// <param name="path"></param>
    private void GetAssetBundelWithAllDepend(string path)
    {
        if (ABDict.ContainsKey(path))
        {
            return;
        }
        else
        {
            AssetBundle newab = LoadABundleWithPath(path);
            string[] dependName = ABManifest.GetAllDependencies(path);
            foreach (var item in dependName)
            {
                //Debug.Log(path + "   " + item);
                GetAssetBundelWithAllDepend(item);
            }

            ABDict.Add(path, newab);
        }       
    }


    /// <summary>
    /// 通过路径加载AB包
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private AssetBundle LoadABundleWithPath(string path)
    {
        AssetBundle assetBundel = null;
        try
        {
            assetBundel = AssetBundle.LoadFromFile(GetAssetsPath()  + path);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
        return assetBundel;
    }

    /// <summary>
    /// 加载主包和依赖文件
    /// </summary>
    private void LoadMainABWithManifest()
    {
        try
        {
            string platFrom = Config.PlatFrom;

            AbMain = AssetBundle.LoadFromFile(GetAssetsPath() + platFrom);
            ABManifest = AbMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        }
        catch (System.Exception e)
        {

            Debug.Log(e);
        }      
    }



    /// <summary>
    /// 获取要加载的目录  
    /// </summary>
    /// <returns></returns>
    private string GetAssetsPath()
    {

        string assetsPath = "";
        assetsPath = Config.ABPath + Config.PlatFrom + "/";
        if (Directory.Exists(assetsPath) && 
            !File.Exists(Config.ABPath + Config.VersionTempName)
            && File.Exists(Config.ABPath + Config.VersionName))  // 完成热更使用热更目录
        {
            return assetsPath;
        }
        else  // 未完成热更使用 StreamAsset目录
        { 
            return Application.streamingAssetsPath + "/ProjectResources/" + Config.PlatFrom + "/";
        }

        
    }

}
