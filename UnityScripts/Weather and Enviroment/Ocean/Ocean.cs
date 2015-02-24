using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ocean : MonoBehaviour
{

    public int width = 32;
    public int height = 32;
    public float s = 10;
    public float scale = 3.0f;
    public Vector3 size = new Vector3(175.0f, 5.0f, 175.0f);
    public float tiles_x = 20;
    public float tiles_y = 20;
    public float windx = 80.0f;
    public float normal_scale = 1;
    public float normalStrength = 0.2f;
    public float choppy_scale = 2.0f;
    public float uv_speed = 1.0f;
    public Material material_ocean;
    public Material material_plane;
    public Material material_underwater;
    public float delayx = 1000;
    public float delayxtemp = 0;

    private float max_LOD = 4;
    private ComplexF[] h0;
    public Texture2D[] caustics;
    private float framesPerSecond = 10;

    private ComplexF[] t_x;
    private ComplexF[] t_y;

    private ComplexF[] n0;
    private ComplexF[] n_x;
    private ComplexF[] n_y;

    private ComplexF[] data;
    private ComplexF[] data_x;

    private Color[] pixelData;
    private Texture2D textureA;
    private Texture2D textureB;

    private Vector3[] baseHeight;
    private Vector2[] baseUV;

    private Mesh baseMesh = null;

    private GameObject child;

    private List<List<Mesh>> tiles_LOD;

    private int g_height;
    private int g_width;

    private int n_width;
    private int n_height;

    private bool drawFrame = true;

    private bool normalDone = false;

    private bool reflectionRefractionEnabled = false;
    private Camera offscreenCam = null;
    private RenderTexture reflectionTexture = null;
    private RenderTexture refractionTexture = null;

    // public variables
    public float foamTime = 0.99999f;
    public Camera mycam;

    public Texture2D foamTexture;
    public Texture2D fresnelTexture;
    public Texture2D bumpTexture;
    public Texture2D maintexTexture;
    public Texture2D causticsTexture;
    public Color surfaceColor;
    public Color waterColor;
    public Color mainColor;
    public Color specularColor;
    public Color transparency;
    public Color sunColor;
    public Light sunposition;
    public float refractionlevel = 0.05f;
    public float sunpower = 2.05f;
    private bool forceOriginalShader = false;

    private Shader shader = null;
    private Shader shader_plane = null;
    private Shader shader_underwater = null;

    private float waterDirtyness = 0.5f;




    private Vector2[] uvs;
    private Vector3[] vertices;
    private Vector3[] normals;
    private Vector4[] tangents;

    private string _GUIText = "";

    public Shader OceanShader;

    //private float oceansize;//= width * s * scale * ( max_LOD * 2 + 1 );

    //public float GetWaterHeightAtLocation (float x , float y ) 
    //{
    //    x = x / size.x;
    //    x = (x-Mathf.Floor (x)) * width;
    //    y = y / size.z;
    //    y = (y-Mathf.Floor (y)) * height;

    //    // TODO: Interpolate
    //    return data[width * Mathf.FloorToInt(y) + Mathf.FloorToInt(x)].GetModulus() * scale / (width * height);
    //}

    public float GetWaterHeightAtLocation(float x, float y)
    {
        x = x / size.x;
        x = (x - Mathf.Floor(x)) * width;
        y = y / size.z;
        y = (y - Mathf.Floor(y)) * height;

        return data[width * Mathf.FloorToInt(y) + Mathf.FloorToInt(x)].Re * scale / (width * height);
    }


    public float GaussianRnd(float x, float y)
    {
        float x1 = Random.value;
        float x2 = Random.value;

        if (x1 == 0.0f)
            x1 = 0.01f;

        return Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Cos(2.0f * Mathf.PI * x2);
        //return Mathf.Cos(x/73+y*81);
    }

    public float P_spectrum(Vector2 vec_k, Vector2 wind)
    {
        float A = vec_k.x > 0.0f ? 1.0f : 0.05f; // Set wind to blow only in one direction - otherwise we get turmoiling water

        float L = wind.sqrMagnitude / 9.81f;
        float k2 = vec_k.sqrMagnitude;
        // Avoid division by zero
        if (vec_k.magnitude == 0.0f)
        {
            return 0.0f;
        }
        return A * Mathf.Exp(-1.0f / (k2 * L * L) - Mathf.Pow(vec_k.magnitude * 0.1f, 2.0f)) / (k2 * k2) * Mathf.Pow(Vector2.Dot(vec_k / vec_k.magnitude, wind / wind.magnitude), 2.0f);// * wind_x * wind_y;
    }


    // Use this for initialization
    void Start()
    {
        //oceansize= width * s * scale * ( max_LOD * 2 + 1 );
        // normal map size
        n_width = 256;
        n_height = 256;

        textureA = new Texture2D(n_width, n_height);
        textureB = new Texture2D(n_width, n_height);
        textureA.filterMode = FilterMode.Bilinear;
        textureB.filterMode = FilterMode.Bilinear;

        if (!SetupOffscreenRendering())
        {
            material_ocean.SetTexture("_BumpMap", textureA);
            material_ocean.SetTextureScale("_BumpMap", new Vector2(normal_scale, normal_scale));

            material_ocean.SetTexture("_BumpMap2", textureB);
            material_ocean.SetTextureScale("_BumpMap2", new Vector2(normal_scale, normal_scale));
        }

        pixelData = new Color[n_width * n_height];

        // Init the water height matrix
        data = new ComplexF[width * height];
        // lateral offset matrix to get the choppy waves
        data_x = new ComplexF[width * height];

        // tangent
        t_x = new ComplexF[width * height];
        t_y = new ComplexF[width * height];

        n_x = new ComplexF[n_width * n_height];
        n_y = new ComplexF[n_width * n_height];

        // Geometry size
        g_height = height + 1;
        g_width = width + 1;

        tiles_LOD = new List<List<Mesh>>();

        for (int LOD = 0; LOD < max_LOD * max_LOD; LOD++)
        {
            tiles_LOD.Add(new List<Mesh>());
        }


        GameObject tile;
        int chDist; // Chebychev distance	

        for (int y = 0; y < tiles_y; y++)
        {
            for (int x = 0; x < tiles_x; x++)
            {

                chDist = (int)Mathf.Max(Mathf.Abs(tiles_y / 2 - y), Mathf.Abs(tiles_x / 2 - x));
                chDist = chDist > 0 ? chDist - 1 : 0;
                //Debug.Log(chDist);
                float cy = y - tiles_y / 2;
                float cx = x - tiles_x / 2;
                tile = new GameObject("Tile" + chDist);

                //~ tile.transform.position.x = ;
                //~ tile.transform.position.y = ;
                //~ tile.transform.position.z = ;
                tile.transform.position = new Vector3(Mathf.RoundToInt(cx * size.x * s), Mathf.RoundToInt(-2.0f * chDist), Mathf.RoundToInt(cy * size.z * s));
                //~ tile.transform.localScale.x = s;
                //~ tile.transform.localScale.y = s;
                //~ tile.transform.localScale.z = s;
                tile.transform.localScale = new Vector3(s, s, s);
                MeshFilter filter = tile.AddComponent(typeof(MeshFilter)) as MeshFilter;
                tile.AddComponent("MeshRenderer");
                //~ tile.AddComponent("MeshCollider");
                //~ MeshCollider col = tile.AddComponent(typeof(MeshCollider)) as MeshCollider;
                //~ col.sharedMesh = filter.mesh;
                //~ col.smoothSphereCollisions = true;
                //~ col.convex = true;
                //~ tile.AddComponent(typeof(RecalculateBounds));


                tile.renderer.material = material_ocean;

                //Make child of this object, so we don't clutter up the
                //scene hierarchy more than necessary.
                tile.transform.parent = transform;

                //Also we don't want these to be drawn while doing refraction/reflection passes,
                //so we'll add the to the water layer for easy filtering.
                tile.layer = LayerMask.NameToLayer("Water");

                // Determine which LOD the tile belongs
                //~ tiles_LOD[chDist].Add ((tile.GetComponent(typeof(MeshFilter)) as MeshFilter).mesh);
                tiles_LOD[chDist].Add(filter.mesh);
            }
        }


        // Init wave spectra. One for vertex offset and another for normal map
        h0 = new ComplexF[width * height];
        n0 = new ComplexF[n_width * n_height];

        // Wind restricted to one direction, reduces calculations
        Vector2 wind = new Vector2(0.0f, windx);

        // Initialize wave generator	
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float yc = y < height / 2 ? y : -height + y;
                float xc = x < width / 2 ? x : -width + x;
                Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / size.x, 2.0f * Mathf.PI * yc / size.z);
                h0[width * y + x] = new ComplexF(GaussianRnd(x, y), GaussianRnd(x, y)) * 0.707f * Mathf.Sqrt(P_spectrum(vec_k, wind));
            }
        }

        for (int y = 0; y < n_height; y++)
        {
            for (int x = 0; x < n_width; x++)
            {
                float yc = y < n_height / 2 ? y : -n_height + y;
                float xc = x < n_width / 2 ? x : -n_width + x;
                Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / (size.x / normal_scale), 2.0f * Mathf.PI * yc / (size.z / normal_scale));
                n0[n_width * y + x] = new ComplexF(GaussianRnd(x, y), GaussianRnd(x, y)) * 0.707f * Mathf.Sqrt(P_spectrum(vec_k, wind));
            }
        }

        GenerateHeightmap();
        GenerateBumpmaps();

    }


    void GenerateHeightmap()
    {

        Mesh mesh = new Mesh();

        int y = 0;
        int x = 0;

        // Build vertices and UVs
        vertices = new Vector3[g_height * g_width];
        tangents = new Vector4[g_height * g_width];
        Vector2[] uv = new Vector2[g_height * g_width];

        Vector2 uvScale = new Vector2(1.0f / (g_width - 1), 1.0f / (g_height - 1));
        Vector3 sizeScale = new Vector3(size.x / (g_width - 1), size.y, size.z / (g_height - 1));

        for (y = 0; y < g_height; y++)
        {
            for (x = 0; x < g_width; x++)
            {
                Vector3 vertex = new Vector3(x, 0.0f, y);
                vertices[y * g_width + x] = Vector3.Scale(sizeScale, vertex);
                uv[y * g_width + x] = Vector2.Scale(new Vector2(x, y), uvScale);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;

        for (y = 0; y < g_height; y++)
        {
            for (x = 0; x < g_width; x++)
            {
                tangents[y * g_width + x] = new Vector4(1.0f, 0.0f, 0.0f, -1.0f);
            }
        }
        mesh.tangents = tangents;

        for (int LOD = 0; LOD < max_LOD; LOD++)
        {
            Vector3[] verticesLOD = new Vector3[(height / (int)Mathf.Pow(2, LOD) + 1) * (width / (int)Mathf.Pow(2, LOD) + 1)];
            Vector2[] uvLOD = new Vector2[(height / (int)Mathf.Pow(2, LOD) + 1) * (width / (int)Mathf.Pow(2, LOD) + 1)];
            int idx = 0;

            for (y = 0; y < g_height; y += (int)Mathf.Pow(2, LOD))
            {
                for (x = 0; x < g_width; x += (int)Mathf.Pow(2, LOD))
                {
                    verticesLOD[idx] = vertices[g_width * y + x];
                    uvLOD[idx++] = uv[g_width * y + x];
                }
            }
            for (int k = 0; k < tiles_LOD[LOD].Count; k++)
            {
                Mesh meshLOD = tiles_LOD[LOD][k];
                meshLOD.vertices = verticesLOD;
                meshLOD.uv = uvLOD;
            }
        }

        //~ int LOD=0;
        // Build triangle indices: 3 indices into vertex array for each triangle
        for (int LOD = 0; LOD < max_LOD; LOD++)
        {
            int index = 0;
            int width_LOD = width / (int)Mathf.Pow(2, LOD) + 1;
            int[] triangles = new int[(height / (int)Mathf.Pow(2, LOD) * width / (int)Mathf.Pow(2, LOD)) * 6];
            for (y = 0; y < height / Mathf.Pow(2, LOD); y++)
            {
                for (x = 0; x < width / Mathf.Pow(2, LOD); x++)
                {
                    // For each grid cell output two triangles
                    triangles[index++] = (y * width_LOD) + x;
                    triangles[index++] = ((y + 1) * width_LOD) + x;
                    triangles[index++] = (y * width_LOD) + x + 1;

                    triangles[index++] = ((y + 1) * width_LOD) + x;
                    triangles[index++] = ((y + 1) * width_LOD) + x + 1;
                    triangles[index++] = (y * width_LOD) + x + 1;
                }
            }
            for (int k = 0; k < tiles_LOD[LOD].Count; k++)
            {
                Mesh meshLOD = tiles_LOD[LOD][k];
                meshLOD.triangles = triangles;
            }
        }

        baseMesh = mesh;
    }

    void GenerateBumpmaps()
    {
        if (!normalDone)
        {
            for (int idx = 0; idx < 2; idx++)
            {
                for (int y = 0; y < n_height; y++)
                {
                    for (int x = 0; x < n_width; x++)
                    {
                        float yc = y < n_height / 2 ? y : -n_height + y;
                        float xc = x < n_width / 2 ? x : -n_width + x;
                        Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / (size.x / normal_scale), 2.0f * Mathf.PI * yc / (size.z / normal_scale));

                        float iwkt = idx == 0 ? 0.0f : Mathf.PI / 2;
                        ComplexF coeffA = new ComplexF(Mathf.Cos(iwkt), Mathf.Sin(iwkt));
                        ComplexF coeffB = coeffA.GetConjugate();

                        int ny = y > 0 ? n_height - y : 0;
                        int nx = x > 0 ? n_width - x : 0;

                        n_x[n_width * y + x] = (n0[n_width * y + x] * coeffA + n0[n_width * ny + nx].GetConjugate() * coeffB) * new ComplexF(0.0f, -vec_k.x);
                        n_y[n_width * y + x] = (n0[n_width * y + x] * coeffA + n0[n_width * ny + nx].GetConjugate() * coeffB) * new ComplexF(0.0f, -vec_k.y);
                    }
                }
                Fourier.FFT2(n_x, n_width, n_height, FourierDirection.Backward);
                Fourier.FFT2(n_y, n_width, n_height, FourierDirection.Backward);

                for (int i = 0; i < n_width * n_height; i++)
                {
                    Vector3 bump = new Vector3(n_x[i].Re * Mathf.Abs(n_x[i].Re), n_y[i].Re * Mathf.Abs(n_y[i].Re), n_width * n_height / scale / normal_scale * normalStrength).normalized * 0.5f;
                    pixelData[i] = new Color(bump.x + 0.5f, bump.y + 0.5f, bump.z + 0.8f);
                    //			pixelData[i] = Color (0.5, 0.5, 1.0);			
                }
                if (idx == 0)
                {
                    textureA.SetPixels(pixelData, 0);
                    textureA.Apply();
                }
                else
                {
                    textureB.SetPixels(pixelData, 0);
                    textureB.Apply();
                }
            }
            normalDone = true;
        }

    }

    bool SetupOffscreenRendering()
    {

        //Check for rendertexture support and return false if not supported
        if (!SystemInfo.supportsRenderTextures)
            return false;


        shader = Shader.Find("Ocean/OceanMain_v1");

        //Bail out if the shader could not be compiled
        if (shader == null)
        {
            Debug.Log("Shader is null");
            _GUIText = "Shader is null";
            shader = OceanShader;
            return false;
        }


        if (!shader.isSupported)
        {
            Debug.Log("Shader is not supported");
            _GUIText = "Shader is not supported";
            return false;
        }

        //TODO: More fail-tests?


        refractionTexture = new RenderTexture(128, 128, 16);
        refractionTexture.wrapMode = TextureWrapMode.Clamp;
        refractionTexture.isPowerOfTwo = true;

        reflectionTexture = new RenderTexture(128, 128, 16);
        reflectionTexture.wrapMode = TextureWrapMode.Clamp;
        reflectionTexture.isPowerOfTwo = true;


        //Spawn the camera we'll use for offscreen rendering (refraction/reflection)
        GameObject cam = new GameObject();
        cam.name = "DeepWaterOffscreenCam";
        cam.transform.parent = transform;
        offscreenCam = cam.AddComponent(typeof(Camera)) as Camera;
        offscreenCam.enabled = false;
        offscreenCam.fieldOfView = 60;
        offscreenCam.farClipPlane = 100000;



        //Hack to make this object considered by the renderer - first make a plane
        //covering the watertiles so we get a decent bounding box, then
        //scale all the vertices to 0 to make it invisible.
        //~ gameObject.AddComponent(typeof(MeshRenderer));

        renderer.material.renderQueue = 1;
        renderer.receiveShadows = true;
        renderer.castShadows = true;

        Mesh m = new Mesh();

        Vector3[] verts = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        Vector3[] n = new Vector3[4];
        int[] tris = new int[6];

        float minSizeX = -1024;
        float maxSizeX = 1024;

        float minSizeY = -1024;
        float maxSizeY = 1024;

        verts[0] = new Vector3(minSizeX, 0.0f, maxSizeY);
        verts[1] = new Vector3(maxSizeX, 0.0f, maxSizeY);
        verts[2] = new Vector3(maxSizeX, 0.0f, minSizeY);
        verts[3] = new Vector3(minSizeX, 0.0f, minSizeY);

        tris[0] = 0;
        tris[1] = 1;
        tris[2] = 2;

        tris[3] = 2;
        tris[4] = 3;
        tris[5] = 0;

        m.vertices = verts;
        m.uv = uv;
        m.normals = n;
        m.triangles = tris;


        MeshFilter mfilter = gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;

        if (mfilter == null)
            mfilter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;

        mfilter.mesh = m;

        m.RecalculateBounds();

        //Hopefully the bounds will not be recalculated automatically
        verts[0] = Vector3.zero;
        verts[1] = Vector3.zero;
        verts[2] = Vector3.zero;
        verts[3] = Vector3.zero;

        m.vertices = verts;

        //Create the material and set up the texture references.
        material_ocean = new Material(shader);

        material_ocean.SetTexture("_Reflection", reflectionTexture);
        material_ocean.SetTexture("_Refraction", refractionTexture);
        material_ocean.SetTexture("_Bump", bumpTexture);
        material_ocean.SetTexture("_Fresnel", fresnelTexture);
        material_ocean.SetTexture("_Foam", foamTexture);
        material_ocean.SetTexture("_MainTex", maintexTexture);
        //material.SetTexture ("_Caustics", caustics);
        material_ocean.SetTexture("_Caustics", causticsTexture);
        material_ocean.SetVector("_Size", new Vector4(size.x, size.y, size.z, 0.0f));
        material_ocean.SetColor("_SurfaceColor", surfaceColor);
        material_ocean.SetColor("_WaterColor", waterColor);
        material_ocean.SetColor("_Color", mainColor);
        material_ocean.SetColor("_SpecColor", specularColor);
        material_ocean.SetColor("_SunColor", sunColor);
        if (sunposition)
        {
            material_ocean.SetVector("_SunPosition", sunposition.transform.position);
        }
        //	material_ocean.SetVector("_SunPosition", sunposition.transform.position);
        //	material_ocean.SetFloat("_SunPower", sunpower);
        material_ocean.SetColor("_Transparency", transparency);
        material_ocean.SetFloat("_RefractionLevel", refractionlevel);

        reflectionRefractionEnabled = true;

        return true;

    }

    void OnDisable()
    {
        if (reflectionTexture != null)
            DestroyImmediate(reflectionTexture);

        if (refractionTexture != null)
            DestroyImmediate(refractionTexture);

        reflectionTexture = null;
        refractionTexture = null;
    }
    // Wave dispersion
    float disp(Vector2 vec_k)
    {
        return Mathf.Sqrt(9.81f * vec_k.magnitude);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            Application.CaptureScreenshot("Screenshot.png");

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = width * y + x;
                float yc = y < height / 2 ? y : -height + y;
                float xc = x < width / 2 ? x : -width + x;
                Vector2 vec_k = new Vector2(2.0f * Mathf.PI * xc / size.x, 2.0f * Mathf.PI * yc / size.z);

                float iwkt = disp(vec_k) * Time.time;
                ComplexF coeffA = new ComplexF(Mathf.Cos(iwkt), Mathf.Sin(iwkt));
                ComplexF coeffB = coeffA.GetConjugate();

                int ny = y > 0 ? height - y : 0;
                int nx = x > 0 ? width - x : 0;

                data[idx] = h0[idx] * coeffA + h0[width * ny + nx].GetConjugate() * coeffB;
                t_x[idx] = data[idx] * new ComplexF(0.0f, vec_k.x) - data[idx] * vec_k.y;

                // Choppy wave calcuations
                if (x + y > 0)
                    data[idx] += data[idx] * vec_k.x / vec_k.magnitude;
            }
        }
        material_ocean.SetFloat("_BlendA", Mathf.Cos(Time.time) * 0.1f);
        material_ocean.SetFloat("_BlendB", Mathf.Sin(Time.time) * 0.5f);

        Fourier.FFT2(data, width, height, FourierDirection.Backward);
        Fourier.FFT2(t_x, width, height, FourierDirection.Backward);

        // Get base values for vertices and uv coordinates.
        if (baseHeight == null)
        {
            Mesh mesh = baseMesh;
            baseHeight = mesh.vertices;
            baseUV = mesh.uv;

            int itemCount = baseHeight.Length;
            uvs = new Vector2[itemCount];
            vertices = new Vector3[itemCount];
            normals = new Vector3[itemCount];
            tangents = new Vector4[itemCount];
        }

        //Vector3 vertex;
        Vector3 uv;
        //Vector3 normal;
        float n_scale = size.x / width / scale;

        float scaleA = choppy_scale / (width * height);
        float scaleB = scale / (width * height);
        float scaleBinv = 1.0f / scaleB;

        for (int i = 0; i < width * height; i++)
        {



            int iw = i + i / width;
            vertices[iw] = baseHeight[iw];
            vertices[iw].x += data[i].Im * scaleA;
            vertices[iw].y = data[i].Re * scaleB;

            normals[iw] = new Vector3(t_x[i].Re, scaleBinv, t_x[i].Im).normalized;

            uv = baseUV[iw];
            uv.x = uv.x + Time.time * uv_speed;
            uvs[iw] = uv;


            if (!((i + 1) % width > 0))
            {
                vertices[iw + 1] = baseHeight[iw + 1];
                vertices[iw + 1].x += data[i + 1 - width].Im * scaleA;
                vertices[iw + 1].y = data[i + 1 - width].Re * scaleB;

                normals[iw + 1] = new Vector3(t_x[i + 1 - width].Re, scaleBinv, t_x[i + 1 - width].Im).normalized;

                uv = baseUV[iw + 1];
                uv.x = uv.x + Time.time * uv_speed;
                uvs[iw + 1] = uv;
            }
        }

        int offset = g_width * (g_height - 1);

        for (int i = 0; i < g_width; i++)
        {
            vertices[i + offset] = baseHeight[i + offset];
            vertices[i + offset].x += data[i % width].Im * scaleA;
            vertices[i + offset].y = data[i % width].Re * scaleB;

            normals[i + offset] = new Vector3(t_x[i % width].Re, scaleBinv, t_x[i % width].Im).normalized;

            uv = baseUV[i + offset];
            uv.x = uv.x - Time.time * uv_speed;
            uvs[i + offset] = uv;
        }

        for (int i = 0; i < g_width * g_height - 1; i++)
        {

            //Need to preserve w in refraction/reflection mode
            if (!reflectionRefractionEnabled)
            {
                if (((i + 1) % g_width) == 0)
                {
                    tangents[i] = (vertices[i - width + 1] + new Vector3(size.x, 0.0f, 0.0f) - vertices[i]).normalized;
                }
                else
                {
                    tangents[i] = (vertices[i + 1] - vertices[i]).normalized;
                }

                tangents[i].w = 1.0f;
            }
            else
            {
                Vector3 tmp = Vector3.zero;

                if (((i + 1) % g_width) == 0)
                {
                    tmp = (vertices[i - width + 1] + new Vector3(size.x, 0.0f, 0.0f) - vertices[i]).normalized;
                }
                else
                {
                    tmp = (vertices[i + 1] - vertices[i]).normalized;
                }

                tangents[i] = new Vector4(tmp.x, tmp.y, tmp.z, tangents[i].w);
            }
        }

        //In reflection mode, use tangent w for foam strength
        if (reflectionRefractionEnabled)
        {
            for (int y = 0; y < g_height; y++)
            {
                for (int x = 0; x < g_width; x++)
                {
                    if (x + 1 >= g_width)
                    {
                        tangents[x + g_width * y].w = tangents[g_width * y].w;

                        continue;
                    }

                    if (y + 1 >= g_height)
                    {
                        tangents[x + g_width * y].w = tangents[x].w;

                        continue;
                    }

                    Vector3 right = vertices[(x + 1) + g_width * y] - vertices[x + g_width * y];
                    Vector3 back = vertices[x + g_width * y] - vertices[x + g_width * (y + 1)];

                    float foam = right.x / (size.x / g_width);


                    if (foam < 0.0f)
                        tangents[x + g_width * y].w = 1;
                    else if (foam < 0.5f)
                        tangents[x + g_width * y].w += 2 * Time.deltaTime;
                    else
                        tangents[x + g_width * y].w -= 0.5f * Time.deltaTime;

                    tangents[x + g_width * y].w = Mathf.Clamp(tangents[x + g_width * y].w / foamTime, 0.0f, 2.0f);
                }
            }
        }

        tangents[g_width * g_height - 1] = (vertices[g_width * g_height - 1] + new Vector3(size.x, 0.0f, 0.0f) - vertices[1]).normalized;

        //~ LOD=0;
        for (int LOD = 0; LOD < max_LOD; LOD++)
        {
            int den = (int)Mathf.Pow(2, LOD);
            int itemcount = (height / den + 1) * (width / den + 1);

            Vector4[] tangentsLOD = new Vector4[itemcount];
            Vector3[] verticesLOD = new Vector3[itemcount];
            Vector3[] normalsLOD = new Vector3[itemcount];
            Vector2[] uvLOD = new Vector2[(height / (int)Mathf.Pow(2, LOD) + 1) * (width / (int)Mathf.Pow(2, LOD) + 1)];
            int idx = 0;

            for (int y = 0; y < g_height; y += den)
            {
                for (int x = 0; x < g_width; x += den)
                {
                    int idx2 = g_width * y + x;
                    verticesLOD[idx] = vertices[idx2];
                    uvLOD[idx] = uvs[g_width * y + x];
                    tangentsLOD[idx] = tangents[idx2];
                    normalsLOD[idx++] = normals[idx2];
                }
            }
            for (int k = 0; k < tiles_LOD[LOD].Count; k++)
            {
                Mesh meshLOD = tiles_LOD[LOD][k];
                meshLOD.vertices = verticesLOD;
                meshLOD.normals = normalsLOD;
                meshLOD.uv = uvLOD;
                meshLOD.tangents = tangentsLOD;
            }
        }
        //oceansize=width*(max_LOD*2+1);
        float width_LOD2 = 175 * s;
        //transform.position.x=mycam.transform.localPosition.x;
        float tmpx = Mathf.RoundToInt(mycam.transform.position.x / width_LOD2);
        float tmpz = Mathf.RoundToInt(mycam.transform.position.z / width_LOD2);
        tmpx = tmpx * width_LOD2 - (width_LOD2 / 2);
        tmpz = tmpz * width_LOD2 - (width_LOD2 / 2);
        //tmpx-=Mathf.RoundToInt(width_LOD2);
        //tmpz-=Mathf.RoundToInt(width_LOD2);

        //~ transform.position.x=tmpx;
        //~ transform.position.z=tmpz;

        transform.position = new Vector3(tmpx, transform.position.y, tmpz);

        //transform.position.z=mycam.transform.localPosition.z*1000;
    }

    void OnWillRenderObject()
    //function OnRenderImage()
    {
        //Recursion guard, don't let the offscreen cam go into a never-ending loop.
        if (Camera.current == offscreenCam) return;

        if (reflectionTexture == null
        || refractionTexture == null)
            return;


        material_ocean.SetTexture("_Reflection", reflectionTexture);
        material_ocean.SetTexture("_Refraction", refractionTexture);
        material_ocean.SetTexture("_Bump", bumpTexture);
        material_ocean.SetTexture("_Fresnel", fresnelTexture);
        material_ocean.SetTexture("_Foam", foamTexture);
        material_ocean.SetTexture("_MainTex", maintexTexture);
        material_ocean.SetVector("_Size", new Vector4(size.x, size.y, size.z, 0.0f));
        material_ocean.SetColor("_SurfaceColor", surfaceColor);
        material_ocean.SetColor("_WaterColor", waterColor);
        material_ocean.SetColor("_Color", mainColor);
        material_ocean.SetColor("_SpecColor", specularColor);
        material_ocean.SetColor("_Transparency", transparency);

        if (caustics.Length > 0)
        {
            int index = (int)(Time.time * framesPerSecond) % caustics.Length;
            causticsTexture = caustics[index];
        }
        if (causticsTexture)
        {
            material_ocean.SetTexture("_Caustics", causticsTexture);
        }
        material_ocean.SetFloat("_RefractionLevel", refractionlevel);
        material_ocean.SetColor("_SunColor", sunColor);
        if (sunposition)
        {
            material_ocean.SetVector("_SunPosition", sunposition.transform.position);
        }
        material_ocean.SetFloat("_SunPower", sunpower);

        RenderReflectionAndRefraction();
    }

    void RenderReflectionAndRefraction()
    {
        //return;
        //camera.ResetWorldToCameraMatrix();

        offscreenCam.enabled = true;
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        QualitySettings.pixelLightCount = 0;


        Camera renderCamera = mycam;

        Matrix4x4 originalWorldToCam = renderCamera.worldToCameraMatrix;

        int cullingMask = renderCamera.cullingMask & ~(1 << LayerMask.NameToLayer("Water"));

        //Reflection pass
        Matrix4x4 reflection = Matrix4x4.zero;

        //TODO: Use local plane here, not global!
        CameraHelper.CalculateReflectionMatrix(ref reflection, new Vector4(0.0f, 1.0f, 0.0f, 0.0f));

        offscreenCam.transform.position = reflection.MultiplyPoint(renderCamera.transform.position);
        offscreenCam.transform.rotation = renderCamera.transform.rotation;
        offscreenCam.transform.Rotate(0, 180, 0);
        offscreenCam.worldToCameraMatrix = originalWorldToCam * reflection;

        offscreenCam.cullingMask = cullingMask;
        offscreenCam.targetTexture = reflectionTexture;
        offscreenCam.clearFlags = renderCamera.clearFlags;

        //Need to reverse face culling for reflection pass, since the camera
        //is now flipped upside/down.
        GL.SetRevertBackfacing(false);

        Vector4 cameraSpaceClipPlane = CameraHelper.CameraSpacePlane(offscreenCam, Vector3.zero, Vector3.up, 1.0f);

        Matrix4x4 projection = renderCamera.projectionMatrix;
        Matrix4x4 obliqueProjection = projection;

        CameraHelper.CalculateObliqueMatrix(ref obliqueProjection, cameraSpaceClipPlane);

        //Do the actual render, with the near plane set as the clipping plane. See the
        //pro water source for details.
        offscreenCam.projectionMatrix = obliqueProjection;
        offscreenCam.Render();


        GL.SetRevertBackfacing(false);

        //Refractionpass
        bool fog = RenderSettings.fog;
        Color fogColor = RenderSettings.fogColor;
        float fogDensity = RenderSettings.fogDensity;

        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.grey;
        RenderSettings.fogDensity = waterDirtyness / 10;

        //TODO: If we want to use this as a refraction seen from under the seaplane,
        //      the cameraclear should be skybox.
        offscreenCam.clearFlags = CameraClearFlags.Color;
        offscreenCam.backgroundColor = Color.grey;

        offscreenCam.targetTexture = refractionTexture;
        obliqueProjection = projection;

        offscreenCam.transform.position = renderCamera.transform.position;
        offscreenCam.transform.rotation = renderCamera.transform.rotation;
        offscreenCam.worldToCameraMatrix = originalWorldToCam;

        cameraSpaceClipPlane = CameraHelper.CameraSpacePlane(offscreenCam, Vector3.zero, Vector3.up, -1.0f);
        CameraHelper.CalculateObliqueMatrix(ref obliqueProjection, cameraSpaceClipPlane);
        offscreenCam.projectionMatrix = obliqueProjection;

        offscreenCam.Render();

        RenderSettings.fog = fog;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        offscreenCam.projectionMatrix = projection;


        offscreenCam.targetTexture = null;

        QualitySettings.pixelLightCount = oldPixelLightCount;

        //	camera.ResetWorldToCameraMatrix();

        //GL.SetRevertBackfacing (true);
    }

    void OnGUI()
    {
        GUILayout.Label(_GUIText);
    }
}
