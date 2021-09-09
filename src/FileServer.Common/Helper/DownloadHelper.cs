﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FileServer.Common.Helper
{
    public static class DownloadHelper
    {
        /*
         * 参考链接：
         * https://www.codenong.com/7948316/
         * https://www.cnblogs.com/gjahead/archive/2007/06/18/787654.html
         * https://bbs.csdn.net/topics/70424557
         * https://www.cnblogs.com/tianma3798/p/13445111.html
         */

        /// <summary>
        /// 下载文件，支持大文件、续传、速度限制。支持续传的响应头Accept-Ranges、ETag，请求头Range 。
        /// Accept-Ranges：响应头，向客户端指明，此进程支持可恢复下载.实现后台智能传输服务（BITS），值为：bytes；
        /// ETag：响应头，用于对客户端的初始（200）响应，以及来自客户端的恢复请求，
        /// 必须为每个文件提供一个唯一的ETag值（可由文件名和文件最后被修改的日期组成），这使客户端软件能够验证它们已经下载的字节块是否仍然是最新的。
        /// Range：续传的起始位置，即已经下载到客户端的字节数，值如：bytes=1474560- 。
        /// 另外：UrlEncode编码后会把文件名中的空格转换中+（+转换为%2b），但是浏览器是不能理解加号为空格的，所以在浏览器下载得到的文件，空格就变成了加号；
        /// 解决办法：UrlEncode 之后, 将 "+" 替换成 "%20"，因为浏览器将%20转换为空格
        /// </summary>
        /// <param name="httpContext">当前请求的HttpContext</param>
        /// <param name="filePath">下载文件的物理路径，含路径、文件名</param>
        /// <param name="speed">下载速度：每秒允许下载的字节数</param>
        /// <returns>true下载成功，false下载失败</returns>
        public static async Task<bool> DownloadFileAsync(HttpContext httpContext, string filePath, long speed, CancellationToken cancellationToken = default)
        {
            bool ret = true;
            try
            {
                #region--验证：HttpMethod，请求的文件是否存在
                switch (httpContext.Request.Method.ToUpper())
                { 
                    //目前只支持GET和HEAD方法
                    case "GET":
                    case "HEAD":
                        break;
                    default:
                        httpContext.Response.StatusCode = 501;
                        return false;
                }
                if (!File.Exists(filePath))
                {
                    httpContext.Response.StatusCode = 404;
                    return false;
                }
                #endregion

                #region 定义局部变量

                long startBytes = 0;
                int packSize = 1024 * 10; //分块读取，每块10K bytes
                string fileName = Path.GetFileName(filePath);
                FileStream myFile = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                BinaryReader br = new BinaryReader(myFile);
                long fileLength = myFile.Length;

                int sleep = (int)Math.Ceiling(1000.0 * packSize / speed);//毫秒数：读取下一数据块的时间间隔
                string lastUpdateTiemStr = File.GetLastWriteTimeUtc(filePath).ToString("r");
                string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + lastUpdateTiemStr;//便于恢复下载时提取请求头;

                #endregion

                #region--验证：文件是否太大，是否是续传，且在上次被请求的日期之后是否被修改过--------------

                if (myFile.Length > Int32.MaxValue)
                {
                    //-------文件太大了-------
                    httpContext.Response.StatusCode = 413;//请求实体太大
                    return false;
                }
                
                if (httpContext.Request.Headers.ContainsKey("If-Range"))//对应响应头ETag：文件名+文件最后修改时间
                {
                    if(httpContext.Request.Headers.TryGetValue("If-Range", out StringValues ifRangeValue))
                    {
                        //----------上次被请求的日期之后被修改过--------------
                        if (ifRangeValue.ToString().Replace("\"", "") != eTag)
                        {
                            //文件修改过
                            httpContext.Response.StatusCode = 412;//预处理失败
                            return false;
                        }
                    }
                }
                #endregion

                try
                {
                    #region -------添加重要响应头、解析请求头、相关验证-------------------

                    httpContext.Response.Clear();
                    httpContext.Features.Get<IHttpResponseBodyFeature>().DisableBuffering();
                    httpContext.Response.ContentType = "application/octet-stream";//MIME类型：匹配任意文件类型
                    httpContext.Response.Headers.Add("Content-MD5", MD5Helper.GetMD5Hash(myFile));//用于验证文件
                    httpContext.Response.Headers.Add("Accept-Ranges", "bytes");//重要：续传必须
                    httpContext.Response.Headers.Add("ETag", "\"" + eTag + "\"");//重要：续传必须
                    httpContext.Response.Headers.Add("Last-Modified", lastUpdateTiemStr);//把最后修改日期写入响应    
                    httpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace("+", "%20"));
                    httpContext.Response.Headers.Add("Content-Length", (fileLength - startBytes).ToString());
                    httpContext.Response.Headers.Add("Connection", "Keep-Alive");
                    if (httpContext.Request.Headers.ContainsKey("Range"))
                    {
                        if (httpContext.Request.Headers.TryGetValue("Range", out StringValues rangeValue))
                        {
                            //------如果是续传请求，则获取续传的起始位置，即已经下载到客户端的字节数------
                            httpContext.Response.StatusCode = 206;//重要：续传必须，表示局部范围响应。初始下载时默认为200
                            string[] range = rangeValue.ToString().Split(new char[] { '=', '-' });//"bytes=1474560-"
                            startBytes = Convert.ToInt64(range[1]);//已经下载的字节数，即本次下载的开始位置  
                            if (startBytes < 0 || startBytes >= fileLength)
                            {
                                // TODO: Find correct status code
                                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                await httpContext.Response.WriteAsync($"Invalid start of range: {startBytes}");
                                return false;
                            }
                        }
                    }
                    if (startBytes > 0)
                    {
                        //------如果是续传请求，告诉客户端本次的开始字节数，总长度，以便客户端将续传数据追加到startBytes位置后----------
                        httpContext.Response.Headers.Add("Content-Range", $" bytes {startBytes}-{fileLength - 1}/{fileLength}");
                    }

                    #endregion

                    #region -------向客户端发送数据块-------------------

                    br.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    int maxCount = (int)Math.Ceiling((fileLength - startBytes + 0.0) / packSize);//分块下载，剩余部分可分成的块数
                    for (int i = 0; i < maxCount; i++)
                    {
                        //客户端中断连接，则暂停
                        if (httpContext.RequestAborted.IsCancellationRequested)
                        {
                            i = maxCount;
                        }
                        else
                        {
                            await httpContext.Response.Body.WriteAsync(br.ReadBytes(packSize), cancellationToken);
                            //输出完成后，释放服务器内存空间
                            await httpContext.Response.Body.FlushAsync(cancellationToken);
                            if (sleep > 1) Thread.Sleep(sleep);
                        }
                    }

                    #endregion
                }
                catch
                {
                    ret = false;
                }
                finally
                {
                    br.Close();
                    myFile.Close();
                }
            }
            catch
            {
                ret = false;
            }
            return ret;
        }
    }
}
