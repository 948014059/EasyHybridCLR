using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class RightClickItemTools
{
    [MenuItem("GameObject/复制路径", false,0)]
    private static void CopyPanelRelativePath()
    {
        //Debug.Log(Selection.objects[0].name);
        GameObject obj = Selection.objects[0] as GameObject;
        string path = obj.name;
        Transform parent = obj.transform.parent;
        while (parent)
        {
            if (parent.name.Contains("Panel"))
            {
                break;
            }
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        Debug.Log(path);
    }
} 

