using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 开启下载协程
/// </summary>
public class UICoroutine : BaseSingleTon<UICoroutine>
{

    public static string NEXT_DOWNLOAD_COROUTINE = "next_download_coroutine";

    private int maxDownCoroutine;
    private int currDownCoroutine = 0;
    private Queue<IEnumerator> downConQueue = new Queue<IEnumerator>();



    private void Awake()
    {
        maxDownCoroutine = Config.MaxDownCoroutine;
        EventCenter.GetInstance().AddEventListener(NEXT_DOWNLOAD_COROUTINE, StartNextIE);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        EventCenter.GetInstance().RemoveEventListener(NEXT_DOWNLOAD_COROUTINE, StartNextIE);

    }

    public void StartConHasMaxNum( IEnumerator con)
   {
        downConQueue.Enqueue(con);
        if (currDownCoroutine < maxDownCoroutine)
        {
            StartIE();
        }
    }

    public void StartNextIE(System.Object obj)
    {
        currDownCoroutine--;
        if (currDownCoroutine < maxDownCoroutine)
        {
            StartIE();
        }
        Debug.Log("当前下载协程数量：" + currDownCoroutine);

    }


    public void StartIE()
    {
        if (downConQueue.Count>0)
        {
            IEnumerator getfromQueue = downConQueue.Dequeue();
            StartCoroutine(getfromQueue);
            currDownCoroutine++;
        }
        
    }

    




}
