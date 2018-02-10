using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Classes : ExcelData
{
    public string id = "0";
    public string name;
    public string type;

    public override string guid
    {
        get
        {
            return id;
        }
    }

    public Classes() { }
}

public class ClassDataManager : LocalFile<ClassDataManager, Classes>
{
    public ClassDataManager() : base()
    {
        canEdit = false;
    }
}




