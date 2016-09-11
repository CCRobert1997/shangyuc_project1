using System;
using UnityEngine;
using System.Collections;

public class TerrainController : MonoBehaviour {

    public Terrain terrainmain;
    public float[,] heightsarray;
    public float addheight = 0.0f;
    public PointLight pointLight;
    // Use this for initialization
    void Start () {
        //get the height and width of the heightmap;
        int xwidth = terrainmain.terrainData.heightmapWidth;
        int zHeight = terrainmain.terrainData.heightmapHeight;
        

        int i, j, t = 0, delta, diff, r, p;
        //i is interator for row, j is interator for column, 2^t + 1 is the scale of diamond or square now
        //delta is the step for row, diff is the step for column
        //r is use in diamond to square to check the row manipulate now is even or odd
        //p is the int which is used to find how many points are used to find height of the derived point, p is 3 or 4; 
        float sumforaverage;

        //initialize heightsarray
        this.heightsarray = terrainmain.terrainData.GetHeights(0, 0, xwidth, zHeight);

        for (i = 0; i < zHeight; i++)
        {
            for (j = 0; j < xwidth; j++)
            {
                heightsarray[i, j] = 0f;
            }
        }


        //random four corners first
        System.Random ra = new System.Random();

        heightsarray[zHeight-1, xwidth - 1] = (0 + ra.Next(100)) / 100.0f;
        heightsarray[zHeight - 1, 0] = ra.Next(100) / 100.0f;
        heightsarray[0, xwidth - 1] = ra.Next(100) / 100.0f;
        heightsarray[0, 0] = (0 + ra.Next(100)) / 100.0f;


        //Diamond-square algorithm
        while (t < Math.Log(xwidth-1, 2))
        {
            //square to diamond
            delta = (zHeight - 1) / (int)Math.Pow(2, t);
            diff = (xwidth -1) / (int)Math.Pow(2, t);
            
            for (i = delta; i < zHeight; i = i + delta)
            {
                for (j = diff; j < xwidth; j = j + diff)
                {
                    sumforaverage = heightsarray[i - delta, j] + heightsarray[i, j - diff] + heightsarray[i - delta, j - diff] + heightsarray[i, j];
                    
                    heightsarray[i - delta/2, j - diff/2] = sumforaverage/4.0f + ((ra.Next(100) - 50) / 100.0f)/ ((float)(Math.Pow(Math.Log(t + 1), ra.Next(20)))+1);
                    
                }
            }
            //diamond to square
            t = t + 1;
            delta = (zHeight - 1) / (int)Math.Pow(2, t);
            diff = (xwidth - 1) / (int)Math.Pow(2, t);
            r = 0;
            for (i = 0; i < zHeight; i = i + delta)
            {
                for (j = ((r + 1) % 2) * diff; j < xwidth; j = j + 2*diff)
                {
                    //float high = -1f;
                    //float  low = 1f;
                    sumforaverage = 0;
                    p = 0;
                    if (i - delta >= 0)
                    {
                        sumforaverage = sumforaverage + heightsarray[i - delta, j];
                        p++;
                        
                    }
                    if (j - diff >= 0)
                    {
                        sumforaverage = sumforaverage + heightsarray[i, j - diff];
                        p++;
                        
                    }
                    if (i + delta < zHeight)
                    {
                        sumforaverage = sumforaverage + heightsarray[i + delta, j ];
                        p++;
                        
                    }
                    if (j + diff < xwidth)
                    {
                        sumforaverage = sumforaverage + heightsarray[i, j + diff];
                        p++;

                    }
                    //int deltarange = (int)(Math.Min(2*(high - low), 1));
                    heightsarray[i, j] = sumforaverage / (float)p + ((ra.Next(100) - 50) / 100.0f)/((float)(Math.Pow(Math.Log(t), ra.Next(20)))+1);
                    //heightsarray[i, j] = sumforaverage / (float)p + ((ra.Next(100) - 50) / 100.0f);
                    //heightsarray[i, j] = sumforaverage / 4.0f + ((ra.Next(deltarange * 100) - deltarange * 100 / 2) / 100.0f);
                }
                r = r + 1;
            }

        }
        terrainmain.terrainData.SetHeights(0, 0, heightsarray);

        MeshFilter terrainmesh = terrainmain.gameObject.AddComponent<MeshFilter>();
        terrainmesh.mesh = this.CreateterrainMesh();

        MeshRenderer renderer = terrainmain.gameObject.AddComponent<MeshRenderer>();
        renderer.material.shader = Shader.Find("Unlit/IcesoilgrassShader");


    }



    Mesh CreateterrainMesh()
    {
        Mesh m = new Mesh();
        float xbound = terrainmain.terrainData.size.x;
        float zbound = terrainmain.terrainData.size.z;
        int xwidth = terrainmain.terrainData.heightmapWidth;
        int zHeight = terrainmain.terrainData.heightmapHeight;
        float Heightbase = terrainmain.terrainData.size.y;
        //this.heightsarray = terrainmain.terrainData.GetHeights(0, 0, xwidth, zHeight);
        /*Debug.Log(xbound);
        Debug.Log(zbound);
        Debug.Log(xwidth);
        Debug.Log(zHeight);
        Debug.Log(Heightbase);
        Debug.Log(heightsarray[0, 0]);*/
        int s = 1;//rescale

        int i, j, k=0;
        m.name = "terrain";

        Vector3[] vertices = new Vector3[6* (xwidth - 1) * (zHeight - 1)/(s*s)];
        Color[] colors = new Color[6 * (xwidth - 1) * (zHeight - 1)/(s*s)];
        Vector3[] normals = new Vector3[6 * (xwidth - 1) * (zHeight - 1) / (s * s)];
        Vector3[] vertexnormals = new Vector3[6 * (xwidth - 1) * (zHeight - 1) / (s * s)];
        Vector2[] uv = new Vector2[6 * (xwidth - 1) * (zHeight - 1)/(s*s)];
        int[] triangles = new int[6 * (xwidth - 1) * (zHeight - 1)/(s*s)];



        for (i=0; i < zHeight - 1; i=i+s)
        {
            //Debug.Log((xwidth - 1) / s);
            for (j = 0; j < xwidth - 1; j=j+s)
            {
                Vector3 vertex1 = new Vector3((float)(j) / (xwidth - 1) * xbound, Heightbase*heightsarray[i, j] + addheight, (float)(i)/(zHeight - 1)*zbound);
                Vector3 vertex2 = new Vector3((float)(j + s) / (xwidth - 1) * xbound, Heightbase*heightsarray[i + s, j + s] + addheight, (float)(i + s) / (zHeight - 1) * zbound);
                Vector3 vertex3 = new Vector3((float)(j + s) / (xwidth - 1) * xbound, Heightbase*heightsarray[i, j + s] + addheight, (float)(i) / (zHeight - 1) * zbound);
                /*Debug.Log(vertex1);
                Debug.Log(vertex2);
                Debug.Log(vertex3);*/
                Vector3 vertex4 = vertex1;
                Vector3 vertex5 = new Vector3((float)(j) / (xwidth - 1) * xbound, Heightbase*heightsarray[i + s, j] + addheight, (float)(i + s) / (zHeight - 1) * zbound);
                Vector3 vertex6 = vertex2;
                vertices[k] = vertex1;
                colors[k] = colorvsheight(heightsarray[i, j]);
                triangles[k] = k;
                k++;
                vertices[k] = vertex2;
                colors[k] = colorvsheight(heightsarray[i+s, j+s]);
                triangles[k] = k;
                k++;
                vertices[k] = vertex3;
                colors[k] = colorvsheight(heightsarray[i, j+s]);
                triangles[k] = k;
                normals[k - 2] = (Vector3.Cross(vertex2 - vertex1, vertex3 - vertex2)).normalized;
                normals[k - 1] = normals[k - 2];
                normals[k] = normals[k - 2];
                k++;
                vertices[k] = vertex4;
                colors[k] = colorvsheight(heightsarray[i, j]);
                triangles[k] = k;
                k++;
                vertices[k] = vertex5;
                colors[k] = colorvsheight(heightsarray[i+s, j]);
                triangles[k] = k;
                k++;
                vertices[k] = vertex6;
                colors[k] = colorvsheight(heightsarray[i+s, j+s]);
                triangles[k] = k;
                normals[k - 2] = (Vector3.Cross(vertex5 - vertex4, vertex6 - vertex5)).normalized;
                //Debug.Log(normals[k - 2]);
                normals[k - 1] = normals[k - 2];
                normals[k] = normals[k - 2];
                k++;

            }
        }
        k = 0;
        //Vector3 n1, n2, n3, n4, n5, n6;
        for (i = 0; i < zHeight - 1; i = i + s)
        {
            //Debug.Log((xwidth - 1) / s);
            for (j = 0; j < xwidth - 1; j = j + s)
            {
                vertexnormals[k] = normals[k];
                if (k + 6*(xwidth - 1) - 6 < 6 * (xwidth - 1) * (zHeight - 1) / (s * s))
                {
                    vertexnormals[k]  = vertexnormals[k] + normals[k + 6 * (xwidth - 1) - 6];
                    if (k + 6 * (xwidth - 1) < 6 * (xwidth - 1) * (zHeight - 1) / (s * s))
                    {
                        vertexnormals[k] = vertexnormals[k] + normals[k + 6 * (xwidth - 1)];
                    }
                }
                if (k - 6 >= 0)
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k - 6];
                }
                //n1 = new Vector3(0, 0, 0);
                //n, n2, n3, n4, n5, n6;
                //vertexnormals[k] = normals[k + 4] + normals[k - 1] + normals[k - 4] + normals[k - 6*(xwidth - 1)] + normals[k + 4 - 6 * (xwidth - 1)]  + normals[k - 4 - 6 * (xwidth - 1)];
                k++;
                vertexnormals[k] = normals[k];
                if (k - 6 * (xwidth - 1) + 6 >= 0)
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k - 6 * (xwidth - 1) + 6];
                }
                if (k - 6 * (xwidth - 1) >= 0)
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k - 6 * (xwidth - 1)];
                }
                if (k + 6 < 6 * (xwidth - 1) * (zHeight - 1) / (s * s))
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k + 6];
                }
                k++;
                vertexnormals[k] = normals[k];
                if (k + 6 < 6 * (xwidth - 1) * (zHeight - 1) / (s * s))
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k + 6];
                }
                if (k + 6 * (xwidth - 1) < 6 * (xwidth - 1) * (zHeight - 1) / (s * s))
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k + 6 * (xwidth - 1)];
                }
                if (k + 6 * (xwidth - 1) + 6 < 6 * (xwidth - 1) * (zHeight - 1) / (s * s))
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k + 6 * (xwidth - 1) + 6];
                }
                k++;
                vertexnormals[k] = vertexnormals[k-3];
                k++;
                vertexnormals[k] = vertexnormals[k - 3];
                k++;
                vertexnormals[k] = normals[k];
                if (k - 6 >= 0)
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k - 6];
                }
                if (k - 6 * (xwidth - 1) >= 0)
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k - 6 * (xwidth - 1)];
                }
                if (k - 6 * (xwidth - 1) - 6 >= 0)
                {
                    vertexnormals[k] = vertexnormals[k] + normals[k - 6 * (xwidth - 1) - 6];
                }
                k++;
                //Debug.Log(k);
                //Debug.Log(6 * (xwidth - 1) * (zHeight - 1) / (s * s));
            }
        }
        m.vertices = vertices;
        m.colors = colors;
        m.normals = vertexnormals;
        m.uv = uv;
        m.triangles = triangles;
        return m;

    }

    Color colorvsheight(float H)
    {
        Color[] icesoilgrass = { Color.white, Color.green, Color.green };
        if (H >= 0.65f)
        {
            return icesoilgrass[0];
        } else if (H <= 0.50f)
        {
            return icesoilgrass[2];
        } else
        {
            return icesoilgrass[1];
        }

    }

    /*void heightcalculate()
    {
        int i, j;
        for (i=0; i< terrainmain.terrainData.heightmapWidth; i++)
        {
            for (j = 0; j < terrainmain.terrainData.heightmapHeight; j++)
            {
                if (i%2==0 && j%2== 0)
                {
                    heightsarray[i, j] = 1f;
                }
            }
        }
    }*/

    // Update is called once per frame
    void Update () {
        // Get renderer component (in order to pass params to shader)
        MeshRenderer renderer = this.gameObject.GetComponent<MeshRenderer>();

        // Pass updated light positions to shader
        renderer.material.SetColor("_PointLightColor", this.pointLight.color);
        renderer.material.SetVector("_PointLightPosition", this.pointLight.GetWorldPosition());


    }
}
