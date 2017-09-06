using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
	public GameObject MenuUI;
	public GameObject Taglist;
	[HideInInspector]public bool is_on = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Fire2") && is_on) {
			GameObject.Destroy (GameObject.FindGameObjectWithTag ("taglist"));
			is_on = false;
		}else if (Input.GetButtonDown ("Fire2") && !is_on) {
			Vector3 rot = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.rotation.eulerAngles;
			rot = new Vector3 (rot.x, rot.y, 0);
			Vector3 pos = GameObject.Find ("CenterEyeAnchor").transform.position + GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.forward * 10f;

			GameObject menuUi = Instantiate (MenuUI, pos, Quaternion.Euler (rot));
			menuUi.GetComponent<Canvas> ().worldCamera = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ();
			is_on = true;
		}


	}
}
