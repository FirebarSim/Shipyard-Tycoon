using System.Collections;
using UnityEngine;

public class World
{
    Tile[,] tiles;
    int width;
    public int Width
    {
        get
        {
            return width;
        }
    }
    int height;
    public int Height
    {
        get
        {
            return height;
        }
    }

    public World (int width = 100, int height = 100)
    {
        this.width = width;
        this.height = height;

        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x,y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (width * height) + " tiles.");

    }

    public void RandomiseTiles ()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (Random.Range(0,2) == 0)
                {
                    tiles[x, y].Type = Tile.TileType.DeepWater;
                }
                else
                {
                    tiles[x, y].Type = Tile.TileType.Land;
                }
            }
        }
    }

    public void ReadMap ()
    {
        int line_index = 0;
        int column_index;
        string[] lines = System.IO.File.ReadAllLines(@"D:\GitHub\Shipyard Tycoon\Shipyard Tycoon\Assets\Maps\Map.txt");
        foreach (string line in lines)
        {
            column_index = 0;
            string[] values = line.Split(' ');
            foreach (string value in values)
            {
                if (value.ToString() == "0")
                {
                    tiles[column_index, line_index].Type = Tile.TileType.DeepWater;
                }
                else if (value.ToString() == "1")
                {
                    tiles[column_index, line_index].Type = Tile.TileType.ShallowWater;
                }
                else if (value.ToString() == "2")
                {
                    tiles[column_index, line_index].Type = Tile.TileType.Drying;
                }
                else
                {
                    tiles[column_index, line_index].Type = Tile.TileType.Land;
                }
                column_index++;
            }
            line_index++;
        }
    }

    public Tile GetTileAt (int x, int y)
    {
        if (x>width ||x < 0)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }
}
