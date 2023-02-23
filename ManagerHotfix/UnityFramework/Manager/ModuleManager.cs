using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;



/// <summary>
/// Module 管理
/// </summary>
public class ModuleManager : BaseSingleTon<ModuleManager>
{
    private Transform UIFrom;
    private Transform TipsFrom;
    private Transform LogFrom;
    private Dictionary<string, GameObject> ModuleDict = new Dictionary<string, GameObject>();


    private void Awake()
    {

        Transform Canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        UIFrom = Canvas.Find("UI");
        LogFrom = Canvas.Find("Log");
        //Debug.Log("--:"+Canvas +"  --:"+ UIFrom);
    }

    /// <summary>
    /// 打开module  使用反射
    /// </summary>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void OpenModule(Type type , Action callBack = null)
    {
        if (type == null)
        {
            Debug.Log("Type 为Null ，请确认");
            return;
        }

        if (ModuleDict.ContainsKey(type.Name))
        {
            ModuleDict[type.Name].gameObject.SetActive(true);
            return;
        }
        
        BaseModule module = (BaseModule)System.Activator.CreateInstance(type);
        MethodInfo moduleInfo = type.GetMethod("GetView");
        Type viewType = (Type)moduleInfo.Invoke(module, null);
        Debug.Log("正在打开Module: " + type.Name + "        获取资源PreFabs:" + module.PreFabs);
        GameObject newGo = CreateGameObject(module.PreFabs, (BaseModule.LayerType)module.layer);       
        newGo.AddComponent(viewType);
        ModuleDict.Add(type.Name, newGo);
        Debug.Log("Module: " + type.Name + "已打开");
        callBack?.Invoke();
    }

    /// <summary>
    /// 关闭module
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isDes"></param>
    public void CloseModule(Type type ,bool isDes = true)
    {
        Debug.Log("正在关闭Module: " + type.Name);
        if (ModuleDict.ContainsKey(type.Name))
        {
            if (!isDes)
            {
                ModuleDict[type.Name].SetActive(false);
            }
            else
            {
                Destroy(ModuleDict[type.Name]);
                ModuleDict.Remove(type.Name);
            }

        }
    }

    /// <summary>
    /// 判断某个module是否打开
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool IsModuleOpen(Type type)
    {
        if (ModuleDict.ContainsKey(type.Name) && ModuleDict[type.Name].activeInHierarchy)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 删除所有的module
    /// </summary>
    public void DesAllModule()
    {
        foreach (var item in ModuleDict)
        {
            Destroy(item.Value);
        }
    }

    /// <summary>
    /// 创建GameObject
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private GameObject CreateGameObject(string path, BaseModule.LayerType type)
    {
        GameObject go = ResourcesManager.GetPrefab(path);

        if (go != null)
        {
            GameObject newGo = Instantiate(go);
            newGo.transform.SetParent(GetFromByType(type));
            Debug.Log("设置父物体："+GetFromByType(type)+ 
                "type:"+ type + "  UIFrom:" + UIFrom);
            newGo.transform.localPosition = Vector3.zero;
            newGo.transform.localScale = Vector3.one;
            newGo.gameObject.SetActive(true);
            return newGo;
        }
        Debug.Log("未找到：" + path);
        return null;
        
    }




    /// <summary>
    /// 获取根路径
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private Transform GetFromByType(BaseModule.LayerType type)
    {
        Transform tf = null;
        switch (type)
        {
            case BaseModule.LayerType.UI:
                tf = UIFrom;
                break;
            case BaseModule.LayerType.Tips:
                break;
            case BaseModule.LayerType.Log:
                tf = LogFrom;
                break;
            default:
                break;
        }
        return tf;
    }
}
