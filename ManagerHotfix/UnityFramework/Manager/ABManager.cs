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
    public delegate void GameObjLoadAsyncCallBack(string str, float pro);


    //private List<string> loadingAssetName = new List<string>();
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
            return ABDict[name].LoadAsset<GameObject>(nameSplit[nameSplit.Length - 1]);
        }
        Debug.Log("δ����ab��");
        return null;
    }

    public T GetAssetFromAB<T>(string _name) where T : UnityEngine.Object
    {
        string name = _name.ToLower();
        GetAssetBundelWithAllDepend(name);
        if (ABDict.ContainsKey(name))
        {
            string[] nameSplit = name.Split("/");
            return ABDict[name].LoadAsset<T>(nameSplit[nameSplit.Length - 1]);
        }
        Debug.Log("δ����ab��");
        return null;
    }




    /// <summary>
    /// Э����AB���м���Ԥ����
    /// </summary>
    /// <param name="_name">Ԥ��������</param>
    /// <param name="gameObjLoadAsyncCallBack">���ȵĻص�</param>
    /// <param name="finishcallBack">���ʱ�Ļص�</param>
    public void GetGameObjectAsync(string _name,
         GameObjLoadAsyncCallBack gameObjLoadAsyncCallBack,
        Action<GameObject> finishcallBack)
    {
        List<string> loadingAssetName = new List<string>();
        string name = _name.ToLower();
        GetAssetBundelAllDependName(name, loadingAssetName);
        StartCoroutine(LoadGameObjectDependAsycn(loadingAssetName, name,
            gameObjLoadAsyncCallBack,
            (go) => {
                finishcallBack?.Invoke(go);
            }));
    }

    /// <summary>
    /// Э�̼���AB����������Ԥ��
    /// </summary>
    /// <param name="paths">��������</param>
    /// <param name="name">Ԥ��������</param>
    /// <param name="gameObjLoadAsyncCallBack">���ȵĻص�</param>
    /// <param name="callBack">���ʱ�Ļص�</param>
    /// <returns></returns>
    public IEnumerator LoadGameObjectDependAsycn(List<string> paths, string name,
        GameObjLoadAsyncCallBack gameObjLoadAsyncCallBack,
        Action<GameObject> callBack)
    {
        int currIndex = 0;
        foreach (var item in paths)
        {
            AssetBundleCreateRequest assetBundlereq = AssetBundle.LoadFromFileAsync(GetAssetsPath() + item);
            yield return assetBundlereq;
            if (assetBundlereq.isDone)
            {
                currIndex += 1;
                ABDict.Add(item, assetBundlereq.assetBundle);
                gameObjLoadAsyncCallBack("Ѱ����Դ��...", currIndex / 1.0f / paths.Count);
            }
        }
        if (ABDict.ContainsKey(name))
        {
            string[] nameSplit = name.Split("/");
            AssetBundleRequest abr = ABDict[name].LoadAssetAsync<GameObject>(nameSplit[nameSplit.Length - 1]);
            while (!abr.isDone)
            {
                gameObjLoadAsyncCallBack("������Դ��...", (abr.progress * 100.0f) / 100.0f);
                yield return new WaitForSeconds(0.1f);
            }
            if (abr.isDone)
            {
                callBack?.Invoke(abr.asset as GameObject);
            }
        }
        else
        {
            callBack?.Invoke(null);
        }

    }

    /// <summary>
    /// ��ȡͼƬ
    /// </summary>
    /// <param name="_name"></param>
    /// <returns></returns>
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

    /// <summary>
    /// ��ȡͼ���е�ͼƬ
    /// </summary>
    /// <param name="_atlasName"></param>
    /// <param name="string_name"></param>
    /// <returns></returns>
    public Sprite GetSpriterFromAtlas(string _atlasName, string string_name)
    {
        string atlasName = _atlasName.ToLower();
        GetAssetBundelWithAllDepend(atlasName);
        if (ABDict.ContainsKey(atlasName))
        {
            string[] nameSplit = atlasName.Split("/");
            SpriteAtlas spa = ABDict[atlasName].LoadAsset<SpriteAtlas>(nameSplit[nameSplit.Length - 1]);
            if (spa != null)
            {
                return spa.GetSprite(string_name);
            }
        }
        return null;
    }

    /// <summary>
    /// ��ȡTextAssets
    /// </summary>
    /// <param name="_path"></param>
    /// <returns></returns>
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

    /// <summary>
    /// ж��
    /// </summary>
    /// <param name="_name"></param>
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

    /// <summary>
    /// ��ȡ���е���������
    /// </summary>
    /// <param name="path"></param>
    private void GetAssetBundelAllDependName(string path, List<string> loadingAssetName)
    {
        if (ABDict.ContainsKey(path) || loadingAssetName.Contains(path))
        {
            return;
        }
        else
        {
            loadingAssetName.Add(path);
            string[] dependName = ABManifest.GetAllDependencies(path);
            foreach (var item in dependName)
            {
                GetAssetBundelAllDependName(item, loadingAssetName);
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
            assetBundel = AssetBundle.LoadFromFile(GetAssetsPath() + path);
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
