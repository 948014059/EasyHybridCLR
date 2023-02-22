using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 事件中心
/// </summary>
/// 
public class EventCenter : BaseSingleTon<EventCenter>
{
    private Dictionary<string, Action<object>> eventDict = new Dictionary<string, Action<object>>();
    public void AddEventListener(string name ,Action<object> action)  //添加监听
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name] += action;
        }
        else
        {
            eventDict.Add(name, action);
        }
    }

    public void RemoveEventListener(string name ,Action<object> action) // 移除监听
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name] -= action;
        }
    }

    public void ClearEvent()
    {
        eventDict.Clear();
    }

    public void EventTrigger(string name , object info = null) // 发送触发事件
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name]?.Invoke(info);
        }
    }



}
