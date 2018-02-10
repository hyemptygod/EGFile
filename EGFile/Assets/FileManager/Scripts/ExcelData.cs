using UnityEngine;
using System.Collections;

/// <summary>
/// 可用于保存到Excel文件的抽象类
/// </summary>
[System.Serializable]
public abstract class ExcelData
{
    /// 唯一标识符
    protected string _guid;

    public abstract string guid
    {
        get;
    }
}

public class ExcelField
{
    public override string ToString()
    {
        var fields = GetType().GetFields();
        var result = "";
        if(fields.Length > 0)
        {
            result = fields[0].GetValue(this).ToString();
            for (int i = 1; i < fields.Length; i++)
            {
                result = string.Format("{0},{1}", result, fields[i].GetValue(this).ToString());
            }
            result = string.Format("({0})", result);
        }
        return result;
    }
}