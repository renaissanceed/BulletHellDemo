using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class StoreData
{
    public List<Item> storeList = new List<Item>();
    public int money;
}
public class Item
{
    [XmlAttribute]
    public int id;
    [XmlAttribute]
    public int attr;
    [XmlAttribute]
    public int price;
    [XmlAttribute]
    public int num;
}
