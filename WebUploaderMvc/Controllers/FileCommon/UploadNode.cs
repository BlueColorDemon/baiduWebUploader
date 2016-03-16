using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUploaderMvc.Controllers
{
    /// <summary>
    /// 上传节点
    /// @author: lzh
    /// </summary>
    public class UploadNode
    {
        #region 分片表单

        /// <summary>
        /// 是否,分片处理
        /// </summary>
        public bool chunked { get; set; }

        /// <summary>
        /// 分片数量
        /// </summary>
        public int chunks { get; set; }
        /// <summary>
        /// 分片序号
        /// </summary>
        public int chunk { get; set; }

        #endregion

        #region 基本表单

        /// <summary>
        /// 文件ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 文件名，包括扩展名（后缀）
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 文件MIMETYPE类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 文件最后修改日期
        /// </summary>
        public string lastModifiedDate { get; set; }
        /// <summary>
        /// 文件体积（字节）
        /// </summary>
        public int size { get; set; }
        /// <summary>
        /// 客户端已上载的单独文件
        /// </summary>
        public HttpPostedFileBase file { get; set; }

        #endregion

        #region 自定义

        /// <summary>
        /// （被分片文件）编号
        /// </summary>
        public string guid { get; set; }


        /// <summary>
        /// 本地位置
        /// </summary>
        public string localPath { get; set; }

        #endregion

        #region 自定义，合并专用

        /// <summary>
        /// （被分片文件）扩展名
        /// </summary>
        public string fileExt { get; set; }

        #endregion

    }
}