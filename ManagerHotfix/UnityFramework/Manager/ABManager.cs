using System;
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
    public Action<string, float> GameObjLoadAsyncCallBack;
    private List<string> loadingAssetName = new List<string>();
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


    public void GetGameObjectAsync(string _name, Action<GameObject> callBack)
    {
        loadingAssetName.Clear();
        string name = _name.ToLower();
        GetAssetBundelAllDependName(name);
        //GameObject go = null;
        StartCoroutine(LoadGameObjectDependAsycn(loadingAssetName, name, (go) => {
            callBack?.Invoke(go);

        }));
    }

    public IEnumerator LoadGameObjectDependAsycn(List<string> paths, string name, Action<GameObject> callBack)
    {
        int currIndex = 0;
        foreach (var item in paths)
        {
            //AssetBundle assetBundle = null;
            AssetBundleCreateRequest assetBundlereq = AssetBundle.LoadFromFileAsync(GetAssetsPath() + item);
            yield return assetBundlereq;
            if (assetBundlereq.isDone)
            {
                currIndex += 1;
                ABDict.Add(item, assetBundlereq.assetBundle);
                GameObjLoadAsyncCallBack.Invoke("Ѱ����Դ��...", currIndex / 1.0f / paths.Count);
            }
        }
    }
    public void GetGameObjectAsycn(List<string> _names, Action<Dictionary<string, GameObject>> callback)
    {
        loadingAssetName.Clear();
        Dictionary<string, GameObject> loadDict = new Dictionary<string, GameObject>();
        foreach (var _name in _names)
        {
            string name = _name.ToLower();
            GetAssetBundelAllDependName(name);
            GameObject go = null;
            StartCoroutine(LoadGameObjectDependAsycn(loadingAssetName, () => {
                if (ABDict.ContainsKey(name))
                {
                    string[] nameSplit = name.Split("/");
                    go = ABDict[name].LoadAsset<GameObject>(nameSplit[nameSplit.Length - 1]);

                    loadDict.Add(name,go);
                }
            }));
        }
        callback?.Invoke(loadDict);             
//return go;
    }

    public IEnumerator LoadGameObjectDependAsycn(List<string> paths,Action callBack)
    {
        foreach (var item in paths)
        {
            //AssetBundle assetBundle = null;
            AssetBundleCreateRequest assetBundlereq = AssetBundle.LoadFromFileAsync(GetAssetsPath() + item);
            yield return assetBundlereq;
            if (assetBundlereq.isDone)
            {
                Debug.Log("item" + item);
                ABDict.Add(item, assetBundlereq.assetBundle);
            }
        }
        callBack?.Invoke();
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

    public string GetTextAssets(string _path)
    {
        string path = _path.ToLower();
        GetAssetBundelWithAllDepend(path);
        if (ABDict.ContainsKey(path))
        {
            string[] nameSplit = path.Split("/");
            return ABDict[path].LoadAsset<TextAsset>(nameSplit[nameSplit.Length - 1]).text;
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
            string[] dependName = ABManifest.GetAllDependencies(path);
            foreach (var item in dependName)
            {
                //Debug.Log(path + "   " + item);
                GetAssetBundelWithAllDepend(item);
            }
            AssetBundle newab = LoadABundleWithPath(path);
            ABDict.Add(path, newab);
        }       
    }


    private void GetAssetBundelAllDependName(string path)
    {
        if (ABDict.ContainsKey(path))
        {
            return;
        }
        else
        {
            loadingAssetName.Add(path);
            string[] dependName = ABManifest.GetAllDependencies(path);
            foreach (var item in dependName)
            {
                //Debug.Log(path + "   " + item);
                GetAssetBundelAllDependName(item);
            }
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
            && File.Exists(Config.ABPath + Config.VersionName)
            && !File.Exists(Config.ABPath + Config.CopyResourceTempName))  // �����Դ���� ��������ȸ� ʱʹ���ȸ�Ŀ¼
        {
            return assetsPath;
        }
        else  // δ����ȸ�ʹ�� StreamAssetĿ¼
        { 
            return Application.streamingAssetsPath + "/ProjectResources/" + Config.PlatFrom + "/";
        }

        
    }

}
