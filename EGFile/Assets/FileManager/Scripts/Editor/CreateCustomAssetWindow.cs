using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateCustomAssetWindow : BaseWindow<CreateCustomAssetWindow>
{
    public override Rect windowPosition
    {
        get
        {
            return new Rect(650, 500, 340, 100);
        }
    }

    [MenuItem("Tools/Create/Custom Asset")]
    public static void OpenWindow()
    {
        CreateCustomAssetWindow.instance.Open();
    }

    List<System.Type> types = new List<System.Type>();
    string[] typeNames;
    int select;

    public void OnEnable()
    {
        types.Clear();
        var allTypes = typeof(CustomAsset<>).Assembly.GetTypes();

        for (int i = 0; i < allTypes.Length; i++)
        {
            if ((allTypes[i].BaseType.Name == typeof(CustomAsset<>).Name || allTypes[i].BaseType.Name == typeof(ScriptableObject).Name) && allTypes[i].Name != typeof(CustomAsset<>).Name)
            {
                types.Add(allTypes[i]);
            }
        }

        typeNames = new string[types.Count];

        for (int i = 0; i < types.Count; i++)
        {
            typeNames[i] = types[i].Name;
        }

        if (typeNames.Length > 0)
        {
            select = 0;
        }
    }

    protected override void OnGUI()
    {
        base.OnGUI();

        using (var scope = new EditorGUILayout.VerticalScope())
        {
            GUILayout.Space(20f);
            select = EditorGUILayout.Popup("Select Type", select, typeNames);

            GUILayout.Space(10f);

            if (GUILayout.Button("Create"))
            {

                if (select >= 0)
                {
                    SaveToLocal.CreateCustomAsset(types[select]);
                    Close();
                }

            }
        }
    }
}
