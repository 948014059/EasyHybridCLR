using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// AB ������
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
    /// ��ʼ��AB��
    /// </summary>
    public void InitData()
    {
        LoadMainABWithManifest();
        //Instantiate(GetGameObject("perfabs/panel/startpanel"));
    }

    /// <summary>
    /// ж������Ab��
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
    /// ���¼����µ�AB��
    /// </summary>
    public void ReLoadAssetBundle()
    {
        DestoryAllAB();
        InitData();
    }

    
    /// <summary>
    /// ����GameObject
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
    public GameObject GetGameObject(string _name)
    {
        //Debug.Log("ʹ��AssetBundle������Դ");
        string name = _name.ToLower();
        GetAssetBundelWithAllDepend(name);
        if (ABDict.ContainsKey(name))
        {
           string[] nameSplit = name.Split("/");
           return   ABDict[name].LoadAsset<GameObject>(nameSplit[nameSplit.Length-1]);
        }
        Debug.Log("δ����ab��");
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
        Debug.Log("δ����ab��");
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
    /// ж��AB��
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
    /// ����AB�� �Լ����Ӧ����Դ
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
    /// ͨ��·������AB��
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
    /// ���������������ļ�
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
    /// ��ȡҪ���ص�Ŀ¼  
    /// </summary>
    /// <returns></returns>
    private string GetAssetsPath()
    {

        string assetsPath = "";
        assetsPath = Config.ABPath + Config.PlatFrom + "/";
        if (Directory.Exists(assetsPath) && 
            !File.Exists(Config.ABPath + Config.VersionTempName)
            && File.Exists(Config.ABPath + Config.VersionName))  // ����ȸ�ʹ���ȸ�Ŀ¼
        {
            return assetsPath;
        }
        else  // δ����ȸ�ʹ�� StreamAssetĿ¼
        { 
            return Application.streamingAssetsPath + "/ProjectResources/" + Config.PlatFrom + "/";
        }

        
    }

}
