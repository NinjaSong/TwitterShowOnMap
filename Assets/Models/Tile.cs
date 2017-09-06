using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Helpers;
using Assets.Models;
using UnityEngine;


namespace Assets
{
    public class Tile : MonoBehaviour
    {
        public Rect Rect;
        public Vector2 cen;
        Dictionary<Vector3, BuildingHolder> BuildingDictionary { get; set; }
        Dictionary<Vector3, WaterHolder> WaterDictionary { get; set; }
        Dictionary<Vector3, ParkHolder> ParkDictionary { get; set; }

        public Tile()
        {
            //Dictionaries to hold buildings/water/parks
            BuildingDictionary = new Dictionary<Vector3, BuildingHolder>();
            WaterDictionary = new Dictionary<Vector3, WaterHolder>();
            ParkDictionary = new Dictionary<Vector3, ParkHolder>();
        }

        public IEnumerator CreateTile(Vector2 realPos, Vector2 worldCenter, int zoom)
        {
            Vector3 tp = transform.position;

            cen = realPos;

            //setting up the URL
            var tilename = Application.persistentDataPath + "/" + realPos.x + "_" + realPos.y;
            var tileurl = realPos.x + "/" + realPos.y;
            var url = "https://tile.mapzen.com/mapzen/vector/v1/water,earth,buildings,roads,landuse/" + zoom + "/";

            //Debug.Log(url);
            JSONObject mapData;

            //If the tile has been created in the past, load from memory
            //otherwise fetch from online and store a copy locally
            //if (File.Exists(tilename))
            //{
            //    var r = new StreamReader(tilename, Encoding.Default);
            //    mapData = new JSONObject(r.ReadToEnd());
            //}
            //else
            //{
            //    var www = new WWW(url + tileurl + ".json?api_key=mapzen-V1twAnp");
            //    yield return www;

            //    print(www.text);
            //    var sr = File.CreateText(tilename);
            //    sr.Write(www.text);
            //    sr.Close();

            //    mapData = new JSONObject(www.text);
            //}

            var www = new WWW(url + tileurl + ".json?api_key=mapzen-V1twAnp");
            yield return www;

            //print(www);
            //print(www.text);
            var sr = File.CreateText(tilename);
            sr.Write(www.text);
            sr.Close();

            mapData = new JSONObject(www.text);

            gameObject.AddComponent<BoxCollider>();

            Rect = GM.TileBounds(realPos, zoom);


		
            //make em
			  CreateBuildings(mapData["buildings"], worldCenter);
            //CreateWater(mapData["water"], worldCenter);
            //CreateParks(mapData["landuse"], worldCenter);
            //CreateRoads(mapData["roads"], worldCenter);
            
        }

		private void CreateBuildings(JSONObject mapData, Vector2 worldCenter)
        {
            //filter to just polygons
            foreach (var geo in mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon"))
            {
                //convert and add points

				var geocenterx = new List<float> ();
				var geocentery = new List<float> ();

                var l = new List<Vector3>();
                for (int i = 0; i < geo["geometry"]["coordinates"][0].list.Count - 1; i++)
                {
                    var c = geo["geometry"]["coordinates"][0].list[i];
					geocenterx.Add (c [1].f);
					geocentery.Add (c [0].f);
                    var bm = GM.LatLonToMeters(c[1].f, c[0].f);

					//c[1].f====>33
                    var pm = new Vector2(bm.x - Rect.center.x, bm.y - Rect.center.y);
                    l.Add(pm.ToVector3xz());
                }


                try
                {
                    var center = l.Aggregate((acc, cur) => acc + cur) / l.Count;

					float centerx = geocenterx.Sum() / geocenterx.Count;
					float centery = geocentery.Sum() / geocentery.Count;



                    if (!BuildingDictionary.ContainsKey(center))
                    {
                        var bh = new BuildingHolder(center, l);
                        for (int i = 0; i < l.Count; i++)
                        {
                            l[i] = l[i] - bh.Center;
                        }
                        BuildingDictionary.Add(center, bh);

                        var m = bh.CreateModel();


						if(geo["properties"]["name"] != null){
							m.name = geo["properties"]["name"].ToString().Split('"')[1];

						}else{
							m.name =  "Unknown_Building";
						}
						var st = m.gameObject.AddComponent<Storage>();
						st.lat = centerx;
						st.lng = centery;

						//Debug.Log(m.name + " --- [" +centerx +"," + centery +"]");
					
						if(geo["properties"]["height"] != null){
							float heighttemp =  float.Parse(geo["properties"]["height"].ToString());
							m.transform.localScale = new Vector3(1,heighttemp/16,1);
						}
						
                        m.transform.parent = this.transform;
                        center = new Vector3(center.x, center.y, center.z);
                        m.transform.localPosition = center;


						m.AddComponent<MeshCollider>();
						m.tag = "Collider";


						if(geo["properties"]["addr_housenumber"] != null){
							
							st.adress = (geo["properties"]["addr_housenumber"].ToString().Split('\"')[1] + " " + geo["properties"]["addr_street"].ToString().Split('\"')[1]);
						}

						//check

//						Collider[] hitC = Physics.OverlapSphere (m.transform.position, 30);
//						for(int i = 0; i < hitC.Length;i++){
//							if(hitC[i].name != "Plane"){
//								GameObject.Destroy(m);
//								break;
//							}else{
//								Debug.Log(hitC[i].name);
//							}
//						}
                    }
                }
                catch (Exception ex)
                {
                    //Debug.Log(ex);
                }
            }
        }

        private void CreateWater(JSONObject mapData, Vector2 worldCenter)
        {
            foreach (var geo in mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon"))
            {
                var l = new List<Vector3>();
                for (int i = 0; i < geo["geometry"]["coordinates"][0].list.Count - 1; i++)
                {
                    var c = geo["geometry"]["coordinates"][0].list[i];
                    var bm = GM.LatLonToMeters(c[1].f, c[0].f);
                    var pm = new Vector2(bm.x - Rect.center.x, bm.y - Rect.center.y);
                    l.Add(pm.ToVector3xz());
                }

                try
                {
                    var center = l.Aggregate((acc, cur) => acc + cur) / l.Count;
                    if (!WaterDictionary.ContainsKey(center))
                    {
                        var bh = new WaterHolder(center, l);
                        for (int i = 0; i < l.Count; i++)
                        {
                            l[i] = l[i] - bh.Center;
                        }
                        WaterDictionary.Add(center, bh);

                        var m = bh.CreateModel();
                        m.name = "water";
						m.tag = "environment";
                        m.transform.parent = this.transform;
                        center = new Vector3(center.x, center.y, center.z);
                        m.transform.localPosition = center;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }

        private void CreateParks(JSONObject mapData, Vector2 worldCenter)
        {
            foreach (var geo in mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon" && x["properties"]["kind"].str == "park"))
            {
                var l = new List<Vector3>();
                for (int i = 0; i < geo["geometry"]["coordinates"][0].list.Count - 1; i++)
                {
                    var c = geo["geometry"]["coordinates"][0].list[i];
                    var bm = GM.LatLonToMeters(c[1].f, c[0].f);
                    var pm = new Vector2(bm.x - Rect.center.x, bm.y - Rect.center.y);
                    l.Add(pm.ToVector3xz());
                }

                try
                {
                    var center = l.Aggregate((acc, cur) => acc + cur) / l.Count;
                    if (!WaterDictionary.ContainsKey(center))
                    {
                        var bh = new ParkHolder(center, l);
                        for (int i = 0; i < l.Count; i++)
                        {
                            l[i] = l[i] - bh.Center;
                        }
                        ParkDictionary.Add(center, bh);

                        var m = bh.CreateModel();
                        m.name = "park";
						m.tag = "environment";
                        m.transform.parent = this.transform;
                        center = new Vector3(center.x, center.y, center.z);
                        m.transform.localPosition = center;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log(ex);
                }
            }
        }

        private void CreateRoads(JSONObject mapData, Vector2 worldCenter)
        {
			
			foreach (var geo in mapData["features"].list) {
				var l = new List<Vector3> ();
				if (geo ["geometry"] ["type"].ToString().Equals( "\"MultiLineString\"")) {
					for (int ii = 0; ii < geo ["geometry"] ["coordinates"].list.Count; ii++) {
						var ll = new List<Vector3> ();
						for (int i = 0; i < geo ["geometry"] ["coordinates"] [ii].list.Count; i++) {	
							var c = geo ["geometry"] ["coordinates"] [ii] [i];
							var bm = GM.LatLonToMeters (c [1].f, c [0].f);

							var pm = new Vector2 (bm.x - Rect.center.x, bm.y - Rect.center.y);

							ll.Add (pm.ToVector3xz ());
						}
						var m = new GameObject ("road2").AddComponent<RoadPolygon> ();
						m.transform.parent = this.transform;
						m.Initialize (geo ["properties"] ["id"].ToString (), this, ll, geo ["properties"] ["kind"].str);
					}
				} else {
					for (int i = 0; i < geo ["geometry"] ["coordinates"].list.Count; i++) {
						var c = geo ["geometry"] ["coordinates"] [i];
						var bm = GM.LatLonToMeters (c [1].f, c [0].f);
						var pm = new Vector2 (bm.x - Rect.center.x, bm.y - Rect.center.y);
						l.Add (pm.ToVector3xz ());
					}

					var m = new GameObject ("road").AddComponent<RoadPolygon> ();
					m.transform.parent = this.transform;


					try {
						m.Initialize (geo ["properties"] ["id"].ToString (), this, l, geo ["properties"] ["kind"].str);

						GameObject.Destroy(m);
					} catch (Exception ex) {
						Debug.Log (ex);
					}
				}
			}
        }

        public void Cleanup()
        {
            foreach(Transform t in transform)
            {
                Destroy(t.gameObject);
            }
        }
    }
}
