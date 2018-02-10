using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

public class ReadLocalFileWindow : BaseWindow<ReadLocalFileWindow>
{
    public override Rect windowPosition
    {
        get
        {
            return new Rect(435, 370, 1050, 355);
        }
    }

    public override string windowTitle
    {
        get
        {
            return "Read File";
        }
    }

    [MenuItem("Tools/Read/Local File")]
    public static void OpenWindow()
    {
        ReadLocalFileWindow.instance.Open();
    }

    public class Info
    {
        public int select;
        public EGFileType loadType;
        public object manager;
        public System.Type dataType;
        public object dataValue;
        public FieldInfo[] dataFields;
        public int dataLength;
        public bool canEdit;
        public bool editing;

        public void Sync(List<System.Type> types)
        {
            types[select].GetMethod("Sync").Invoke(manager, new object[] { loadType });
        }
        
        public void Read(List<System.Type> types)
        {
            manager = System.Activator.CreateInstance(types[select]);
            types[select].GetMethod("Read").Invoke(manager, new object[] { loadType });
            Load();
        }

        public void Load()
        {
            dataType = manager.GetType().BaseType.GetGenericArguments()[1];
            dataFields = dataType.GetFields();
            dataValue = manager.GetType().GetField("dataList");
            dataLength = (int)manager.GetType().GetProperty("length", BindingFlags.Instance | BindingFlags.Public).GetValue(manager, null);
            canEdit = (bool)manager.GetType().GetField("canEdit").GetValue(manager);
        }

        public object GetData(int index)
        {
            return manager.GetType().GetMethod("get_Item", BindingFlags.Instance | BindingFlags.Public).Invoke(manager, new object[] { index });
        }

        public void AddData(object data)
        {
            manager.GetType().GetMethod("AddData").Invoke(manager, new object[] { data });

            Load();
        }

        public void Save()
        {
            manager.GetType().GetMethod("Create").Invoke(manager, null);
            Load();
        }

        public void Remove(object data)
        {
            manager.GetType().GetMethod("RemoveData").Invoke(manager, new object[] { data });
            Load();
        }
    }
    public Info info = new Info();

    List<System.Type> types = new List<System.Type>();
    string[] typeNames;

    public GUISkin mySkin;

    public void OnEnable()
    {
        types.Clear();

        var allTypes = typeof(LocalFile<,>).Assembly.GetTypes();

        for (int i = 0; i < allTypes.Length; i++)
        {
            if (allTypes[i].BaseType.Name == typeof(LocalFile<,>).Name)
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
            info.select = 0;
        }
    }

    private void OnFocus()
    {
        if(info.select < 0 || types.Count <= info.select)
        {
            return;
        }

        info.Read(types);
        
    }

    protected override void OnGUI()
    {
        
        base.OnGUI();

        using (var scope = new EditorGUILayout.VerticalScope("box"))
        {
            var select = info.select;
            select = EditorGUILayout.Popup("Select Type", select, typeNames);
            if(select != info.select)
            {
                info.select = select;
                info.Read(types);
            }

            var loadType = info.loadType;
            loadType = (EGFileType)EditorGUILayout.EnumPopup("Read Mode", loadType);
            if(loadType != info.loadType)
            {
                info.loadType = loadType;
                info.Read(types);
            }

            if (GUILayout.Button("Sync"))
            {
                info.Sync(types);
            }
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        using (var scope = new EditorGUILayout.VerticalScope("box"))
        {
            GUILayout.Space(10f);

            using (var scope1 = new EditorGUILayout.VerticalScope("box"))
            {
                EditorGUILayout.LabelField(info.manager.GetType().ToString(), mySkin.GetStyle("Title"));
            }

            if (info.dataFields.Length <= 0)
            {
                return;
            }

            var width = position.width * 0.85f / (info.dataFields.Length + 1);

            using (var scope1 = new EditorGUILayout.VerticalScope("box"))
            {
                using (var scope2 = new EditorGUILayout.HorizontalScope())
                {
                    using (var scope3 = new EditorGUILayout.HorizontalScope("box"))
                    {
                        EditorGUILayout.LabelField("id", mySkin.GetStyle("ItemButton"),GUILayout.Width(100));
                    }
                    for (int i = 0; i < info.dataFields.Length; i++)
                    {
                        using (var scope3 = new EditorGUILayout.HorizontalScope("box"))
                        {
                            EditorGUILayout.LabelField(info.dataFields[i].Name, mySkin.GetStyle("ItemName"), GUILayout.Width(width));
                        }
                    }
                }

                for (int i = 0; i < info.dataLength; i++)
                {
                    var data = info.GetData(i);

                    using (var scope2 = new EditorGUILayout.HorizontalScope())
                    {
                        using (var scope3 = new EditorGUILayout.HorizontalScope("box"))
                        {
                            if (info.canEdit)
                            {
                                if(GUILayout.Button("R:"+i.ToString(),GUILayout.Width(48)))
                                {
                                    info.Remove(data);
                                }

                                if (GUILayout.Button("E:" + i.ToString(),GUILayout.Width(48)))
                                {
                                    var window = EditorWindow.GetWindow<FielItemShow>();
                                    window.index = i;
                                    window.Show();
                                }
                            }
                            else
                            {
                                EditorGUILayout.LabelField(i.ToString(), mySkin.GetStyle("ItemButton"), GUILayout.Width(100));
                            }
                            
                        }
                        for (int j = 0; j < info.dataFields.Length; j++)
                        {
                            using (var scope3 = new EditorGUILayout.HorizontalScope("box"))
                            {
                                var value = info.dataFields[j].GetValue(data);
                                if(value != null)
                                {
                                    EditorGUILayout.LabelField(value.ToString(), mySkin.GetStyle("ItemValue"), GUILayout.Width(width));
                                }
                                else
                                {
                                    EditorGUILayout.LabelField("null", mySkin.GetStyle("ItemValue"), GUILayout.Width(width));
                                }
                            }
                        }
                    }
                }
            }
            
        }

        EditorGUILayout.EndScrollView();

        using (var scope = new EditorGUILayout.VerticalScope("box"))
        {
            if (GUILayout.Button("Add"))
            {
                var data = System.Activator.CreateInstance(info.dataType);
                info.AddData(data);

                var window = EditorWindow.GetWindow<FielItemShow>();
                window.index = info.dataLength - 1;
                window.Show();
            }
        }
    }
}
