using System;
using System.IO;
using System.Text;
using System.Xml;

namespace UnityMethods.Files
{
    public class OperateSettingsFile
    {
        public OperateSettingsFile() { }

        private static readonly string SettingsPath = "Settings.config";

        #region 读Settings文件

        public static string ReadSettingsData(string section, string key, string noText)
        {
            if (File.Exists(SettingsPath))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                    {
                        IgnoreComments = true   //忽略文档里面的注释
                    };
                    XmlReader reader = XmlReader.Create(SettingsPath, xmlReaderSettings);
                    xmlDocument.Load(reader);
                    reader.Close();

                    XmlNode keyNode = xmlDocument.SelectSingleNode("Configuration/" + section + "/" + key);
                    if (null != keyNode)
                    {
                        return keyNode.InnerText;
                    }
                    else
                    {
                        return noText;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return "";
            }
        }

        #endregion

        #region 写Settings文件

        public static bool WriteSettingsData(string section, string key, string value)
        {
            if (!File.Exists(SettingsPath))
            {
                try
                {
                    XmlTextWriter myXmlTextWriter = new XmlTextWriter(SettingsPath, Encoding.UTF8)
                    {
                        Formatting = Formatting.Indented
                    };

                    myXmlTextWriter.WriteStartDocument(true);
                    myXmlTextWriter.WriteStartElement("Configuration");
                    myXmlTextWriter.WriteEndElement();

                    myXmlTextWriter.Flush();
                    myXmlTextWriter.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            if (File.Exists(SettingsPath))
            {
                try
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                    {
                        IgnoreComments = true   //忽略文档里面的注释
                    };
                    XmlReader reader = XmlReader.Create(SettingsPath, xmlReaderSettings);
                    xmlDocument.Load(reader);
                    reader.Close();

                    XmlNode root = xmlDocument.SelectSingleNode("Configuration");
                    CreateOrWriteNode(ref xmlDocument, ref root, section, key, value);
                    xmlDocument.Save(SettingsPath);
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return false;
            }
        }

        private static void CreateOrWriteNode(ref XmlDocument doc, ref XmlNode root, string section, string key, string value)
        {
            try
            {
                XmlNode sectionRoot = root.SelectSingleNode(section);
                if (null == sectionRoot)
                {
                    XmlElement xmlSection = doc.CreateElement(section);

                    XmlElement xmlKey = doc.CreateElement(key);
                    xmlKey.InnerText = value;
                    xmlSection.AppendChild(xmlKey);
                    root.AppendChild(xmlSection);
                }
                else
                {
                    XmlNode keyNode = sectionRoot.SelectSingleNode(key);
                    if (null == keyNode)
                    {
                        XmlElement xmlKey = doc.CreateElement(key);
                        xmlKey.InnerText = value;
                        sectionRoot.AppendChild(xmlKey);
                    }
                    else
                    {
                        keyNode.InnerText = value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
