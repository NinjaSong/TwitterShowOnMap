using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class SQLDataConn : MonoBehaviour {

	[SerializeField] Material HightLight;
	[SerializeField] GameObject InfomationUI;
	GameObject Info;
	bool act = false;

	string conn;
	IDbConnection dbconn;

	void Start () {
		conn = "URI=file:" + Application.dataPath + "/BuildingInfo.db";
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown (0) ) {
			
			RaycastHit hitInfo = new RaycastHit();
			bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
			if (hit && hitInfo.transform.GetComponent<Storage>() != null) {
				
				int sqlN = hitInfo.transform.GetComponent<Storage> ().SQLNum;
				string GTID = hitInfo.transform.GetComponent<Storage> ().GTID;
				if (act) {
					GameObject.Destroy (Info);
					act = false;
				}
				SQLreader (sqlN,GTID);

			}
		}

	}
	void SQLreader(int sqlN,string GTID)
	{
		IDbCommand dbcmd = dbconn.CreateCommand ();
		string query = "SELECT infoSummary.* FROM infoSummary WHERE Number = " + sqlN.ToString();
		dbcmd.CommandText = query;
		IDataReader reader = dbcmd.ExecuteReader ();
		while (reader.Read ()) {
			string name = reader.GetString (2);
			string address = reader.GetString(5);
			float lat = reader.GetFloat(3);
			float lng = reader.GetFloat(4);
			string infoshow = "Name: " + name +"\nAddress: " + address +"\nLat: "+ lat.ToString() +" Lng: " +lng.ToString() +"\n";
			if (!reader.IsDBNull (6)) {
				int area = reader.GetInt32 (6);
				int conDate = reader.GetInt32(7);
				string use = reader.GetString(8);
				infoshow = "Name: " + name +"\nAddress: " + address +"\nLat: "+ lat.ToString() +" Lng: " +lng.ToString() 
					+ "\nArea: " +area.ToString() +"\tConstrDate: " + conDate.ToString() +"\tUse: " + use;
			}
			Info = Instantiate (InfomationUI, new Vector3(0,0,0), Quaternion.identity);
			Info.GetComponentInChildren<UnityEngine.UI.Text> ().text = infoshow;
			act = true;
		}
		query = "SELECT name FROM sqlite_master WHERE name = 'GTID_"+ GTID +"'";
		dbcmd.Dispose ();
		dbcmd.CommandText = query;
		reader = dbcmd.ExecuteReader ();
		if (reader.IsDBNull (0)) {
			Info.GetComponentInChildren<UnityEngine.UI.Button> ().transform.gameObject.SetActive (false);
		} else {
			query = "SELECT AVG(Power),MIN(Power), MAX(Power) FROM GTID_" + GTID;
			dbcmd.Dispose ();
			dbcmd.CommandText = query;
			reader = dbcmd.ExecuteReader ();
			while(reader.Read()){
				string report = "AVG Power : " + reader.GetDouble(0).ToString() +"\nMIN Power : "
					+ reader.GetDouble(1).ToString() + "\nMAX Power : " + reader.GetDouble (2).ToString();
				Info.transform.GetChild (3).GetComponentInChildren<UnityEngine.UI.Text> ().text = report;
		
			}
		}
		reader.Close ();
		reader = null;
		dbcmd.Dispose ();
		dbcmd = null;
	}
}
