using UnityEngine;
using System.Collections;

public class TileManager {
	public static readonly sbyte[,] move = {{-1,-1}, {0,-1},{1,-1},{1,0},{1,1},{0,1},{-1,1},{-1,0}};
	private static readonly sbyte[] visitOrder = {0, 1, -1, 2, -2, 3, -3, 4}; 

	public short w, h;
	public Tile[,] tiles;
	//change to bitset
	bool[,] visited;

	public Tile ground, wall;

	public TileManager(short w, short h)
	{
		this.w = w;
		this.h = h;
		tiles = new Tile[w, h];
		visited = new bool[w, h];
		ground = new Tile (Resources.Load<Texture2D>("Ground"), Tile.ground);
		wall = new Tile (Resources.Load<Texture2D> ("Wall"), Tile.wall);

		InitialiseMap ();
	}

	public static byte getDir(int dx, int dy) {
		for(byte i = 0; i < 8; i++) 
			if(dx == move[i,0] && dy == move[i,1]) 
				return i;
		throw new UnityException ("Invalid dir dx, dy");
	}

	public Path findPath(float sx, float sy, float tx, float ty) {
		return findPath ((int)(sx * 0.03125f), (int)(sy * 0.03125f), (int)(tx * 0.03125f), (int)(ty * 0.03125f));
	}
	public Path findPath(Vector3 s, Vector3 t) {
		int sx = (int)(s.x * 0.03125f);
		int sy = (int)(s.y * 0.03125f);
		int tx = (int)(t.x * 0.03125f);
		int ty = (int)(t.y * 0.03125f);
		return findPath (sx, sy, tx, ty);
	}
	public Path findPath(int sx, int sy, int tx, int ty) {
		Path path = new Path (sx, sy);
		Path last = path;
		int dx, dy, dir, nDir, xx, yy;
		bool added = true;


		for (int i = 0; i < w; i++)
			for (int j = 0; j < h; j++)
				visited[i,j] = false;

		visited[last.x,last.y] = true;
		//Debug.Log ("----Find path " );

		while (added && (last.x != tx || last.y != ty)) {
			dx = last.x > tx ? -1 : (last.x < tx) ? 1 : 0;
			dy = last.y > ty ? -1 : (last.y < ty) ? 1 : 0;
			if(dx != 0 && dy != 0) {
				int x = last.x-tx;
				if(x < 0) x = -x;
				int y = last.y-ty;
				if(y < 0) y = -y;
				if(x > y) dy = 0;
				else if(y > x) dx = 0;
			}
			dir = getDir(dx, dy);
			added = false;
			for(int i = 0; i < visitOrder.Length; i++) {
				nDir = (dir+visitOrder[i]+8)%8;
				xx = last.x+move[nDir,0];
				yy = last.y+move[nDir,1];

				if(isAccesibleInt(xx, yy)
				   && !visited[xx, yy] && (((nDir & 1) != 0) ||
				   (isAccesibleInt(last.x+move[(nDir+1)%8,0], last.y+move[(nDir+1)%8,1])
				 && isAccesibleInt(last.x+move[(nDir+7)%8,0], last.y+move[(nDir+7)%8,1]))
				                        )) {

					last.next = new Path(xx, yy);
					last = last.next;
					visited[last.x, last.y] = true;
					added = true;
					break;
				}
			}
		}
		return path;
	}
	public bool isAccesible(Vector3 pos) {
		return isAccesible (pos.x, pos.y);
	}
	public bool isAccesibleInt(int x, int y) {
		if (x < 0 || y < 0 || x >= w || y >= h)
			return false;
		return tiles [x, y].type == Tile.ground;
	}
	public bool isAccesible(float x, float y) {
		Tile t = getTileAt (x,y);
		if (t == null) return false;
		return t.type == Tile.ground;
	}
	public Tile getTileAt(float xx, float yy) {
		xx *= 0.03125f;
		yy *= 0.03125f;
		int x = (int)(xx);
		int y = (int)(yy);

		if (xx < 0 || yy < 0 || x >= w || y >= h)
				return null;

		return tiles [x, y];
	}
	public bool reachedTile(float xPos, float yPos, int x, int y) {
		xPos *= 0.03125f;
		yPos *= 0.03125f;
		return (((int)(xPos+0.5f)) == x && ((int)(yPos+0.5f)) == y);
	}
	void InitialiseMap()
	{
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				tiles[i,j] = ground;
			}
		}
		InsertWalls ();
	}

	void InsertWalls()
	{
		byte numOfW = 50;
		byte wLen;
		bool isHorizontal;

		byte bW, bH;

		for (int i = 0; i < numOfW; i++) {
			wLen = (byte)Random.Range (3, 7);
			isHorizontal = (Random.Range (0, 2) > 0) ? true : false;
			if(isHorizontal) {
				bW = wLen; 
				bH = 1;
			}
			else {
				bW = 1;
				bH = wLen;
			}

			short rH = (short)Random.Range(0, this.h-bH+1);
			short rW = (short)Random.Range(0, this.w-bW+1);

			if(IsPlaceable(rW, rH, bW, bH))
			{
				PlaceBlock(rW, rH, bW, bH, wall);  
			}
		}
	}

	bool IsPlaceable(short x, short y, byte w, byte h)
	{
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				if(tiles[x + i, j+ y] != ground || x + i == MapGenerator.Instance.W / 2 || y + j == MapGenerator.Instance.H / 2) return false;
			}
		}
		return true;
	}

	void PlaceBlock(short x, short y, byte w, byte h, Tile block)
	{
		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
					tiles [x + i, j + y] = block;
			}
		}
	}
}
