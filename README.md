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
 * 第三方SDK (可放到热更程序集，待测)
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






















