using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.SceneManagement;
using UnityEngine;
//using static UnityEditor.PlayerSettings;
using UnityEngine.SocialPlatforms;

public class FogOfWarManager : MonoBehaviour
{
    [HideInInspector]
    public Texture2D fogOfWar;
    public Vector2Int size = new Vector2Int(16, 16);

    public Material fogOfWarMaterial;

    private static FogOfWarManager _instance;
    public static FogOfWarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("FogOfWarManager is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        size = new Vector2Int(MapManager.Instance.map.tiles.GetLength(0), MapManager.Instance.map.tiles.GetLength(1));

        PrepareFogOfWar();

        /* Shader testing values
        fogOfWar.SetPixel(1, 1, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(1, 2, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(1, 3, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(1, 4, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(2, 1, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(2, 2, new Color(0, 0, 0, 1));
        fogOfWar.SetPixel(2, 3, new Color(0, 0, 0, 1));
        fogOfWar.SetPixel(2, 4, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(3, 1, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(3, 2, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(3, 3, new Color(0, 255, 0, 1));
        fogOfWar.SetPixel(3, 4, new Color(0, 255, 0, 1));*/
        GenerateFogMap();
        HideObjectsBehindFog();

        fogOfWarMaterial.SetInt("FogSizeX", size.x);
        fogOfWarMaterial.SetInt("FogSizeY", size.y);

        MapManager.Instance.onUnitMoved += () => GenerateFogMap();
        GameManager.onTurnEnded += () => GenerateFogMap();
        MapManager.Instance.onUnitMoved += () => HideObjectsBehindFog();
        GameManager.onTurnEnded += () => HideObjectsBehindFog();
    }

    public void PrepareFogOfWar()
    {
        //making the mesh
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[]{ new Vector3(-0.5f, 0.5f, 0), new Vector3(size.x - 0.5f, 0.5f, 0), new Vector3(size.x - 0.5f, -size.y + 0.5f, 0), new Vector3(-0.5f, -size.y + 0.5f, 0) };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
        mesh.triangles = new int[] {0, 1, 3, 1, 2, 3};

        //adding the object and assigning the mesh
        GameObject fogDisplay = new GameObject();
        fogDisplay.transform.position = new Vector3(0, 0, -0.75f);

        MeshFilter mf = fogDisplay.AddComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshRenderer mr = fogDisplay.AddComponent<MeshRenderer>();
        mr.material = fogOfWarMaterial;
    }

    public void GenerateFogMap()
    {
        fogOfWar = new Texture2D(size.x, size.y);
        fogOfWar.wrapMode = TextureWrapMode.Clamp;

        //Making seen tiles grey 

        for (int x = 0; x < fogOfWar.width; x++)
        {
            for (int y = 0; y < fogOfWar.height; y++)
            {
                if (GameManager.Instance.GetCurrentTeamObj().visitedTiles[x, y]) fogOfWar.SetPixel(x, y, new Color(0, 1, 0, 1));
            }
        }

        //Clearing the fog around player's units

        foreach (KeyValuePair<Vector2Int, Building> kvp in MapManager.Instance.map.getAllBuildings())
        {
            if (kvp.Value.teamID != GameManager.Instance.currentTeam) continue;
            _ClearFogAroundMapObject(kvp.Value);
        }

        foreach (KeyValuePair<Vector2Int, Unit> kvp in MapManager.Instance.map.getAllUnits())
        {
            if (kvp.Value.teamID != GameManager.Instance.currentTeam) continue;
            _ClearFogAroundMapObject(kvp.Value);
        }

        fogOfWar.Apply();

        fogOfWarMaterial.SetTexture("_FogAmountTex", fogOfWar);
    }

    public void HideObjectsBehindFog()
    {
        foreach (KeyValuePair<Vector2Int, SpriteRenderer> kvp in MapManager.Instance.GetAllBuildingRenderers())
        {
            if (fogOfWar.GetPixel(kvp.Key.x, kvp.Key.y).r < 0.5)
                kvp.Value.gameObject.SetActive(true);
            else
                kvp.Value.gameObject.SetActive(false);
        }

        foreach (KeyValuePair<Vector2Int, SpriteRenderer> kvp in MapManager.Instance.GetAllUnitRenderers())
        {
            if (fogOfWar.GetPixel(kvp.Key.x, kvp.Key.y).g < 0.5)
                kvp.Value.gameObject.SetActive(true);
            else
                kvp.Value.gameObject.SetActive(false);
        }
    }

    private void _ClearFogAroundMapObject(MapObject mapObject)
    {
        int range = mapObject.GetVisionRange();
        Vector2Int pos = mapObject.position;

        for (int x = pos.x - range; x <= pos.x + range; x++)
        {
            for (int y = pos.y - range; y <= pos.y + range; y++)
            {
                if (x >= 0 && x < fogOfWar.width && y >= 0 && y < fogOfWar.height && Math.Abs(x - pos.x) + Math.Abs(y - pos.y) <= range)
                {
                    fogOfWar.SetPixel(x, y, new Color(0, 0, 0, 1));
                    GameManager.Instance.GetCurrentTeamObj().visitedTiles[x, y] = true;
                }
            }
        }
    }
}
