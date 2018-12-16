using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldController : MonoBehaviour {

    public static WorldController Instance { get; protected set; }

    //Tile Sprites
    public Sprite landSprite;
    public Sprite deepSprite;
    public Sprite shallowSprite;
    public Sprite dryingSprite;

    Dictionary<Tile, GameObject> tileGameObjectMap;
    Dictionary<InstalledObject, GameObject> installedObjectGameObjectMap;

    Dictionary<string, Sprite> installedObjectSprites;

    //The world and tile data
    public World World { get; protected set; }

    // Use this for initialization
    void Start () {

        //Load the Sprites from the resources folder
        LoadSprites();

        if (Instance != null) {
            Debug.Log("There should never be two World Controllers");
        }
        Instance = this;

        //Create a new world with the base map
        World = new World();
        World.ReadMap();

        World.RegisterInstalledObjectCreated(OnInstalledObjectCreated);

        //Instantiate the dictionary for which GameObject is which tile.
        tileGameObjectMap = new Dictionary<Tile, GameObject>();
        installedObjectGameObjectMap = new Dictionary<InstalledObject, GameObject>();

        //Create GameObject for each tile to show them
        for (int x = 0; x< World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                //Get the Tile Data
                Tile tile_data = World.GetTileAt(x, y);

                //Create a new GameObject and add it to the scene.
                GameObject tile_go = new GameObject();

                //Add out tile GO pair to the Dictionary
                tileGameObjectMap.Add(tile_data, tile_go);

                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);
                tile_go.transform.SetParent(this.transform, true);
                //tile_go.layer = 10;

                //Add a Sprite renderer and set the sprite based off the base map.
                SpriteRenderer tile_sr = tile_go.AddComponent<SpriteRenderer>();
                if(tile_data.Type == Tile.TileType.Land)
                {
                    tile_sr.sprite = landSprite;
                }
                else if(tile_data.Type == Tile.TileType.DeepWater)
                {
                    tile_sr.sprite = deepSprite;
                }
                else if (tile_data.Type == Tile.TileType.ShallowWater)
                {
                    tile_sr.sprite = shallowSprite;
                }
                else if (tile_data.Type == Tile.TileType.Drying)
                {
                    tile_sr.sprite = dryingSprite;
                }
                //tile_sr.sortingLayerName = "Tiles";

                //Register a callback so that the GameObject gets updated whenever the tile type chages 
                tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );
            }
        }

        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);

	}
    	
	// Update is called once per frame
	void Update () {

	}

    void OnTileTypeChanged (Tile tile_data, GameObject tile_go)
    {
        if (tile_data.Type == Tile.TileType.Land)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = landSprite;
        }
        else if (tile_data.Type == Tile.TileType.DeepWater)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = deepSprite;
        }
        else if (tile_data.Type == Tile.TileType.ShallowWater)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = shallowSprite;
        }
        else if (tile_data.Type == Tile.TileType.Drying)
        {
            tile_go.GetComponent<SpriteRenderer>().sprite = dryingSprite;
        }
        else
        {
            Debug.LogError("Unrecognised Tile Type");
        }
    }

    public Tile GetTileAtWorldCoord(Vector3 coord)
    {
        //Get the tile at a Unity World Coordinate.
        int x = Mathf.FloorToInt(coord.x);
        int y = Mathf.FloorToInt(coord.y);

        return WorldController.Instance.World.GetTileAt(x, y);
    }

    public void OnInstalledObjectCreated(InstalledObject obj) {
        //Create a visual GameObject linked to this data
        //Debug.Log("OnInstalledObjectCreated");

        // FIXME: No multitile objects or rotation

        GameObject obj_go = new GameObject();

        //Add our InstalledObject GO pair to the Dictionary
        installedObjectGameObjectMap.Add(obj, obj_go);

        obj_go.name = obj.objectType + "_" + obj.tile.X + "_" + obj.tile.Y;
        obj_go.transform.position = new Vector3(obj.tile.X, obj.tile.Y, 0);
        obj_go.transform.SetParent(this.transform, true);
        //obj_go.layer = 11;

        //Add a Sprite renderer and set the sprite based off the base map.
        SpriteRenderer obj_sr = obj_go.AddComponent<SpriteRenderer>();
        obj_sr.sprite = GetSpriteForInstalledObject(obj);
        obj_sr.sortingLayerName = "InstalledObjects";


        //Register a callback so that the GameObject gets updated whenever the object chages 
        obj.RegisterInstalledObjectChangedCallback(OnInstalledObjectChanged);
    }

    void OnInstalledObjectChanged(InstalledObject obj) {
        // Make sure that the graphics are correct
        if (installedObjectGameObjectMap.ContainsKey(obj) == false) {
            Debug.LogError("OnInstalledObjectChanged - trying to change visuals for Installed Object not in map.");
            return;
        }
        GameObject obj_go = installedObjectGameObjectMap[obj];
        obj_go.GetComponent<SpriteRenderer>().sprite = GetSpriteForInstalledObject(obj);
    }

    Sprite GetSpriteForInstalledObject(InstalledObject obj) {
        if (obj.linksToNeighbour == false) {
            return installedObjectSprites[obj.objectType];
        }
        else {
            int x = obj.tile.X;
            int y = obj.tile.Y;
            //Here it is more complicated
            string spriteName = obj.objectType + "_";
            //Check neighbours NESW
            Tile t;
            t = World.GetTileAt(x, y + 1);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                spriteName += "N";
            }
            t = World.GetTileAt(x + 1, y);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                spriteName += "E";
            }
            t = World.GetTileAt(x, y - 1);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                spriteName += "S";
            }
            t = World.GetTileAt(x - 1, y);
            if (t != null && t.installedObject != null && t.installedObject.objectType == obj.objectType) {
                spriteName += "W";
            }

            if(installedObjectSprites.ContainsKey(spriteName) == false) {
                Debug.LogError("GetSpriteForInstalledObject - No such Sprite " + spriteName);
                return null;
            }

            return installedObjectSprites[spriteName];
        }
    }

    void LoadSprites () {
        installedObjectSprites = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Images/InstalledObjects/");
        foreach (Sprite s in sprites) {
            //Debug.Log(s);
            installedObjectSprites[s.name] = s;
        }
    }
}
