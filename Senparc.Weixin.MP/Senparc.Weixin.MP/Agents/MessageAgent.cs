﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;

namespace Senparc.Weixin.MP.Agent
{
    /// <summary>
    /// 代理请求
    /// 注意！使用代理必然导致网络访问节点增加，会加重响应延时，
    ///       因此建议准备至少2-3秒的延迟时间的准备，
    ///       如果增加2-3秒后远远超过5秒的微信服务器等待时间，
    ///       需要慎重使用，否则可能导致用户无法收到消息。
    /// 
    /// 此外这个类中的方法也可以用于模拟服务器发送消息到自己的服务器进行测试。
    /// </summary>
    public static class MessageAgent
    {
        /// <summary>
        /// 获取Xml结果。
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string RequestXml(string url, string token, Stream stream)
        {
            string timestamp = DateTime.Now.Ticks.ToString();
            string nonce = "GodBlessYou";
            string echostr = Guid.NewGuid().ToString("n");
            string signature = CheckSignature.GetSignature(timestamp, nonce, token);
            url += string.Format("{0}signature={1}&timestamp={2}&nonce={3}&echostr={4}",
                    url.Contains("?") ? null : "?", signature, timestamp, nonce, echostr);
            var responseXml = HttpUtility.RequestUtility.HttpPost(url, null, stream);
            return responseXml;
        }

        /// <summary>
        /// 获取Xml结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string RequestXml(string url, string token, string xml)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //这里用ms模拟Request.InputStream
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(xml);
                    sw.Flush();
                    sw.BaseStream.Position = 0;
                    return RequestXml(url, token, sw.BaseStream);
                }
            }
        }
    }
}
