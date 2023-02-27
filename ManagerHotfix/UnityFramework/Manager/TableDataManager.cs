using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public  class TableDataManager :BaseSingleTon<TableDataManager>
{

    //private Dictionary<string,List<object>> AllTableDataDict = new Dictionary<string, List<object>> ();

    private Dictionary<string, BaseTable> AllTableDataDict = new Dictionary<string, BaseTable>();

    public T GetTableDataByType<T>() where T : BaseTable
    {
        BaseTable tableObj = System.Activator.CreateInstance<T>();
        if (tableObj == null)
        {
            Debug.Log("未找到对应配置表cs");
            return null;
        }
        if (AllTableDataDict.ContainsKey(typeof(T).Name))
        {
            return AllTableDataDict[typeof(T).Name] as T;
        }
        else
        {
            string tablePath = tableObj.GetTablePath();
            string tableData = ResourcesManager.GetTextAsset(tablePath);
            return AddTableData<T>(tableData, tableObj);
        }
        
    }

    public T AddTableData<T>(string tableData,BaseTable table) where T : BaseTable
    {
        string[] dataLines = tableData.Split('\n');
        string[] variableNames = new string[] { };
        List<T> tableline = new List<T>();
        for (int i = 0; i < dataLines.Length; i++)
        {
            if (dataLines[i] == "")
            {
                continue;
            }
            if (i == 0)
            {
                variableNames = dataLines[0].Split(',');
            }
            else if (i >= 1)
            {
                string[] lineSplit = dataLines[i].Split(',');
                T configDataLineObj = Activator.CreateInstance<T>();

                for (int j = 0; j < variableNames.Length; j++)
                {
                    FieldInfo variable = typeof(T).GetField(variableNames[j].Trim());
                    switch (variable.FieldType.ToString())
                    {
                        case "System.Int32":
                            variable.SetValue(configDataLineObj, int.Parse(lineSplit[j]));
                            break;
                        case "System.String":
                            variable.SetValue(configDataLineObj, lineSplit[j]);
                            break;
                        default:
                            break;
                    }
                }
                tableline.Add(configDataLineObj);
            }
        }
        FieldInfo dataList = typeof(T).GetField("data");
        dataList.SetValue(table, tableline);
        AllTableDataDict.Add(typeof(T).Name, table);
        return table as T;
    }




    //public List<T> GetTableDataByType<T>() where T :BaseTable
    //{
    //    T table = System.Activator.CreateInstance<T>();
    //    if (table == null)
    //    {
    //        Debug.Log("未找到对应配置表");
    //        return null;
    //    }

    //    // 获取数据并转换成对应类型
    //    if (AllTableDataDict.ContainsKey(typeof(T).Name))
    //    {
    //        return AllTableDataDict[typeof(T).Name].Select(c => (T)c).ToList();
    //        //return AllTableDataDict[typeof(T).Name];
    //    }
    //    else
    //    {
    //        string tablePath = table.GetTablePath();
    //        string tableData = ResourcesManager.GetTextAsset(tablePath);
    //        List<object> data = AddTableData<T>(tableData);
    //        return data.Select(c => (T)c).ToList();
    //        //return data;
    //    }
    //}

    ///// <summary>
    ///// 读取txt表数据，并实例化到List<object>中。
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="tableData"></param>
    ///// <returns></returns>
    //private List<object> AddTableData<T>(string tableData) where T :BaseTable
    //{
    //    string[] dataLines = tableData.Split('\n');
    //    string[] variableNames = new string[] { };
    //    List<object> tableline = new List<object>();


    //    for (int i = 0; i < dataLines.Length; i++)
    //    {
    //        if (dataLines[i] == "")
    //        {
    //            continue;
    //        }
    //        if (i == 1)
    //        {
    //            variableNames = dataLines[1].Split(',');
    //        }
    //        else if (i >= 2)
    //        {
    //            string[] lineSplit = dataLines[i].Split(',');
    //            BaseTable configDataLineObj = (BaseTable)Activator.CreateInstance<T>();

    //            for (int j = 0; j < variableNames.Length; j++)
    //            {
    //                FieldInfo variable = typeof(T).GetField(variableNames[j].Trim());
    //                switch (variable.FieldType.ToString())
    //                {
    //                    case "System.Int32":
    //                        variable.SetValue(configDataLineObj, int.Parse(lineSplit[j]));
    //                        break;
    //                    case "System.String":
    //                        variable.SetValue(configDataLineObj, lineSplit[j]);
    //                        break;
    //                    default:
    //                        break;
    //                }
    //            }
    //            tableline.Add(configDataLineObj);
    //        }          
    //    }
    //    AllTableDataDict.Add(typeof(T).Name, tableline);        
    //    return tableline;
    //}




}
