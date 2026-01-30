using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;

public class CharacterProfileRenderer : EditorWindow
{
    [MenuItem("Tools/Render Character Profiles (Use Camera Transform)")]
    public static void RenderProfilesWithTransform()
    {
        // Camera transform from user
        Vector3 camPosition = new Vector3(1.0f, 1.2f, 2.0f);
        Vector3 camEuler = new Vector3(335.914f, 202.082f, 0.0f);
        Quaternion camRotation = Quaternion.Euler(camEuler);
        float camFov = 60f;
        bool camOrtho = false;
        Vector3 lookTarget = new Vector3(-1.78814E-07f, 1.5f, 0.106645f);

        string inputPath = "Assets/_AssetPacks/KayKit/Characters/KayKit - Adventurers (for Unity)/Prefabs/Characters";
        string clipFolder = "Assets/_AssetPacks/KayKit/Characters/Animations/Animations/Rig_Large/General";
        string outputPath = "Assets/_Custom/Images/Profiles_CameraTransform";
        Directory.CreateDirectory(outputPath);

        string[] guids = AssetDatabase.FindAssets("idle_A t:AnimationClip", new[] { clipFolder });
        AnimationClip idleClip = null;
        if (guids.Length > 0)
        {
            string clipPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        }

        if (idleClip == null)
        {
            Debug.LogError("idle_A AnimationClip not found in " + clipFolder);
            return;
        }

        GameObject camObj = new GameObject("ProfileCam_Transform");
        Camera cam = camObj.AddComponent<Camera>();
        cam.backgroundColor = Color.clear;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.orthographic = camOrtho;
        cam.fieldOfView = camFov;
        cam.transform.position = camPosition;
        cam.transform.rotation = camRotation;

        GameObject lightObj = new GameObject("ProfileLight_Transform");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50f, 30f, 0f);
        light.intensity = 1.4f;

        if (!Directory.Exists(inputPath))
        {
            Debug.LogError("Characters folder not found: " + inputPath);
            DestroyImmediate(camObj);
            DestroyImmediate(lightObj);
            return;
        }

        string[] files = Directory.GetFiles(inputPath, "*.prefab", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(file);
            if (model == null) continue;

            GameObject instance = PrefabUtility.InstantiatePrefab(model) as GameObject;
            if (instance == null) continue;

            try
            {
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;

                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>();
                if (renderers == null || renderers.Length == 0)
                {
                    Debug.LogWarning("No renderers in prefab: " + file);
                    DestroyImmediate(instance);
                    continue;
                }

                // Optionally make camera look at provided target
                cam.transform.position = camPosition;
                cam.transform.rotation = camRotation;
                cam.fieldOfView = camFov;
                cam.orthographic = camOrtho;
                cam.transform.LookAt(lookTarget, Vector3.up);

                AnimationMode.StartAnimationMode();
                float sampleTime = Mathf.Clamp(idleClip.length * 0.5f, 0f, idleClip.length);
                AnimationMode.SampleAnimationClip(instance, idleClip, sampleTime);

                int res = 512;
                RenderTexture rt = new RenderTexture(res, res, 24);
                cam.targetTexture = rt;
                cam.Render();

                Texture2D tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
                RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, res, res), 0, 0);
                tex.Apply();

                string relative = file.Replace(inputPath, "").TrimStart('\\', '/');
                string folder = Path.GetDirectoryName(relative);
                string outFolder = Path.Combine(outputPath, folder ?? "");
                Directory.CreateDirectory(outFolder);

                string outPath = Path.Combine(outFolder, model.name + ".png");
                File.WriteAllBytes(outPath, tex.EncodeToPNG());

                RenderTexture.active = null;
                cam.targetTexture = null;
                DestroyImmediate(rt);
                DestroyImmediate(tex);

                AnimationMode.StopAnimationMode();

                Debug.Log("Rendered profile with transform: " + outPath);
            }
            finally
            {
                DestroyImmediate(instance);
            }
        }

        DestroyImmediate(camObj);
        DestroyImmediate(lightObj);
        AssetDatabase.Refresh();
        Debug.Log("Character profile rendering with provided transform complete.");
    }
}
