using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public abstract class CustomAsset<T> : ScriptableObject where T : CustomAssetItem, new() 
{
    public List<T> dataList = new List<T>();

    public int length
    {
        get
        {
            return dataList.Count;
        }
    }

    public T this[int index]
    {
        get
        {
            if (index < length)
            {
                return dataList[index];
            }
            return default(T);
        }
    }

    public string fileName
    {
        get
        {
            return this.GetType().ToString();
        }
    }

    public T GetDataByName(string name)
    {
        return dataList.Find((a) => a.name == name);
    }

    public T GetDataById(string id)
    {
        return dataList.Find((a) => a.id == id);
    }

    public void AddData(T data)
    {
        if(GetDataById(data.id) == null && GetDataByName(data.name) == null)
        {
            dataList.Add(data);
        }
        else
        {

        }
    }

    public bool Remove(T data)
    {
        return dataList.Remove(data);
    }

    public void Read(EGFileType fileType = EGFileType.Asset)
    {

        if (!Application.isPlaying)
        {
#if UNITY_EDITOR
            
#endif
        }
    }

    public string[] names
    {
        get
        {
            var result = new string[dataList.Count];

            for (int i = 0; i < dataList.Count; i++)
            {
                result[i] = dataList[i].name;
            }

            return result;
        }
    }
}

public class CustomAssetItem
{
    public string id;
    public string name;
}