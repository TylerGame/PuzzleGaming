using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;
using UnityEngine;

[XmlRoot(ElementName = "puzzle")]
public class PuzzleXML
{
    [XmlElement(ElementName = "definition")]
    public string definition;
}

public class XmlUtils
{
    public static T ImportXml<T>(string path)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Exception import xml file : " + e);
            return default;
        }
    }
}
