using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct MapPoint
{
    public short X, Y;
    public MapPoint(short X, short Y)
    {
        this.X = X;
        this.Y = Y;
    }
}

public class Val
{
    public int val;
    public Val(int val) { this.val = val; }
}

public class MapGenerator : MonoBehaviour
{
    public GameObject straight;
    public GameObject curved;
    public GameObject railStraight;
    public GameObject railCurved;
    public GameObject straightHole;
    public GameObject curvedHole;
    public GameObject blockJump;
    public GameObject box;
    public GameObject blockBoxPush;

    public GameObject bSimple;
    public GameObject finishPoint;

    public static int level;

    void Start()
    {
        Debug.Log("Creating level " + level);
        GeneratePath(map, (short)(20 + level * 3));
        GenerateType(map, 100 * level/*200000*/);
        Debug.Log("size AAA " + map.Count);
        MapPoint last = new MapPoint(0, 0);
        foreach(KeyValuePair<MapPoint, Val> item in map)
        {    //x,y
            if(item.Value.val == int.MaxValue) continue;
            GameObject g = null;
            last = item.Key;

            if((item.Value.val & TYPE_BLOCK) != 0)
            {
                g = blockBoxPush;
                g.transform.position = new Vector3(item.Key.X, 0, item.Key.Y);
                Debug.Log("BLOCK ROT " + (item.Value.val & DIR_MASK));

                GameObject b = box;
                b.transform.position = new Vector3(item.Key.X, 0.5f, item.Key.Y);
                Instantiate(b);

                switch((item.Value.val & DIR_MASK))
                {
                    case 0:
                        g.transform.rotation = Quaternion.Euler(-90, -90, 0);
                        break;
                    case 1:
                        g.transform.rotation = Quaternion.Euler(-90, 180, 0);
                        break;
                    case 2:
                        g.transform.rotation = Quaternion.Euler(-90, 90, 0);
                        break;
                    case 3:
                        g.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        break;
                }
            }
            else
            {
                GameObject _straight = ((item.Value.val & S_CRACKED) == 0) ? straight : straightHole;
                GameObject _curved = ((item.Value.val & S_CRACKED) == 0) ? curved : curvedHole;
                bool curvedB = !((item.Value.val & DIR_MASK) == _4 || (item.Value.val & DIR_MASK) == _2);

                g = (curvedB) ? _curved : _straight;
                float x = item.Key.X;
                float z = item.Key.Y;

                g.transform.position = new Vector3(x, 0, z);

                switch((item.Value.val & DIR_MASK))
                {
                    case _4:
                        g.transform.rotation = Quaternion.Euler(-90, 0, 0);
                        break;
                    case _2:

                        g.transform.rotation = Quaternion.Euler(-90, 90, 0);

                        break;
                    case _1:

                        g.transform.rotation = Quaternion.Euler(-90, 180, 0);

                        break;
                    case _3:

                        g.transform.rotation = Quaternion.Euler(-90, -90, 0);

                        break;
                    case _7:

                        g.transform.rotation = Quaternion.Euler(-90, 90, 0);

                        break;
                    case _9:

                        g.transform.rotation = Quaternion.Euler(-90, 0, 0);

                        break;
                }
                if((item.Value.val & S_NORAIL) == 0)
                {
                    GameObject rail = curvedB ? railCurved : railStraight;
                    rail.transform.position = g.transform.position;
                    rail.transform.rotation = g.transform.rotation;
                    Instantiate(rail);
                }
            }
            Instantiate(g);
        }
        finishPoint.transform.position = new Vector3(last.X, 0, last.Y);
        Instantiate(finishPoint);
    }

    public void shear(GameObject o)
    {
        MeshFilter mf = o.GetComponent<MeshFilter>();
        //		mf.sharedMesh = (Mesh) Instantiate(mf.sharedMesh );
        //myRenderer.sharedMesh = (Mesh) Instantiate( myRenderer.sharedMesh );
        Mesh mesh = mf.mesh;
        //Mesh mesh = mf.sharedMesh;

        Vector3[] vertices = mesh.vertices;

        int i = 0;
        while(i < vertices.Length)
        {
            //			float distance = Vector3.Distance(vertices[i], hitPoint);
            //			Vector3 dir = (vertices[i] - hitPoint);
            //			if(dir.magnitude < hitRadius){
            //				float amount = 1 - dir.magnitude / hitRadius;
            //				Vector3 vertMove = hitDir * amount;
            //				vertices[i] += vertMove;
            //			}
            vertices[i].x *= 5;
            //vertices[i].z += vertices[i].x;
            i++;
        }
        mesh.vertices = vertices;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        //		MeshCollider col = o.GetComponent<MeshCollider> ();

        //		Destroy (o.GetComponent<MeshCollider> ());
        //		o.AddComponent ("MeshCollider");
    }

    public const byte _1 = 3, _3 = 6, _7 = 9, _9 = 12, _2 = 10, _4 = 5;
    public const byte DIR_MASK = 15;
    public static readonly byte[] dirs = { _1, _3, _7, _9, _2, _4 };
    public static readonly sbyte[,] move = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
    public static readonly byte[] dirIndexToBin = { 8, 4, 2, 1 };
    public static readonly byte[] dirBinToIndex = { 0, 3, 2, 0, 1, 0, 0, 0, 0 };
    public Dictionary<MapPoint, Val> map = new Dictionary<MapPoint, Val>();
    public static BlockType B_SIMPLE, B_JUMP, B_MOVINGBOX;
    public static Val EMPTY = new Val(int.MaxValue);

    static MapGenerator()
    {
        B_SIMPLE = new BlockType();
        B_SIMPLE.w = 3;
        B_SIMPLE.h = 3;
        B_SIMPLE.xEntry = 1;
        B_SIMPLE.xExit = 0;
        B_SIMPLE.yExit = 2;

        B_JUMP = new BlockType();
        B_JUMP.w = 1;
        B_JUMP.h = 7;
        B_JUMP.xEntry = 0;
        B_JUMP.xExit = 0;
        B_JUMP.yExit = 6;

        B_MOVINGBOX = new BlockType();
        B_MOVINGBOX.w = 3;
        B_MOVINGBOX.h = 4;
        B_MOVINGBOX.xEntry = 1;
        B_MOVINGBOX.xExit = 0;
        B_MOVINGBOX.yExit = 3;
    }
    //public static void PrintMap()
    //{


    //    int size = 50;
    //    int[,] arr = new int[size, size];
    //    foreach (KeyValuePair<MapPoint, Val> item in map)
    //    {
    //        arr[item.Key.X + 5, item.Key.Y + 5] = item.Value.val;
    //    }

    //    for (int i = 0; i < size; i++)
    //    {
    //        for (int j = 0; j < size; j++)
    //        {
    //            if (arr[j, i] == 0)
    //                Console.Write(" ");
    //            else
    //            {
    //                char ch = ' ';
    //                switch (arr[j, i])
    //                {
    //                    case 5/*_4*/: ch = '-';
    //                        break;
    //                    case 10/*_2*/: ch = '|';
    //                        break;
    //                    case 3/*_1*/: ch = '¬';
    //                        break;
    //                    case 6/*_3*/: ch = 'Г';
    //                        break;
    //                    case 9/*_7*/: ch = '˩';
    //                        break;
    //                    case 12/*_9*/: ch = 'L';
    //                        break;

    //                }
    //                Console.Write(ch);
    //            }
    //        }
    //        Console.WriteLine();
    //    }
    //}

    public static byte GetShapeDirection(byte shape, byte dirToRemoveBinary)
    {
        return dirBinToIndex[shape & (~dirToRemoveBinary)];
    }

    public static byte GetPossibleShapes(Dictionary<MapPoint, Val> map, byte dirIndex, short lengthLeft, short x, short y, byte[] store)
    {
        byte count = 0, tmp;

        bool add;
        for(int i = 0; i < dirs.Length; i++)
        {
            if((dirs[i] & dirIndexToBin[dirIndex]) != 0)
            {

                add = true;
                tmp = GetShapeDirection(dirs[i], dirIndexToBin[dirIndex]);
                Debug.Log("shape " + i);
                for(int j = 0; j < lengthLeft; j++)
                {

                    if(map.ContainsKey(new MapPoint((short)(x + move[tmp, 0] * j), (short)(y + move[tmp, 1] * j))))
                    {
                        add = false;
                        Debug.Log("CONTAINS + x " + (x + move[tmp, 0] * j) + ", y " + (y + move[tmp, 1] * j));
                        break;
                    }
                }
                if(add)
                    store[count++] = dirs[i];
            }
        }
        return count;
    }

    public static void GeneratePath(Dictionary<MapPoint, Val> map, short length)
    {
        int x = 0, y = 0;
        //store - stores possible shapes of next tile
        byte[] store = new byte[4];
        byte dirIndex = 3, size, tmp;
        map.Add(new MapPoint(0, 0), new Val(_4));
        // Console.WriteLine("Adding " + x + "," + y + "..." + dirIndex);
        bool spawnBlock = false;
        BlockType toSpawn = B_MOVINGBOX;

        for(int i = 1; i < length; i++)
        {
            x -= move[dirIndex, 0];
            y -= move[dirIndex, 1];
            if(Random.Range(0, 10) == 0) spawnBlock = true;
            if(spawnBlock && i + 1 < length && IfBPlaceableSpawn(map, x, y, dirIndex, toSpawn))
            {
                spawnBlock = false;
                Debug.Log("BLOCK SPAWNED ");
                switch((dirIndex + 2) % 4)
                {
                    case 0:
                        x = x + toSpawn.xExit;
                        y = y - toSpawn.yExit;
                        break;
                    case 1:
                        x = x + toSpawn.yExit;
                        y = y + toSpawn.xExit;
                        break;
                    case 2:
                        x = x + toSpawn.xExit;
                        y = y + toSpawn.yExit;
                        break;
                    case 3:
                        x = x - toSpawn.yExit;
                        y = y + toSpawn.xExit;
                        break;
                }
                continue;
            }
            size = GetPossibleShapes(map, dirIndex, (short)(length - i), (short)x, (short)y, store);
            if(size == 0) { Debug.Log("ERROR no possible shape " + x + ", " + y + ", dirI " + dirIndex); return; }
            tmp = store[Random.Range(0, size)];
            Debug.Log("Adding " + x + "," + y + "...sh " + tmp + ", " + dirIndex);
            map.Add(new MapPoint((short)x, (short)y), new Val(tmp));

            dirIndex = (byte)((GetShapeDirection(tmp, dirIndexToBin[dirIndex]) + 2) % 4);
            // Console.WriteLine("new drep " + dirIndex);
        }
    }

    public static bool IfBPlaceableSpawn(Dictionary<MapPoint, Val> map, int x, int y, byte dirIndex, BlockType block)
    {
        dirIndex = (byte)((dirIndex + 2) % 4);
        int xI = 0, yI = 0;
        byte w = block.w, h = block.h;
        switch(dirIndex)
        {
            case 0:
                xI = x - block.xEntry;
                yI = y - block.h + 1;
                break;

            case 1:
                xI = x;
                yI = y - block.xEntry;
                w = h;
                h = block.w;
                break;

            case 2:
                xI = x - block.w + block.xEntry + 1;
                yI = y;
                break;

            case 3:
                xI = x - block.w + 1;
                yI = y - block.w + block.xEntry + 1;
                w = h;
                h = block.w;
                break;
        }

        for(int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
            {
                if(map.ContainsKey(new MapPoint((short)(xI + i), (short)(yI + j)))) return false;
            }
        }

        Debug.Log("d " + dirIndex);
        for(int i = 0; i < w; i++)
        {
            for(int j = 0; j < h; j++)
                //if(x+move[dirIndex,0] == xI+i && y+move[dirIndex,1] == yI+j)
                if(x == xI + i && y == yI + j)
                {
                    map.Add(new MapPoint((short)(xI + i), (short)(yI + j)), new Val(dirIndex | TYPE_BLOCK));
                    Debug.Log("Adding BLOCK " + (xI + i) + "," + (yI + j));
                }
                else
                {
                    map.Add(new MapPoint((short)(xI + i), (short)(yI + j)), EMPTY);
                    Debug.Log("Adding " + (xI + i) + "," + (yI + j));
                }
        }
        return true;
    }

    public static byte TYPE_SIMPLE = 0;
    public static byte TYPE_BLOCK = 16;

    public static byte S_NORAIL = 32;
    public static byte S_THIN = 64;
    public static short S_CRACKED = 128;
    public static short S_TILED = 256;
    public static short S_SLOPE = 512;

    public static float NORAIL_THIN = 10;
    public static float RAIL_THIN = 7;
    public static float NORAIL_THICK = 5;
    public static float CRACKS = 3;
    public static float TILT = 13;
    public static float SLOPE = 7;

    public static byte TILT_MIN = 3;
    public static byte TILT_MAX = 9;
    public static byte SLOPE_MIN = 3;
    public static byte SLOPE_MAX = 9;

    public static void GenerateType(Dictionary<MapPoint, Val> map, int lvlDiff)
    {
        //int length = 10+r.Next(20);
        int length = map.Count;
        float diffRatio = lvlDiff / (float)length;

        bool bCracks = diffRatio > CRACKS;
        bool bTilt = diffRatio > TILT;
        bool bSlope = diffRatio > SLOPE;
        int repTilt = 0, repSlope = 0;

        int type;
        if(diffRatio > NORAIL_THIN)
        {
            type = TYPE_SIMPLE | S_NORAIL | S_THIN;
        }
        else if(diffRatio > RAIL_THIN)
        {
            type = TYPE_SIMPLE | S_THIN;
        }
        else if(diffRatio > NORAIL_THICK)
        {
            type = TYPE_SIMPLE | S_NORAIL;
        }
        else
        {
            type = TYPE_SIMPLE;
        }
        foreach(Val item in map.Values)
        {
            if(item.val == int.MaxValue) continue;
            if((item.val & TYPE_BLOCK) != 0) continue;
            item.val |= type;
            if(bCracks && Random.Range(0, 10) == 0)
                item.val |= S_CRACKED;
            if(repTilt > 0)
            {
                item.val |= S_TILED;
                repTilt--;
            }
            else if(bTilt && Random.Range(0, 10) == 0) repTilt = TILT_MIN + Random.Range(0, TILT_MAX - TILT_MIN);
            if(repSlope > 0)
            {
                item.val |= S_SLOPE;
                repSlope--;
            }
            else if(bSlope && Random.Range(0, 10) == 0) repSlope = SLOPE_MIN + Random.Range(0, SLOPE_MAX - SLOPE_MIN);
        }
    }
}