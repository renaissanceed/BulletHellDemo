using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class SerializerDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        XmlSerializer keySer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSer = new XmlSerializer(typeof(TValue));

        reader.Read();
        while (reader.NodeType != XmlNodeType.EndElement)
        {
            TKey key = (TKey)keySer.Deserialize(reader);
            TValue value = (TValue)valueSer.Deserialize(reader);
            this.Add(key,value);
        }
        reader.Read();
    }

    public void WriteXml(XmlWriter writer)
    {
        XmlSerializer keySer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSer = new XmlSerializer(typeof(TValue));

        foreach (KeyValuePair<TKey,TValue> item in this)
        {
            keySer.Serialize(writer,item.Key);
            valueSer.Serialize(writer,item.Value);
        }
    }
}
