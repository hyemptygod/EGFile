using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadStudent : MonoBehaviour {

    [HideInInspector]
    public int selectIndex;


	// Use this for initialization
	void Start ()
    {
        var student = StudentDataManager.instance[selectIndex];
        Debug.Log(student.id + ":" + student.name);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
