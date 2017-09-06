using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
	
[RequireComponent(typeof(AudioSource))]
public class UIPopUp : MonoBehaviour {

	Color backcolor = new Color(0.843f,0.843f,0.325f,0.8f);
	float tagsizew = 0;
	float tagsizeh = 0;
	bool coloredit =false;
	bool sizeedit = false;
	float direction;
	string tagstr;
	TagUIManager UiManager;

	void Start () {
		UiManager = GameObject.Find ("Maneger").GetComponent<TagUIManager> ();
		DirectionSelect ();
		InputField maininput = transform.GetComponentInChildren<InputField>();
		maininput.Select ();
		maininput.text = "";
	}

	public void ClickSubmit()
	{
		InputField maininput = transform.GetComponentInChildren<InputField>();
		tagstr = maininput.text.ToString();
		GameObject newtag = Instantiate(UiManager.thetag,UiManager.point,Quaternion.identity);
		string tagname = "Tag_" + UiManager.tagindex;
		UiManager.tagindex++;
		newtag.gameObject.name = tagname;
		UiManager.taglist.Add (newtag);

		var rotaionVector = newtag.gameObject.transform.rotation.eulerAngles;
		rotaionVector.y = direction;
		newtag.gameObject.transform.rotation = Quaternion.Euler (rotaionVector);

		RectTransform rt = newtag.gameObject.transform.GetComponent<RectTransform> ();

		rt.localScale += new Vector3 (tagsizew, tagsizeh,0);

		if (tagsizew == 1) {
			Transform rrt = newtag.gameObject.transform.GetChild (1).GetComponent<Transform> ();
			rrt.localScale += new Vector3 (-5f, 0,0);
			Transform textscale =  newtag.gameObject.transform.GetChild (0).GetComponent<Transform> ();
			textscale.localScale = new Vector3 (0.05f,0.06f,0.2f);
		}
		if(tagsizew == 1.6f) {
			Transform rrt = newtag.gameObject.transform.GetChild (1).GetComponent<Transform> ();
			rrt.localScale += new Vector3 (-6.7f,0,0);
			Transform textscale =  newtag.gameObject.transform.GetChild (0).GetComponent<Transform> ();
			textscale.localScale = new Vector3 (0.04f,0.05f,0.2f);
		}


		Material backgroundcolor = newtag.gameObject.transform.GetComponent<Renderer>().material;
		Material flagcolor = newtag.gameObject.transform.GetChild (1).GetComponent<Renderer> ().material;
		backgroundcolor.color = backcolor;

		flagcolor.color = backcolor;

		TextMesh tagtext = newtag.gameObject.transform.GetComponentInChildren<TextMesh>();
		tagtext.text = tagstr;
		AudioSource aud = newtag.AddComponent<AudioSource> ();	
		aud.clip = transform.GetComponent<AudioSource> ().clip;
		newtag.transform.GetChild (3).GetComponent<Canvas>().worldCamera = GameObject.Find ("CenterEyeAnchor").transform.GetComponent<Camera> ();
		newtag.GetComponentInChildren<OVRRaycaster> ().pointer = FindObjectOfType<OVRGazePointer> ().gameObject;

		GameObject.Destroy(transform.gameObject);
		UiManager.turn_on = false;
		string foldername = @"C:\Users\Public\Documents\UnityTags";
		System.IO.Directory.CreateDirectory (foldername);

		Application.CaptureScreenshot ("C:/Users/Public/Documents/UnityTags/"+tagname+".png");
	}


	public void DirectionSelect()
	{
		Vector3 newPosition = UiManager.point;
		Vector3 positon2 = UiManager.point2;
		Vector3 positon3 = UiManager.point3;
		Debug.DrawLine (newPosition,positon2,Color.blue,100f);
		Debug.DrawLine (positon2,positon3,Color.red,100f);
		Debug.DrawLine (positon3,newPosition,Color.white,100f);
		Plane p = new Plane();
		p.Set3Points (newPosition,positon3,positon2);
		//Debug.Log ("p.normal"+p.normal);

		if (p.normal.x == 0) {
			if (p.normal.z == -1f) {//front
				direction = 0f;
			} else {//back
				direction = 180f;
			}
		} else if (p.normal.z == 0) {
			if (p.normal.x == 1f) {//left
				direction = -90f;
			} else {//right
				direction = 90f;
			}
		} else {
			float angle = Vector3.Angle (new Vector3 (0, 0, -1f), p.normal);
			//Debug.Log("angle"+angle);
			if (p.normal.x < 0) {
				direction = angle;
			} else {
				direction = -angle;
			}
		}
	}
	public void ClickCancel()
	{
		GameObject.Destroy(transform.gameObject);
		UiManager.turn_on = false;
	}

	public void ClickDelete()
	{
		UiManager.deleteItem = UiManager.hitItem.name;
		GameObject.Destroy(UiManager.hitItem);

		GameObject.Destroy(transform.gameObject);
		UiManager.turn_on = false;
	}

	public void ColorRed()
	{		
		backcolor = new Color(0.95f,0.247f,0.247f,0.8f);
		coloredit = true;
	}
	public void ColorYellow()
	{
		backcolor = new Color(0.843f,0.843f,0.325f,0.8f);
		coloredit = true;
	}
	public void ColorGreen()
	{
		backcolor = new Color(0.588f,0.843f,0.325f,0.8f);
		coloredit = true;
	}
	public void SizeSmall()
	{
		sizeedit = true;
		tagsizew = 0;
		tagsizeh = 0;
	}
	public void SizeMedium()
	{
		sizeedit = true;
		tagsizew = 1;
		tagsizeh = 0.8f;
	}

	public void SizeLarge()
	{
		sizeedit = true;
		tagsizew = 	1.6f;
		tagsizeh =  1.2f;
	}

	public void recordDown()
	{
		AudioSource aud = transform.gameObject.GetComponent<AudioSource> ();
		aud.clip = null;
		aud.clip = Microphone.Start ("Microphone",false,100,44100);
		transform.GetChild (4).GetChild (2).gameObject.SetActive (true);
		transform.GetChild (4).GetChild (2).gameObject.GetComponent<CountSecVoice> ().S ();
	}

	public void recordUp()
	{
		AudioSource aud = transform.gameObject.GetComponent<AudioSource> ();
		Microphone.End ("Microphone");

		GameObject saveinfo = transform.GetChild (4).GetChild (2).gameObject;
		transform.GetChild (4).GetChild (2).gameObject.GetComponent<CountSecVoice> ().SStop ();
		saveinfo.GetComponent<Text> ().text = "Saved!";

		aud.Play ();
	}

	public void clickToListen()
	{
		AudioSource aud = transform.GetComponent<AudioSource> ();
		if (aud.clip != null) {
			aud.Play ();
		}
	}


	public void EditClickSubmit()
	{
		InputField maininput = transform.GetComponentInChildren<InputField>();
		tagstr = maininput.text.ToString();
		TextMesh tagtext = UiManager.hitItem.transform.GetComponentInChildren<TextMesh>();
		tagtext.text = tagtext.text.ToString() + "<color=cyan><i>" +tagstr +"</i></color>";

		if (coloredit) {
			Material backgroundcolor = UiManager.hitItem.transform.GetComponent<Renderer> ().material;
			backgroundcolor.color = backcolor;
			Material flagcolor = UiManager.hitItem.transform.GetChild (1).GetComponent<Renderer> ().material;
			flagcolor.color = backcolor;
		}


		RectTransform rt = UiManager.hitItem.transform.GetComponent<RectTransform> ();
		Vector3 originscale = rt.localScale;
		if (sizeedit) {//change
			
			rt.localScale = new Vector3 (tagsizew + 1, tagsizeh + 1, rt.localScale.z);
			if (tagsizew == 0) {

				Transform textscale =  UiManager.hitItem.gameObject.transform.GetChild (0).GetComponent<Transform> ();
				textscale.localScale = new Vector3 (0.1f,0.1f,0.2f);
				tagtext.text = UiManager.hitItem.transform.GetChild (0).GetComponent<WordWrapTextMesh> ().wordwrapmesh (tagtext.text, 15);
			}
			if (tagsizew == 1) {

				Transform textscale =  UiManager.hitItem.gameObject.transform.GetChild (0).GetComponent<Transform> ();
				textscale.localScale = new Vector3 (0.05f,0.06f,0.2f);
				tagtext.text = UiManager.hitItem.transform.GetChild (0).GetComponent<WordWrapTextMesh> ().wordwrapmesh (tagtext.text, 28);
			}
			if(tagsizew == 1.6f) {

				Transform textscale =  UiManager.hitItem.gameObject.transform.GetChild (0).GetComponent<Transform> ();
				textscale.localScale = new Vector3 (0.04f,0.05f,0.2f);
				tagtext.text = UiManager.hitItem.transform.GetChild (0).GetComponent<WordWrapTextMesh> ().wordwrapmesh (tagtext.text, 39);
			}
		}


		GameObject.Destroy(transform.gameObject);
		UiManager.turn_on = false;
	}

}

