using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Hotfix.UI
{
    class testView :MonoBehaviour
    {
        private void Awake()
        {
            UnityEngine.Debug.LogError("这是热更游戏脚本..... 的热更新log");

            
            Stopwatch sw = new Stopwatch();

            sw.Start();
            TestData data = TableDataManager.GetInstance().GetTableDataByType<TestData>();
            UnityEngine.Debug.Log(data.GetDataByID(1).name);

            sw.Stop();
            UnityEngine.Debug.Log(string.Format("total:{0} ms", sw.ElapsedMilliseconds));

            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            ItemData data2 = TableDataManager.GetInstance().GetTableDataByType<ItemData>();
            UnityEngine.Debug.Log(data2.GetDataByID(5).name);
            sw2.Stop();
            UnityEngine.Debug.Log(string.Format("total:{0} ms", sw2.ElapsedMilliseconds));
            
        }
    }
}
