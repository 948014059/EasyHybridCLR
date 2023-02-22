using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class LoadDll : MonoBehaviour
{


    public static List<string> AOTMetaAssemblyNames { get; } = new List<string>()
    {
        "mscorlib.dll",
        "System.dll",
        "System.Core.dll",
    };

    void Start()
    {
        StartCoroutine(DownLoadAssets(this.StartGame));
    }

    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();

    public static byte[] GetAssetData(string dllName)
    {
        return s_assetDatas[dllName];
    }

    private string GetWebRequestPath(string asset)
    {
        var path = $"{Application.streamingAssetsPath}/{asset}";
        var hotfixabPath = ""; //Config.ABPath + Config.PlatFrom+ "/Assembly-CSharp.dll.bytes";

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

    IEnumerator DownLoadAssets(Action onDownloadComplete)
    {
        var assets = new List<string>
        {
            //"prefabs",
            "ManagerHotfix.dll",
            "Assembly-CSharp.dll",
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


    void StartGame()
    {
        LoadMetadataForAOTAssemblies();

#if !UNITY_EDITOR
        System.Reflection.Assembly.Load(GetAssetData("ManagerHotfix.dll"));
        System.Reflection.Assembly.Load(GetAssetData("Assembly-CSharp.dll"));
#endif
        //System.Reflection.Assembly.Load(GetAssetData("Hotfix.dll"));

        Debug.Log("开始游戏了");
        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
        Type startType = ass.GetType("StartGame");

        GameObject go = new GameObject("hotfix");
        go.AddComponent(startType);
        //Type startType = ass.GetType("StartGameModule");
        //Debug.Log("11111   "+  startType + "   ");
        //ModuleManager.GetInstance().OpenModule(startType,()=> {
        //    ModuleManager.GetInstance().CloseModule(typeof(UpdateModule));
        //});


        //AssetBundle prefabAb = AssetBundle.LoadFromMemory(GetAssetData("prefabs"));
        //GameObject testPrefab = Instantiate(prefabAb.LoadAsset<GameObject>("HotUpdatePrefab.prefab"));
    }



    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyNames)
        {
            byte[] dllBytes = GetAssetData(aotDllName);
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }
}
