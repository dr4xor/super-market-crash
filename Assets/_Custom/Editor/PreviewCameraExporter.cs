using UnityEngine;
using UnityEditor;
using System.IO;

public class PreviewCameraExporter : EditorWindow
{
    [MenuItem("Tools/Preview Camera Exporter")]
    public static void ShowWindow()
    {
        GetWindow<PreviewCameraExporter>("Preview Camera Exporter");
    }

    void OnGUI()
    {
        GUILayout.Label("Active Preview Camera", EditorStyles.boldLabel);

        Camera cam = GetActiveCamera();
        if (cam == null)
        {
            EditorGUILayout.HelpBox("No active SceneView or camera found. Open a SceneView or Prefab Stage.", MessageType.Warning);
            if (GUILayout.Button("Refresh")) Repaint();
            return;
        }

        Vector3 pos = cam.transform.position;
        Vector3 euler = cam.transform.rotation.eulerAngles;
        float fov = cam.fieldOfView;
        bool ortho = cam.orthographic;

        EditorGUILayout.Vector3Field("Position", pos);
        EditorGUILayout.Vector3Field("Rotation (Euler)", euler);
        EditorGUILayout.FloatField("Field of View", fov);
        EditorGUILayout.Toggle("Orthographic", ortho);

        GUILayout.Space(8);

        // Compute target bounds center if selection is a prefab or selected in Hierarchy
        Vector3 target = Vector3.zero;
        string targetInfo = "(none)";
        GameObject selected = Selection.activeGameObject;
        if (selected != null)
        {
            Bounds b;
            if (TryGetRenderBounds(selected, out b))
            {
                target = b.center;
                targetInfo = "Selection center";
            }
        }
        else
        {
            // Try project selection (asset)
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (!string.IsNullOrEmpty(path))
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (asset != null)
                {
                    GameObject instance = PrefabUtility.InstantiatePrefab(asset) as GameObject;
                    if (instance != null)
                    {
                        Bounds b;
                        if (TryGetRenderBounds(instance, out b))
                        {
                            target = b.center;
                            targetInfo = "Asset prefab center";
                        }
                        DestroyImmediate(instance);
                    }
                }
            }
        }

        EditorGUILayout.Vector3Field("Target (look at)", target);
        EditorGUILayout.LabelField("Target Info", targetInfo);

        GUILayout.Space(6);
        if (GUILayout.Button("Copy Camera Transform to Clipboard"))
        {
            string snippet = BuildSnippet(pos, euler, fov, ortho, target);
            GUIUtility.systemCopyBuffer = snippet;
            Debug.Log("Camera transform copied to clipboard.");
        }

        if (GUILayout.Button("Refresh")) Repaint();
    }

    static Camera GetActiveCamera()
    {
        // Prefer the last active SceneView camera (works in Prefab Mode and Scene)
        if (SceneView.lastActiveSceneView != null && SceneView.lastActiveSceneView.camera != null)
            return SceneView.lastActiveSceneView.camera;

        // Fall back to Camera.current or main
        if (Camera.current != null) return Camera.current;
        if (Camera.main != null) return Camera.main;
        return null;
    }

    static bool TryGetRenderBounds(GameObject go, out Bounds bounds)
    {
        bounds = new Bounds(go.transform.position, Vector3.zero);
        Renderer[] rs = go.GetComponentsInChildren<Renderer>();
        if (rs == null || rs.Length == 0) return false;
        bounds = rs[0].bounds;
        foreach (var r in rs) bounds.Encapsulate(r.bounds);
        return true;
    }

    static string BuildSnippet(Vector3 pos, Vector3 euler, float fov, bool ortho, Vector3 target)
    {
        string nl = System.Environment.NewLine;
        string s = "// Camera transform for prefab preview" + nl;
        s += "Vector3 camPosition = new Vector3(" + FloatStr(pos.x) + "f, " + FloatStr(pos.y) + "f, " + FloatStr(pos.z) + "f);" + nl;
        s += "Vector3 camEuler = new Vector3(" + FloatStr(euler.x) + "f, " + FloatStr(euler.y) + "f, " + FloatStr(euler.z) + "f);" + nl;
        s += "Quaternion camRotation = Quaternion.Euler(camEuler);" + nl;
        s += "float camFov = " + FloatStr(fov) + "f;" + nl;
        s += "bool camOrtho = " + (ortho ? "true" : "false") + ";" + nl;
        s += nl;
        s += "// Apply to a camera instance:" + nl;
        s += "// camera.transform.position = camPosition;" + nl;
        s += "// camera.transform.rotation = camRotation;" + nl;
        s += "// camera.fieldOfView = camFov;" + nl;
        s += "// camera.orthographic = camOrtho;" + nl;
        s += nl;
        s += "// Optional: look at target" + nl;
        s += "Vector3 lookTarget = new Vector3(" + FloatStr(target.x) + "f, " + FloatStr(target.y) + "f, " + FloatStr(target.z) + "f);" + nl;
        s += "// camera.transform.LookAt(lookTarget);" + nl;
        return s;
    }

    static string FloatStr(float v)
    {
        return v.ToString("G6");
    }
}
