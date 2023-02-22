using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config { 

    /// <summary>
    /// Editor 资源路径
    /// </summary>
    public static string ABPathEditor = Application.dataPath.Replace("Assets", "ProjectResources/");
    /// <summary>
    /// Ab 资源路径
    /// </summary>
    public static string EditorPath = "Assets/ProjectResources/";

    /// <summary>
    /// 热更AB包保存路径
    /// </summary>
    public static string ABPath = Application.persistentDataPath + "/ProjectResources/";
    /// <summary>
    /// Log 保存路径
    /// </summary>
    public static string LogFilePath = Application.persistentDataPath + "/";

    /// <summary>
    /// 当前的设备
    /// </summary>
#if UNITY_STANDALONE_WIN
    public static string PlatFrom = "StandaloneWindows64";
# elif UNITY_ANDROID
    public static string PlatFrom = "Android";
#endif

    /// <summary>
    /// 热更服务器地址
    /// </summary>
    public static string UpdateUrl = "http://192.168.2.221:88/ProjectResources/";
    public static string VersionName = PlatFrom +"/VersionConfig.txt";
    public static string VersionTempName = PlatFrom +"/VersionTempConfig.txt";
    
    public static int HttpTimeOut = 5; 
    public static int MaxDownCoroutine = 10; // 最大下载协程数量
    public static int BuffDownLength =1024 *20; // 每次下载多少字节


    //public static string ABPath = Application.persistentDataPath;
}
