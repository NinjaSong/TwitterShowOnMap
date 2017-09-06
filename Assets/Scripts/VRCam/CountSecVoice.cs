using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountSecVoice : MonoBehaviour {

	bool isstop = false;
	// Use this for initialization
	public void S()
	{
		isstop = false;
		StartCoroutine (StartCount());
	}

	public void SStop()
	{
		isstop = true;
	}
	
	public IEnumerator StartCount()
	{
		int count = 0;
		while (count < 100 && !isstop) {
			transform.GetComponent<UnityEngine.UI.Text> ().text = (100 - count).ToString () + " s";
			count++;
			yield return new  WaitForSeconds(1);
		}
	}
}
