using System;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    Tile[,] tiles;

    Dictionary<string, InstalledObject> installedObjectPrototypes;

    public int Width { get; protected set; }
    public int Height { get; protected set; }

    Action<InstalledObject> cbInstalledObjectCreated;
    Action<Tile> cbTileChanged;
    
    // TODO: Most likely to be replaced with a dedicated Queue class.
    public Queue<Job> jobQueue;

    //Initialises a new instance of the World class.
    public World (int width = 100, int height = 100)
    {
        jobQueue = new Queue<Job>();

        Width = width;
        Height = height;

        tiles = new Tile[Width, Height];

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                tiles[x,y] = new Tile(this, x, y);
            }
        }

        Debug.Log("World created with " + (Width * Height) + " tiles.");

        CreateInstalledObjectPrototypes();

    }

    void CreateInstalledObjectPrototypes()
    {
        installedObjectPrototypes = new Dictionary<string, InstalledObject>();

        installedObjectPrototypes.Add("SeaWall", InstalledObject.CreatePrototype("SeaWall",0,1,1,true));
    }

    public void RandomiseTiles ()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (UnityEngine.Random.Range(0,2) == 0)
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
        if (x>Width || x<0 || y>Height || y<0)
        {
            Debug.LogError("Tile (" + x + ", " + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public void PlaceInstalledObject(string objectType, Tile t)
    {
        // TODO: This assumes 1x1 objects with no rotation.

        if(installedObjectPrototypes.ContainsKey(objectType) == false)
        {
            Debug.LogError("InstalledObjectPrototypes doesn't contain a proto for key: " + objectType);
            return;
        }

        InstalledObject obj = InstalledObject.PlaceInstance(installedObjectPrototypes[objectType], t);

        if (obj == null) {
            //Failed to place object, likely something there already.
            return;
        }

        if(cbInstalledObjectCreated != null) {
            cbInstalledObjectCreated(obj);
        }
    }

    public void RegisterInstalledObjectCreated (Action<InstalledObject> callbackfunc) {
        cbInstalledObjectCreated += callbackfunc;
    }

    public void UnregisterInstalledObjectCreated(Action<InstalledObject> callbackfunc) {
        cbInstalledObjectCreated -= callbackfunc;
    }

    public bool IsInstalledObjectPlacementValid (string objType, Tile t) {
        if(installedObjectPrototypes.ContainsKey(objType) == false) {
            Debug.LogError("IsInstalledObjectPlacementValid - Object Type " + objType + " is not in object prototypes.");
        }
        return installedObjectPrototypes[objType].funcPositionValidation(t);
    }
}
