using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagUIManager : MonoBehaviour {

	public GameObject TagPopUI;
	public GameObject thetag;
	public GameObject TagEditUI;

	[HideInInspector] public bool turn_on = false;
	[HideInInspector] public GameObject hitItem;
	[HideInInspector] public string deleteItem;
	[HideInInspector] public int tagindex = 0;
	[HideInInspector] public List<GameObject> taglist;
	[HideInInspector] public Vector3 point;
	[HideInInspector] public Vector3 point2;
	[HideInInspector] public Vector3 point3;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TagUIPopUp(Vector3 hitpoint,Vector3 hitpoint2,Vector3 hitpoint3)
	{
		Vector3 rot = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.rotation.eulerAngles;
		rot = new Vector3 (rot.x, rot.y, 0);
		Vector3 pos = GameObject.Find ("CenterEyeAnchor").transform.position + GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.forward * 3f;
		//Debug.Log ("click");
		if (turn_on == false) {
			point = hitpoint;
			point2 = hitpoint2;
			point3 = hitpoint3;
			GameObject newtag = Instantiate (TagPopUI, pos + new Vector3 (0, 0.3f, 0), Quaternion.Euler (rot));
			newtag.GetComponent<Canvas> ().worldCamera = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ();
			turn_on = true;
		}

	}

	public void TagEdit(Vector3 hitpoint, GameObject hititem)
	{
		
		Vector3 rot = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.rotation.eulerAngles;
		rot = new Vector3 (rot.x, rot.y, 0);
		Vector3 pos = hitpoint - GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ().transform.forward * 6f;
		hitItem = hititem;
		if (turn_on == false) {
			point = hitpoint;

			GameObject newtag = Instantiate (TagEditUI, pos + new Vector3 (0, 0.3f, 0), Quaternion.Euler (rot));
			newtag.GetComponent<Canvas> ().worldCamera = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ();
			if (hititem.GetComponent<AudioSource> () != null) {
				newtag.GetComponent<AudioSource> ().clip = hititem.GetComponent<AudioSource> ().clip;
			}

			newtag.transform.GetChild (6).GetComponent<UnityEngine.UI.Text> ().text = hititem.transform.GetChild (0).GetComponent<TextMesh> ().text;
			turn_on = true;
		}
	}
}
