using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour {

	public GameObject thisWindow;

	public void ExitNo()
	{
		transform.gameObject.SetActive (false);
	}

	public void ExitYes()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}


}