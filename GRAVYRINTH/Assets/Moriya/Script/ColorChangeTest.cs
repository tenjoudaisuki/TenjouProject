using UnityEngine;
using System.Collections;

public class ColorChangeTest : MonoBehaviour {

    SkinnedMeshRenderer smr;
    int a = 0;

	// Use this for initialization
	void Start () {
        smr = GetComponent<SkinnedMeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        
        Material m = smr.materials[1];
        m.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
	}
}
