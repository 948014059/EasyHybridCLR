using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

class ExcelReaderTools:EditorWindow
{
    static ExcelReaderTools window;
    private string pythonScriptPath = "C:/UnityFiles/UnityHotFixFrameWork/ExcelData/Excel2CsvAndCsharp.py";
    private string exchelPath = "C:/UnityFiles/UnityHotFixFrameWork/ExcelData/";
    private string txtSavePath = "/ProjectResources/ExcelData/";
    private string csSavePath = "/Hotfix/ExcelDataRead/";
    private string excelExtension = "xls";

    [MenuItem("Tools/ExcelTool")]
    static void ShowWindow()
    {
        window = (ExcelReaderTools)EditorWindow.GetWindow(typeof(ExcelReaderTools),false);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Python脚本位置");
        pythonScriptPath = GUILayout.TextField(pythonScriptPath);
        if (GUILayout.Button("浏览"))
        {
            pythonScriptPath = EditorUtility.OpenFilePanel("选择脚本", pythonScriptPath,"py");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Excel路径");
        exchelPath = GUILayout.TextField(exchelPath);
        if (GUILayout.Button("浏览"))
        {
            exchelPath = EditorUtility.OpenFolderPanel("选择Excel路径", exchelPath, "");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("txt保存路径");
        txtSavePath = GUILayout.TextField(txtSavePath);
        if (GUILayout.Button("浏览"))
        {
            txtSavePath = EditorUtility.OpenFolderPanel("选择txt保存路径", txtSavePath,"");
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("csharp保存路径");
        csSavePath = GUILayout.TextField(csSavePath);
        if (GUILayout.Button("浏览"))
        {
            csSavePath = EditorUtility.OpenFolderPanel("选择脚本", csSavePath, "");
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("开始转换"))
        {
            //Debug.Log(pythonScriptPath+ txtSavePath+ csSavePath);
            StartExchange();
        }

    }

    private void StartExchange()
    {
        string cmdStr = 
            "python " + pythonScriptPath + 
            " --excelpath=" +exchelPath + 
            " --cspath=" + Application.dataPath +csSavePath + 
            " --txtpath=" + Application.dataPath+txtSavePath +
            " --extension=" + excelExtension;

        //UnityEngine.Debug.Log(cmdStr);

        Process.Start("CMD.exe", "/k " + cmdStr);


        //Process processCmd = new Process();
        //processCmd.StartInfo.FileName = "cmd.exe";
        //processCmd.StartInfo.RedirectStandardInput = true;
        //processCmd.StartInfo.CreateNoWindow = false;
        //processCmd.StartInfo.RedirectStandardError = true;
        //processCmd.StartInfo.RedirectStandardOutput = true;
        //processCmd.StartInfo.UseShellExecute = false;
        //processCmd.StartInfo.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;
        //processCmd.StartInfo.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
        //processCmd.Start();
        //processCmd.StandardInput.WriteLine(cmdStr);
        //processCmd.StandardInput.AutoFlush = true;
        //string output = processCmd.StandardOutput.ReadToEnd();
        //UnityEngine.Debug.Log(output);
        //processCmd.StandardInput.Flush();
    }
}

