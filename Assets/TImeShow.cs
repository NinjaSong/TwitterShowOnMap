using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TImeShow : MonoBehaviour {

	[SerializeField]SQLTimeColor TimeColorScript;
	Text Timeshow;
	void Start () {
		Timeshow = transform.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		Timeshow.text = "Time : " +TimeColorScript.nowTime;
	}
}
