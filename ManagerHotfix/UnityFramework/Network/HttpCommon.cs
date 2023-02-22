using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HttpCommon :BaseSingleTon<HttpCommon>
{
    

    //public void DownloadFile()
    //{
    //    StartCoroutine(Down());
    //}


    public void StartHttpRequest(string url, string method, Action<string> successCallBack = null, Action<string> fileCallBack = null)
    {
        Debug.Log("获取版本更新文件："+url);
        StartCoroutine(HttpRequest(url,method,successCallBack,fileCallBack));
    }

    IEnumerator HttpRequest(string url,string method ,Action<string> successCallBack = null , Action<string> fileCallBack = null)
    {
        UnityWebRequest request = new UnityWebRequest(url,method);
        request.timeout = Config.HttpTimeOut;
        DownloadHandlerBuffer Download = new DownloadHandlerBuffer();
        request.downloadHandler = Download;

        yield return request.SendWebRequest();

        if (request.isDone)
        {
            if (request.isHttpError || request.isNetworkError)
            {
                fileCallBack?.Invoke(request.error.ToString());
            }
            else
            {
                Debug.Log("httpRequest: " + request.downloadHandler.text);
                successCallBack?.Invoke(request.downloadHandler.text);
            }
        }

        
    }



}

