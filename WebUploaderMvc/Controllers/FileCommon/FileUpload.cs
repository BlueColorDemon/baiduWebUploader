using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebUploaderMvc.Controllers
{
    public class FileUpload
    {
        /// <summary>
        /// 一次性上传
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="lastModifiedDate"></param>
        /// <param name="size"></param>
        /// <param name="file"></param>
        /// <param name="localPath">本地路径</param>
        /// <returns></returns>
        public static FileMsg ProcessSplit(string id, string name, string type, string lastModifiedDate, int size, HttpPostedFileBase file, string localPath)
        {
            try
            {
                string ex = Path.GetExtension(file.FileName);
                string localName = Guid.NewGuid().ToString("N") + ex;
                if (!System.IO.Directory.Exists(localPath))
                {
                    System.IO.Directory.CreateDirectory(localPath);
                }
                file.SaveAs(Path.Combine(localPath, localName));

                return new FileMsg() { Status = true, LocalName = localName };
            }
            catch (Exception ex)
            {
                return new FileMsg() { Status = false, Error = ex.ToString() };
            }
        }

        /// <summary>
        /// 分片上传
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="lastModifiedDate"></param>
        /// <param name="size"></param>
        /// <param name="chunks"></param>
        /// <param name="chunk"></param>
        /// <param name="file"></param>
        /// <param name="localPath">本地路径</param>
        /// <returns></returns>
        public static FileMsg ProcessSplit(string id, string name, string type, string lastModifiedDate, int size, int chunks, int chunk, HttpPostedFileBase file, string localPath)
        {


            return null;
        }


        public void MergeFiles(HttpContext context)
        {

        }






        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ResData_ProcessSplit(HttpContext context)
        {

            string urlPath="";
            #region 请求参数
            string id = context.Request["id"] == null ? "" : context.Request["id"];
            string parentFolder = context.Request["parentFolder"] == null ? "" : context.Request["parentFolder"];
            string name = context.Request["name"] == null ? "" : context.Request["name"];
            string type = context.Request["type"] == null ? "" : context.Request["type"];
            string lastModifiedDate = context.Request["lastModifiedDate"] == null ? "" : context.Request["lastModifiedDate"];
            int size = context.Request["size"] == null ? -1 : int.Parse(context.Request["size"]);
            HttpPostedFile file = context.Request.Files[0];
            #endregion

            //如果进行了分片
            if (context.Request.Form.AllKeys.Any(m => m == "chunk"))
            {
                #region 分片上传
                //取得chunk和chunks
                int chunk = Convert.ToInt32(context.Request.Form["chunk"]);//当前分片在上传分片中的顺序（从0开始）
                int chunks = Convert.ToInt32(context.Request.Form["chunks"]);//总分片数
                //根据GUID创建用该GUID命名的临时文件夹
                string localPath = Path.Combine(urlPath, parentFolder);
                string folder = context.Server.MapPath(localPath + "/" + context.Request["guid"] + "/");
                string path = folder + chunk;

                //建立临时传输文件夹
                if (!Directory.Exists(Path.GetDirectoryName(folder)))
                {
                    Directory.CreateDirectory(folder);
                }

                FileStream addFile = new FileStream(path, FileMode.Append, FileAccess.Write);
                BinaryWriter AddWriter = new BinaryWriter(addFile);
                //获得上传的分片数据流
                HttpPostedFile splitFile = context.Request.Files[0];
                Stream stream = splitFile.InputStream;

                BinaryReader TempReader = new BinaryReader(stream);
                //将上传的分片追加到临时文件末尾
                AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                //关闭BinaryReader文件阅读器
                TempReader.Close();
                stream.Close();
                AddWriter.Close();
                addFile.Close();

                TempReader.Dispose();
                stream.Dispose();
                AddWriter.Dispose();
                addFile.Dispose();

                var fileJson = new { chunked = true, hasError = false, f_ext = Path.GetExtension(splitFile.FileName) };
                context.Response.Write(JsonConvert.SerializeObject(fileJson));
                #endregion
            }
            else//没有分片直接保存
            {
                string filePathName = string.Empty;

                string localPath = Path.Combine(HttpRuntime.AppDomainAppPath, "Upload", parentFolder);
                if (context.Request.Files.Count == 0)
                {
                    var tempJson = new { chunked = false, hasError = false, jsonrpc = 2.0, error = new { code = 102, message = "保存失败" }, id = "id" };
                    context.Response.Write(JsonConvert.SerializeObject(tempJson));
                }

                string ex = Path.GetExtension(file.FileName);
                filePathName = Guid.NewGuid().ToString("N") + ex;
                if (!System.IO.Directory.Exists(localPath))
                {
                    System.IO.Directory.CreateDirectory(localPath);
                }
                file.SaveAs(Path.Combine(localPath, filePathName));

                var fileJson = new { chunked = false, hasError = false, jsonrpc = "2.0", id = "id", filePath = Path.Combine(urlPath, parentFolder, filePathName) };
                context.Response.Write(JsonConvert.SerializeObject(fileJson));
            }
        }

        /// <summary>
        /// 合并分片文件,分片文件上传2
        /// </summary>
        /// <param name="context"></param>
        public void ResData_MergeFiles(HttpContext context)
        {
            #region 请求参数
            string id = context.Request["id"] == null ? "" : context.Request["id"];
            string parentFolder = context.Request["parentFolder"] == null ? "" : context.Request["parentFolder"];
            string name = context.Request["name"] == null ? "" : context.Request["name"];
            string type = context.Request["type"] == null ? "" : context.Request["type"];
            string lastModifiedDate = context.Request["lastModifiedDate"] == null ? "" : context.Request["lastModifiedDate"];
            int size = context.Request["size"] == null ? -1 : int.Parse(context.Request["size"]);
            //HttpPostedFile file = context.Request.Files[0];
            #endregion
            string guid = context.Request["guid"];
            string fileExt = context.Request["fileExt"];
            string localPath = Path.Combine(urlPath, parentFolder);
            string root = context.Server.MapPath(localPath);
            string sourcePath = Path.Combine(root, guid + "/");//源数据文件夹
            var fileGuid = Guid.NewGuid().ToString("N");
            string targetPath = Path.Combine(root, fileGuid + fileExt);//合并后的文件

            DirectoryInfo dicInfo = new DirectoryInfo(sourcePath);
            if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
            {
                System.IO.FileInfo[] files = dicInfo.GetFiles();
                foreach (System.IO.FileInfo file in files.OrderBy(f => int.Parse(f.Name)))
                {
                    FileStream addFile = new FileStream(targetPath, FileMode.Append, FileAccess.Write);
                    BinaryWriter AddWriter = new BinaryWriter(addFile);

                    //获得上传的分片数据流
                    Stream stream = file.Open(FileMode.Open);
                    BinaryReader TempReader = new BinaryReader(stream);
                    //将上传的分片追加到临时文件末尾
                    AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                    //关闭BinaryReader文件阅读器
                    TempReader.Close();
                    stream.Close();
                    AddWriter.Close();
                    addFile.Close();

                    TempReader.Dispose();
                    stream.Dispose();
                    AddWriter.Dispose();
                    addFile.Dispose();
                }
                DeleteFolder(sourcePath);

                string filePathName = string.Empty;

                string ex = fileExt;
                filePathName = fileGuid + ex;

                var fileJson = new { chunked = true, hasError = false, savePath = targetPath, jsonrpc = "2.0", id = "id", filePath = Path.Combine(urlPath, parentFolder, filePathName) };
                context.Response.Write(JsonConvert.SerializeObject(fileJson));


                //var fileJson = new { chunked = false, hasError = false, jsonrpc = "2.0", id = "id", filePath = Path.Combine(urlPath, parentFolder, filePathName) };
                //context.Response.Write(JsonConvert.SerializeObject(fileJson));
            }
            else
            {
                context.Response.Write("{\"hasError\" : true}");
            }
        }

        /// <summary>
        /// 删除文件夹及其内容,分片文件上传3
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string strPath)
        {
            //删除这个目录下的所有子目录
            if (Directory.GetDirectories(strPath).Length > 0)
            {
                foreach (string fl in Directory.GetDirectories(strPath))
                {
                    Directory.Delete(fl, true);
                }
            }
            //删除这个目录下的所有文件
            if (Directory.GetFiles(strPath).Length > 0)
            {
                foreach (string f in Directory.GetFiles(strPath))
                {
                    System.IO.File.Delete(f);
                }
            }
            Directory.Delete(strPath, true);
        }



    }



}