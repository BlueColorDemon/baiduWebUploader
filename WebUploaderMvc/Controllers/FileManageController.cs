using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebUploaderMvc.Controllers
{
    public class FileManageController : Controller
    {
        // GET: FileManage
        public ActionResult Index()
        {
            return View();
        }

        public void Upload()
        {
            var uploadNode = new UploadNode();

            uploadNode.chunked = Request["chunks"] == null ? false : true;
            uploadNode.guid = Request["guid"] == null ? null : Request["guid"];//自定义
            uploadNode.fileExt = Request["fileExt"] == null ? null : Request["fileExt"];//自定义，合并专用
            uploadNode.chunks = Request["chunks"] == null ? 0 : int.Parse(Request["chunks"]);
            uploadNode.chunk = Request["chunk"] == null ? 0 : int.Parse(Request["chunk"]);


            uploadNode.id = Request["id"] == null ? null : Request["id"];
            uploadNode.name = Request["name"] == null ? null : Request["name"];
            uploadNode.type = Request["type"] == null ? null : Request["type"];
            uploadNode.lastModifiedDate = Request["lastModifiedDate"] == null ? null : Request["lastModifiedDate"];
            uploadNode.size = Request["size"] == null ? 0 : int.Parse(Request["size"]);
            uploadNode.file = Request.Files.Count == 0 ? null : Request.Files[0];
            string saveFolder = Request["saveFolder"] == null ? null : Request["saveFolder"];
            uploadNode.localPath = System.IO.Path.Combine(Server.MapPath("~/Upload"), saveFolder);//自定义


            UploadResult uploadReslut = null;

            if (uploadNode.fileExt == null && uploadNode.chunks == 0)
            {//上传
                uploadReslut = FileUpload.Process(uploadNode);
            }
            else if (uploadNode.fileExt == null && uploadNode.chunks > 0)
            {//分片上传
                uploadReslut = FileUpload.ProcessSplit(uploadNode);
            }
            else if (uploadNode.fileExt != null && uploadNode.chunks == 0)
            {//合并分片
                uploadReslut = FileUpload.MergeSplitFiles(uploadNode);
            }

            //string json = JsonConvert.SerializeObject(uploadReslut);
            //HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(json, Encoding.GetEncoding("UTF-8"), "application/json") };
            //return result;
            Response.Write(JsonConvert.SerializeObject(uploadReslut));
        }



    }
}