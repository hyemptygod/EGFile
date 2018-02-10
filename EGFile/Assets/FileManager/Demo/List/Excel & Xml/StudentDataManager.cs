using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : ExcelField
{
    public string name;
    public int distance;
    public float position;
    public bool isSb;

    public Skill() { }
}

[System.Serializable]
public class Student : ExcelData
{
    public string id = "000";
    public string name;
    public int age;
    public int sex;
    public int grade;
    public int classes;
    public Vector2 testV2;
    public Vector3 textV3;
    public Skill skill;

    /// <summary>
    /// 唯一标识符为学号
    /// </summary>
    public override string guid
    {
        get
        {
            return id;
        }
    }

    public Student() {}
}

public class StudentDataManager : LocalFile<StudentDataManager,Student>
{
    public StudentDataManager() : base()
    {
        /// canEdit is true : can be modified in edit 
        canEdit = true;
    }
}
