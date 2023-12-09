# 1. PowerShell执行策略

 PowerShell 执行策略如下所示:

## AllSigned

* 脚本可以运行。

* 要求所有脚本和配置文件都由受信任的发布者签名，包括在本地计算机上编写的脚本。

* 在从尚未分类为受信任或不受信任的发布者运行脚本之前，提示你。

* 运行已签名但恶意的脚本的风险。
  
  ## Bypass

* 不阻止任何操作，并且没有任何警告或提示。

* 此执行策略适用于将 PowerShell 脚本内置到较大应用程序中的配置，或用于 PowerShell 是具有其自己的安全模型的程序基础的配置。
  
  ## Default

* 设置默认执行策略。

* Restricted 对于 Windows 客户端。

* 适用于 Windows 服务器的 RemoteSigned。
  
  ## RemoteSigned

* Windows 服务器计算机的默认执行策略。

* 脚本可以运行。

* 需要受信任的发布者对从 Internet 下载的脚本和配置文件（包括电子邮件和即时消息程序）进行数字签名。

* 对于在本地计算机上编写且未从 Internet 下载的脚本，不需要数字签名。

* 如果脚本未受阻止（例如使用 cmdlet），则运行从 Internet 下载且 Unblock-File 未签名的脚本。

* 从 Internet 来源（而不是 Internet）运行未签名脚本的风险，以及可能是恶意的已签名脚本的风险。
  
  ## Restricted

* Windows 客户端计算机的默认执行策略。

* 允许单个命令，但不允许脚本。

* 阻止运行所有脚本文件，包括格式化和配置文件 () 、模块脚本文件 () 和 .ps1xml .psm1 PowerShell 配置文件 .ps1 () 。
  
  ## Undefined

* 当前作用域中未设置执行策略。

* 如果所有作用域中的执行策略都是 ，则有效执行策略 Undefined 适用于 Restricted Windows 客户端 ，RemoteSigned 适用于 Windows Server。
  
  ## Unrestricted

* 不能更改非 Windows 计算机的默认执行策略。

* 未签名的脚本可以运行。 存在运行恶意脚本的风险。

* 在运行不是来自本地 Intranet 区域中的脚本和配置文件之前，警告用户。
  
  ### 备注
  
  在未区分通用命名约定 (UNC) 路径与 Internet 路径的系统上，可能不允许 UNC 路径标识的脚本使用 RemoteSigned 执行策略运行。

# 2. 执行策略范围

可以设置仅在特定范围内有效的执行策略，有效值为 **Scope MachinePolicy 、UserPolicy Process 、CurrentUser 和 LocalMachine**。 LocalMachine 是设置执行策略时的默认选项。
这些值**Scope**按优先顺序列出。 优先的策略在当前会话中有效，即使设置了限制性更强且优先级较低的策略。
有关详细信息，请参阅 [Set-ExecutionPolicy](https://docs.microsoft.com/zh-cn/previous-versions/powershell/module/microsoft.powershell.security/set-executionpolicy?view=powershell-7.1)。

## MachinePolicy

由组策略计算机的所有用户设置。

## UserPolicy

由组策略当前用户的设置。

## Process

Process 范围仅影响当前PowerShell会话。执行策略保存在环境变量中 ``$env:PSExecutionPolicyPreference`` ，而不是注册表中。关闭 PowerShell 会话后，将删除变量和值。

## CurrentUser

执行策略仅影响当前用户。 它存储在注册表**HKEY_CURRENT_USER**项中 。

## LocalMachine

执行策略会影响当前计算机上所有用户。它存储在注册表 **HKEY_LOCAL_MACHINE** 项中 。

# 3. 使用 PowerShell 管理执行策略

获取有效执行策略：

```powershell
Get-ExecutionPolicy
```

获取影响当前会话的所有执行策略，并按优先顺序显示这些策略:

```powershell
Get-ExecutionPolicy -List
```

更改执行策略：

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

删除特定范围的执行策略，将执行策略设置为 **Undefined**:

```powershell
Set-ExecutionPolicy -ExecutionPolicy Undefined -Scope CurrentUser
```
