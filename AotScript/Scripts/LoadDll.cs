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

        Debug.Log("??????????");
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
    /// ??aot assembly????????metadata?? ??????????aot????????????????
    /// ????????????????AOT????????????native????????????????????????????????????
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        /// ????????????????????AOT dll??????????????????????????dll????????????
        /// ??????dll????????????????????????????????LoadMetadataForAOTAssembly??????????
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyNames)
        {
            byte[] dllBytes = GetAssetData(aotDllName);
            // ????assembly??????dll????????????hook??????aot??????????native????????????????????????????
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }
}
