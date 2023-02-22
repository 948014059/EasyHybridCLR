using UnityEngine;
using UnityEditor;
using System.IO;
using LitJson;
using System.Collections.Generic;
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;

public class AssetBundleTools : EditorWindow
{
    public static string ProjectResourcesPath = Application.dataPath + "/ProjectResources/";
    public static string ABSavePath = Application.dataPath.Replace("Assets", "ProjectResources/");
#if UNITY_STANDALONE_WIN
    public static string PlatFrom = "StandaloneWindows64";
# elif UNITY_ANDROID
    public static string PlatFrom = "Android";
#endif

    public static string VersionName = PlatFrom + "/VersionConfig.txt";
    static AssetBundleTools window;
    public string Version = "1.0.0";
    public int TargetPlatformId = 1;
    public bool IsGenHotFix = true;
    public bool IsCopyAB2StreamingAssets = false;

    public BuildTarget[] buildTargets = new BuildTarget[]
    {
        BuildTarget.StandaloneWindows64,
        BuildTarget.Android,
        BuildTarget.iOS,
    };
    
    [MenuItem("AssetBundelTools/�������")]
    static void showWindow()
    {
        Caching.ClearCache();
        window = (AssetBundleTools)EditorWindow.GetWindow(typeof(AssetBundleTools), false);
        window.Show();
    }

    private void OnGUI()
    {

        if (GUILayout.Button("���ģʽ"))
        {
            Dictionary<string, bool> ABMacro = new Dictionary<string, bool>() { { "ASSETBUNDLE", true } };

            //Debug.Log(Version + "" + TargetPlatformId);
            //Build(buildTargets[TargetPlatformId], Version);
           // string Macro = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            string  Macro = string.Empty;
            foreach (var item in ABMacro)
            {
                if (item.Value)
                {
                    Macro += string.Format("{0};", item.Key);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, Macro);
            Debug.Log(Macro);
            window.Close();
        }
        if (GUILayout.Button("�༭��ģʽ"))
        {
            Dictionary<string, bool> ABMacro = new Dictionary<string, bool>() { { "ASSETBUNDLE", false } };

            //Debug.Log(Version + "" + TargetPlatformId);
            //Build(buildTargets[TargetPlatformId], Version);
            // string Macro = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
            string Macro = string.Empty;
            foreach (var item in ABMacro) 
            {
                if (item.Value)
                {
                    Macro += string.Format("{0};", item.Key);
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, Macro);
            Debug.Log(Macro);
            window.Close();
        }

        Version = GUILayout.TextArea(Version);
        IsGenHotFix = GUILayout.Toggle(IsGenHotFix, "�Ƿ���������hotfix");
        IsCopyAB2StreamingAssets =GUILayout.Toggle(IsCopyAB2StreamingAssets, "�Ƿ�AB�����Ƶ�StreamingAssets(��һ�δ��ʹ��)");


        TargetPlatformId = GUILayout.Toolbar(TargetPlatformId, new[] {"Window" , "Android" ,"Ios" });
        if (GUILayout.Button("��ʼ��AB��"))
        {
            //Debug.Log(Version + "" + TargetPlatformId);
            Build(buildTargets[TargetPlatformId],Version);
        }
    }

    /// <summary>
    /// ���
    /// </summary>
    /// <param name="target"></param>
    /// <param name="version"></param>
    private void Build(BuildTarget target,string version)
    {
        Caching.ClearCache();
        string[] filePaths = Directory.GetDirectories(ProjectResourcesPath,"*.*" ,SearchOption.TopDirectoryOnly);
        string savePath = ABSavePath + target.ToString();


        DeleteOldBundelFiles(savePath);
        if (!Directory.Exists(savePath)) 
        {
            Directory.CreateDirectory(savePath);
        }

        SetAssetBundlesName(filePaths);
        BuildPipeline.BuildAssetBundles(savePath, BuildAssetBundleOptions.ChunkBasedCompression, target);

        CreateHotfixDllAndCopy(target);
        SetVersionJson(target);
        
        if (IsCopyAB2StreamingAssets)
        {
            CopyFilesRecursively(ABSavePath,Application.streamingAssetsPath+ "/ProjectResources/");
        }


    }

    /// <summary>
    /// �������µ�HotFix.dll�ļ�
    /// </summary>
    /// <param name="target"></param>
    private void GenHotFixDll(BuildTarget target)
    { 
        BuildAssetsCommand.BuildAssetBundleByTarget(target);
        CompileDllCommand.CompileDll(target);
        BuildAssetsCommand.CopyABAOTHotUpdateDlls(target);
    }


    private void CopyFilesRecursively(string sourcePath, string targetPath)
    {
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*",SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }
        foreach (string newPath in Directory.GetFiles(sourcePath,"*.*" ,SearchOption.AllDirectories))
        {
            File.Copy(newPath,newPath.Replace(sourcePath,targetPath),true);
        }
    }


    /// <summary>
    /// �����ɵ�hotFix dll ���Ƶ�AB������Ŀ¼��
    /// </summary>
    /// <param name="target"></param>
    private void CreateHotfixDllAndCopy(BuildTarget target)
    {
        if (IsGenHotFix)
        {
            GenHotFixDll(target);
        }

        string hotFixPath = Application.streamingAssetsPath + "/Assembly-CSharp.dll.bytes";
        string newPath = ABSavePath + target.ToString() + "/Assembly-CSharp.dll.bytes";
        File.Copy(hotFixPath, newPath, true);
        AssetDatabase.Refresh();
    }


    private static Dictionary<string, string> md5Dict = new Dictionary<string, string>();

    /// <summary>
    /// ���ɰ汾�Ա��ļ�
    /// </summary>
    ///  
    private void SetVersionJson(BuildTarget target)
    {
        string versionPath = ABSavePath + target.ToString() + VersionName.Replace(target.ToString(),"");
        //Debug.Log(versionPath);
        if (File.Exists(versionPath)) File.Delete(versionPath);
        JsonData versonJson = new JsonData();
        string[] allFiles = Directory.GetFiles(ABSavePath + target.ToString(), "*.*",SearchOption.AllDirectories);
        md5Dict.Clear();

        long allLength = 0; 
        foreach (var item in allFiles) 
        {
            string path = ABSavePath + target.ToString() + "/";
            string fileName = item.Replace("\\","/").Replace(path, "");
            string extension = Path.GetExtension(item);
            if (extension == ".manifest" && fileName != target.ToString()+ ".manifest")
            {
                if (File.Exists(fileName))
                {
                    Debug.Log("---->" + fileName);
                    File.Delete(fileName);
                }
                continue;
            }
            if (extension != ".meta")
            {
                string fileMd5 = Utils.GetMD5HashFromFile(item);
                int fileLen = File.ReadAllBytes(item).Length;
                allLength += fileLen;
                md5Dict.Add(fileName, fileMd5 + "+" + fileLen);
            }

        }

        JsonData filesdata = new JsonData();
        foreach (var Dictitem in md5Dict)
        {
            JsonData jd = new JsonData();
            jd["file"] = Dictitem.Key;
            string[] nAndL = Dictitem.Value.Split('+');
            jd["md5"] = nAndL[0];
            jd["length"] = nAndL[1];
            filesdata.Add(jd);
        }

        versonJson["version"] = Version;
        versonJson["length"] = allLength;
        versonJson["files"] = filesdata;
        File.WriteAllText(versionPath,versonJson.ToJson());
        md5Dict.Clear();


    }



    /// <summary>
    /// ����AB������
    /// </summary>
    /// <param name="projectPaths"></param>
    private void SetAssetBundlesName(string[] projectPaths)
    {
        foreach (var path in projectPaths)
        {
            //Debug.Log(path);
            string[] childPaths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            string childPathName, extension, directoryName;
            foreach (var childPath in childPaths)
            {
                extension = Path.GetExtension(childPath);
                Debug.Log("------->" + extension);
                if (extension != ".meta" && extension!= ".DS_Store")
                {
                    childPathName = Path.GetFileNameWithoutExtension(childPath);
                    directoryName = Path.GetDirectoryName(childPath).Replace("\\", "/");

                    AssetImporter aImp = AssetImporter.GetAtPath(childPath.Replace(Application.dataPath, "Assets"));
                    string abName = directoryName.Replace(ProjectResourcesPath, "") + "/" + childPathName;
                    aImp.assetBundleName = abName.Replace(" ","");

                }
            }
        }
    }

    /// <summary>
    /// ɾ���ɵ�AB��
    /// </summary>
    /// <param name="savePath"></param>
    private void DeleteOldBundelFiles(string savePath)
    {
        Debug.Log(savePath + Directory.Exists(savePath));
        if (Directory.Exists(savePath))
        { 
            // Directory.Delete(savePath);
            DirectoryInfo di = new DirectoryInfo(savePath);
            di.Delete(true);
        }
        
        
    }
}