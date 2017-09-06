using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Helpers;
using Assets;

public class tileCheck : MonoBehaviour
{
	//Vector2[,] localTerrain = new Vector2[9, 13];
	//GameObject[,] tiles = new GameObject[9, 13];
	Vector2[,] localTerrain = new Vector2[2, 2];
	GameObject[,] tiles = new GameObject[2, 2];
	float currX;
	float currZ;
	float oldX;
	float oldZ;
	public GameObject tile;

	public Vector2 LatLng; 
	Vector2 Center;
	public Vector2 startTile;
	Vector2 Position;
	private int Zoom = 16;
	string status = "start";
	// Use this for initialization

	IEnumerator Start()
	{
		Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
		//SimplePool.Preload(tile, 117);
		SimplePool.Preload(tile, 4);
		currX = oldX = Mathf.Floor(transform.position.x);
		currZ = oldZ = Mathf.Floor(transform.position.z);
		Center = calcTile(LatLng.x, LatLng.y);
		//Debug.Log("Center: " + Center);
		startTile = Center;
		Position = posInTile(LatLng.x, LatLng.y);


		//Debug.Log(Position);
		Vector3 pos = new Vector3((Position.x - 0.5f) * 611, 0, (0.5f - Position.y) * 611);
		//transform.position = pos;
		//Debug.Log(pos);
		status = "no location service";

		tiles[0, 0] = SimplePool.Spawn(tile, Vector3.zero, Quaternion.identity);
		StartCoroutine(tiles[0, 0].GetComponent<Assets.Tile>().CreateTile(new Vector2(Center.x, Center.y), Vector3.zero, 16));

		updateBoard();
		yield break;
	}

	//checks if theres a tile in that location, if not then put one down
	void updateBoard()
	{
//		for (int i = -3; i <= 5; i++)
//		{
//			for (int j = -7; j <= 5; j++)
//			{
//				localTerrain[i + 3, j + 7] = new Vector2(Center.x + i, Center.y + j);
//				tiles[i + 3, j + 7] = SimplePool.Spawn(tile, new Vector3(currX + i * 612, 0f, currZ + j * 612), Quaternion.identity);
//				StartCoroutine(tiles[i + 3, j + 7].GetComponent<Tile>().CreateTile(new Vector2(Center.x + i, Center.y - j), new Vector3(currX + i * 612, 0f, currZ + j * 611), 16));
//			}
//		}
				for (int i = -1; i <= 0; i++)
				{
					for (int j = -1; j <= 0; j++)
					{
						localTerrain[i + 1, j + 1] = new Vector2(Center.x + i, Center.y + j);
						tiles[i + 1, j + 1] = SimplePool.Spawn(tile, new Vector3(currX + i * 612, 0f, currZ + j * 612), Quaternion.identity);
						StartCoroutine(tiles[i + 1, j + 1].GetComponent<Tile>().CreateTile(new Vector2(Center.x + i, Center.y - j), new Vector3(currX + i * 612, 0f, currZ + j * 611), 16));
					}
				}
		cleanup(0);
	}

	//cleanup tiles outside a certain range
	void cleanup(float sec)
	{
		Collider[] include = Physics.OverlapSphere(transform.position, 1800f);
		Collider[] exclude = Physics.OverlapSphere(transform.position, 10000f);
		var outside = exclude.Except(include);
		foreach (Collider g in outside)
		{
			if (g.tag == "Tile")
			{
				g.GetComponent<Tile>().Cleanup();
				SimplePool.Despawn(g.gameObject);
			}
		}
	}

	Vector2 calcTile(float lat, float lng)
	{
		//pseudo
		//n = 2 ^ zoom
		//xtile = n * ((lon_deg + 180) / 360)
		//ytile = n * (1 - (log(tan(lat_rad) + sec(lat_rad)) / π)) / 2

		float n = Mathf.Pow(2, Zoom);
		float xtile = n * ((lng + 180) / 360);
		float ytile = n * (1 - (Mathf.Log(Mathf.Tan(Mathf.Deg2Rad * lat) + (1f / Mathf.Cos(Mathf.Deg2Rad * lat))) / Mathf.PI)) / 2f;
		return new Vector2((int)xtile, (int)ytile);
	}

	Vector2 posInTile(float lat, float lng)
	{
		float n = Mathf.Pow(2, Zoom);
		float xtile = n * ((lng + 180) / 360);
		float ytile = n * (1 - (Mathf.Log(Mathf.Tan(Mathf.Deg2Rad * lat) + (1f / Mathf.Cos(Mathf.Deg2Rad * lat))) / Mathf.PI)) / 2f;
		return new Vector2(xtile - (int)xtile, ytile - (int)ytile);
	}

}

