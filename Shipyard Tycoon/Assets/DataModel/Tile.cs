using System.Collections;
using UnityEngine;
using System;

public class Tile
{

    public enum TileType { DeepWater, ShallowWater, Drying, Land };

    Action<Tile> cbTileTypeChanged;

    public TileType Type
    {
        get
        {
            return type;
        }
        set
        {
            TileType oldType = type;
            type = value;
            //Callback to indicate a change
            if (cbTileTypeChanged != null && oldType != type)
            {
                cbTileTypeChanged(this);
            }
        }
    }

    TileType type = TileType.Land;

    public LooseObject looseObject { get; protected set; }
    public InstalledObject installedObject { get; protected set; }
    public Job pendingInstalledObject;
    public World world { get; protected set; }
    public int X { get; protected set; }
    public int Y { get; protected set; }

    public Tile (World world, int x, int y)
    {
        this.world = world;
        this.X = x;
        this.Y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

    public bool PlaceObject(InstalledObject objectInstance)
    {
        if (objectInstance == null)
        {
            installedObject = null;
            return true;
        }


        if (installedObject != null)
        {
            Debug.LogError("Trying to install an object to a tile that already has an installed object.");
            return false;
        }

        installedObject = objectInstance;
        return true;
    }
}
