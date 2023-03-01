using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class TestData : BaseTable
{
   
    /// <summary>
    /// 唯一id
    /// </summary>
   public int id;
   
    /// <summary>
    /// 名称1
    /// </summary>
   public string name;
   
    /// <summary>
    /// icon地址1
    /// </summary>
   public string icon;
   
    /// <summary>
    /// 模型地址1
    /// </summary>
   public string model;
   
    /// <summary>
    /// 类型1
    /// </summary>
   public string type;
   
    /// <summary>
    /// 提示1
    /// </summary>
   public string tips;

   public List<TestData> data = new List<TestData>();

   public TestData GetDataByID(int id)
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
       return "ExcelData/TestData";
   }
}
