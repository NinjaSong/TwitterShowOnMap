using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class SQLTimeColor : MonoBehaviour {
	string GTID;
	Material colorBall;
	[HideInInspector]public string nowTime;
	[HideInInspector]public double nowAVG;
	[HideInInspector]public double totalAVG;
	[HideInInspector]public double min;
	[HideInInspector]public double max;
	Color yellow;
	Color maxRed;
	Color minGreen;

	string conn;
	IDbConnection dbconn;

	void Start () {
		conn = "URI=file:" + Application.dataPath + "/BuildingInfo.db";
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();
		IDbCommand dbcmd = dbconn.CreateCommand ();
		string query = "SELECT AVG(a.Power), MIN(a.Power), MAX(a.Power) FROM GTID_" + this.GetComponent<Storage>().GTID + " AS a";
		dbcmd.CommandText = query;
		IDataReader reader = dbcmd.ExecuteReader ();
		while (reader.Read ()) {
			try{
			totalAVG = reader.GetDouble (0);
			}catch(Exception ex){
				Debug.Log (this.GetComponent<Storage>().GTID);
			}
			min = reader.GetDouble (1);
			max = reader.GetDouble (2); 
			//Debug.Log (totalAVG +" min : " + min +" --- max: " +max);
		}
		colorBall = new Material (Shader.Find("Diffuse"));
		yellow = new Color();
		minGreen = new Color();
		maxRed = Color.red;
		ColorUtility.TryParseHtmlString ("#F0DE81FF",out yellow);
		ColorUtility.TryParseHtmlString ("#4DC853FF",out minGreen);
		colorBall.color = yellow;
		this.GetComponent<Renderer> ().material = colorBall;
		dbcmd.Dispose ();
		StartCoroutine (WeeklyUpdate());
	}


	IEnumerator WeeklyUpdate()
	{
		yield return new WaitForSeconds (3f);
		int week = 0;
		IDbCommand dbcmd = dbconn.CreateCommand ();
		while (week < 244){
			yield return new WaitForSeconds (1f);
			string query = "SELECT a.Time, AVG(a.Power) FROM (SELECT * FROM GTID_" + this.GetComponent<Storage> ().GTID + " LIMIT 576 OFFSET " + (week * 576).ToString () + " ) AS a";
			dbcmd.CommandText = query;
			IDataReader reader = dbcmd.ExecuteReader ();
			while (reader.Read ()) {
				nowTime = reader.GetString (0);
				if (!reader.IsDBNull (1)) {
					nowAVG = reader.GetDouble (1);
				} else {
					nowAVG = 0;
				}
				if (nowAVG >= totalAVG) {
					double change = (nowAVG - totalAVG) / (max - totalAVG);
					colorBall.color = Color.Lerp (yellow,maxRed,(float)change);

				} else {
					double change = (nowAVG - min) / (totalAVG - min);
					colorBall.color = Color.Lerp (minGreen,yellow,(float)change);
				}

			}

			week++;
			dbcmd.Dispose ();

		}

		dbcmd.Dispose ();
		dbcmd = null;

	}
	// Update is called once per frame
	void Update () {
		
	}
}
