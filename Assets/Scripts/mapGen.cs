using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Helpers;
using Assets.Models;
using Assets;
using UnityEngine;

public class mapGen : MonoBehaviour {
    
    public Vector2 cen;
    public int zoom;
    Dictionary<Vector3, BuildingHolder> BuildingDictionary { get; set; }
    Dictionary<Vector3, WaterHolder> WaterDictionary { get; set; }
    Dictionary<Vector3, ParkHolder> ParkDictionary { get; set; }
    JSONObject mapData;

    // Use this for initialization
    IEnumerator Start () {
        BuildingDictionary = new Dictionary<Vector3, BuildingHolder>();
        WaterDictionary = new Dictionary<Vector3, WaterHolder>();
        ParkDictionary = new Dictionary<Vector3, ParkHolder>();

        var url = "https://tile.mapzen.com/mapzen/vector/v1/water,earth,buildings,roads,landuse/" + zoom + "/";

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                Vector2 realPos = calcTile(cen.x + i, cen.y - j);
                var tileurl = realPos.x + "/" + realPos.y;
                var query = url + tileurl + ".json?api_key=mapzen-V1twAnp";
                print(query);
                var www = new WWW(query);
                yield return www;
                print(www.text);
                mapData = new JSONObject(www.text);

                Rect Rect = GM.TileBounds(realPos, zoom);
                print(Rect);

                Vector2 worldCenter = new Vector2(i * 612f, (float)611.5*j);
                print(worldCenter);
                CreateBuildings(mapData["buildings"], worldCenter, Rect);
                CreateWater(mapData["water"], worldCenter, Rect);
                CreateParks(mapData["landuse"], worldCenter, Rect);
                //CreateRoads(mapData["roads"], worldCenter, Rect);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateBuildings(JSONObject mapData, Vector2 worldCenter, Rect Rect)
    {
        //filter to just polygons
        foreach (var geo in mapData["features"].list.Where(x => x["geometry"]["type"].str == "Polygon"))
        {
            //convert and add points
            var l = new List<Vector3>();
            for (int i = 0; i < geo["geometry"]["coordinates"][0].list.Count - 1; i++)
            {
                var c = geo["geometry"]["coordinates"][0].list[i];
                var bm = GM.LatLonToMeters(c[1].f, c[0].f);
                var pm = new Vector2(bm.x - Rect.center.x, bm.y - Rect.center.y);
                l.Add(pm.ToVector3xz());
            }
            //make them buildings
            try
            {
                var center = l.Aggregate((acc, cur) => acc + cur) / l.Count;
                print(center);
                if (!BuildingDictionary.ContainsKey(center))
                {
                    var bh = new BuildingHolder(center, l);
                    for (int i = 0; i < l.Count; i++)
                    {
                        l[i] = l[i] - bh.Center;
                    }
                    BuildingDictionary.Add(center, bh);

                    var m = bh.CreateModel();
                    m.name = "building";
                    m.transform.parent = this.transform;
                    center = new Vector3(center.x + worldCenter.x, center.y, center.z + worldCenter.y);
                    m.transform.localPosition = center;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    private void CreateWater(JSONObject mapData, Vector2 worldCenter, Rect Rect)
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

    private void CreateParks(JSONObject mapData, Vector2 worldCenter, Rect Rect)
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

    //private void CreateRoads(JSONObject mapData, Vector2 worldCenter, Rect Rect)
    //{
    //    foreach (var geo in mapData["features"].list)
    //    {
    //        var l = new List<Vector3>();

    //        for (int i = 0; i < geo["geometry"]["coordinates"].list.Count; i++)
    //        {
    //            var c = geo["geometry"]["coordinates"][i];
    //            var bm = GM.LatLonToMeters(c[1].f, c[0].f);
                
    //            var pm = new Vector2(bm.x - Rect.center.x, bm.y - Rect.center.y);
    //            l.Add(pm.ToVector3xz());
    //        }

    //        var m = new GameObject("road").AddComponent<RoadPolygon>();
    //        m.transform.parent = this.transform;
    //        try
    //        {
    //            m.Initialize(geo["properties"]["id"].str, gameObject, l, geo["properties"]["kind"].str);
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.Log(ex);
    //        }
    //    }
    //}


    Vector2 calcTile(float lat, float lng)
    {
        //pseudo    
        //n = 2 ^ zoom
        //xtile = n * ((lon_deg + 180) / 360)
        //ytile = n * (1 - (log(tan(lat_rad) + sec(lat_rad)) / π)) / 2

        float n = Mathf.Pow(2, zoom);
        float xtile = n * ((lng + 180) / 360);
        float ytile = n * (1 - (Mathf.Log(Mathf.Tan(Mathf.Deg2Rad * lat) + (1f / Mathf.Cos(Mathf.Deg2Rad * lat))) / Mathf.PI)) / 2f;
        return new Vector2((int)xtile, (int)ytile);
    }
}
