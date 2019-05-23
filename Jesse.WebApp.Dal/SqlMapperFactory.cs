using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using IBatisNet.DataMapper;
using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper.Configuration;
using IBatisNet.DataMapper.SessionStore;
using System.Xml;
using System.IO;

namespace Jesse.WebApp.Dal
{
    /// <summary>
    /// ISqlMapper工厂类，获取单例实例
    /// </summary>
    public static class SqlMapperFactory
    {
        private static readonly Hashtable hb = new Hashtable();
        /// <summary>
        /// 获取ISqlMapper的单例实例
        /// </summary>
        /// <param name="configFile">SqlMap配置文件名称</param>
        /// <param name="isWebHost">是否web宿主，值为true 或 false</param>
        /// <returns></returns>
        public static ISqlMapper GetSqlMap(string configFile, string isWebHost)
        {
            if (string.IsNullOrEmpty(configFile))
                throw new Exception("configFile could not be null!");
            configFile = configFile.ToLower();
            if (!hb.Contains(configFile))
            {
                lock (typeof(SqlMapperFactory))
                {
                    if (!hb.Contains(configFile))
                        Initial(configFile, isWebHost);
                }
            }
            return (ISqlMapper)hb[configFile];
        }
        /// <summary>
        /// 获取ISqlMapper的单例示例
        /// </summary>
        /// <param name="strSqlConn">数据库连接</param>
        /// <param name="configFile">sqlmap配置文件</param>
        /// <param name="isWebHost">是否web宿主，值为true 或 false</param>
        /// <returns></returns>
        public static ISqlMapper GetSqlMap(string strSqlConn, string configFile, string isWebHost)
        {
            if (string.IsNullOrEmpty(configFile) || string.IsNullOrEmpty(strSqlConn))
                throw new Exception("configFile or strSqlConn could not be null!");
            if (!hb.Contains(strSqlConn))
            {
                lock (typeof(SqlMapperFactory))
                {
                    if (!hb.Contains(strSqlConn))
                        Initial(strSqlConn, configFile, isWebHost);
                }
            }
            return (ISqlMapper)hb[strSqlConn];
        }

        private static void Initial(string strSqlConn, string configFile, string isWebHost)
        {
            //ConfigureHandler handler = new ConfigureHandler(Configure);
            DomSqlMapBuilder builder = new DomSqlMapBuilder();

            //创建xmldocument，并修改连接字符串
            string sqlMapXml = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
            string sqlMapNameSpace = "http://ibatis.apache.org/dataMapper";

            XmlDocument xmlDoc = UpdateXmlNode(sqlMapXml, "//ab:dataSource[@name='SqlConn']", sqlMapNameSpace, "connectionString", strSqlConn);
            ISqlMapper sqlMapper = builder.Configure(xmlDoc);

            if (isWebHost.ToLower() == "true")
                sqlMapper.SessionStore = new HybridWebThreadSessionStore(sqlMapper.Id);
            hb.Add(strSqlConn, sqlMapper);
        }
        private static void Initial(string configFile, string isWebHost)
        {
            ConfigureHandler handler = new ConfigureHandler(Configure);
            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            ISqlMapper sqlMapper = builder.ConfigureAndWatch(configFile, handler);
            if (isWebHost.ToLower() == "true")
                sqlMapper.SessionStore = new HybridWebThreadSessionStore(sqlMapper.Id);
            hb.Add(configFile, sqlMapper);
        }

        private static void Configure(object obj)
        {
            hb.Clear();
        }
        private static XmlDocument UpdateXmlNode(string xmlPath, string nodePath, string xmlNamespace, string attribute, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlPath);

            XmlNamespaceManager xmlNameSpaceMng = new XmlNamespaceManager(xmlDoc.NameTable);
            if (!string.IsNullOrEmpty(xmlNamespace))
                xmlNameSpaceMng.AddNamespace("ab", xmlNamespace);

            XmlNode xmlNode = xmlDoc.SelectSingleNode(nodePath, xmlNameSpaceMng);
            if (xmlNode != null)
            {
                XmlElement xmlele = xmlNode as XmlElement;
                xmlele.SetAttribute(attribute, value);
                return xmlDoc;
            }
            else
                throw new Exception(string.Format("在文件{0}中未能找到节点{1}", xmlPath, nodePath));
        }
    }
}

