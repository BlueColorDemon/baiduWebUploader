using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUploaderMvc.Controllers
{
    /// <summary>
    /// 上传结果
    /// @author: lzh
    /// </summary>
    public class UploadResult
    {
        /// <summary>
        /// 处理状态：成功、失败
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        ///  本地文件名称
        /// </summary>
        public string LocalName { get; set; }
        /// <summary>
        /// 错误描述
        /// </summary>
        public string Error { get; set; }


        #region 分片处理→合并上传接口参数使用

        /// <summary>
        /// 是否,分片处理
        /// </summary>
        public bool chunked { get; set; }
        /// <summary>
        /// （被分片文件）编号
        /// </summary>
        public string guid { get; set; }
        /// <summary>
        /// （被分片文件）扩展名
        /// </summary>
        public string fileExt { get; set; }

        #endregion
    }
}