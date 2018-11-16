using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldController : MonoBehaviour {

    public Sprite landSprite;
    public Sprite deepSprite;
    public Sprite shallowSprite;
    public Sprite dryingSprite;

    World world;

	// Use this for initialization
	void Start () {
        world = new World();
        //world.RandomiseTiles();
        world.ReadMap();

        //Create GameObject for each tile to show them
        for (int x = 0; x< world.Width; x++)
        {
            for (int y = 0; y < world.Height; y++)
            {
                Tile tile_data = world.GetTileAt(x, y);

                GameObject tile_go = new GameObject();
                tile_go.name = "Tile_" + x + "_" + y;
                tile_go.transform.position = new Vector3(tile_data.X, tile_data.Y, 0);

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

                tile_data.RegisterTileTypeChangedCallback( (tile) => { OnTileTypeChanged(tile, tile_go); } );
            }
        }

	}

    float randomiseTileTimer = 2f;
    	
	// Update is called once per frame
	void Update () {
        randomiseTileTimer -= Time.deltaTime;

        if(randomiseTileTimer < 0)
        {
            world.RandomiseTiles();
            randomiseTileTimer = 2f;
        }
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
}
