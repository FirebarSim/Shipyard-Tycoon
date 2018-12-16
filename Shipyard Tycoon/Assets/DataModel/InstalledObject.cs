using System;
using UnityEngine;

public class InstalledObject
{
    public Tile tile { get; protected set; } //Base installed tile, but may cover multiple tiles.
    public string objectType { get; protected set; } //Tells us the visual model for the sprite.
    float movementCost = 1f; //Multiplier, i.e. 2 means half speed. Can be summed. 0 = impassable
    int width = 1;
    int height = 1;
    public bool linksToNeighbour { get; protected set; } //Things like walls link to their neighbours

    Action<InstalledObject> cbInstalledObjectChanged;

    public Func<Tile, bool> funcPositionValidation;

    // TODO implement rotation
    // TODO implement larger objects

    protected InstalledObject ()
    {

    }

    //Used to create prototypical object
    static public InstalledObject CreatePrototype( string objectType, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbour = false)
    {
        InstalledObject obj = new InstalledObject();
        obj.objectType = objectType;
        obj.movementCost = movementCost;
        obj.width = width;
        obj.height = height;
        obj.linksToNeighbour = linksToNeighbour;

        obj.funcPositionValidation = obj.__IsValidPosition;

        return obj;
    }

    // Install object in tile
    static public InstalledObject PlaceInstance(InstalledObject proto, Tile tile) {
        if(proto.funcPositionValidation(tile) == false) {
            Debug.LogError("PlaceInstance - Position Validity Function Returned False");
            return null;
        }

        InstalledObject obj = new InstalledObject();

        obj.objectType = proto.objectType;
        obj.movementCost = proto.movementCost;
        obj.width = proto.width;
        obj.height = proto.height;
        obj.linksToNeighbour = proto.linksToNeighbour;
        obj.tile = tile;

        if(tile.PlaceObject(obj) == false)
        {
            //Werent able to place object in this tile
            return null;
        }

        if(obj.linksToNeighbour) {
            //This type of object or furniture links itself to its neighbours and may affect them. We tell the nighbours that they have a new neighbour. Trigger their OnChangedCallback.
            Tile t;
            t = tile.world.GetTileAt(obj.tile.X, obj.tile.Y + 1);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.cbInstalledObjectChanged(t.installedObject);
            }
            t = tile.world.GetTileAt(obj.tile.X + 1, obj.tile.Y);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.cbInstalledObjectChanged(t.installedObject);
            }
            t = tile.world.GetTileAt(obj.tile.X, obj.tile.Y - 1);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.cbInstalledObjectChanged(t.installedObject);
            }
            t = tile.world.GetTileAt(obj.tile.X - 1, obj.tile.Y);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                t.installedObject.cbInstalledObjectChanged(t.installedObject);
            }
        }

        return obj;
    }

    public void RegisterInstalledObjectChangedCallback(Action<InstalledObject> callback) {
        cbInstalledObjectChanged += callback;
    }

    public void UnregisterInstalledObjectChangedCallback(Action<InstalledObject> callback) {
        cbInstalledObjectChanged -= callback;
    }

    public bool IsValidPosition (Tile t) {
        return funcPositionValidation(t);
    }

    public bool __IsValidPosition (Tile t) {
        // FIXME: Should never be called directly
        //Check that we satisfy the requirements to install an object somewhere.
        //Make sure the tile is drying or land
        if (t.Type != Tile.TileType.Land) {
            return false;
        }

        if (t.installedObject != null) {
            return false;
        }

        return true;
    }

    public bool __IsValidPosition_Door(Tile t) {
        // FIXME: Should never be called directly
        if (__IsValidPosition(t) == false) {
            return false;
        }

        return true;
    }
}
