using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoadStudent))]
public class LoadLocalFileEditor : Editor
{
    LoadStudent _target;
    string[] names;

    public void OnEnable()
    {
        _target = target as LoadStudent;
        names = StudentDataManager.instance.Names();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        using (var scope = new EditorGUILayout.VerticalScope("box"))
        {
            _target.selectIndex = EditorGUILayout.Popup("Guid", _target.selectIndex, names);
        }
    }
}
