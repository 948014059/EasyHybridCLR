using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// 使用Http下载文件
/// </summary>
public class HttpDownLoad : DownLoadItem
{

    string tempFileExt = ".temp";
    string tempSaveFilePath;


    public HttpDownLoad(string url , string path) : base(url, path)
    {
        tempSaveFilePath = string.Format("{0}/{1}{2}", savePath, fileNameWithoutExt, tempFileExt);
    }

    public override void StartDownLoad(Action<long> callback = null , Action<long,string> finishCallBack = null)
    {
        base.StartDownLoad(callback, finishCallBack);
        System.GC.Collect();
        // 使用协程下载
        UICoroutine.GetInstance().StartConHasMaxNum(DownLoad(callback, finishCallBack));
        //UICoroutine.GetInstance().StartConHasMaxNum(test(callback, finishCallBack));
        //DownLoadItem(callback, finishCallBack);
    }

    IEnumerator DownLoad(Action<long> callback = null, Action<long, string> finishCallBack = null)
    {
        Debug.Log("开始下载：" + srcUrl);
        UnityWebRequest request = UnityWebRequest.Get(srcUrl);

        FileStream fileStream;
        if (File.Exists(tempSaveFilePath))
        {
            fileStream = File.OpenWrite(tempSaveFilePath);
            currentLength = fileStream.Length;
            fileStream.Seek(currentLength, SeekOrigin.Current); // 移动游标到最后
            //request.AddRange((int)currentLength); // 移动请求的数据标头
            request.SetRequestHeader("Range", "bytes=" + (int)currentLength + "-");
        }
        else
        {
            fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
            currentLength = 0;
        }
        yield return request.SendWebRequest();
        if (request.isDone)
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("网络错误...." + request.error);
            }
            else
            {
                //Debug.Log(request.downloadHandler.data.Length);
                Stream stream = new MemoryStream(request.downloadHandler.data);
                fileLength = request.downloadHandler.data.Length + currentLength;

                isStartDownLoad = true;
                int lengthOnce;
                int buffMaxLength = Config.BuffDownLength;
                while (currentLength < fileLength)
                {
                    byte[] buffer = new byte[buffMaxLength];
                    if (stream.CanRead)
                    {
                        lengthOnce = stream.Read(buffer, 0, buffer.Length);
                        currentLength += lengthOnce;
                        fileStream.Write(buffer, 0, lengthOnce);
                        callback?.Invoke(lengthOnce);
                    }
                    else
                    {
                        break;
                    }
                    yield return null;
                }

                isStartDownLoad = false;
                stream.Close();
                fileStream.Close();
                File.Move(tempSaveFilePath, saveFilePath);
                finishCallBack?.Invoke(GetLength(), fileNameWithoutExt);
            }
        }


    }



    IEnumerator DownLoadItem(Action<long> callback = null, Action<long, string> finishCallBack = null)
    {
        Debug.Log("开始下载："+srcUrl);
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(srcUrl);
        request.Method = "GET";
        FileStream fileStream;
        // 是否存在下载的 临时文件 （断点续传）
        if (File.Exists(tempSaveFilePath))
        {
            fileStream = File.OpenWrite(tempSaveFilePath);
            currentLength = fileStream.Length;
            fileStream.Seek(currentLength, SeekOrigin.Current); // 移动游标到最后
            request.AddRange((int)currentLength); // 移动请求的数据标头

        }
        else
        {
            fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
            currentLength = 0;
        }


        HttpWebResponse response = null;
        try
        {
            response = (HttpWebResponse)request.GetResponse();
        }
        catch (WebException webe)
        {
            if (webe.Message == "The remote server returned an error: (416) Requested Range Not Satisfiable.")
            {
                Debug.Log("已下载完成");
                fileStream.Close();
                File.Move(tempSaveFilePath, saveFilePath);
                finishCallBack?.Invoke(GetLength(), fileNameWithoutExt);
            }
        }


        if (response!=null)
        {
            Stream stream = response.GetResponseStream();
            fileLength = response.ContentLength + currentLength;

            isStartDownLoad = true;
            int lengthOnce;
            int buffMaxLength = Config.BuffDownLength;
            while (currentLength < fileLength)
            {
                byte[] buffer = new byte[buffMaxLength];
                if (stream.CanRead)
                {
                    lengthOnce = stream.Read(buffer, 0, buffer.Length);
                    currentLength += lengthOnce;
                    fileStream.Write(buffer, 0, lengthOnce);
                    callback?.Invoke(GetCurrentLength());
                }
                else
                {
                    break;
                }
                yield return null;
            }

            isStartDownLoad = false;
            response.Close();
            stream.Close();
            fileStream.Close();
            File.Move(tempSaveFilePath, saveFilePath);
            finishCallBack?.Invoke(GetLength(), fileNameWithoutExt);
        }

        
    }

    public override void Destroy()
    {
    }

    public override long GetCurrentLength()
    {
        return currentLength;
    }

    public override long GetLength()
    {
        return fileLength;
    }

    public override float GetProcess()
    {
        if(fileLength > 0)
        {
            return Mathf.Clamp((float)currentLength / fileLength, 0, 1);
        }
        return 0;
    }
}

