namespace TopMethods.NetWork
{
    /// <summary>
    /// 状态码
    /// </summary>
    public enum StatusCode_Enum
    {
        /// <summary>
        /// 成功
        /// </summary>
        OK = 200,
        /// <summary>
        /// 失败
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// API请求错误
        /// </summary>
        API_Error = 500,
        /// <summary>
        /// Token失效
        /// </summary>
        Token_Invalid = 102,
    }

    /// <summary>
    /// 下载任务状态
    /// </summary>
    public enum DownLoadTaskStatus
    {
        /// <summary>
        /// 开始下载
        /// </summary>
        Start,
        /// <summary>
        /// 正在下载
        /// </summary>
        DownLoading,
        /// <summary>
        /// 下载完成
        /// </summary>
        Finish,
        /// <summary>
        /// 下载暂停
        /// </summary>
        Pause,
        /// <summary>
        /// 下载出错
        /// </summary>
        Error,
    }

    /// <summary>
    /// HTTP请求处理类型
    /// </summary>
    public enum HttpsType
    {
        /// <summary>
        /// Get 请求数据
        /// </summary>
        Request,
        /// <summary>
        /// Post 数据
        /// </summary>
        Post,
        /// <summary>
        /// 上传文件
        /// </summary>
        UpLoadFile,
        /// <summary>
        /// 下载文件
        /// </summary>
        DownLoadFile,
        /// <summary>
        /// 进度信息
        /// </summary>
        ProgressInfo,
        /// <summary>
        /// 获取图片保存在本地
        /// </summary>
        GetImage,
        IM_Upload_File,
    }


    /// <summary>
    /// HTTP请求提示信息显示类型
    /// </summary>
    public enum HttpTaskInfoType
    {
        /// <summary>
        /// 不显示提示信息
        /// </summary>
        NotShowToast,
        /// <summary>
        /// 仅提示，只显示提示信息。不会与对应请求关联。
        /// </summary>
        OnlyShowToast,
        /// <summary>
        /// 关联提示，（当网络请求不成功时）关联到对应请求
        /// </summary>
        AssociationHint
    }

}
