using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Globalization;
using System;

public class TwInitial : MonoBehaviour {
	string conn;
	IDbConnection dbconn;
	[HideInInspector] public DateTime nowTime;
	[SerializeField] GameObject user;
	// Use this for initialization
	void Start () {
		UnityEngine.AI.NavMeshAgent agent = transform.GetComponent<UnityEngine.AI.NavMeshAgent> ();
		conn = "URI=file:" + Application.dataPath + "/TwData.db";
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();
		IDbCommand dbcmd = dbconn.CreateCommand ();
		string queryusers = "SELECT MIN(Twcsv.Time), Twcsv.* FROM Twcsv, (SELECT Count(a.userID),* FROM Twcsv AS a WHERE a.userID NOT LIKE UPPER('tmj%') AND a.userID NOT LIKE UPPER('%Wx') " +
		                    "AND a.Position LIKE '33.7%' " +
			"GROUP BY a.userID ORDER BY COUNT(a.userID) DESC LIMIT 35) AS b WHERE Twcsv.userID = b.userID GROUP BY b.userID ORDER BY Twcsv.Time";
		dbcmd.CommandText = queryusers;
		IDataReader reader = dbcmd.ExecuteReader ();
		while (reader.Read ()) {
			string userID = reader.GetString (3);
			float lat = float.Parse(reader.GetString (2).Split(',')[0],CultureInfo.InvariantCulture);
			float lng = float.Parse(reader.GetString (2).Split(',')[1],CultureInfo.InvariantCulture);
			Vector3 initPosition = TransLatLngToVector3 (lat,lng);

			UnityEngine.AI.NavMeshHit closestHit;
			if (UnityEngine.AI.NavMesh.SamplePosition (initPosition, out closestHit, 500,UnityEngine.AI.NavMesh.AllAreas)) {
				initPosition = closestHit.position;
			}

			GameObject theuser = Instantiate (user, initPosition, Quaternion.identity);

			theuser.name = userID;

		}
		dbcmd.Dispose ();

		string querytime = "SELECT Twcsv.Time FROM Twcsv, (" + queryusers + ") AS b WHERE Twcsv.userID = b.userID ORDER BY Twcsv.Time";
		dbcmd.CommandText = querytime;
		reader = dbcmd.ExecuteReader ();
		while (reader.Read ()) {
			nowTime = reader.GetDateTime (0);
			break;
		}
		dbcmd.Dispose ();

		dbcmd = null;
	}

//	void Update()
//	{
//		nowTime = nowTime.AddSeconds (1);
//		Debug.Log (nowTime);
//	}

	public Vector3 TransLatLngToVector3 (float lat,float lng)
	{
		return new Vector3 (111403 * lng +9401771,3f, 134027 *lat -4527011);
	}
}
