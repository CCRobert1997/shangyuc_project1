using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Terrain terrainmain;

    public float speed = 0.2f;

    public float mouseX;
    public float mouseY;

    private float VerticalRotationMin = 90f;
    private float VerticalRotationMax = 270f;

    int camerainitialheightindex = 0;
    //variables of terrain 
    private TerrainController TerrainController;
    //float[,] basefloor;
    float basescale;
    float xbound;
    float zbound;
    int xwidth;
    int zHeight;
    float Heightbase;
    // Use this for initialization
    void Start() {
        TerrainController = terrainmain.gameObject.GetComponent<TerrainController>();
        xbound = terrainmain.terrainData.size.x;
        zbound = terrainmain.terrainData.size.z;
        xwidth = terrainmain.terrainData.heightmapWidth;
        zHeight = terrainmain.terrainData.heightmapHeight;
        Heightbase = terrainmain.terrainData.size.y;
        this.transform.eulerAngles = new Vector3(10f, 45f, 0f);
        

    }

    // Update is called once per frame
    void Update() {
        if (camerainitialheightindex == 0)
        {
            this.transform.position = (Vector3.up) * (TerrainController.addheight + 200f) + (new Vector3(100f, Heightbase * TerrainController.heightsarray[(int)(100f/xbound*(zHeight - 1)), (int)(100f/zbound*(xwidth - 1))], 100f));
            camerainitialheightindex = 1;
        }
        
        /*if (Input.GetKey(KeyCode.T))
            {
                Debug.Log((TerrainController.heightsarray[(int)(((float)(this.transform.position.x)) / ((float)(xbound)) * zHeight), (int)((((float)(this.transform.position.z)) / ((float)zbound)) * xwidth)] - this.transform.position.y) * (Vector3.up));
                Debug.Log(this.transform.position.y);
                Debug.Log(TerrainController.heightsarray[(int)((((float)(this.transform.position.z)) / ((float)zbound)) * xwidth), (int)(((float)(this.transform.position.x)) / ((float)(xbound)) * zHeight)]);
            }*/

        if (this.transform.position.y <= (TerrainController.addheight + 45f) + Heightbase * TerrainController.heightsarray[(int)((((float)(this.transform.position.z)) / ((float)zbound)) * (xwidth - 1)), (int)(((float)(this.transform.position.x)) / ((float)(xbound)) * (zHeight - 1))])
        {
            
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {

                //this.transform.position = (Vector3.up) * 5f + this.transform.position + (((TerrainController.addheight + 20f) + Heightbase * TerrainController.heightsarray[(int)((((float)(this.transform.position.z)) / ((float)zbound)) * xwidth), (int)(((float)(this.transform.position.x)) / ((float)(xbound)) * zHeight)] - this.transform.position.y) * (Vector3.up));
                this.transform.position = (Vector3.up) * 5f + this.transform.position;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                this.transform.Translate(Vector3.forward * speed);
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.transform.Translate(Vector3.back * speed);
            }
            if (Input.GetKey(KeyCode.A))
            {
                this.transform.Translate(Vector3.left * speed);
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.Translate(Vector3.right * speed);
            }
            
        }


        float angle = 0.3f;
        if (Input.GetKey(KeyCode.Q))
        {
            this.transform.Rotate(0, -angle, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            this.transform.Rotate(0, angle, 0);
        }
        HandleMouseRotation();

        mouseX = Input.mousePosition.x;
        mouseY = Input.mousePosition.y;

        /*if (Input.GetKey(KeyCode.T))
        {
            //Debug.Log(zHeight);
            //Debug.Log(xbound);
            Debug.Log(TerrainController.heightsarray[(int)(((float)(this.transform.position.x)) / ((float)(xbound)) * zHeight), (int)((((float)(this.transform.position.z)) / ((float)zbound)) * xwidth)]);
        }*/
        

        
        this.transform.position = new Vector3
        (
            Mathf.Clamp(this.transform.position.x, 50f, ((float)xbound) -50f),
            Mathf.Clamp(this.transform.position.y, -1000f, 1000f),
            Mathf.Clamp(this.transform.position.z, 50f, ((float)zbound) - 50f)
        );

        

    } 

    



    void LateUpdate()
    {
        
    } 



    /*void OnCollissionEnter(Collider collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            this.transform.Translate(new Vector3 (0f, 0f, 0f));
        }
    }*/


    public void HandleMouseRotation()
    {
        float factor = 0.1f;
        if (Input.GetMouseButton(0))
        {
            var cameraRotationZ = (Input.mousePosition.x - mouseX) * factor;
            if (Input.mousePosition.x != mouseX)
            {
                this.transform.Rotate(0, 0, -cameraRotationZ);
            }
            var cameraRotationX = (Input.mousePosition.y - mouseY) * factor;
            // && (this.transform.rotation.x + cameraRotationX) <= VerticalRotationMax
            if (Input.mousePosition.y != mouseY)
            {
                
                this.transform.Rotate(-cameraRotationX, 0, 0);
            }
        }
    }
}
