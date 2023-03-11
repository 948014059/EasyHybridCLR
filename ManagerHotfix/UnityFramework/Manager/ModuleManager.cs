using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;



/// <summary>
/// Module ����
/// </summary>
public class ModuleManager : BaseSingleTon<ModuleManager>
{
    private Transform UIFrom;
    private Transform TipsFrom;
    private Transform LogFrom;
    private Dictionary<string, GameObject> ModuleDict = new Dictionary<string, GameObject>();
    public Queue<System.Object> openModelObj = new Queue<System.Object>(); // ��UI������Ϣ����


    private void Awake()
    {

        Transform Canvas = GameObject.FindGameObjectWithTag("Canvas").transform;
        UIFrom = Canvas.Find("UI");
        LogFrom = Canvas.Find("Log");
        //Debug.Log("--:"+Canvas +"  --:"+ UIFrom);
    }

    /// <summary>
    /// ��module  ʹ�÷���
    /// </summary>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void OpenModule(Type type , Action callBack = null)
    {
        if (type == null)
        {
            Debug.Log("Type ΪNull ����ȷ��");
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
        Debug.Log("���ڴ�Module: " + type.Name + "        ��ȡ��ԴPreFabs:" + module.PreFabs);
        GameObject newGo = CreateGameObject(module.PreFabs, (BaseModule.LayerType)module.layer);       
        newGo.AddComponent(viewType);
        ModuleDict.Add(type.Name, newGo);
        Debug.Log("Module: " + type.Name + "�Ѵ�");
        callBack?.Invoke();
    }

    /// <summary>
    /// ʹ�÷��ʹ� ���ҿ��Դ���Obj(ֻ�д�ʱ������ֵ,�򿪺����ز��ᴫ��)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="callBack"></param>
    /// <param name="obj"></param>
    public void OpenModule<T>( Action callBack = null,System.Object obj = null) where T : BaseModule
    {
        if (ModuleDict.ContainsKey(typeof(T).Name))
        {
            ModuleDict[typeof(T).Name].gameObject.SetActive(true);
            return;
        }
        BaseModule module = (T)System.Activator.CreateInstance(typeof(T));
        Type viewType = module.GetView();
        openModelObj.Enqueue(obj);
        GameObject newGo = CreateGameObject(module.PreFabs, (BaseModule.LayerType)module.layer);
        newGo.AddComponent(viewType);
        ModuleDict.Add(typeof(T).Name, newGo);
        Debug.Log("Module: " + typeof(T).Name + "�Ѵ�");
        callBack?.Invoke();
    }


    /// <summary>
    /// �ر�module
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isDes"></param>
    public void CloseModule(Type type ,bool isDes = true)
    {
        Debug.Log("���ڹر�Module: " + type.Name);
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
    /// ���͹ر�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="isDes"></param>
    public  void CloseModule<T>(bool isDes = true) where T : BaseModule
    {
        Debug.Log("���ڹر�Module: " + typeof(T).Name);
        if (ModuleDict.ContainsKey(typeof(T).Name))
        {
            if (!isDes)
            {
                ModuleDict[typeof(T).Name].SetActive(false);
            }
            else
            {
                Destroy(ModuleDict[typeof(T).Name]);
                ModuleDict.Remove(typeof(T).Name);
            }

        }
    }

    /// <summary>
    /// �ж�ĳ��module�Ƿ��
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
    /// �����ж�ĳ��UI�Ƿ��
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool IsModuleOpen<T>()
    {
        if (ModuleDict.ContainsKey(typeof(T).Name) && ModuleDict[typeof(T).Name].activeInHierarchy)
        {
            return true;
        }
        return false;
    }


    /// <summary>
    /// ɾ�����е�module
    /// </summary>
    public void DesAllModule()
    {
        foreach (var item in ModuleDict)
        {
            Destroy(item.Value);
        }
    }

    /// <summary>
    /// ����GameObject
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
            Debug.Log("���ø����壺"+GetFromByType(type)+ 
                "type:"+ type + "  UIFrom:" + UIFrom);
            newGo.transform.localPosition = Vector3.zero;
            newGo.transform.localScale = Vector3.one;
            newGo.gameObject.SetActive(true);
            return newGo;
        }
        Debug.Log("δ�ҵ���" + path);
        return null;
        
    }


    /// <summary>
    /// ��ȡ��·��
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
