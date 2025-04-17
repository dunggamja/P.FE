using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class GridTileGenerator : EditorWindow
{
    private Material tileMaterial;
    private int gridWidth = 10;
    private int gridHeight = 10;
    private float tileSize = 1f;
    private string parentName = "GridTiles";
    private Vector3 startPosition = Vector3.zero;
    private Color tileColor1 = new Color(0.8f, 0.8f, 0.8f);
    private Color tileColor2 = new Color(0.6f, 0.6f, 0.6f);
    private int textureSize = 256;

    private const string PATH_ROOT = "Assets/ResourcesPrivate/terrain/test";

    [MenuItem("Tools/Grid Tile Generator")]
    public static void ShowWindow()
    {
        GetWindow<GridTileGenerator>("Grid Tile Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Tile Generator", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        GUILayout.Label("Texture Settings", EditorStyles.boldLabel);
        tileColor1 = EditorGUILayout.ColorField("Tile Color 1", tileColor1);
        tileColor2 = EditorGUILayout.ColorField("Tile Color 2", tileColor2);
        textureSize = EditorGUILayout.IntField("Texture Size", textureSize);

        if (GUILayout.Button("Generate Checker Texture"))
        {
            GenerateCheckerTexture();
        }

        EditorGUILayout.Space();
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        tileMaterial = (Material)EditorGUILayout.ObjectField("Tile Material", tileMaterial, typeof(Material), false);
        gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);
        tileSize = EditorGUILayout.FloatField("Tile Size", tileSize);
        parentName = EditorGUILayout.TextField("Parent Name", parentName);
        startPosition = EditorGUILayout.Vector3Field("Start Position", startPosition);

        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid();
        }
    }

    private void GenerateCheckerTexture()
    {
        // 텍스처 생성
        Texture2D texture = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[textureSize * textureSize];

        int tileSize = textureSize / 8; // 8x8 체크무늬

        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                int tileX = x / tileSize;
                int tileY = y / tileSize;
                bool isEven = (tileX + tileY) % 2 == 0;
                pixels[y * textureSize + x] = isEven ? tileColor1 : tileColor2;
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();

        // 텍스처 저장
        string path = $"{PATH_ROOT}/texture";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string texturePath = $"{path}/GridCheckerTexture.png";
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(texturePath, bytes);
        AssetDatabase.Refresh();

        // 머티리얼 생성
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Texture2D savedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
        material.mainTexture = savedTexture;
        material.enableInstancing = true;
        material.mainTextureScale = new Vector2(1, 1); // 텍스처 스케일 초기화

        string materialPath = $"{path}/GridMaterial.mat";
        AssetDatabase.CreateAsset(material, materialPath);
        AssetDatabase.SaveAssets();

        tileMaterial = material;

        Debug.Log($"Checker texture and material generated at {path}");
    }

    private void GenerateGrid()
    {
        if (tileMaterial == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign a material!", "OK");
            return;
        }

        // 기존 그리드 삭제
        GameObject existingGrid = GameObject.Find(parentName);
        if (existingGrid != null)
        {
            DestroyImmediate(existingGrid);
        }

        // 새로운 부모 오브젝트 생성
        GameObject parent = new GameObject(parentName);
        parent.transform.position = startPosition;

        // 메시 생성
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();

        // 각 타일의 정점과 삼각형 생성
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                float xPos = startPosition.x + x * tileSize;
                float zPos = startPosition.z + z * tileSize;

                // 현재 타일의 시작 정점 인덱스
                int vertexIndex = vertices.Count;

                // 정점 추가
                vertices.Add(new Vector3(xPos, startPosition.y, zPos));
                vertices.Add(new Vector3(xPos + tileSize, startPosition.y, zPos));
                vertices.Add(new Vector3(xPos + tileSize, startPosition.y, zPos + tileSize));
                vertices.Add(new Vector3(xPos, startPosition.y, zPos + tileSize));

                // 법선 추가 (위로 향하게)
                for (int i = 0; i < 4; i++)
                {
                    normals.Add(Vector3.up);
                }

                // 삼각형 추가 (시계 방향)
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 3);
                triangles.Add(vertexIndex + 2);

                // UV 좌표 추가 (타일 크기에 맞게 조정)
                float uvScale = 1f / tileSize; // 타일 크기에 반비례하도록 조정
                uvs.Add(new Vector2(xPos * uvScale, zPos * uvScale));
                uvs.Add(new Vector2((xPos + tileSize) * uvScale, zPos * uvScale));
                uvs.Add(new Vector2((xPos + tileSize) * uvScale, (zPos + tileSize) * uvScale));
                uvs.Add(new Vector2(xPos * uvScale, (zPos + tileSize) * uvScale));
            }
        }

        // 메시 설정
        mesh.vertices  = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv        = uvs.ToArray();
        mesh.normals   = normals.ToArray();
        mesh.RecalculateBounds();

        // 메시 필터와 렌더러 추가
        MeshFilter meshFilter          = parent.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer      = parent.AddComponent<MeshRenderer>();

        meshFilter.mesh                = mesh;
        meshRenderer.material          = tileMaterial;
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        meshRenderer.receiveShadows    = true;

        Debug.Log($"Grid generated with {gridWidth}x{gridHeight} tiles using a single mesh");
    }
} 