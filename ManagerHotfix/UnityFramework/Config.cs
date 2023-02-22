using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config { 

    /// <summary>
    /// Editor ��Դ·��
    /// </summary>
    public static string ABPathEditor = Application.dataPath.Replace("Assets", "ProjectResources/");
    /// <summary>
    /// Ab ��Դ·��
    /// </summary>
    public static string EditorPath = "Assets/ProjectResources/";

    /// <summary>
    /// �ȸ�AB������·��
    /// </summary>
    public static string ABPath = Application.persistentDataPath + "/ProjectResources/";
    /// <summary>
    /// Log ����·��
    /// </summary>
    public static string LogFilePath = Application.persistentDataPath + "/";

    /// <summary>
    /// ��ǰ���豸
    /// </summary>
#if UNITY_STANDALONE_WIN
    public static string PlatFrom = "StandaloneWindows64";
# elif UNITY_ANDROID
    public static string PlatFrom = "Android";
#endif

    /// <summary>
    /// �ȸ���������ַ
    /// </summary>
    public static string UpdateUrl = "http://192.168.2.221:88/ProjectResources/";
    public static string VersionName = PlatFrom +"/VersionConfig.txt";
    public static string VersionTempName = PlatFrom +"/VersionTempConfig.txt";
    
    public static int HttpTimeOut = 5; 
    public static int MaxDownCoroutine = 10; // �������Э������
    public static int BuffDownLength =1024 *20; // ÿ�����ض����ֽ�


    //public static string ABPath = Application.persistentDataPath;
}
