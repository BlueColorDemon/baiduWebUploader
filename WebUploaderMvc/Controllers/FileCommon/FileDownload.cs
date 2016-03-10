using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUploaderMvc.Controllers
{
    public class FileDownload
    {
        public static void ResponseFile(string path, string showFileName, HttpContext context, bool hasfileName, bool isIE = false)
        {
            context = HttpContext.Current;

            System.IO.Stream iStream = null;
            byte[] buffer = new Byte[10000];
            int length;
            long dataToRead;
            string filename;
            if (!hasfileName)
            {
                filename = System.IO.Path.GetFileName(path);
                filename = showFileName;
            }
            else
            {
                filename = "down_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                filename = showFileName;
            }

            try
            {
                iStream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                dataToRead = iStream.Length;
                context.Response.ContentType = "application/octet-stream";
                if (isIE)
                {
                    //ie浏览器
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8));
                }
                else
                {
                    //火狐浏览器，或者非ie浏览器
                    context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);//多此一举
                }

                while (dataToRead > 0)
                {
                    if (context.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        context.Response.OutputStream.Write(buffer, 0, length);
                        context.Response.Flush();

                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.Write(ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    iStream.Close();
                }
            }
        }


    }
}