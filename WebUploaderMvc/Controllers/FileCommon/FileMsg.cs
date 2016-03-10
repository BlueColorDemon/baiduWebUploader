using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUploaderMvc.Controllers
{
    /// <summary>
    /// 文件处理信息
    /// @author: lzh
    /// </summary>
    public class FileMsg
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
    }
}