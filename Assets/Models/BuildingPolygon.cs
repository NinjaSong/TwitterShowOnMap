using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Helpers;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

    class BuildingPolygon : MonoBehaviour
    {
        public void Initialize(List<Vector3> verts)
        {
            GetComponent<MeshFilter>().mesh = CreateMesh(verts);

        }

        private static Mesh CreateMesh(List<Vector3> verts)
        {
            var height = 20;
            Triangulator tr = new Triangulator(verts.Select(x => x.ToVector2xz()).ToArray());
            int[] tris = tr.Triangulate();
            List<Vector3> allVerts = new List<Vector3>(verts.Select(x => new Vector3(x.x, height, x.z)));
            allVerts.AddRange(verts);
            List<int> triangles = new List<int>(tris);
            int n = verts.Count;
            for (int i = 0; i < n; i++)
            {
                triangles.Add(i);
                triangles.Add((i + n + 1) % (2*n));
                triangles.Add((i + 1) % n);

                triangles.Add(i);
                triangles.Add(i + n);
                triangles.Add((i + n + 1) % (2*n));
            }

            triangles.Add(0);
            triangles.Add(2 * n - 1);
            triangles.Add(n);

            Vector3[] newVertices = new Vector3[triangles.Count];
            for (int i = 0; i < triangles.Count; i++)
            {
                newVertices[i] = allVerts[triangles[i]];
                triangles[i] = i;
            }
            
            Mesh m = new Mesh();
            m.vertices = newVertices;
            m.triangles = triangles.ToArray();
            m.RecalculateNormals();
            m.RecalculateBounds();
            return m;
            ////// convert polygon to triangles
            //var poly = verts.Select(x => x.ToVector2xz()).Reverse().ToArray();
            //Triangulator triangulator = new Triangulator(poly);
            //int[] tris = triangulator.Triangulate();
            //Mesh m = new Mesh();
            ////m.vertices = verts.ToArray();
            ////m.triangles = tris;
            ////m.RecalculateNormals();
            ////m.RecalculateBounds();
            ////return m;
            //Vector3[] vertices = new Vector3[poly.Length * 2];

            //for (int i = 0; i < poly.Length; i++)
            //{
            //    vertices[i].x = poly[i].x;
            //    vertices[i].y = 10;
            //    vertices[i].z = poly[i].y; // front vertex
            //    vertices[i + poly.Length].x = poly[i].x;
            //    vertices[i + poly.Length].y = 0;
            //    vertices[i + poly.Length].z = poly[i].y;  // back vertex    
            //}
            //int[] triangles = new int[tris.Length * 2 + poly.Length * 6];
            //int count_tris = 0;
            //for (int i = 0; i < tris.Length; i ++)
            //{
            //    triangles[i] = tris[i];
            //} // front vertices
            //count_tris += tris.Length;
            ////for (int i = 0; i < tris.Length; i+=3)
            ////{
            ////    triangles[count_tris + i] = tris[i] + poly.Length;
            ////    triangles[count_tris + i + 1] = tris[i + 1] + poly.Length;
            ////    triangles[count_tris + i + 2] = tris[i + 2] + poly.Length;
            ////}
            //for (int i = 0; i < tris.Length; i += 3)
            //{
            //    triangles[count_tris + i] = tris[i + 2] + poly.Length;
            //    triangles[count_tris + i + 1] = tris[i + 1] + poly.Length;
            //    triangles[count_tris + i + 2] = tris[i] + poly.Length;
            //} // back vertices
            //count_tris += tris.Length;
            //for (int i = 0; i < poly.Length; i++)
            //{
            //    // triangles around the perimeter of the object
            //    int n = (i + 1) % poly.Length;
            //    triangles[count_tris] = i;
            //    triangles[count_tris + 1] = n;
            //    triangles[count_tris + 2] = i + poly.Length;
            //    triangles[count_tris + 3] = n;
            //    triangles[count_tris + 4] = n + poly.Length;
            //    triangles[count_tris + 5] = i + poly.Length;
            //    count_tris += 6;
            //}

            //m.vertices = vertices;
            //m.triangles = triangles;
            //m.RecalculateNormals();
            //m.RecalculateBounds();
            //return m;
            //List<Vector3> baseVertices = new List<Vector3>();
            //foreach(var vert in verts)
            //{
            //    baseVertices.Add(vert);
            //}

            //List<Vector3> verticesList = new List<Vector3>(baseVertices);
            //List<Vector3> verticesExtrudedList = new List<Vector3>();
            //List<int> indices = new List<int>();

            //for (int i = 0; i < verticesList.Count; i++)
            //{
            //    verticesExtrudedList.Add(new Vector3(verticesList[i].x, 10, verticesList[i].z));
            //}

            //verticesList.AddRange(verticesExtrudedList);

            //int n = baseVertices.Count;
            //for (int i = 0; i < baseVertices.Count; i++)
            //{
            //    int i1 = i;
            //    int i2 = (i1 + 1) % n;
            //    int i3 = i1 + n;
            //    int i4 = i2 + n;

            //    indices.Add(i1);
            //    indices.Add(i3);
            //    indices.Add(i4);

            //    indices.Add(i1);
            //    indices.Add(i4);
            //    indices.Add(i2);
            //}

            //Mesh mesh = new Mesh();
            //mesh.Clear();
            //mesh.vertices = verticesList.ToArray();
            //mesh.triangles = indices.ToArray();

            //mesh.RecalculateNormals();
            //mesh.RecalculateBounds();

            //return mesh;
            //var tris = new Triangulator(verts.Select(x => x.ToVector2xz()).ToArray());
            //var mesh = new Mesh();

            //var vertices = verts.Select(x => new Vector3(x.x, 20f, x.z)).ToList();
            //var indices = tris.Triangulate().ToList();
            ////var uv = new List<Vector2>();

            //var numberOfPointsPerRing = vertices.Count;
            //for (int index = 0; index < numberOfPointsPerRing; index++)
            //{
            //    var v = vertices[index];
            //    vertices.Add(new Vector3(v.x, 0, v.z));
            //}

            //for (int i = 0; i < numberOfPointsPerRing - 1; i++)
            //{
            //    indices.Add(i);
            //    indices.Add(i + numberOfPointsPerRing);
            //    indices.Add(i + numberOfPointsPerRing + 1);
            //    indices.Add(i);
            //    indices.Add(i + numberOfPointsPerRing + 1);
            //    indices.Add(i + 1);
            //}

            //indices.Add(numberOfPointsPerRing - 1);
            //indices.Add(numberOfPointsPerRing);
            //indices.Add(0);

            //indices.Add(numberOfPointsPerRing - 1);
            //indices.Add(numberOfPointsPerRing + numberOfPointsPerRing - 1);
            //indices.Add(numberOfPointsPerRing);


            ////Lower pointposition hinzufügen
            //foreach (Vector3 point in verts)
            //{
            //    vertices.Add(new Vector3(point.x, 0, point.z));
            //}


            //int pointAbove = numberOfPointsPerRing;
            //for (int i = 0; i < numberOfPointsPerRing; i++)
            //{
            //    //right upper triangle
            //    indices.Add(i);             //right down
            //    indices.Add(i + 1 + pointAbove);//left up
            //    indices.Add(i + pointAbove); //right up

            //    //left lower triangle
            //    indices.Add(i + 1);             //right down
            //    indices.Add(i + 1 + pointAbove);//left up
            //    indices.Add(i); //right up

            //}



            //mesh.vertices = vertices.ToArray();
            //mesh.triangles = indices.ToArray();

            //mesh.RecalculateNormals();
            //mesh.RecalculateBounds();

            //return mesh;
        }

	
    }
}
