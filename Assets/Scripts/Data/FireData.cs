using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
/// <summary>
/// 开火点数据集合
/// </summary>
public class FireData
{
    public List<FireInfo> fireInfoList = new List<FireInfo>();
}

/// <summary>
/// 单条开火点数据
/// </summary>
public class FireInfo
{
    [XmlAttribute]
    public int id;//开火点id
    [XmlAttribute]
    public int type;//开火点类型 1顺序，2散弹
    [XmlAttribute]
    public int num;//数量 该组子弹有多少个
    [XmlAttribute]
    public float cd;//每颗子弹 开火时间的间隔
    [XmlAttribute]
    public string ids;//关联的子弹ID,发射哪种的子弹 1,10 代表在1-10的id子弹中去随机
    [XmlAttribute]
    public float delay;//每组发射子弹后间隔 时间
}
