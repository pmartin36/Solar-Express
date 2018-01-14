#if(UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MeshGen : MonoBehaviour {

	public int Rows, Columns;
	private int _rows, _columns;
	public bool Save;

	MeshRenderer mr;
	MeshFilter mf;

	float startTime;

	public float DistFromCenter;
	public float Cutoff;
	public bool Spawned;

	List<Vector3> vertices;

	// Use this for initialization
	void Start () {
		vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		mr = GetComponent<MeshRenderer>();

		_rows = Rows + 1;
		_columns = Columns + 1;

		//vertices.AddRange(new []{
		//	new Vector3(0, 1, 0), new Vector3(1, 1, 0),
		//	new Vector3(0, 0, 0), new Vector3(0, 0, 0)
		//});

		
		for (int y = 0; y < _rows; y++) {
			for (int x = 0; x < _columns; x++) {
				int i = (y * (_columns) + x)*2;
				Vector2 add = new Vector2(x / (float)Columns, y / (float)Rows);
				vertices.Add(add);

				add = add + new Vector2(0, 1f);
				vertices.Add(add);

				if (y > 0) {
					//backward triangle
					if (x > 0) {
						int _i = i;
						if (x % 2 != 0) {
							_i++;
						}
						triangles.Add(_i - (_columns + 1)*2);
						triangles.Add(_i);
						triangles.Add(_i - (_columns)*2);
					}

					//forward triangle
					if (x < Columns) {
						int _i = i;
						if (x % 2 == 0) {
							_i++;
						}
						triangles.Add(_i - (_columns)*2);
						triangles.Add(_i);
						triangles.Add(_i + 2);
					}
				}
			}
		}
		

		Mesh m = new Mesh();
		m.vertices = vertices.ToArray();
		m.triangles = triangles.ToArray();
		m.uv = vertices.Select((v,i) => new Vector2(v.x, i%2==0?v.y:v.y-1f)).ToArray();
		m.RecalculateNormals();
		mf = GetComponent<MeshFilter>();
		mf.sharedMesh = m;

		if(Save) {
			AssetDatabase.CreateAsset(m, string.Format("Assets/Resources/m{0}x{1}",Rows,Columns));
		}

		startTime = Time.time;
	}

	private Vector2[] getUVs(List<Vector3> v3) {
		Vector2[] v2 = new Vector2[v3.Count];
		for (int i = 0; i < v3.Count; i++) {
			v2[i] = Vector2.zero;
			//if on the top row
			if (i % 2 == 0) {
				v2[i].y = 1;
			}
			//every other column
			if (((i - 2) % 4 == 0) || ((i - 3) % 4 == 0)) {
				v2[i].x = 1;
			}
		}
		return v2;
	}

	// Update is called once per frame
	void Update () {
		if(!Spawned) {
			Mesh m = mf.sharedMesh;
			m.vertices = vertices.Select((v, i) =>
			   new Vector3(v.x, i%2!=0 ? v.y-0.5f+DistFromCenter : v.y+0.5f-DistFromCenter)).ToArray();
			m.triangles = m.triangles;
			m.RecalculateNormals();
			mf.sharedMesh = m;

			mr.material.SetFloat("_ColorCutoff", Cutoff);
			mr.material.SetFloat("_AlphaModifier", Cutoff+0.4f);
		}
	}
}
#endif