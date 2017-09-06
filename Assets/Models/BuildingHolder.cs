using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Models
{
    public class BuildingHolder
    {
        public GameObject GameObject { get; set; }
        public Vector3 Center { get; set; }
        public float Rotation { get; set; }
        public bool IsModelCreated;
        private List<Vector3> _verts;

        public BuildingHolder(Vector3 center, List<Vector3> verts)
        {
            IsModelCreated = false;
            Center = center;
            _verts = verts;
        }

        public GameObject CreateModel()
        {
            if (IsModelCreated)
                return null;
            
            var m = new GameObject().AddComponent<BuildingPolygon>();
            m.transform.position = Center;
            m.gameObject.GetComponent<Renderer>().material = Resources.Load("buildingMaterial") as Material;
            m.Initialize(_verts);
            IsModelCreated = true;
            return m.gameObject;
        }

        float SurfaceArea(List<Vector3> verts)
        {
            var list = verts.Select(x => new Vector3(x.x, 0, x.z)).ToList();
            float temp = 0;
            int i = 0;
            for (; i < list.Count; i++)
            {
                if (i != list.Count - 1)
                {
                    float mulA = list[i].x * list[i + 1].z;
                    float mulB = list[i + 1].x * list[i].z;
                    temp = temp + (mulA - mulB);
                }
                else
                {
                    float mulA = list[i].x * list[0].z;
                    float mulB = list[0].x * list[i].z;
                    temp = temp + (mulA - mulB);
                }
            }
            temp *= 0.5f;
            return Mathf.Abs(temp);
        }

    }
}
