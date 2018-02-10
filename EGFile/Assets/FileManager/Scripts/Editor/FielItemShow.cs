using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class FielItemShow : BaseWindow<FielItemShow>
{
    public int index;
    public GUISkin mySkin;

    public ReadLocalFileWindow.Info info;

    bool change;

    public override Rect windowPosition
    {
        get
        {
            return new Rect(740, 375, 500, 410);
        }
    }

    protected override void OnGUI()
    {
        info = ReadLocalFileWindow.instance.info;

        if (index < 0 || index >= info.dataLength)
        {
            Debug.Log("sssss");
            return;
        }

        var data = info.GetData(index);

        using (var scope = new EditorGUILayout.VerticalScope())
        {
            using (var scope1 = new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(info.dataType.ToString(), mySkin.GetStyle("Title"));
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            using (var scope1 = new EditorGUILayout.VerticalScope("box"))
            {
                for (int i = 0; i < info.dataFields.Length; i++)
                {
                    using (var scope2 = new EditorGUILayout.VerticalScope("box"))
                    {
                        if (data != null)
                        {
                            var newValue = DrawField(info.dataFields[i], data);
                            if(info.dataFields[i].GetValue(data) == null || newValue != info.dataFields[i].GetValue(data))
                            {
                                info.dataFields[i].SetValue(data, newValue);
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            using (var scope1 = new EditorGUILayout.VerticalScope("box"))
            {
                if (GUILayout.Button("Save"))
                {
                    info.Save();
                }
            }
        }
    }

    object DrawField(FieldInfo field,object data)
    {
        object objectValue = field.GetValue(data);

        if (field.FieldType == typeof(string))
        {
            var value = "";
            if(objectValue != null)
            {
                value = objectValue.ToString();
            }
            value = EditorGUILayout.TextField(field.Name, value);

            return value;
        }
        else if(field.FieldType == typeof(int))
        {
            var value = (int)objectValue;
            value = EditorGUILayout.IntField(field.Name, value);
            return value;
        }
        else if(field.FieldType == typeof(float))
        {
            var value = (float)objectValue;
            value = EditorGUILayout.FloatField(field.Name, value);
            return value;
        }
        else if(field.FieldType == typeof(bool))
        {
            var value = (bool)objectValue;
            value = EditorGUILayout.Toggle(field.Name, value);
            return value;
        }
        else if(field.FieldType == typeof(Vector3))
        {
            var value = (Vector3)objectValue;
            value = EditorGUILayout.Vector3Field(field.Name, value);
            return value;
        }
        else if(field.FieldType == typeof(Vector2))
        {
            var value = (Vector2)objectValue;
            value = EditorGUILayout.Vector2Field(field.Name, value);
            return value;
        }
        else if(field.FieldType.BaseType == typeof(ExcelField))
        {
            var itemFields = field.FieldType.GetFields();
            using (var scope = new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.LabelField(field.Name);
                for (int i = 0; i < itemFields.Length; i++)
                {
                    using (var scope1 = new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(10f);
                        if(objectValue == null)
                        {
                            objectValue = System.Activator.CreateInstance(field.FieldType);
                        }
                        var newValue = DrawField(itemFields[i], objectValue);
                        if(itemFields[i].GetValue(objectValue) == null || newValue != itemFields[i].GetValue(objectValue))
                        {
                            itemFields[i].SetValue(objectValue, newValue);
                        }
                    }
                }
            }
            return objectValue;
        }
        else
        {
            return null;
        }
    }
}
