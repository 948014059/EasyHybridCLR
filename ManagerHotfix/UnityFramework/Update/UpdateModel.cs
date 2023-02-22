using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class UpdateModel : BaseModel
{
    public class VersionData
    {
        public string version;
        public long length;
        public List<FileData> filedatas = new List<FileData>();
    }

    public class FileData
    {
        public string filename;
        public string md5;
        public string length;
    }


    public List<FileData> needHotUpdateList = new List<FileData>();
    public long needUpdateFileLength = 0;
    public int currDownNum = 0;
    public long currDownLength = 0;
    public float downLoadTime = 0;
    public bool IsStartDown = false;
    public string serverData;


    public override void Init()
    {
    }

    public override void ResetModel()
    {
    }





    

    private void RemoveSameNameFromList(string name)
    {

        foreach (var item in needHotUpdateList)
        {
            //Debug.Log(item.filename.Trim() + "-----"+ name.Trim() + "---->"+ string.Compare(name.Trim(), item.filename.Trim()).ToString()+
            //    "-" + item.filename.Trim().Length +"-"+ name.Trim().Length);
            if (item.filename.Trim() == name.Trim())
            {
                needHotUpdateList.Remove(item);
                break;
            }
        }
    }
    public void SaveDownLoadFileTemp(string Str)
    {

        using (StreamWriter sw = File.AppendText(Config.ABPath + Config.VersionTempName))
        {
            sw.WriteLineAsync(Str);
        }
    }

    public void CheckDownLoadTempFile()
    {
        if (File.Exists(Config.ABPath + Config.VersionTempName))
        {
            string downFileTempStr = Utils.GetLocalFileData(Config.ABPath + Config.VersionTempName);
            string[] TempList = downFileTempStr.Split("\n");
            foreach (var item in TempList)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    RemoveSameNameFromList(item);
                }
            }
        }
    }

    /// <summary>
    /// 解析版本json
    /// </summary>
    /// <param name="jsonStr"></param>
    /// <returns></returns>
    public VersionData GetVersionJsonData(string jsonStr)
    {
        JsonData jsonData = JsonMapper.ToObject(jsonStr);
        VersionData versiondata = new VersionData();
        versiondata.version = (string)jsonData["version"];
        versiondata.length = long.Parse(jsonData["length"].ToString());
        foreach (JsonData item in jsonData["files"])
        {
            FileData fileData = new FileData();
            fileData.filename = (string)item["file"];
            fileData.md5 = (string)item["md5"];
            fileData.length = (string)item["length"];
            versiondata.filedatas.Add(fileData);
        }
        return versiondata;
    }



    public void ContrastVersion(List<FileData> serverDataList, List<FileData> localDataList)
    {
        foreach (FileData item in serverDataList)
        {
            if (NeedUpdateFile(localDataList, item))
            {
                //Debug.Log("需要跟新:" + item.filename);
                needHotUpdateList.Add(item);
            }
        }
    }


    private bool NeedUpdateFile(List<FileData> localData, FileData filedata)
    {
        foreach (var item in localData)
        {
            if (item.filename == filedata.filename && item.md5 == filedata.md5)
            {
                return false;
            }
        }
        return true;
    }

}

