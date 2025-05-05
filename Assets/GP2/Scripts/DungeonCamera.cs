using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
    public DungeonTiler DungeonTiler;

    [Header("Camera Settings")]
    public float MovementSpeed;
    public float ZoomSpeed;

    private Camera MainCamera;
    
    /* Scale camera settings to see the generated map.*/
    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        
        MainCamera = GetComponent<Camera>();

        BoundsInt dungeonBounds = DungeonTiler.BackgroundTilemap.cellBounds;

        MainCamera.orthographicSize = dungeonBounds.size.x / 2f;
        transform.position = new Vector3(dungeonBounds.center.x, dungeonBounds.center.y, -10);
        
    }

    /* Handles camera controls.*/
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, MovementSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, MovementSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, -MovementSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-MovementSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(MovementSpeed * Time.deltaTime, 0, 0);
        }
        
        if (Input.GetKey(KeyCode.Q))
        {
            MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize - ZoomSpeed * Time.deltaTime, 1f, 200f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            MainCamera.orthographicSize = Mathf.Clamp(MainCamera.orthographicSize + ZoomSpeed * Time.deltaTime, 1f, 200f);
        }
    }
}
