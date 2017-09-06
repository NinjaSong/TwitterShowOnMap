using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayRecord : MonoBehaviour {
	AudioSource aud ;
	TagUIManager TagM;
	void Start()
	{

		TagM = GameObject.Find ("Maneger").GetComponent<TagUIManager> ();

	}

	public void PlayRecordOnce()
	{
		//aud.clip = null;
//		try{
//			aud = transform.GetComponent<AudioSource> ();
//		}catch(Exception ex){
//			aud.clip = null;
//		}
		Debug.Log ("play!" + transform.name);
		transform.GetComponent<AudioSource> ().Play ();


	}

	public void popEdit()
	{
		TagM.TagEdit (transform.position, transform.gameObject);

	}
}
