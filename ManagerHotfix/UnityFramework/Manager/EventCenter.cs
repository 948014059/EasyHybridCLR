using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// �¼�����
/// </summary>
/// 
public class EventCenter : BaseSingleTon<EventCenter>
{
    private Dictionary<string, Action<object>> eventDict = new Dictionary<string, Action<object>>();
    public void AddEventListener(string name ,Action<object> action)  //��Ӽ���
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

    public void RemoveEventListener(string name ,Action<object> action) // �Ƴ�����
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

    public void EventTrigger(string name , object info = null) // ���ʹ����¼�
    {
        if (eventDict.ContainsKey(name))
        {
            eventDict[name]?.Invoke(info);
        }
    }



}
