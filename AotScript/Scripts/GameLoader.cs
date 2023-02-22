using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class GameLoader : MonoBehaviour
{
    public static GameLoader Instance;
    public static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    // AOT����
    public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        StartCoroutine(DownLoadAssets(this.StartGame));
    }
    public  byte[] GetAssetData(string dllName)
    {
        return s_assetDatas[dllName];
    }

    public string GetWebRequestPath(string asset,string abPath = "" , string platFrom = "")
    {
        var path = $"{Application.streamingAssetsPath}/{asset}";
        var hotfixabPath = abPath + platFrom + "/Assembly-CSharp.dll.bytes";

        if (asset == "Assembly-CSharp.dll")
        {
            if (File.Exists(hotfixabPath))
            {
                return "file://" + hotfixabPath;
            }
        }

        if (asset == "ManagerHotfix.dll")
        {
            return "http://192.168.2.221:88/ProjectResources/ManagerHotfix.dll.bytes";
        }

        if (!path.Contains("://"))
        {
            path = "file://" + path;
        }
        if (path.EndsWith(".dll"))
        {

            path += ".bytes";
        }
        return path;
    }

    public  void LoadMetadataForAOTAssemblies()
    {
        /// ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
        /// �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyNames)
        {
            byte[] dllBytes = GetAssetData(aotDllName);
            // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    void StartGame()
    {
        LoadMetadataForAOTAssemblies();

#if !UNITY_EDITOR
        System.Reflection.Assembly.Load(GetAssetData("ManagerHotfix.dll"));
        //System.Reflection.Assembly.Load(GetAssetData("Assembly-CSharp.dll"));
#endif

        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "ManagerHotfix");
        Type startType = ass.GetType("StartGame");
        this.gameObject.AddComponent(startType);
        Debug.Log("ass:" + ass + " startType:"+startType);
    }


    public IEnumerator DownloadHotFixAssets(string name,Action callback , string abPath = "", string platFrom = "")
    {
        string dllPath = GetWebRequestPath(name, abPath , platFrom);
        Debug.Log($"start download asset:{dllPath}");
        UnityWebRequest www = UnityWebRequest.Get(dllPath);
        yield return www.SendWebRequest();
#if UNITY_2020_1_OR_NEWER
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
#else
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
#endif
        else
        {
            // Or retrieve results as binary data
            byte[] assetData = www.downloadHandler.data;
            Debug.Log($"dll:{name}  size:{assetData.Length}");
            s_assetDatas[name] = assetData;
        }
        callback?.Invoke();
    }




    private IEnumerator DownLoadAssets(Action onDownloadComplete)
    {
        var assets = new List<string>
        {
            "ManagerHotfix.dll",
            //"Assembly-CSharp.dll",
        }.Concat(AOTMetaAssemblyNames);

        foreach (var asset in assets)
        {
            string dllPath = GetWebRequestPath(asset);
            Debug.Log($"start download asset:{dllPath}");
            UnityWebRequest www = UnityWebRequest.Get(dllPath);
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
#else
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
#endif
            else
            {
                // Or retrieve results as binary data
                byte[] assetData = www.downloadHandler.data;
                Debug.Log($"dll:{asset}  size:{assetData.Length}");
                s_assetDatas[asset] = assetData;
            }
        }

        onDownloadComplete();
    }



}
