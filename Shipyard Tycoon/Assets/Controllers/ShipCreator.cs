using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCreator : MonoBehaviour {

    public float Length = 100f;
    public float Bow_length = 45f;
    public float Taper = 0.8f;
    public float Taper_length = 15f;
    public float Width = 15f;
    public float Height = 1f;
    float length;
    float bow_length;
    float taper;
    float taper_length;
    float width;
    float height;
    Vector3[] vertices = {};
    int[] triangles = {};
    GameObject go;

    // Use this for initialization
    void Start() {

        length = FeetToUnits(Length);
        bow_length = FeetToUnits(Bow_length);
        taper = Taper;
        taper_length = FeetToUnits(Taper_length);
        width = FeetToUnits(Width);
        height = FeetToUnits(Height);


        vertices = CreateVerticies(length, width, taper_length, taper, bow_length);
        triangles = CreateTriangles(vertices);

        go = new GameObject();
        go.name = "Vessel";
        go.transform.position = new Vector3(0, 0, 0);
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshFilter>();
        Mesh msh = go.GetComponent<MeshFilter>().mesh;
        msh.Clear();
        msh.vertices = vertices;
        msh.triangles = triangles;
        msh.RecalculateNormals();

        
    }

    // Update is called once per frame
    void Update() {

        Length = Mathf.Clamp(Length, 30f,10000f);
        Bow_length = Mathf.Clamp(Bow_length, 0, Length - Taper_length);
        Taper = Mathf.Clamp(Taper, 0,1);
        Taper_length = Mathf.Clamp(Taper_length, 0, Length - Bow_length);
        Width = Mathf.Clamp(Width, 0,10000);
        Height = Mathf.Clamp(Height, 0,10000);

        length = FeetToUnits(Length);
        bow_length = FeetToUnits(Bow_length);
        taper = Taper;
        taper_length = FeetToUnits(Taper_length);
        width = FeetToUnits(Width);
        height = FeetToUnits(Height);


        vertices = CreateVerticies(length, width, taper_length, taper, bow_length);
        Mesh msh = go.GetComponent<MeshFilter>().mesh;
        msh.vertices = vertices;
    }

    Vector3[] CreateVerticies(float length, float width, float taper_length, float taper, float bow_length, int bow_verts = 31) {
        List<Vector3> verts = new List<Vector3>();

        verts.Add(new Vector3(-length / 2, taper * -width / 2));
        verts.Add(new Vector3(-length / 2 + taper_length, -width / 2));
        verts.Add(new Vector3(length / 2 - bow_length, -width / 2));
        for (int i = 0; i < bow_verts; i++) {
            float wd = (i + 1) * (width / (bow_verts + 1)) + (-width / 2);
            float ln = Mathf.Sqrt(
                Mathf.Pow(bow_length, 2f)
                * (1 - (Mathf.Pow(wd, 2f) / Mathf.Pow(width / 2, 2f))));
                //+ (length / 2) - bow_length);
            verts.Add(new Vector3(ln + (length / 2) - bow_length, wd));
        }
        verts.Add(new Vector3(length / 2 - bow_length, width / 2));
        verts.Add(new Vector3(-length / 2 + taper_length, width / 2));
        verts.Add(new Vector3(-length / 2, taper * width / 2));

        return verts.ToArray();
    }

    int[] CreateTriangles (Vector3[] verts) {
        List<int> trisList = new List<int>();
        for (int i = 0; i < verts.Length / 2; i++) {
            trisList.Add(i);
            trisList.Add(i + 1);
            trisList.Add(verts.Length - i - 1);
            trisList.Add(verts.Length - i - 1);
            trisList.Add(i + 1);
            trisList.Add(verts.Length - i - 2);
        }
        trisList.Add(Mathf.FloorToInt(verts.Length / 2) - 1);
        trisList.Add(Mathf.FloorToInt(verts.Length / 2));
        trisList.Add(Mathf.FloorToInt(verts.Length / 2) + 1);

        return trisList.ToArray();
    }

    float FeetToUnits(float feet) {
        return feet / 16f;
    }
}
