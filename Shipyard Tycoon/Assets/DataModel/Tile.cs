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

    LooseObject looseObject;
    InstalledObject installedObject;

    World world;
    int x;
    public int X
    {
        get
        {
            return x;
        }
    }
    int y;
    public int Y
    {
        get
        {
            return y;
        }
    }

    public Tile (World world, int x, int y)
    {
        this.world = world;
        this.x = x;
        this.y = y;
    }

    public void RegisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public void UnregisterTileTypeChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged -= callback;
    }

}
