using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Reflection;

public abstract class XmlData
{

    public static object Load(string file_name,System.Type class_type)
    {
        object result = null;

        if (!File.Exists(SaveToLocal.XmlPath(file_name)))
        {
            var method = class_type.GetMethod("Save");
            var obj = System.Activator.CreateInstance(class_type);
            var parameters = new object[] { class_type.ToString(), class_type };
            method.Invoke(obj, parameters);

            return obj;
        }

        using (FileStream fs = new FileStream(SaveToLocal.XmlPath(file_name), FileMode.Open))
        {
            XmlSerializer serializer = new XmlSerializer(class_type);
            result = serializer.Deserialize(fs);
        }
        return result;
    }

    public void Create()
    {
        using (FileStream fs = new FileStream(SaveToLocal.XmlPath(GetType().ToString()), FileMode.OpenOrCreate, FileAccess.Write))
        {
            XmlSerializer serializer = new XmlSerializer(GetType());
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(fs, settings);
            serializer.Serialize(writer, this);
            fs.SetLength(fs.Position);
        }

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
