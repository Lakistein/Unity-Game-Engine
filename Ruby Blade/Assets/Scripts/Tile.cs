using UnityEngine;
public class Tile {
	public const byte ground = 0, wall = 1;

	public Texture2D tile;
	public byte type;

	public Tile(Texture2D tile, byte type)
	{
		this.tile = tile;
		this.type = type;
	}
}
