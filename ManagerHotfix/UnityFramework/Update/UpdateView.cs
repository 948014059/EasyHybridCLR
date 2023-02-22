using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System.IO;




public class UpdateView : MonoBehaviour
{

    private Text updatText;
    private Transform AllowUpdateTf;
    private Slider updataSlider;


    HttpDownLoad httpDownLoad;

    private UpdateModel updateModel;

    private void Awake()
    {
        updateModel = ModelManager.GetInstance().GetModelInstence<UpdateModel>();
        System.Net.ServicePointManager.DefaultConnectionLimit = 50;

        updatText = transform.Find("testTips").GetComponent<Text>();
        updataSlider = transform.Find("Slider").GetComponent<Slider>();
        AllowUpdateTf = transform.Find("Tips");
        AllowUpdateTf.gameObject.SetActive(false);
        


    }

    void Start()
    {
        CheckVersionData();
        BtnClickEvent();
    }
    void Update()
    {
        if (updateModel.IsStartDown)
        {
            updateModel.downLoadTime += Time.deltaTime;
            SetUpdateText("正在更新中...[" + updateModel.currDownNum + "/" + updateModel.needHotUpdateList.Count + "]   " + (Utils.ByteLength2KB(updateModel.currDownLength) / updateModel.downLoadTime).ToString("0.0") + "KB/S");
            updataSlider.value = (float)updateModel.currDownNum / updateModel.needHotUpdateList.Count;
            if (updateModel.currDownNum >= updateModel.needHotUpdateList.Count)
            {
                updataSlider.value = 1;
                updateModel.IsStartDown = false;
                Utils.DelFile(Config.ABPath + Config.VersionTempName);
                Utils.SaveStringToPath(updateModel.serverData, Config.ABPath + Config.VersionName);
                SetUpdateText("更新完成...");
                LoadHotFix();
            }
        }
    }


    public void CheckVersionData()
    {

        Action<string> callBack = (_serverData) => {
            updateModel.serverData = _serverData;
            SetUpdateText("正在对比本地文件...");
            updateModel.needHotUpdateList.Clear();
            UpdateModel.VersionData serverVersionData = updateModel.GetVersionJsonData(updateModel.serverData);

            string localVersion = Utils.GetLocalFileData(Config.ABPath + Config.VersionName);
            if (string.IsNullOrEmpty(localVersion)) //本地无版本配置文件
            {
                updateModel.needHotUpdateList = serverVersionData.filedatas;
            }
            else // 对比配置文件
            {
                UpdateModel.VersionData localVersionData = updateModel.GetVersionJsonData(localVersion);
                updateModel.ContrastVersion(serverVersionData.filedatas, localVersionData.filedatas);
            }

            updateModel.CheckDownLoadTempFile();

            if (updateModel.needHotUpdateList.Count > 0)
            {
                foreach (var item in updateModel.needHotUpdateList)
                {
                    updateModel.needUpdateFileLength += long.Parse(item.length);
                }

                AllowUpdateTf.gameObject.SetActive(true);
                AllowUpdateTf.Find("updateText").GetComponent<Text>().text = string.Format("version:{0} \n检测到版本跟新,大小：{1}MB \n是否跟新",
                    serverVersionData.version, Utils.ByteLength2MB(updateModel.needUpdateFileLength).ToString("0.0")
                );
            }
            else
            {
                SetUpdateText("比对完成，无需更新...");
                LoadHotFix();
            }
        };

        SetUpdateText("获取跟新文件中...");
        HttpCommon.GetInstance().StartHttpRequest(Config.UpdateUrl + Config.VersionName, "GET", (serverData) =>
        {
            callBack(serverData);
        }, (error) => {
            Debug.LogError(error);
        });

    }


    private void BtnClickEvent()
    {

        AllowUpdateTf.Find("downloadBtn").GetComponent<Button>().onClick.AddListener(()=> {
            //Debug.Log("开始更新...................");
            StartUpdate();
        });
    }

    private void StartUpdate() 
    {
        AllowUpdateTf.gameObject.SetActive(false);
        updateModel.IsStartDown = true;
        Debug.Log("需要热更数量：" + updateModel.needHotUpdateList.Count);
        for (int i = 0; i < updateModel.needHotUpdateList.Count; i++)
        {
            string fileUrl = Config.UpdateUrl + Config.PlatFrom + "/" + updateModel.needHotUpdateList[i].filename;
            httpDownLoad = new HttpDownLoad(fileUrl, Config.ABPath + Config.PlatFrom);
            httpDownLoad.StartDownLoad((currLength) =>
            {
                //long templength = currDownLength + currLength;
            },
            (fileLength, fileName) =>
            {
                updateModel.currDownNum += 1;
                updateModel.currDownLength += fileLength;
                EventCenter.GetInstance().EventTrigger(UICoroutine.NEXT_DOWNLOAD_COROUTINE);
                updateModel.SaveDownLoadFileTemp(fileName.Substring(1, fileName.Length - 1));
            });
        }
    }


    private void SetUpdateText(string text)
    {
        updatText.text = text;
    }

    private void LoadHotFix()
    {
        ABManager.GetInstance().ReLoadAssetBundle();
        GameObject.Find("GameLoader").AddComponent<LoadDll>();
    }


}
