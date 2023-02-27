using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ItemData : BaseTable
{
   
    /// <summary>
    /// 唯一id
    /// </summary>
   public int id;
   
    /// <summary>
    /// 名称
    /// </summary>
   public string name;
   
    /// <summary>
    /// icon地址
    /// </summary>
   public string icon;
   
    /// <summary>
    /// 模型地址
    /// </summary>
   public string model;
   
    /// <summary>
    /// 类型
    /// </summary>
   public string type;
   
    /// <summary>
    /// 提示
    /// </summary>
   public string tips;

   public List<ItemData> data = new List<ItemData>();

   public ItemData GetDataByID(int id)
   {
       foreach (var item in data)
       {
           if (item.id == id)
           {
               return item;
           }
       }
       return null;
   }

   public override string GetTablePath()
   {
       return "ExcelData/ItemData";
   }
}
