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
    /// 名称
    /// </summary>
   public string name;
   
    /// <summary>
    /// 其他
    /// </summary>
   public string other;
   
    /// <summary>
    /// 然后
    /// </summary>
   public string then;
   
    /// <summary>
    /// 这个
    /// </summary>
   public string this_;
   
    /// <summary>
    /// 没有
    /// </summary>
   public string no_;

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
