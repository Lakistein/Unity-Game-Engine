using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //PathGenerator.map.Add(new MapPoint(0,0),5);
            //PathGenerator.map.Add(new MapPoint(1, 0), 5);
            //PathGenerator.map.Add(new MapPoint(-1, 0), 5);

            //Console.WriteLine("test " + PathGenerator.GetShapeDirection(PathGenerator._1, PathGenerator.dirRep[2]));

            try
            {
                PathGenerator.GeneratePath(50);
            }
            catch (Exception e) { Console.WriteLine(e); }
            PathGenerator.PrintMap();





            Console.ReadLine();

        }
    }
}

public struct MapPoint
{
    public short X, Y;
    public MapPoint(short X, short Y)
    {
        this.X = X;
        this.Y = Y;
    }
}
public class Val {
    public int val;
    public Val(int val) { this.val = val; }
}
public class PathGenerator
{
    public static byte _1 = 3, _3 = 6, _7 = 9, _9 = 12, _2 = 10, _4 = 5;
    public static byte[] dirs = { _1, _3, _7, _9, _2, _4 };
    public static sbyte[,] move = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
    public static byte[] dirIndexToBin = { 8, 4, 2, 1 };
    public static byte[] dirBinToIndex = { 0, 3, 2, 0, 1, 0, 0, 0, 0 };
    public static Dictionary<MapPoint, Val> map = new Dictionary<MapPoint, Val>();
    static Random r = new Random();

    public static void PrintMap()
    {
        

        int size = 50;
        int[,] arr = new int[size, size];
        foreach (KeyValuePair<MapPoint, Val> item in map)
        {
            arr[item.Key.X + 5, item.Key.Y + 5] = item.Value.val;
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (arr[j, i] == 0)
                    Console.Write(" ");
                else
                {
                    char ch = ' ';
                    switch (arr[j, i])
                    {
                        case 5/*_4*/: ch = '-';
                            break;
                        case 10/*_2*/: ch = '|';
                            break;
                        case 3/*_1*/: ch = '¬';
                            break;
                        case 6/*_3*/: ch = 'Г';
                            break;
                        case 9/*_7*/: ch = '˩';
                            break;
                        case 12/*_9*/: ch = 'L';
                            break;

                    }
                    Console.Write(ch);
                }
            }
            Console.WriteLine();
        }
    }

    public static byte GetShapeDirection(byte shape, byte dirToRemoveBinary)
    {
        return dirBinToIndex[shape & (~dirToRemoveBinary)];
    }



    public static byte GetPossibleShapes(byte dirIndex, short lengthLeft, short x, short y, byte[] store)
    {
        byte count = 0, tmp;
        //if (map.ContainsKey(new MapPoint((short)(x - move[dirIndex, 0]), (short)(y - move[dirIndex, 1]))))
        //{


        //}
        //else {

        for (int i = 0; i < dirs.Length; i++)
        {
            if ((dirs[i] & dirIndexToBin[dirIndex]) != 0)
            {
                tmp = GetShapeDirection(dirs[i], dirIndexToBin[dirIndex]);
                for (int j = 0; j < lengthLeft; j++)
                {
                    if (map.ContainsKey(new MapPoint((short)(x + move[tmp, 0] - move[dirIndex, 0] * j), (short)(y + move[tmp, 1] - move[dirIndex, 1] * j))))
                        goto loop;
                }
                store[count++] = dirs[i];
            }
        loop: ;
        }
        //}
        return count;
    }

    public static void GeneratePath(short length)
    {
        int x = 0, y = 0;
        //store - stores possible shapes of next tile
        byte[] store = new byte[4];
        byte dirIndex = 3, size, tmp;
        map.Add(new MapPoint(0, 0), new Val(_4));
        Console.WriteLine("Adding " + x + "," + y + "..." + dirIndex);

        for (int i = 1; i < length; i++)
        {
            x -= move[dirIndex, 0];
            y -= move[dirIndex, 1];
            size = GetPossibleShapes(dirIndex, (short)(length - i), (short)x, (short)y, store);
            if (size == 0) return;
            tmp = store[r.Next(size)];
            Console.WriteLine("Adding " + x + "," + y + "...sh " + tmp + ", " + dirIndex);
            map.Add(new MapPoint((short)x, (short)y), new Val(tmp));

            dirIndex = (byte)((GetShapeDirection(tmp, dirIndexToBin[dirIndex]) + 2) % 4);
            Console.WriteLine("new drep " + dirIndex);
        }

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

    public static void GenerateType(int lvlDiff) {
        //int length = 10+r.Next(20);
        int length = map.Count;
        float diffRatio = lvlDiff / (float)length;

        bool bCracks = diffRatio > CRACKS;
        bool bTilt = diffRatio > TILT;
        bool bSlope = diffRatio > SLOPE;
        int repTilt = 0, repSlope = 0;

        int type;
        if (diffRatio > NORAIL_THIN)
        {
            type = TYPE_SIMPLE | S_NORAIL | S_THIN;
        }
        else if(diffRatio > RAIL_THIN) {
            type = TYPE_SIMPLE | S_THIN;
        }
        else if (diffRatio > NORAIL_THICK)
        {
            type = TYPE_SIMPLE | S_NORAIL;
        }
        else {
            type = TYPE_SIMPLE;
        }
        foreach (Val item in map.Values)
        {
            item.val |= type;
            if (bCracks && r.Next(10) == 0) 
                item.val |= S_CRACKED;
            if (repTilt > 0) {
                item.val |= S_TILED;
                repTilt--;
            }
            else if(bTilt && r.Next(10)==0) repTilt = TILT_MIN+r.Next(TILT_MAX-TILT_MIN);
            if (repSlope > 0)
            {
                item.val |= S_SLOPE;
                repSlope--;
            }
            else if (bSlope && r.Next(10) == 0) repSlope = SLOPE_MIN + r.Next(SLOPE_MAX - SLOPE_MIN);
        }

    }
}
