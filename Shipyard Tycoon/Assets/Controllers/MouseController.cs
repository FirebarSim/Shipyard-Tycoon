using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor.Events;

public class MouseController : MonoBehaviour {

    public GameObject selectCursorPrefab;

    public float scrollSpeed = 3f;

    bool buildModeIsObjects;
    int terrainMode = 0;
    string buildModeObjectType;

    Vector3 currMousePosition;
    Vector3 lastMousePosition;
    Vector3 dragStartPosition;

    List<GameObject> dragPreviewGameObjects;

	// Use this for initialization
	void Start () {
        dragPreviewGameObjects = new List<GameObject>();
    }
	
	// Update is called once per frame
	void Update () {
        //Update Mouse position
        currMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        currMousePosition.z = 0;

        //UpdateCursor();
        UpdateDragging();
        UpdateCameraMovement();
    }

    //void UpdateCursor()
    //{
    //    //Update Select Cursor
    //    Tile tileUnderMouse = WorldController.Instance.GetTileAtWorldCoord(currMousePosition);
    //    if (tileUnderMouse != null)
    //    {
    //        selectCursor.SetActive(true);
    //        Vector3 selectPosition = new Vector3(tileUnderMouse.X, tileUnderMouse.Y, 0);
    //        selectCursor.transform.position = selectPosition;
    //    }
    //    else
    //    {
    //        selectCursor.SetActive(false);
    //    }
    //}

    void UpdateDragging ()
    {
        //If Over a UI Element Bailout!
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        //Start Drag
        if (Input.GetMouseButtonDown(0)) {
            dragStartPosition = currMousePosition;
        }

        int start_x = Mathf.FloorToInt(dragStartPosition.x);
        int end_x = Mathf.FloorToInt(currMousePosition.x);
        int start_y = Mathf.FloorToInt(dragStartPosition.y);
        int end_y = Mathf.FloorToInt(currMousePosition.y);
        //Correct dragging that is not up and right
        if (end_x < start_x) {
            int tmp = end_x;
            end_x = start_x;
            start_x = tmp;
        }
        if (end_y < start_y) {
            int tmp = end_y;
            end_y = start_y;
            start_y = tmp;
        }

        //Clean Up Old Drag Previews
        while (dragPreviewGameObjects.Count>0) {
            GameObject go = dragPreviewGameObjects[0];
            dragPreviewGameObjects.RemoveAt(0);
            SimplePool.Despawn(go);
        }

        //During Drag
        if (Input.GetMouseButton(0)) {
            //Display a preview of the drag area
            for (int x = start_x; x <= end_x; x++) {
                for (int y = start_y; y <= end_y; y++) {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null) {
                        // Display the building hint on top of this position
                        GameObject go = SimplePool.Spawn(selectCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        go.transform.SetParent(this.transform, true);
                        dragPreviewGameObjects.Add(go);
                    }
                }
            }
        }

        //End Drag
        if (Input.GetMouseButtonUp(0)) {
            for (int x = start_x; x <= end_x; x++) {
                for (int y = start_y; y <= end_y; y++) {
                    Tile t = WorldController.Instance.World.GetTileAt(x, y);
                    if (t != null) {
                        if(buildModeIsObjects == true) {
                            //Instantly Builds an InstalledObject
                            //WorldController.Instance.World.PlaceInstalledObject(buildModeObjectType, t);
                            //Check if we can build in the selected tile and that there is no pending job for a tile
                            string objectType = buildModeObjectType;
                            if( WorldController.Instance.World.IsInstalledObjectPlacementValid(objectType, t) && t.pendingInstalledObject == null) {
                                // Tile is valid for this installed object so queue up the job.
                                Job j = new Job(t, (theJob) => {
                                    WorldController.Instance.World.PlaceInstalledObject(objectType, theJob.tile);
                                    t.pendingInstalledObject = null;
                                });
                                // FIXME: Clunky to have to manually set an explicit conflict flag. Easy to forget to clear it.
                                //Inform the tile of the pending job
                                t.pendingInstalledObject = j;
                                j.RegisterJobCancelCallback((theJob) => { theJob.tile.pendingInstalledObject = null; } );
                                //Add the job to the queue
                                WorldController.Instance.World.jobQueue.Enqueue(j);
                                Debug.Log("Job Queue Size: " + WorldController.Instance.World.jobQueue.Count);
                            }
                        }
                        else if (terrainMode == 1) {
                            if (t.Type == Tile.TileType.ShallowWater) {
                                t.Type = Tile.TileType.DeepWater;
                            }
                            else if (t.Type == Tile.TileType.Drying) {
                                t.Type = Tile.TileType.ShallowWater;
                            }
                            else if (t.Type == Tile.TileType.Land) {
                                t.Type = Tile.TileType.Drying;
                            }
                        }
                        else if (terrainMode == 2) {
                            if (t.Type == Tile.TileType.DeepWater) {
                                t.Type = Tile.TileType.ShallowWater;
                            }
                            else if (t.Type == Tile.TileType.ShallowWater) {
                                t.Type = Tile.TileType.Drying;
                            }
                            else if (t.Type == Tile.TileType.Drying) {
                                t.Type = Tile.TileType.Land;
                            }
                        }
                    }
                }
            }
        }
    }

    //void OnInstalledObjectJobComplete(string objectType, Tile t) {
    //    WorldController.Instance.World.PlaceInstalledObject(objectType, t);
    //}

    void UpdateCameraMovement ()
    {
        //Handle screen drag
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            Vector3 diff = lastMousePosition - currMousePosition;
            Camera.main.transform.Translate(diff);
        }
        //Update Mouse position
        lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //First item tagged at main camera
        lastMousePosition.z = 0;

        scrollSpeed = Mathf.Clamp(scrollSpeed, 1f, 5f);
        Camera.main.orthographicSize -= Camera.main.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 3f, 75f);
    }

    public void SetMode_Dredge ()
    {
        buildModeIsObjects = false;
        terrainMode = 1;
    }

    public void SetMode_Infill ()
    {
        buildModeIsObjects = false;
        terrainMode = 2;
    }

    public void SetMode_BuildInstalledObject (string objectType)
    {
        //Sea Wall is an installed object not a tile
        buildModeIsObjects = true;
        buildModeObjectType = objectType;
    }

}
