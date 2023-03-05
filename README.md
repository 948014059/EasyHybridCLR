# 接入HybridCLR
* https://gitee.com/focus-creative-games/hybridclr_unity.git
* Window > Package Manager > + > add package from git url 
* HybridCLR > installer > 安装

# HybirdCLR 设置
* 关闭增量GC  File > BuildSettings > Player Settings > Using incremental GC
* IL2CPP 打包
* .Net 版本

# 划分程序集 （AOT/Hotfix）一般方案 
* 该方案 将更新逻辑以及资源管理脚本放到AOT中，无法修改更新逻辑以及资源管理逻辑。需要修改的话，只能更新完整包。
  
* 这里使用 Unity 默认的Assembly-CSharp 作为热更新程序集 。
* 重新建立一个AOT程序集 作为AOT程序集。
* AOT 程序集 为重新建立的 Aot.asmdef ,设置调用HybirdCLR.Runtime
* Hotfix 程序集为默认的 Assembly-CSharp , 由于是最优先的，可以调用AOT程序集。
* Global.Editor 程序集是Editor脚本，设置调用HybirdCLR.Editor 以及Aot程序。  platforms 选择 Editor。 
* Version Defines 添加
  * Resource com.focus-creative-games.hybirdclr_unity
  * NEW_HYBRIDCLR_API
  * [1.1.18,10]

* 不同程序集带有的内容
  * AOT（Aot）:
    * 第三方SDK
    * 模块管理
    * 文件管理
    * 资源管理
    * 游戏启动
    * 配置表
  * Hotfix(Assembly-CSharp):
    * 逻辑
    * 通讯
    * 事件
    * 对象池
  * Global.Editor:
    * 打包脚本
    * 批量脚本等

# 划分程序集 （AOT/ManagerHotfix/Hotfix） 可热更新热更新（本文档使用的方案）
* 该方案将资源管理脚本抽出来作为一个程序集，优点是可直接更新资源管理脚本以及热更新逻辑。但是游戏开始时需要加载多一个资源管理程序集，需要抉择加载的时机。

* 将资源管理 作为一个新的程序集
* 游戏启动时，加载Aot程序后加载资源管理程序集
  * 直接网络加载 （不能过大，影响体验）
  * 下载到本地 （需要一套校验更新逻辑）
* 由资源管理程序集进行更新

* 程序集划分（ManagerHotfix、Hotfix可根据需要划分）
  * AOT（Aot）:
    * SDK
    * 启动脚本
  * ManagerHotfix:
    * 模块管理
    * 文件管理
    * 资源管理
    * 通讯管理
    * 配置表管理
    * 游戏跟新逻辑
    * 游戏启动
  * Hotfix(Assembly-CSharp):
    * 逻辑
    * 事件系统
    * 对象池系统
  * Global.Editor:
      * 打包脚本
      * 批量脚本等


# 文件
```
Assets
-AotScript              AOT程序集
    - 3rd
    - GameLoader.cs
-Editor                 编辑器脚本
    - ...
-Hotfix                 热更程序集
    - ...
-ManagerHotfix          资源管理程序集
    - ...
-Plugins                依赖
    - ...
-ProjectResources       资源保存目录 (AB包打包目录)
    - Images
    - Sounds
    - Prefabs
    - Effects
    - ...
-Scenes                 场景
    - SampleScene       只有一个场景
-StreamingAssets        只读文件夹
    - ...
```

# AotScript
* 只存放2个东西
 * 第三方SDK (据说可以可放到热更程序集，待测)
 * GameLoader.cs
   * 程序入口
   * 单例
   * 通过协程 + UnityWebRequest 加载本地AOT程序
   * 加载云端 ManagerHotfix程序
   * 补充元数据
   * 加载热更程序集（ManagerHotfix）
   * 使用反射进入到资源管理程序集的热更新逻辑


# Editor
* 各种编辑器脚本
  * AssetBundleTools.cs 打包工具
  * BuildAssetsCommand.cs HbridyCLR 工具
  * BuildPlayerCommand.cs HbridyCLR 工具
  * RightClickItemTools.cs 右键工具
  * ....

# Hotfix
* UI
  * 处理UI逻辑脚本
* Game
  * 游戏逻辑
* ...(proto、Excel等)

# ManagerHotfix
* UnityFramework
  * Manager 各种管理脚本
  * NetWork 网络脚本（请求/下载等）
  * Base    基类
  * Update  资源更新
  * Utils   工具
  * Config.cs  配置
* StartGame.cs（UpdateModule.cs） 通过该脚本进行资源跟新，并进入到游戏主逻辑中。
  * 下载资源以及逻辑dll
  * 加载逻辑dll
  * 补充元数据
  * 加载逻辑dll程序
  * 可以开始游戏了



# AB包打包加载问题

* 打空包，资源游戏开始时下载。
  * 最少需要更新界面资源，如何热更更新界面？

* 游戏给初始资源到可读路径，进入游戏后拷贝到读写路径（使用该方法）。
  * 会造成双份资源。
  * 可以一次性给所有资源。
  * 通过判断读写路径中是否有资源 来判断 读可读路径资源还是读写路径资源。


# 配置表管理
* excel格式
  * 数据类型展示支持int、string。
  * 表格第一列数据 是控制是否导出该配置
  * 表格第一行为数据类型。
  * 第二行为提示内容summary
  * 第三行为使用名称
  * 第四行开始是数据
 
  例如
  |bool| int | string  | string  | string  |
  |----|----|----|----|----|
  |是否导出|唯一id|名称|iconUrl|ModdelUrl|
  |1|id|name|icon|modul|
  |1|1|dada1|123456|456789|
  |0|2|dada2|123456|456789|
  |1|3|dada3|123456|456789|
  |0|4|dada4|123456|456789|

  导出后的表格为：
  |id|name|icon|modul|
  |----|----|----|----|
  |1|dada1|123456|456789|
  |3|dada3|123456|456789|



* 通过python脚本生成txt和CSharp文件
  * 通过传递下列参数执行
  ```
    --excelpath EXCELPATH  #excel保存路径
    --cspath CSPATH        #生成的Csharp文件保存路径
    --txtpath TXTPATH      #生成的txt文件保存路径
    --extension EXTENSION  #excel的后缀名称，默认为xls
  ```
* 编辑器工具
  * 使用编辑器工具是为了更加无缝地生成文件
  * 主要就是使用C#运行Cmd命令，从而运行python脚本。
  * 所以，需要在编辑器工具ExcelReaderTools中获取python脚本运行时需要的参数。
  * 然后进行组装。
  * 运行 Process.Start("CMD.exe", "/k " + cmdStr);
* 配置表管理工具
  * TableDataManager.cs
  * 使用反射,创建新的BaseTable对象,填充数据
  * 将数据保存到一个新的BaseTable的data中，并使用字典缓存起来
  * 读取表数据
  * 使用 
```
  TestData data = TableDataManager.GetInstance().GetTableDataByType<TestData>();
```
















