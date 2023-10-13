using UnityEngine;
using UnityEditor;
using System.IO;

public class SpriteGeneratorEditor : EditorWindow
{
    private string spriteName = "NewSprite";
    private int width = 64;
    private int height = 64;
    private Color lowColor = Color.blue;
    private Color baseColor = Color.white;
    private Color highColor = Color.red;
    private Texture2D previewTexture;
    private int seed;
    private float noiseScale = 0.1f;
    private float threshold = 0.5f;

    [MenuItem("Tools/Sprite Generator")]
    public static void ShowWindow()
    {
        GetWindow<SpriteGeneratorEditor>("Sprite Generator");
    }

  private void OnGUI()
{
    EditorGUILayout.LabelField("Sprite Generator", EditorStyles.boldLabel);
    GUILayout.Space(10);

    spriteName = EditorGUILayout.TextField("Name", spriteName);
    width = EditorGUILayout.IntField("Width", width);
    height = EditorGUILayout.IntField("Height", height);

    EditorGUI.BeginChangeCheck(); // Start detecting changes

    lowColor = EditorGUILayout.ColorField("Low Color", lowColor);
    baseColor = EditorGUILayout.ColorField("Base Color", baseColor);
    highColor = EditorGUILayout.ColorField("High Color", highColor);
    noiseScale = EditorGUILayout.Slider("Noise Scale", noiseScale, 0.01f, 1f);
    threshold = EditorGUILayout.Slider("Threshold", threshold, 0f, 1f);

    if (EditorGUI.EndChangeCheck()) // If changes detected, redraw preview
    {
        GeneratePreviewWithoutNewSeed();
    }

    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace(); // This centers the following GUI elements horizontally

    // Create a box with the previewTexture as background
    GUIStyle previewStyle = new GUIStyle();
    if (previewTexture != null)
    {
        previewStyle.normal.background = previewTexture;
    }

    GUILayout.Box("", previewStyle, GUILayout.Width(256), GUILayout.Height(256));

    GUILayout.FlexibleSpace(); // This will keep it centered by pushing equally from both sides
    GUILayout.EndHorizontal();
    GUILayout.FlexibleSpace();
    GUILayout.BeginHorizontal();
    GUILayout.FlexibleSpace();
    
    if (GUILayout.Button("Preview", GUILayout.Width(100)))
    {
        UpdateSeed();
        GeneratePreviewWithoutNewSeed();
    }
    if (GUILayout.Button("Save", GUILayout.Width(100)))
    {
        SaveSprite();
    }
    
    GUILayout.FlexibleSpace();
    GUILayout.EndHorizontal();
    GUILayout.Space(10);
}

    private void UpdateSeed()
    {
        seed = Random.Range(0, 99999);
    }

    private void GeneratePreviewWithoutNewSeed()
    {
        previewTexture = new Texture2D(width, height);

        for (int y = 0; y < previewTexture.height; y++)
        {
            for (int x = 0; x < previewTexture.width; x++)
            {
                float p = Mathf.PerlinNoise(x * noiseScale + seed, y * noiseScale + seed);
                Color interpolatedColor = p < threshold ? Color.Lerp(lowColor, baseColor, p / threshold) : Color.Lerp(baseColor, highColor, (p - threshold) / (1f - threshold));
                previewTexture.SetPixel(x, y, interpolatedColor);
            }
        }

        previewTexture.Apply();
        Repaint();
    }

    private void SaveSprite()
    {
        string path = "Assets/GeneratedSprites";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string assetPath = $"{path}/{spriteName}.png";
        int count = 1;
        while (File.Exists(assetPath))
        {
            assetPath = $"{path}/{spriteName}_{count}.png";
            count++;
        }

        byte[] bytes = previewTexture.EncodeToPNG();
        File.WriteAllBytes(assetPath, bytes);
        AssetDatabase.Refresh();
    }
}
