using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// 自定义Attribute
/// </summary>
public class DescriptionAttribute : Attribute
{
    private string description;
    public DescriptionAttribute(string str_description)
    {
        description = str_description;
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }
}

/// <summary>
/// 测试数据类
/// </summary>
public class TestData : ExcelData
{
    [Description("编号")]
    public int id;

    [Description("名称")]
    public string name;

    [Description("修改日期")]
    public string updateTime;

    [Description("Struct测试")]
    public TestStruct testStruct;

    [Description("unityObject测试")]
    public Vector3 vector3;

    public override string guid
    {
        get
        {
            return string.Format("{0}_{1}", id, name);
        }
    }

    public TestData() { }

    public TestData(int id,string name,string update_time, TestStruct test_struct)
    {
        this.id = id;
        this.name = name;
        this.updateTime = update_time;
        this.testStruct = test_struct;
        vector3 = Vector3.zero;
    }

    
}

public class TestStruct  : ExcelField
{
    public string name;
    public string path;

    public TestStruct(string name,string path)
    {
        this.name = name;
        this.path = path;
    }
}