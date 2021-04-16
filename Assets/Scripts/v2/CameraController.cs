using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float cameraSpeed = 70f;
    public float scrollSpeed = 5f;
    public float minScrollY = 20f;
    public float maxScrollY = 100f;
    
    public float clampMinZ = -205f;
    public float clampMaxZ = 50f;
    public float clampMinX = -100f;
    public float clampMaxX = 100f;
    

    public float panBorderThickness = 10;

    private bool doMovement = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Wenn man sich nicht mehr bewegen möchten einfach Escape drücken
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            doMovement = !doMovement;
        }
        if (!doMovement)
            return;

        //Hier die x,y Bewegungen der Kamera
        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            if (transform.position.z <= clampMaxZ)
            {
                transform.Translate(Vector3.forward * (cameraSpeed * Time.deltaTime), Space.World);
            }
        }
        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            if (transform.position.x >= clampMinX)
            {
                transform.Translate(Vector3.left * (cameraSpeed * Time.deltaTime), Space.World);
            }
        }
        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            if (transform.position.z >= clampMinZ)
            {
                transform.Translate(Vector3.back * (cameraSpeed * Time.deltaTime), Space.World);
            }
        }
        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            if (transform.position.x <= clampMaxX)
            {
                transform.Translate(Vector3.right * (cameraSpeed * Time.deltaTime), Space.World);
            }
        }
        
        // Hier das Scrolling
        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scrollWheel * 1000 * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minScrollY, maxScrollY);
        transform.position = pos;
    }
}
