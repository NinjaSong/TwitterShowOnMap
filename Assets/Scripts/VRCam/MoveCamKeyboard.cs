using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamKeyboard : MonoBehaviour {

	private Vector3 origin;
	private float speed;
	public Transform Cam;
	// Use this for initialization
	void Start () {
		origin = transform.position;
		speed = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.W)) {
			Cam.position += transform.forward * speed*2 ;
		}//go ahead

		if (Input.GetKey (KeyCode.S)) {
			Cam.position -= transform.forward * speed*2 ;
		}//go back

		if (Input.GetKey (KeyCode.A)) {
			Cam.eulerAngles -= new Vector3 (0,speed/2,0);
		}//go left

		if (Input.GetKey (KeyCode.D)) {
			Cam.eulerAngles += new Vector3 (0,speed/2,0);
		}//go right
		if (Input.GetKey (KeyCode.Q)) {
			Cam.position += new Vector3 (0, speed, 0);
		}//go up
		if (Input.GetKey (KeyCode.E)) {
			Cam.position -= new Vector3 (0, speed, 0);
		}//go down
	}
}
