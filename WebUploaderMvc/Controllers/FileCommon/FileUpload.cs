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
        /// <param name="uploadNode"></param>
        /// <returns></returns>
        public static UploadResult Process(UploadNode uploadNode)
        {
            try
            {
                string ex = Path.GetExtension(uploadNode.file.FileName);
                string localName = Guid.NewGuid().ToString("N") + ex;
                if (!Directory.Exists(uploadNode.localPath))
                {
                    Directory.CreateDirectory(uploadNode.localPath);
                }
                uploadNode.file.SaveAs(Path.Combine(uploadNode.localPath, localName));

                return new UploadResult() { Status = true, LocalName = localName };
            }
            catch (Exception ex)
            {
                return new UploadResult() { Status = false, Error = ex.ToString() };
            }
        }

        /// <summary>
        /// 分片上传,分片文件上传1
        /// </summary>
        /// <param name="uploadNode"></param>
        /// <returns></returns>
        public static UploadResult ProcessSplit(UploadNode uploadNode)
        {//uploadNode.guid + uploadNode.id =唯一
            try
            {
                string guidFolder = Path.Combine(uploadNode.localPath, uploadNode.guid + uploadNode.id) + "/";
                string localName = Path.Combine(guidFolder, uploadNode.chunk.ToString());
                //建立临时传输文件夹
                if (!Directory.Exists(Path.GetDirectoryName(guidFolder)))
                {
                    Directory.CreateDirectory(guidFolder);
                }

                FileStream addFile = new FileStream(localName, FileMode.Append, FileAccess.Write);
                BinaryWriter AddWriter = new BinaryWriter(addFile);
                //获得上传的分片数据流
                HttpPostedFileBase splitFile = uploadNode.file;
                Stream stream = splitFile.InputStream;

                BinaryReader TempReader = new BinaryReader(stream);
                //将上传的分片追加到临时文件末尾
                AddWriter.Write(TempReader.ReadBytes((int)stream.Length));
                //关闭BinaryReader文件阅读器
                TempReader.Close();
                stream.Close();
                AddWriter.Close();
                addFile.Close();

                return new UploadResult() { Status = true, LocalName = localName, chunked = uploadNode.chunked, guid = uploadNode.guid, fileExt = Path.GetExtension(uploadNode.file.FileName) };
            }
            catch (Exception ex)
            {
                return new UploadResult() { Status = false, Error = ex.ToString() };
            }
        }

        /// <summary>
        /// 分片合并,分片文件上传2
        /// </summary>
        /// <param name="uploadNode"></param>
        /// <returns></returns>
        public static UploadResult MergeSplitFiles(UploadNode uploadNode)
        {//uploadNode.guid + uploadNode.id =唯一
            try
            {
                string sourcePath = Path.Combine(uploadNode.localPath, uploadNode.guid + uploadNode.id + "/");//源数据文件夹
                var fileGuid = Guid.NewGuid().ToString("N");
                string targetPath = Path.Combine(uploadNode.localPath, fileGuid + "." + uploadNode.fileExt);//合并后的文件

                DirectoryInfo dicInfo = new DirectoryInfo(sourcePath);
                if (Directory.Exists(Path.GetDirectoryName(sourcePath)))
                {
                    FileInfo[] files = dicInfo.GetFiles();
                    foreach (FileInfo file in files.OrderBy(f => int.Parse(f.Name)))
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
                    }
                    DeleteFolder(sourcePath);

                    string filePathName = string.Empty;

                    filePathName = fileGuid + uploadNode.fileExt;

                    return new UploadResult() { Status = true, LocalName = filePathName };
                }
                else
                {
                    return new UploadResult() { Status = false, Error = "（被分片文件）文件夹丢失" };
                }
            }
            catch (Exception ex)
            {
                return new UploadResult() { Status = false, Error = ex.ToString() };
            }
        }

        /// <summary>
        /// 删除文件夹及其内容,分片文件上传3
        /// </summary>
        /// <param name="dir"></param>
        private static void DeleteFolder(string strPath)
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
                    File.Delete(f);
                }
            }
            Directory.Delete(strPath, true);
        }



    }



}