using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class BatchIconRenderer : EditorWindow
{
    [MenuItem("Tools/Render Icons")]
    static void RenderIcons()
    {
        List<string> inputPaths = new List<string>
        {
            "Assets/_AssetPacks/Fruit Market/Prefabs",
            "Assets/_AssetPacks/Cute Supermarket Pack/Prefabs"
        };

        string outputBaseFolder = "Assets/_Custom/Images/Icons";
        Directory.CreateDirectory(outputBaseFolder);

        // Create camera with top-down isometric view
        GameObject cameraObj = new GameObject("IconCam");
        Camera cam = cameraObj.AddComponent<Camera>();
        cam.backgroundColor = Color.clear;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.orthographic = false;
        cam.fieldOfView = 40f;

        Light light = new GameObject("IconLight").AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(45f, 45f, 0f);
        light.intensity = 1.5f;

        foreach (string inputPath in inputPaths)
        {
            if (!Directory.Exists(inputPath))
            {
                Debug.LogWarning($"Folder not found: {inputPath}");
                continue;
            }

            ProcessFolder(inputPath, outputBaseFolder, cam);
        }

        DestroyImmediate(light.gameObject);
        DestroyImmediate(cam.gameObject);
        AssetDatabase.Refresh();
        Debug.Log("Done rendering icons.");
    }

    static void ProcessFolder(string folderPath, string outputBasePath, Camera cam)
    {
        // Get relative path for output folder structure
        string relativePath = folderPath.Replace("Assets/_AssetPacks/", "").Replace("/Prefabs", "");
        string outputFolder = Path.Combine(outputBasePath, relativePath);
        Directory.CreateDirectory(outputFolder);

        // Process .prefab files in current folder
        foreach (string file in Directory.GetFiles(folderPath))
        {
            if (file.EndsWith(".prefab"))
            {
                RenderPrefabIcon(file, outputFolder, cam);
            }
        }

        // Recursively process subfolders
        foreach (string subfolder in Directory.GetDirectories(folderPath))
        {
            ProcessFolder(subfolder, outputBasePath, cam);
        }
    }

    static void RenderPrefabIcon(string prefabPath, string outputFolder, Camera cam)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (model == null)
        {
            Debug.LogWarning($"Failed to load: {prefabPath}");
            return;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(model) as GameObject;
        if (instance == null)
        {
            Debug.LogWarning($"Failed to instantiate: {prefabPath}");
            return;
        }

        try
        {
            // Calculate bounds
            Bounds bounds = new Bounds(instance.transform.position, Vector3.zero);
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();

            if (renderers.Length == 0)
            {
                Debug.LogWarning($"No renderers found in: {prefabPath}");
                DestroyImmediate(instance);
                return;
            }

            foreach (Renderer r in renderers)
                bounds.Encapsulate(r.bounds);

            // Standard Unity preview angle (front-right isometric)
            float size = bounds.extents.magnitude;
            Vector3 cameraDistance = new Vector3(size, size, size) * 2.0f;
            cam.transform.position = bounds.center + cameraDistance;
            cam.transform.LookAt(bounds.center, Vector3.up);

            // Render to texture
            RenderTexture rt = new RenderTexture(256, 256, 24);
            cam.targetTexture = rt;
            cam.Render();

            Texture2D tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            RenderTexture.active = rt;
            tex.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
            tex.Apply();

            string outputPath = Path.Combine(outputFolder, model.name + ".png");
            byte[] pngData = tex.EncodeToPNG();
            File.WriteAllBytes(outputPath, pngData);

            Debug.Log($"Rendered icon: {outputPath}");

            // Cleanup
            RenderTexture.active = null;
            DestroyImmediate(rt);
            DestroyImmediate(tex);
        }
        finally
        {
            DestroyImmediate(instance);
        }
    }
}
