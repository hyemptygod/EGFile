using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LocalFile<T,V> where T : LocalFile<T,V> ,new() where V : ExcelData, new()
{
    static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
                _instance = new T();
            return _instance;
        }
    }

    public List<V> dataList = new List<V>();

    public int length
    {
        get
        {
            return dataList.Count;
        }
    }

    public V this[int index]
    {
        get
        {
            if (index < length)
            {
                return dataList[index];
            }
            return default(V);
        }
    }

    /// canEdit is true : can be modified in edit 
    public bool canEdit = true;

    public LocalFile()
    {
        Read(EGFileType.Xml);
    }

    public V GetData(string guid)
    {
        if(dataList == null)
        {
            dataList = new List<V>();
        }

        if (string.IsNullOrEmpty(guid))
        {
            return null;
        }

        return dataList.Find((a) => a.guid.Equals(guid));
    }

    /// <summary>
    /// 添加数据
    /// </summary>
    /// <param name="data"></param>
    public void AddData(V data)
    {
        
        if (GetData(data.guid) == null)
        {
            dataList.Add(data);
            Create();
        }
        else
        {
            Debug.Log("<color=red>add failed!already exists</color>");
        }
    }

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="data"></param>
    public void RemoveData(V data)
    {
        if (GetData(data.guid) != null)
        {
            dataList.Remove(GetData(data.guid));
            Create();
        }
        else
        {
            Debug.Log("<color=red>remove failed!not exists</color>");
        }
    }

    public string[] Names()
    {
        var names = new string[length];

        for (int i = 0; i < length; i++)
        {
            names[i] = dataList[i].guid;
        }

        return names;
    }

    public void Clear()
    {
        dataList.Clear();
        Create();
    }

    public void Read(EGFileType type)
    {
        switch (type)
        {
            case EGFileType.Xml:
                dataList = dataList.ReadFromXml<V>();
                break;
            case EGFileType.Excel:
                dataList = dataList.ReadFromExcel<V>();
                break;
        }
    }

    public void Create()
    {
        dataList.SaveToXml<V>();
        dataList.SaveToExcel<V>();
    }

    public void Sync(EGFileType fileType)
    {
        Read(fileType);
        Create();
    }
}