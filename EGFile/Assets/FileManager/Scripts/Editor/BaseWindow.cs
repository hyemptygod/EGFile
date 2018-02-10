using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public abstract class BaseWindow<T> : EditorWindow where T : EditorWindow
{

    private static T _instance;
    public static T instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = EditorWindow.GetWindow<T>(false);
            }
            return _instance;
        }
    }

    public abstract Rect windowPosition
    {
        get;
    }

    public virtual string windowTitle
    {
        get
        {
            return WordSplitting(instance.GetType().ToString());
        }
    }

    protected Vector2 scrollPos;

    public virtual void Open()
    {
        titleContent = new GUIContent(windowTitle);
        position = windowPosition;
        instance.Show();
    }

    protected virtual void OnGUI()
    {

    }

    public string WordSplitting(string word)
    {
        word = char.ToUpper(word[0]) + word.Substring(1);

        var matches = Regex.Matches(word, @"[A-Z]+[a-z]*");

        var words = new List<string>();

        foreach (Match item in matches)
        {
            words.Add(item.Value);
        }

        if (words.Count <= 0)
        {
            return word;
        }

        var result = words[0];
        for (int i = 1; i < words.Count; i++)
        {
            result = string.Format("{0} {1}", result, words[i]);
        }

        return result;
    }
}

