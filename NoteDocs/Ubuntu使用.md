# Ubuntu使用手册

## 一、更换阿里软件源

1. 查看ubuntu的Codename

   执行命令：

   ```bash
   lsb_release -a | grep Codename | awk '{print $2}'
   # 或
   lsb_release -a 
   ```

   执行结果如下：

   ```bash
   root@rong:/mnt/c/Users/Denny# lsb_release -a
   No LSB modules are available.
   Distributor ID: Ubuntu
   Description:    Ubuntu 22.04 LTS
   Release:        22.04
   Codename:       jammy
   ```
2. 备份系统源

   ```bash
   sudo mv /etc/apt/sources.list /etc/apt/sources.list.bak
   ```
3. 写入阿里的源

   ```bash
   # 编辑文件
   vim /etc/apt/sources.list
   # 写入内容为：
   deb http://mirrors.aliyun.com/ubuntu/ jammy main restricted universe multiverse
   deb-src http://mirrors.aliyun.com/ubuntu/ jammy main restricted universe multiverse
   deb http://mirrors.aliyun.com/ubuntu/ jammy-security main restricted universe multiverse
   deb-src http://mirrors.aliyun.com/ubuntu/ jammy-security main restricted universe multiverse
   deb http://mirrors.aliyun.com/ubuntu/ jammy-updates main restricted universe multiverse
   deb-src http://mirrors.aliyun.com/ubuntu/ jammy-updates main restricted universe multiverse
   deb http://mirrors.aliyun.com/ubuntu/ jammy-proposed main restricted universe multiverse
   deb-src http://mirrors.aliyun.com/ubuntu/ jammy-proposed main restricted universe multiverse
   deb http://mirrors.aliyun.com/ubuntu/ jammy-backports main restricted universe multiverse
   deb-src http://mirrors.aliyun.com/ubuntu/ jammy-backports main restricted universe multiverse
   ```

   其中 ``jammy``为上一步查看的 ``Codename``
4. 执行更新

   ```bash
   sudo apt-get update
   ```

## 二、Ubuntu基本使用

1. 查看系统版本

   ```bash
   cat /etc/issue
   # Ubuntu 22.04 LTS \n \l
   ```
2.

## 三、安装Docker

1. 卸载旧版，准备安装新社区版Docker Engine-Community

   ```bash
   sudo apt-get remove docker docker-engine docker.io containerd runc
   ```
2. 配置docker仓库
   安装 apt 依赖包，用于通过HTTPS来获取仓库：

   ```bash
   sudo apt-get install apt-transport-https ca-certificates curl gnupg-agent software-properties-common
   ```
   添加 Docker 的官方 GPG 密钥:

   ```bash
   curl -fsSL https://mirrors.ustc.edu.cn/docker-ce/linux/ubuntu/gpg | sudo apt-key add -
   ```
   验证指纹密钥：

   ```bash
   $ sudo apt-key fingerprint 0EBFCD88

   pub   rsa4096 2017-02-22 [SCEA]
         9DC8 5822 9FC7 DD38 854A  E2D8 8D81 803C 0EBF CD88
   uid           [ unknown] Docker Release (CE deb) <docker@docker.com>
   sub   rsa4096 2017-02-22 [S]
   ```
   使用以下指令设置稳定版仓库:

   ```bash
   sudo add-apt-repository "deb [arch=amd64] https://mirrors.ustc.edu.cn/docker-ce/linux/ubuntu/ $(lsb_release -cs) stable"
   sudo apt-get update
   ```
   安装最新版本的 Docker Engine-Community 和 containerd：

   ```bash
   sudo apt-get install docker-ce docker-ce-cli containerd.io
   ```
   安装特定版本的 Docker Engine-Community，列出安装版本列表：

   ```bash
   $ apt-cache madison docker-ce
    docker-ce | 5:20.10.17~3-0~ubuntu-jammy | https://mirrors.ustc.edu.cn/docker-ce/linux/ubuntu jammy/stable amd64 Packages
    docker-ce | 5:20.10.16~3-0~ubuntu-jammy | https://mirrors.ustc.edu.cn/docker-ce/linux/ubuntu jammy/stable amd64 Packages
    docker-ce | 5:20.10.15~3-0~ubuntu-jammy | https://mirrors.ustc.edu.cn/docker-ce/linux/ubuntu jammy/stable amd64 Packages
    docker-ce | 5:20.10.14~3-0~ubuntu-jammy | https://mirrors.ustc.edu.cn/docker-ce/linux/ubuntu jammy/stable amd64 Packages
    docker-ce | 5:20.10.13~3-0~ubuntu-jammy | https://mirrors.ustc.edu.cn/docker-ce/linux/ubuntu jammy/stable amd64 Packages
   ```
   使用第二列中的版本字符串安装特定版本，例如 ``5:20.10.17~3-0~ubuntu-jammy``：

   ```bash
   sudo apt-get install docker-ce=<VERSION_STRING> docker-ce-cli=<VERSION_STRING> containerd.io
   ```
   验证docker是否安装成功：

   ```bash
   sudo docker run hello-world
   ```
   删除安装包：

   ```bash
   sudo apt-get purge docker-ce
   ```
   删除镜像、容器、配置文件等内容：

   ```bash
   sudo rm -rf /var/lib/docker
   ```
