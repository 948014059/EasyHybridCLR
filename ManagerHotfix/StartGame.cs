using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class StartGame : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        Debug.LogError("这是热更 资源管理脚本.....");
        Debug.LogError("这是热更 资源管理脚本..... 的热更新log");

        //StartCoroutine(DownLoadAssets(this.StartG));
        StartCoroutine(GameLoader.Instance.DownloadHotFixAssets("Assembly-CSharp.dll", () =>
        {
            GameLoader.Instance.LoadMetadataForAOTAssemblies();
#if !UNITY_EDITOR
        //System.Reflection.Assembly.Load(GetAssetData("ManagerHotfix.dll"));
        System.Reflection.Assembly.Load(GameLoader.Instance.GetAssetData("Assembly-CSharp.dll"));
#endif
            StartG();

        },Config.ABPath,Config.PlatFrom));


    }

    private void StartG()
    {


        Assembly ass = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == "Assembly-CSharp");
        foreach (var item in ass.GetTypes())
        {
            Debug.Log("ass:" + ass + "          startType:" + item);

        }
        Type startType = ass.GetType("Assets.Hotfix.UI.testmodule");

        ModuleManager.GetInstance().OpenModule(startType, () =>
        {
            Debug.Log("打开ui测试");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
