using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour {
	Menu MenuManeger;
	TagUIManager Tagmanager;
	// Use this for initialization
	void Start () {
		MenuManeger = GameObject.Find ("Maneger").GetComponent<Menu> ();
		Tagmanager = GameObject.Find ("Maneger").GetComponent<TagUIManager> ();
	}

	public void OpenTagList()
	{
		Debug.Log (" OpenTagList");
		Vector3 rot = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.rotation.eulerAngles;
		rot = new Vector3 (rot.x, rot.y, 0);
		Vector3 pos = GameObject.Find ("CenterEyeAnchor").transform.position + GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.forward * 10f-
			GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.right * 3f;;

		GameObject taglistUi = Instantiate (MenuManeger.Taglist, pos, Quaternion.Euler (rot));
		taglistUi.GetComponent<Canvas> ().worldCamera = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ();
		GameObject.Destroy(transform.gameObject);
	}

	public void Cancle()
	{
		GameObject.Destroy(this.gameObject);

		MenuManeger.is_on = false;
	}

	public void Closetaglist()
	{
		GameObject.Destroy(transform.gameObject);

		MenuManeger.is_on = false;
	}
}
