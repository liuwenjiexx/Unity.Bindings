using UnityEngine;
using System.Collections;

[System.Reflection.DefaultMember("Value")]
public class TestDefaultMember : MonoBehaviour {

    public string Value
    {
        get; set;
    }
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    void OnGUI()
    {
        GUILayout.Label("default:" + Value);
    }
}
