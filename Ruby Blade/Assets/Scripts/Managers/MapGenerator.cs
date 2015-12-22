using UnityEngine;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance;
    Material mat;
    public TileManager tm;
    Mesh tileMesh;
    public int W, H;
    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start()
    {
        mat = GameObject.Find("AK-47").GetComponent<SpriteRenderer>().material;

        tm = new TileManager((short)W, (short)H);
        tileMesh = CreateMesh();
        DrawMap();
    }

    public Path debugPath;
    void OnDrawGizmos()
    {
        if(debugPath == null)
            return;
        Path last = debugPath;
        Path p = debugPath.next;
        while(p != null)
        {
            Gizmos.DrawLine(new Vector2(last.x * 32, last.y * 32), new Vector2(p.x * 32, p.y * 32));
            last = p;
            p = p.next;
        }
    }

    void DrawMap()
    {
        for(int i = 0; i < tm.w; i++)
        {
            for(int j = 0; j < tm.h; j++)
            {
                GameObject t = DrawTexture(tm.tiles[i, j].tile, tileMesh);
                t.transform.position = new Vector2(i * 32, j * 32);
                t.transform.localScale = new Vector2(16, 16);
            }
        }
    }
    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[]
        {
            new Vector3( 1, 1,  0),
            new Vector3( 1, -1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(-1, -1, 0),
        };

        Vector2[] uv = new Vector2[]
        {
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(0, 0),
        };

        int[] triangles = new int[]
        {
            0, 1, 2,
            2, 1, 3,
        };

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    // Create a mesh and bind the 'msn' texture to it
    public GameObject DrawTexture(Texture tex, Mesh _m1)
    {

        // Create object
        //Mesh _m1 = CreateMesh();
        GameObject item = (GameObject)new GameObject(
            "Tile",
            typeof(MeshRenderer), // Required to render
            typeof(MeshFilter)    // Required to have a mesh
            );
        item.GetComponent<MeshFilter>().mesh = _m1;
        item.renderer.castShadows = false;
        item.renderer.receiveShadows = false;
        item.renderer.material = mat;
        // Set texture
        //var tex = (Texture) Resources.Load ("Ground");
        item.renderer.material.mainTexture = tex;

        // Set shader for this sprite; unlit supporting transparency
        // If we dont do this the sprite seems 'dark' when drawn. 
        //var shader = Shader.Find("Unlit/Transparent");
        item.renderer.material.shader = mat.shader;

        // Set position

        return item;
    }
}