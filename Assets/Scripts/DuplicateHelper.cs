using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine;
using UnityEditor;
using Assets;

public class DuplicateHelper : MonoBehaviour {
	string conn;

	void Start () {
		conn = "URI=file:" + Application.dataPath + "/BuildingInfo.db";
		StartCoroutine ( info());
//		StreamWriter sw = new StreamWriter("testlatlng.txt");
//		GameObject[] infos = GameObject.FindGameObjectsWithTag ("info");
//		foreach (GameObject b in infos) {
//			if(b.GetComponent<Storage>()!=null){
//				string line = b.GetComponent<Storage>().lat.ToString() +"\t" +b.GetComponent<Storage>().lng.ToString() + "\t";
//				Vector3 position = transform.TransformPoint (b.transform.position);
//				line += position.x.ToString () + "\t" + position.z.ToString ();
//				sw.WriteLine (line);
//			}
//		}
//		Debug.Log ("--------------");

	}
	IEnumerator info()
	{
		yield return new WaitForSeconds (1); 
		print ("--------------!");
		GameObject[] infos = GameObject.FindGameObjectsWithTag ("info");

		int count = 0;
		foreach (GameObject b in infos) {
			Collider[] hitC = Physics.OverlapSphere (b.transform.position,4.5f);
			if (hitC.Length > 0 && hitC [0].gameObject.GetComponent<Storage> () != null) {
				//Debug.Log ("hit none" + b.GetComponent<Storage> ().address_google + "Belong" + b.transform.parent.name);
			
				Storage s = b.GetComponent<Storage> ();
				if (hitC [0].gameObject.GetComponent<Storage> ().address_google == s.address_google) {
					hitC [0].gameObject.GetComponent<Storage> ().GTID = s.GTID;
				}
				

			}
		}
		StartCoroutine ( StaticAndDatabase());
//		Debug.Log (Physics.CheckSphere(streams.transform.position,0.5f));
//		Collider[] hitC = Physics.OverlapSphere (streams.transform.position,0.5f);
//		int i = 0;
//		while (i < hitC.Length) {
//			Debug.Log (i + " " + hitC [i].name);
//			i++;
//		}


	}

	IEnumerator giveinfo()
	{
		yield return new WaitForSeconds (5); 

		GameObject[] infos = GameObject.FindGameObjectsWithTag ("info");
		GameObject[] builds = GameObject.FindGameObjectsWithTag ("Collider");
		foreach (GameObject build in builds) {
			Mesh m = build.GetComponent<MeshFilter> ().mesh;
			build.GetComponent<MeshCollider> ().sharedMesh = m;
			build.GetComponent<MeshCollider> ().convex = true;
		}

		yield return new WaitForSeconds (2); 
		foreach (GameObject information in infos) {
			
			Collider[] hitC = Physics.OverlapSphere (information.transform.position,3.5f);

			if (hitC.Length == 0) {
				//Debug.Log ("hit none" + information.GetComponent<Storage> ().address_google + "Belong" + information.transform.parent.name);
			} else {
				Storage s = information.GetComponent<Storage> ();
				if (hitC [0].gameObject.GetComponent<Storage> () == null) {
					Storage s_ = hitC [0].gameObject.AddComponent<Storage> ();
					s_.lat = s.lat;
					s_.lng = s.lng;
					s_.adress = s.adress;
					s_.address_google = s.address_google;
					s_.name = s.name;
				} else {
					//Debug.Log ("duplicate" + information.name + " ---in ---" + information.transform.parent.name);
				}

			}


		}
		print (">>>>>> Now Saving >>>>>>>");

		int ciyb = 0;
		foreach (GameObject b in builds) {
			ciyb++;
			Mesh meshobj = b.GetComponent<MeshFilter>().mesh;
			AssetDatabase.CreateAsset (meshobj, "Assets/Mesh/m"+ciyb+".fbx" );
			AssetDatabase.SaveAssets();
		}

		print ("all done!");

//		yield return new WaitForSeconds (2); 
//		print ("checking>>>>>>");
//		yield return new WaitForSeconds (2); 
//		int count = 0;
//		foreach (GameObject build in builds) {
//			if (build.GetComponent<Storage> () == null) {
//				count++;
//				Debug.Log ("No Storage in stream " + count + " " + build.name + " in  " + build.transform.transform.parent.name);
//			}
//		}
//
//		print (">>>>>> ALL DONE" + ">>>>>stream: " + count );


	}

	IEnumerator CheckDupilcate()
	{
		yield return new WaitForSeconds (4); 
		print ("Start Detecting!");

		GameObject[] streams = GameObject.FindGameObjectsWithTag ("Collider");
		int sum = streams.Length;
		int count = 0;
		Dictionary <Vector3, int> streamlist = new Dictionary <Vector3,int> ();

		foreach (GameObject b in streams) {
			//yield return new WaitForSeconds (0.01f);
			count++;
			//Debug.Log (count + "/" + sum);

			try
			{
				Vector3 position = b.transform.TransformPoint(transform.localPosition);
				position.x = (int)position.x/2 * 2f;
				position.z = (float)Math.Round((int)position.z/2 *2f);
				streamlist.Add(position,1);

			}catch(ArgumentException){
				Debug.Log (b);
				GameObject.Destroy (b);
			}


		}
	}

	IEnumerator AddressToStatic()
	{
		yield return new WaitForSeconds (10); 
		print ("Start Detecting!");

		GameObject[] streams = GameObject.FindGameObjectsWithTag ("Collider");
		int sum = streams.Length;
		int count = 0;
		foreach (GameObject b in streams) {
			yield return new WaitForSeconds (0.01f);
			count++;
			Debug.Log (count + "/" + sum);


			if (b.GetComponent<Storage> () != null) {
					Collider[] hitC = Physics.OverlapSphere (b.transform.position, 40);
					for (int i = 0; i < hitC.Length; i++) {
						if (hitC [i].tag == "collider") {
							if (hitC [i].gameObject.GetComponent<Storage> () == null) {
								var add = hitC [i].gameObject.AddComponent<Storage> ();
								add.adress = b.GetComponent<Storage> ().adress;
								
								if (b.name != "building") {
									hitC [i].gameObject.name = b.name;
								}
								Destroy(b.gameObject);
								break;
							} else if (hitC [i].gameObject.GetComponent<Storage> () != null && hitC [i].gameObject.GetComponent<Storage> ().adress == b.GetComponent<Storage> ().adress) {

								
								Destroy(b.gameObject);
								break;
							}
						}
					}
			}else if(b.GetComponent<Storage> () == null){
				b.name = "NoA" + count;
				Collider[] hitC = Physics.OverlapSphere (b.transform.position, 40);
				for (int i = 0; i < hitC.Length; i++) {
					if (hitC [i].tag == "collider") {
						if (hitC [i].gameObject.GetComponent<Storage> () == null) {
							var add = hitC [i].gameObject.AddComponent<Storage> ();
							add.adress = "";
							if (b.name != "building") {
								hitC [i].gameObject.name = b.name;
							}
//							sql = "INSERT INTO Storage (Name, Address, Type) VALUES ( '" + count+hitC [i].gameObject.name.ToString().Replace("'"," ") + "','Null','Static')";
							Destroy(b.gameObject);
							break;
						} else if (hitC [i].gameObject.GetComponent<Storage> () != null && hitC [i].gameObject.GetComponent<Storage> ().adress != "") {
//							sql = "INSERT INTO Storage (Name, Address, Type) VALUES ( '" + count+b.gameObject.name.ToString().Replace("'"," ") + "','Null','Real Time')";
							break;
						} else if(hitC [i].gameObject.GetComponent<Storage> () != null && hitC [i].gameObject.GetComponent<Storage> ().adress == "") {
//							sql = "";

							Destroy(b.gameObject);
							break;
						} 
					}
				}

			}
//			dbcmd.CommandText = sql;
//			dbcmd.ExecuteNonQuery ();
		
		}//end foreach
//		dbcmd.Dispose();
//		dbcmd = null;
//		dbconn.Close ();
//		dbconn = null;
		StartCoroutine(StreamingSaveAndDatabase());

	}


	IEnumerator StreamingSaveAndDatabase()
	{
		yield return new WaitForSeconds (3f);
		Debug.Log ("Saving Streams Mesh >>>>>");
	

//		IDbConnection dbconn;
//		dbconn = (IDbConnection)new SqliteConnection (conn);
//		dbconn.Open ();
//		IDbCommand dbcmd = dbconn.CreateCommand ();
//
		GameObject[] streams = GameObject.FindGameObjectsWithTag ("TAG");
		int count = 0;
//		string sql = "";
		foreach (GameObject b in streams) {
			count++;
			yield return new WaitForSeconds (0.05f);
			Mesh meshobj = b.GetComponent<MeshFilter>().mesh;
			AssetDatabase.CreateAsset (meshobj, "Assets/Mesh/m" + count.ToString() + ".fbx" );
			AssetDatabase.SaveAssets();
//			if (b.GetComponent<Storage> () == null) {
//				sql = "INSERT INTO Storage (Name, Address, Type) VALUES ( '" + count + "-Streaming','Null','Real Time Streaming')";
//			} else {
//				sql = "INSERT INTO Storage (Name, Address, Type) VALUES ( '" + count + "-Streaming','" + b.GetComponent<Storage> ().adress + "','Real Time Streaming')";
//			}
//			dbcmd.CommandText = sql;
//			dbcmd.ExecuteNonQuery ();
		}
		print ("-----------------done----------------");
		StartCoroutine (info());
//		dbcmd.Dispose();
//		dbcmd = null;
//		dbconn.Close ();
//		dbconn = null;
//		StartCoroutine (StaticAndDatabase());

	}

	IEnumerator StaticAndDatabase()
	{

		Debug.Log ("Start -----------SQL----------");
		yield return new WaitForSeconds (10f);

		IDbConnection dbconn;
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();
		IDbCommand dbcmd = dbconn.CreateCommand ();

		GameObject[] statics = GameObject.FindGameObjectsWithTag ("collider");
		GameObject[] streams = GameObject.FindGameObjectsWithTag ("Collider");
		int count = 0;
		string sql = "";
		foreach (GameObject b in statics) {
			count++;
			Debug.Log ("Now: "+count+ " / " + statics.Length);
			Debug.Log ("This: " + b.name +" --- " + b.transform.parent.name);


			if (b.GetComponent<Storage> () != null) {
				b.GetComponent<Storage> ().SQLNum = count;
				string address = b.GetComponent<Storage> ().adress == "" ? b.GetComponent<Storage> ().address_google : b.GetComponent<Storage> ().adress;
				if (b.GetComponent<Storage> ().GTID != "") {
					sql = "INSERT INTO Summary (Number,GTID,Name,Lat,Lng,Address,Area,ConstructionDate,Use) VALUES " +
						"( " + b.GetComponent<Storage> ().SQLNum + ",'" + b.GetComponent<Storage> ().GTID + "','" + b.GetComponent<Storage> ().name.ToString().Replace("'"," ") + "'," + b.GetComponent<Storage> ().lat + "," + b.GetComponent<Storage> ().lng + ",'"+ address.ToString().Replace("'"," ") +"',null,null,null)";
				} else {
					sql = "INSERT INTO Summary (Number,GTID,Name,Lat,Lng,Address,Area,ConstructionDate,Use) VALUES " +
						"( " + b.GetComponent<Storage> ().SQLNum + ",null,'" + b.GetComponent<Storage> ().name.ToString().Replace("'"," ") + "'," + b.GetComponent<Storage> ().lat + "," + b.GetComponent<Storage> ().lng + ",'"+ address.ToString().Replace("'"," ") +"',null,null,null)";
				}//b.GetComponent<Storage> ().lat +"," +b.GetComponent<Storage> ().lng +",'"+ b.GetComponent<Storage> ().adress =="" ? b.GetComponent<Storage>().address_google : b.GetComponent<Storage>().adress +"',null,null,null)";
			} else {
				sql = "INSERT INTO Summary (Number,GTID,Name,Lat,Lng,Address,Area,ConstructionDate,Use) VALUES " +
					"( " + count + ",null,null,null,null,null,null,null,null)";
			}
			dbcmd.CommandText = sql;
			dbcmd.ExecuteNonQuery ();
		}
		Debug.Log ("Static done");

		foreach (GameObject b in streams) {
			count++;
			Debug.Log ("Now: "+(count - statics.Length).ToString()+ " / " + streams.Length);
			Debug.Log ("This: " + b.name +" --- " );


			if (b.GetComponent<Storage> () != null) {
				b.GetComponent<Storage> ().SQLNum = count;
				string address = b.GetComponent<Storage> ().adress == "" ? b.GetComponent<Storage> ().address_google : b.GetComponent<Storage> ().adress;
				if (b.GetComponent<Storage> ().GTID != "") {
					sql = "INSERT INTO Summary (Number,GTID,Name,Lat,Lng,Address,Area,ConstructionDate,Use) VALUES " +
						"( " + b.GetComponent<Storage> ().SQLNum + ",'" + b.GetComponent<Storage> ().GTID + "','" + b.GetComponent<Storage> ().name.ToString().Replace("'"," ") + "'," + b.GetComponent<Storage> ().lat + "," + b.GetComponent<Storage> ().lng + ",'"+ address.ToString().Replace("'"," ") +"',null,null,null)";
				} else {
					sql = "INSERT INTO Summary (Number,GTID,Name,Lat,Lng,Address,Area,ConstructionDate,Use) VALUES " +
						"( " + b.GetComponent<Storage> ().SQLNum + ",null,'" + b.GetComponent<Storage> ().name.ToString().Replace("'"," ") + "'," + b.GetComponent<Storage> ().lat + "," + b.GetComponent<Storage> ().lng + ",'"+ address.ToString().Replace("'"," ") +"',null,null,null)";
				}//b.GetComponent<Storage> ().lat +"," +b.GetComponent<Storage> ().lng +",'"+ b.GetComponent<Storage> ().adress =="" ? b.GetComponent<Storage>().address_google : b.GetComponent<Storage>().adress +"',null,null,null)";
			} else {
				sql = "INSERT INTO Summary (Number,GTID,Name,Lat,Lng,Address,Area,ConstructionDate,Use) VALUES " +
					"( " + count + ",null,null,null,null,null,null,null,null)";
			}
			dbcmd.CommandText = sql;
			dbcmd.ExecuteNonQuery ();
		}

		Debug.Log ("All done");

		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close ();
		dbconn = null;
	}

	IEnumerator EnvironmentSave()
	{

		Debug.Log ("Start Save water & park-------------\n----------------------");
		yield return new WaitForSeconds (15f);

		GameObject[] statics = GameObject.FindGameObjectsWithTag ("environment");
		int count = 0;
		int sum = statics.Length;
		foreach (GameObject b in statics) {
			count++;
			Debug.Log (count + "/" + sum);
			yield return new WaitForSeconds (0.03f);
			Mesh meshobj = b.GetComponent<MeshFilter>().mesh;
			AssetDatabase.CreateAsset (meshobj, "Assets/Mesh/environment" + count.ToString() );
			AssetDatabase.SaveAssets();
		}
		Debug.Log ("done");
		//StartCoroutine (RoadSave());
	}

	IEnumerator RoadSave()
	{

		Debug.Log ("Start Save road-------------\n----------------------");
		yield return new WaitForSeconds (5f);

		GameObject[] statics = GameObject.FindGameObjectsWithTag ("road");
		int count = 0;
		int sum = statics.Length;
		Debug.Log (sum);

		List<CombineInstance> roadList1 = new List<CombineInstance> ();
		List<CombineInstance> roadList2 = new List<CombineInstance> ();
		List<CombineInstance> roadList3 = new List<CombineInstance> ();
		List<CombineInstance> roadList4 = new List<CombineInstance> ();

		CombineInstance test = new CombineInstance();
		GameObject ts = new GameObject ();
		CombineInstance test2 = new CombineInstance();
		GameObject ts2 = new GameObject ();
		CombineInstance test3 = new CombineInstance();
		GameObject ts3 = new GameObject ();
		CombineInstance test4 = new CombineInstance();
		GameObject ts4 = new GameObject ();
		Mesh combineRoadMesh1 = new Mesh ();
		Mesh combineRoadMesh2 = new Mesh ();
		Mesh combineRoadMesh3 = new Mesh ();
		Mesh combineRoadMesh4 = new Mesh ();


		foreach (GameObject b in statics) {
			count++;
			Debug.Log (count + "/" + sum);
			CombineInstance combine = new CombineInstance ();
			Mesh meshobj = b.GetComponent<MeshFilter>().mesh;
			combine.mesh = meshobj;

			combine.transform = b.transform.localToWorldMatrix;

			if (count >= sum / 4 * 3) {
				roadList4.Add (combine);
			}else if(count >= sum / 4 * 2) {
				roadList3.Add (combine);
			}else if(count >= sum / 4) {
				roadList2.Add (combine);
			}else {
				roadList1.Add (combine);
			}

		}

		combineRoadMesh1.CombineMeshes (roadList1.ToArray());
		combineRoadMesh2.CombineMeshes (roadList2.ToArray());
		combineRoadMesh3.CombineMeshes (roadList3.ToArray());
		combineRoadMesh4.CombineMeshes (roadList4.ToArray());
		test.mesh = combineRoadMesh1;
		test.transform = ts.transform.localToWorldMatrix;
		test2.mesh = combineRoadMesh2;
		test2.transform = ts2.transform.localToWorldMatrix;
		test3.mesh = combineRoadMesh3;
		test3.transform = ts3.transform.localToWorldMatrix;
		test4.mesh = combineRoadMesh4;
		test4.transform = ts4.transform.localToWorldMatrix;

		//Mesh combineAllMesh = new Mesh ();
		//combineAllMesh.CombineMeshes (totalMesh, false);
		ts.AddComponent<MeshFilter> ().mesh = combineRoadMesh1;
		ts.AddComponent<MeshRenderer> ();
		AssetDatabase.CreateAsset (combineRoadMesh1, "Assets/Mesh/road11");
		AssetDatabase.SaveAssets();

		ts2.AddComponent<MeshFilter> ().mesh = combineRoadMesh2;
		ts2.AddComponent<MeshRenderer> ();
		AssetDatabase.CreateAsset (combineRoadMesh2, "Assets/Mesh/road22");
		AssetDatabase.SaveAssets();

		ts3.AddComponent<MeshFilter> ().mesh = combineRoadMesh3;
		ts3.AddComponent<MeshRenderer> ();
		AssetDatabase.CreateAsset (combineRoadMesh3, "Assets/Mesh/road33");
		AssetDatabase.SaveAssets();

		ts4.AddComponent<MeshFilter> ().mesh = combineRoadMesh4;
		ts4.AddComponent<MeshRenderer> ();
		AssetDatabase.CreateAsset (combineRoadMesh4, "Assets/Mesh/road44");
		AssetDatabase.SaveAssets();
		Debug.Log ("done");
	}

	IEnumerator StreamSave()
	{

		Debug.Log ("Start Save stream building-------------\n----------------------");
		yield return new WaitForSeconds (5f);

		GameObject[] statics = GameObject.FindGameObjectsWithTag ("Collider");
		int count = 0;
		int sum = statics.Length;
		Debug.Log (sum);

		List<CombineInstance> List1 = new List<CombineInstance> ();
		List<Mesh> Listm = new List<Mesh>();

		CombineInstance test = new CombineInstance();
		GameObject ts = new GameObject ();

		Mesh combineMesh1 = new Mesh ();


		foreach (GameObject b in statics) {
			count++;
			Debug.Log (count + "/" + sum);
			CombineInstance combine = new CombineInstance ();
			combine.mesh = b.GetComponent<MeshFilter>().mesh;
			 

			combine.transform = b.transform.localToWorldMatrix;
			List1.Add (combine);
			Listm.Add (combine.mesh);

		}

		combineMesh1.CombineMeshes (List1.ToArray(),false);

		test.mesh = combineMesh1;
		test.transform = ts.transform.localToWorldMatrix;


		//Mesh combineAllMesh = new Mesh ();
		//combineAllMesh.CombineMeshes (totalMesh, false);
		ts.AddComponent<MeshFilter> ().mesh = combineMesh1;
		ts.AddComponent<MeshRenderer> ();
		//AssetDatabase.CreateAsset (Listm, "Assets/Mesh/am");
		AssetDatabase.SaveAssets();
		Debug.Log(combineMesh1.subMeshCount);

		int i = -1;
		foreach (GameObject b in statics) {
			b.GetComponent<MeshFilter>().mesh = Listm[i++];
		}

		Debug.Log ("done");
	}

}
