using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using Mono.Data.Sqlite;
using System.Data;
using System.Globalization;


public class UserAI : MonoBehaviour {
	public string status;
	NavMeshAgent agent;
	[SerializeField] GameObject TwBox;
	List <DateTime> times;
	List <Vector3> positions;
	List <string> contents;

	string conn;
	IDbConnection dbconn;

	DateTime StartTime;//there is a standard time later, now just for one user
	int index;
	bool pathpend = false;

	void Start () {
		conn = "URI=file:" + Application.dataPath + "/TwData.db";
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();
		IDbCommand dbcmd = dbconn.CreateCommand ();
		string queryuser = "SELECT a.Time, a.Position, a.content FROM Twcsv AS a WHERE a.userID ='"+transform.name +"'";
		dbcmd.CommandText = queryuser;
		IDataReader reader = dbcmd.ExecuteReader ();
		times = new List<DateTime>();
		positions = new List<Vector3> ();
		contents = new List<string> ();

		while (reader.Read ()) {
			//Debug.Log(reader.GetDateTime(0));
			times.Add (reader.GetDateTime (0));
			float lat = float.Parse(reader.GetString (1).Split(',')[0],CultureInfo.InvariantCulture);
			float lng = float.Parse(reader.GetString (1).Split(',')[1],CultureInfo.InvariantCulture);
			Vector3 initPosition = TransLatLngToVector3 (lat,lng);
			positions.Add (initPosition);
			contents.Add (reader.GetString(2));

		}
		dbcmd.Dispose ();
		dbcmd = null;

		index = 0;

		StartTime = FindObjectOfType<TwInitial>().nowTime;

		agent = GetComponent<NavMeshAgent> ();
		if(agent.destination.y > 100f){
			GameObject.Destroy(transform.gameObject);
		}

	}

	// Update is called once per frame
	void Update () {
		StartTime =StartTime.AddSeconds (1);
		status = index + "/" + times.Count + "to " + agent.destination+"/" + positions[index] + "sta: " + agent.pathStatus;

		if (StartTime < times [0]) {
			transform.GetComponent<MeshRenderer> ().enabled = false;
			transform.GetComponent<Collider> ().enabled = false;
		} else {
			transform.GetComponent<MeshRenderer> ().enabled = true;
			transform.GetComponent<Collider> ().enabled = true;
		}//For time control

		/*
		 * Actually there are __ status:
		 *  1. position in navmesh and createde destination succussflly --> Success part (calculate speed)
		 *  2. position not in navmesh
		 * 		2.1 nopath --> find the closest position
		 *  	
		 * 
		 * 
		 */


		if ((pathpend && !agent.hasPath) ||(pathpend && agent.pathStatus == NavMeshPathStatus.PathPartial)) {
			pathpend = false;
			NavMeshHit hit;
			if (NavMesh.SamplePosition (positions [index], out hit, 500, NavMesh.AllAreas)) {
				agent.SetDestination (hit.position);
				NavMeshPath pat = new NavMeshPath ();

				agent.CalculatePath (agent.destination, pat);
				agent.speed = CalSpeed (pat);
				for (int i = 0; i < pat.corners.Length - 1; i++) {
					Debug.DrawLine (pat.corners [i], pat.corners [i + 1], Color.blue,1000);
				}
			} else {
				Debug.Log (transform.name + "----------------");
			}
		}// find the closest position

		if (pathpend && !agent.pathPending && index+1 <= times.Count) {
			pathpend = false;
			NavMeshPath pat = new NavMeshPath ();
			agent.CalculatePath (agent.destination, pat);
			agent.speed = CalSpeed (pat);
			for (int i = 0; i < pat.corners.Length - 1; i++) {
				Debug.DrawLine (pat.corners [i], pat.corners [i + 1], Color.green,1000);
			}

		} // Success Part

		if (StartTime >= times [index] && index + 1 < times.Count) {
			//Tw:
			GameObject box =  Instantiate(TwBox,positions[index],Quaternion.identity);
			box.name = transform.name + "/ " + index;
			Debug.Log("now: " + StartTime + " Going to: " + positions [index]);
			//then move
			agent.SetDestination (positions [index + 1]);

			NavMeshPath pat = new NavMeshPath ();
			agent.CalculatePath (agent.destination, pat);

			for (int i = 0; i < pat.corners.Length - 1; i++) {
				Debug.DrawLine (pat.corners [i], pat.corners [i + 1], Color.red,1000);
			}

			pathpend = true;
			Debug.DrawRay (positions [index], Vector3.up * 600, Color.red, 1000, false);
			//Debug.Log ("now: " + times [index] + " Going to: " + positions [index]);

			index++;
		} else if (index + 1 == times.Count){
			transform.GetComponent<MeshRenderer> ().enabled = false;
			transform.GetComponent<Collider> ().enabled = false;
		}

	}

	Vector3 TransLatLngToVector3 (float lat,float lng)
	{
		return new Vector3 (111403 * lng +9401771,3f, 134027 *lat -4527011);


	}

	float CalSpeed(NavMeshPath path){
		
		if (path.status == NavMeshPathStatus.PathComplete) {
			if (positions [index] == positions [index - 1]) {
				return 0;
			}
			float dis = agent.remainingDistance;
			//add 1 sec per frame in Update()
			TimeSpan interval = times [index] - times [index - 1];
			float time = (float)interval.TotalSeconds * Time.deltaTime;
			return dis / time;
		} else if(path.status == NavMeshPathStatus.PathPartial){
			TimeSpan interval = times [index] - times [index - 1];
			float time = (float)interval.TotalSeconds * Time.deltaTime;
			float dis = Vector3.Distance (positions [index], positions [index - 1]);
			return dis / time;
		}else {
			return 0;
		}
	}
}
