
## 修改Nuget缓存包位置
NuGet包默认缓存位置：C:\Users\{username}\.nuget\packages

本地包的默认位置：C:\Program Files (x86)\Microsoft SDKs\NuGetPackages //使用VS 工具 选项 Nuget包管理器  这里面也可以看到 

配置文件地址：C:\Program Files (x86)\NuGet\Config   

修改配置文件为以下内容：

```
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="Microsoft Visual Studio Offline Packages" value="D:\vs2022\NuGetPackages\"/>
  </packageSources>
<config>
  <add key="globalPackagesFolder" value="D:\vs2022\.nuget\packages" />
</config>
</configuration>

```