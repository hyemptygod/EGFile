using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Excel;
using OfficeOpenXml;
using System.Text.RegularExpressions;

public static class SaveToLocal
{
    public static bool enableLog = true;
#if UNITY_EDITOR

    [UnityEditor.MenuItem("Tools/Enable Log",false,0)]
    public static void EnableLog()
    {
        enableLog = !enableLog;
    }
    [UnityEditor.MenuItem("Tools/Enable Log", true, 0)]
    public static bool EnableLogValidate()
    {
        UnityEditor.Menu.SetChecked("Tools/Enable Log", enableLog);
        return true;
    }

#endif

    public static string xmlPath = Application.dataPath + "/Data/Xml/";
    public static string xmlSuffix = ".xml";
    public static string excelPath = Application.dataPath + "/Data/Excel/";
    public static string excelSuffix = ".xlsx";
    public static string jsonPath = Application.dataPath + "/Data/Json/";
    public static string jsonSuffix = ".json";
    public static string customPath = "Assets/Resources/";
    public static string customSuffix = ".asset";

    public static string XmlPath(string fileName)
    {
        return string.Format("{0}{1}{2}", xmlPath,fileName, xmlSuffix);
    }

    public static string ExcelPath(string fileName)
    {
        return string.Format("{0}{1}{2}", excelPath,fileName, excelSuffix);
    }

    public static string JsonPath(string fileName)
    {
        return string.Format("{0}{1}{2}", jsonPath, fileName, jsonSuffix);
    }

    public static string CustomAssetPath(string fileName)
    {
        return string.Format("{0}{1}{2}", customPath,fileName, customSuffix);
    }

    /// <summary>
    /// xml read list
    /// </summary>
    public static List<T> ReadFromXml<T>(this List<T> current, string fileName = "") where T : ExcelData
    {
        if (!Directory.Exists(xmlPath))
        {
            Directory.CreateDirectory(xmlPath);
            Debug.Log(string.Format("{0}{1}:<color=red>file does not exist</color>", fileName,xmlSuffix));
        }

        fileName = string.IsNullOrEmpty(fileName) ? typeof(T).ToString() : fileName;

        if (!File.Exists(XmlPath(fileName)))
        {
            Debug.Log(string.Format("{0}{1}:<color=red>file does not exist </color>", fileName, xmlSuffix));
            current.SaveToXml<T>(fileName);
            return current;
        }

        using (FileStream fs = new FileStream(XmlPath(fileName), FileMode.Open))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            current = (List<T>)serializer.Deserialize(fs);
            fs.Close(); 

            if (enableLog)
            {
                Debug.Log(string.Format("{0}{1}:<color=green>read success</color>", fileName, xmlSuffix));
            }
        }
        return current;

    }

    /// <summary>
    /// xml save list
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="excelName"></param>
    /// <param name="data"></param>
    public static void SaveToXml<T>(this List<T> current, string fileName = "")
    {
        if (!Directory.Exists(xmlPath))
        {
            Directory.CreateDirectory(xmlPath);
        }

        fileName = string.IsNullOrEmpty(fileName) ? typeof(T).ToString() : fileName;

        using (var fs = File.Create(XmlPath(fileName)))
        {
            XmlSerializer serilizer = new XmlSerializer(typeof(List<T>));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(fs, settings);
            serilizer.Serialize(writer, current);
            fs.SetLength(fs.Position);
            fs.Close();

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            if (enableLog)
            {
                Debug.Log(string.Format("{0}{1}:<color=green>write success!</color>", fileName, xmlSuffix));
            }
        }

    }

    /// <summary>
    /// excel read
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="current"></param>
    /// <param name="excelName"></param>
    /// <param name="sheetName"></param>
    /// <returns></returns>
    public static List<T> ReadFromExcel<T>(this List<T> current, string excelName = "", string sheetName = "") where T : ExcelData, new()
    {
        if (!Directory.Exists(excelPath))
        {
            Directory.CreateDirectory(excelPath);
            Debug.Log(string.Format("{0}{1}:<color=red>does not exist</color>", excelName,excelSuffix));
            return null;
        }

        excelName = string.IsNullOrEmpty(excelName) ? typeof(T).ToString() : excelName;
        sheetName = string.IsNullOrEmpty(sheetName) ? typeof(T).ToString() : sheetName;

        if (!File.Exists(ExcelPath(excelName)))
        {
            Debug.Log(string.Format("{0}{1}:<color=red>does not exist</color>", excelName, excelSuffix));
            //current.SaveToExcel<T>(excelName, sheetName);
            return current;
        }

        using (var fs = File.Open(ExcelPath(excelName), FileMode.Open, FileAccess.Read))
        {
            using (var excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs))
            {
                var table = excelReader.AsDataSet().Tables[sheetName];
                var fields = typeof(T).GetFields();
                int rowCount = table.Rows.Count;
                int columnCount = table.Columns.Count;

                /// first row is item name
                var variableNameList = new List<string>();
                for (int i = 0; i < columnCount; i++)
                {
                    variableNameList.Add(table.Rows[0][i].ToString());
                }
                for (int i = 1; i < rowCount; i++)
                {
                    var item = new T();
                    var row = table.Rows[i];
                    for (int j = 0; j < fields.Length; j++)
                    {
                        var field = fields[j];
                        var index = variableNameList.IndexOf(field.ToExcelFieldName());
                        if (index < 0)
                            Debug.LogError(string.Format("{0}{1}<color=red>don't have {2} field</color>", excelName,excelSuffix, field.Name));
                        else
                            field.SetValue(item, ConvertObject(row[j], field.FieldType));
                    }
                    current.Add(item);
                }
            }

            if (enableLog)
            {
                Debug.Log(string.Format("{0}{1}[{2}]:<color=green>read success<color=green>", excelName, excelSuffix, sheetName));
            }

        }
        return current;
    }

    public static string ToExcelFieldName(this FieldInfo field)
    {
        var attribs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        var attrib_name = "";
        if (attribs.Length > 0)
        {
            attrib_name = string.Format("({0})", ((DescriptionAttribute)attribs[0]).Description);
            for (int i = 1; i < attribs.Length; i++)
            {
                attrib_name = string.Format("{0}({1})", attrib_name, ((DescriptionAttribute)attribs[1]).Description);
            }
        }

        return WordSplitting(field.Name) + attrib_name;

    }

    public static object ConvertObject(object value, System.Type type)
    {
        if (value == null)
        {
            if (type.IsGenericType)
            {
                return System.Activator.CreateInstance(type);
            }
            return null;
        }
        else
        {
            if (type.Equals(value.GetType()))
            {
                return value;
            }
            else
            {
                if (type.IsUnityObject() || type.IsClass)
                {
                    return StringToObject(value.ToString(), type);
                }
                else
                {
                    if (type.IsEnum)
                    {
                        if (value is string)
                            return System.Enum.Parse(type, value.ToString());
                        else
                            return System.Enum.ToObject(type, value);
                    }
                    else
                    {
                        return System.Convert.ChangeType(value, type);
                    }
                }
            }
        }
    }

    public static bool IsUnityObject(this Type type)
    {
        return type.ToString().StartsWith("UnityEngine.");
    }

    public static object StringToObject(string current, System.Type type)
    {
        current = current.Replace("(", "").Replace(")", "");
        var split = current.Split(new string[] { "," }, System.StringSplitOptions.None);

        var actual_parameters = new object[split.Length];
        var constructors = type.GetConstructors();
        for (int i = 0; i < constructors.Length; i++)
        {
            var formal_parameters = constructors[i].GetParameters();
            if (formal_parameters.Length == actual_parameters.Length)
            {
                for (int j = 0; j < formal_parameters.Length; j++)
                {
                    actual_parameters[j] = ConvertObject(split[j], formal_parameters[j].ParameterType);
                }
            }
        }
        return System.Activator.CreateInstance(type, actual_parameters);

    }

    /// <summary>
    /// excel write
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="current">list data</param>
    /// <param name="fileName">excel file name</param>
    /// <param name="sheet_name">sheetName</param>
    public static void SaveToExcel<T>(this List<T> current, string excel_name = "", string sheet_name = "") where T : ExcelData, new()
    {
        if (!Directory.Exists(excelPath))
        {
            Directory.CreateDirectory(excelPath);
        }

        excel_name = string.IsNullOrEmpty(excel_name) ? typeof(T).ToString() : excel_name;
        sheet_name = string.IsNullOrEmpty(sheet_name) ? typeof(T).ToString() : sheet_name;

        var fileInfo = new FileInfo(ExcelPath(excel_name));

        using (var package = new ExcelPackage(fileInfo))
        {
            if (package.Workbook.Worksheets[sheet_name] != null)
            {
                package.Workbook.Worksheets.Delete(sheet_name);
            }

            var workSheet = package.Workbook.Worksheets.Add(sheet_name);

            var fields = typeof(T).GetFields();

            /// filed name
            for (int titleId = 0; titleId < fields.Length; titleId++)
            {
                workSheet.Cells[1, titleId + 1].Value = fields[titleId].ToExcelFieldName();
            }

            /// data
            for (int i = 0; i < current.Count; i++)
            {
                for (int j = 0; j < fields.Length; j++)
                {
                    workSheet.Cells[i + 2, j + 1].Value = fields[j].GetValue(current[i]);
                }
            }

            package.Save();

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            if (enableLog)
            {
                Debug.Log(string.Format("{0}{1}[{2}]:<color=green>write success!</color>", excel_name, excelSuffix, sheet_name));
            }

        }
    }

#if UNITY_EDITOR
    public static void CreateCustomAsset(System.Type type)
    {
        var result = ScriptableObject.CreateInstance(type);

        if (result == null)
        {
            Debug.Log(type + " not found");
            return;
        }

        if (!Directory.Exists(Application.dataPath + "/Resources/"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/");
        }

        UnityEditor.AssetDatabase.CreateAsset(result, CustomAssetPath(type.ToString()));
        UnityEditor.EditorUtility.SetDirty(result);
        UnityEditor.AssetDatabase.Refresh();
    }

#endif

    public static string WordSplitting(string word)
    {
        word = char.ToUpper(word[0]) + word.Substring(1);

        var matches = Regex.Matches(word, @"[A-Z]+[a-z]*");

        var words = new List<string>();

        foreach (Match item in matches)
        {
            words.Add(item.Value);
        }

        if (words.Count <= 0)
        {
            return word;
        }

        var result = words[0];
        for (int i = 1; i < words.Count; i++)
        {
            result = string.Format("{0} {1}", result, words[i]);
        }

        return result;
    }
}

public enum EGFileType
{
    Xml,
    Excel,
    Asset,
}
