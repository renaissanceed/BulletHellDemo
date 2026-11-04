using System;
using System.IO;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class XmlDataMar
{
    private static XmlDataMar instance = new XmlDataMar();
    public static XmlDataMar Instacne => instance;
    private XmlDataMar() {}

    /// <summary>
    /// 保存数据到xml文件中
    /// </summary>
    /// <param name="data">数据</param>
    /// <param name="flieName">文件路径</param>
    public void SavaData(object data ,string flieName)
    {
        //得到存储路径
        string path = Application.persistentDataPath + "/" +  flieName + ".xml";
        //存储文件
        using (StreamWriter r = new StreamWriter(path))
        {
            //序列化
            XmlSerializer xml = new XmlSerializer(data.GetType());
            xml.Serialize(r,data);
        }
        
    } 

/// <summary>
/// 从xml文件中读取内容
/// </summary>
/// <param name="type">类型</param>
/// <param name="flieName">文件路径</param>
/// <returns></returns>
    public object LoadData(Type type ,string flieName)
    {
        //首先要判断文件是否存在
        //得到存储路径
        string path = Application.persistentDataPath + "/" +  flieName + ".xml";
        if (!File.Exists(path))
        {
            path = Application.streamingAssetsPath + "/" +  flieName + ".xml";
            if (!File.Exists(path))
            {
                //如果根本不存在文件，两个路径都找过了
                //直接new一个问价给外部 无非里面 都是默认值
                return Activator.CreateInstance(type);
            }
        }
        //存在就读取数据
        using (StreamReader r = new StreamReader(path))
        {
            //反序列化
            XmlSerializer xml = new XmlSerializer(type);
            return xml.Deserialize(r);
        }
    } 
}
