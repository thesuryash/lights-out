// StripBakedLighting.cs
// Put this anywhere in an "Editor" folder if you want the menu item to appear.
// The MonoBehaviour itself can live in any folder.

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;

public class StripBakedLighting : MonoBehaviour
{
    [Tooltip("Leave empty to use this GameObject as the root. " +
             "Or drop in a prefab/model root to process that instead.")]
    public GameObject rootModel;

    // Right-click on the component header → Strip Baked Lighting
    [ContextMenu("Strip Baked Lighting")]
    public void StripNow()
    {
        GameObject root = rootModel != null ? rootModel : gameObject;
        if (root == null)
        {
            Debug.LogWarning("StripBakedLighting: No root model set and no GameObject found.");
            return;
        }

        int rendererCount = 0;
        int materialCount = 0;
        int meshesWithVertexColors = 0;

        var renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            rendererCount++;

            // Clear baked lightmap linkage
            r.lightmapIndex = -1;
            r.lightmapScaleOffset = Vector4.zero;

#if UNITY_2020_1_OR_NEWER
            // Prefer probes instead of baked GI if available
            try
            {
                var prop = r.GetType().GetProperty("receiveGI");
                if (prop != null)
                {
                    // Renderer.receiveGI is an enum in newer Unity versions
                    // 0 = Lightmap, 1 = LightProbes (check Unity docs if version differs)
                    prop.SetValue(r, System.Enum.Parse(prop.PropertyType, "LightProbes"), null);
                }
            }
            catch { /* ignore reflection failures */ }
#endif

            // Process materials
            var mats = r.sharedMaterials;
            foreach (var m in mats)
            {
                if (m == null) continue;
                materialCount++;

                // Turn off emission
                m.DisableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", Color.black);
                if (m.HasProperty("_EmissionMap"))
                    m.SetTexture("_EmissionMap", null);

                // Clear common "baked" texture slots (if present)
                if (m.HasProperty("_LightMap")) m.SetTexture("_LightMap", null);
                if (m.HasProperty("_ShadowMap")) m.SetTexture("_ShadowMap", null);
                if (m.HasProperty("_OcclusionMap")) m.SetTexture("_OcclusionMap", null);
                if (m.HasProperty("_DetailMask")) m.SetTexture("_DetailMask", null);

                // Disable GI flags so it doesn't try to use baked data
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            }

            // Check for vertex colors (common place for baked lighting)
            var mf = r.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                var mesh = mf.sharedMesh;
                if ((mesh.colors != null && mesh.colors.Length > 0) ||
                    (mesh.colors32 != null && mesh.colors32.Length > 0))
                {
                    meshesWithVertexColors++;
                }
            }

            var smr = r as SkinnedMeshRenderer;
            if (smr != null && smr.sharedMesh != null)
            {
                var mesh = smr.sharedMesh;
                if ((mesh.colors != null && mesh.colors.Length > 0) ||
                    (mesh.colors32 != null && mesh.colors32.Length > 0))
                {
                    meshesWithVertexColors++;
                }
            }
        }

        Debug.Log(
            $"[StripBakedLighting] Done on '{root.name}'. " +
            $"Renderers: {rendererCount}, Materials touched: {materialCount}, " +
            $"Meshes with vertex colors (likely baked lighting in colors): {meshesWithVertexColors}."
        );

        if (meshesWithVertexColors > 0)
        {
            Debug.LogWarning(
                "[StripBakedLighting] Detected vertex colors on some meshes. " +
                "If lighting still looks baked after this, it's probably baked into vertex colors or albedo textures. " +
                "Use a shader that ignores vertex colors and/or replace albedo textures."
            );
        }
    }

#if UNITY_EDITOR
    // Optional: Menu item to run on the currently selected GameObject
    [MenuItem("Tools/Lighting/Strip Baked Lighting From Selected")]
    private static void StripFromSelected()
    {
        var selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("StripBakedLighting: No GameObject selected.");
            return;
        }

        // Use a temporary component so we can reuse StripNow logic
        var temp = selected.AddComponent<StripBakedLighting>();
        temp.rootModel = selected;
        temp.StripNow();
        DestroyImmediate(temp);
    }
#endif
}
