using UnityEngine;
using UnityEditor;

public class URPTexturePacker : EditorWindow
{
    [SerializeField] private Texture2D metallicTexture;
    [SerializeField] private Texture2D roughnessTexture;

    [MenuItem("Tools/URP Texture Packer")]
    public static void ShowWindow()
    {
        GetWindow<URPTexturePacker>("URP Texture Packer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Metallic + Roughness ? Unity Packed Texture", EditorStyles.boldLabel);

        metallicTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Metallic Texture",
            metallicTexture,
            typeof(Texture2D),
            false
        );

        roughnessTexture = (Texture2D)EditorGUILayout.ObjectField(
            "Roughness Texture",
            roughnessTexture,
            typeof(Texture2D),
            false
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Pack Textures"))
        {
            PackTextures();
        }
    }

    private bool EnsureReadable(Texture2D texture)
    {
        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

        if (!importer.isReadable)
        {
            importer.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            return true;
        }

        return false;
    }

    private void PackTextures()
    {
        if (metallicTexture == null || roughnessTexture == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign both textures.", "OK");
            return;
        }

        if (metallicTexture.width != roughnessTexture.width ||
            metallicTexture.height != roughnessTexture.height)
        {
            EditorUtility.DisplayDialog("Error", "Textures must be the same size.", "OK");
            return;
        }

        bool metallicWasModified = EnsureReadable(metallicTexture);
        bool roughnessWasModified = EnsureReadable(roughnessTexture);

        int width = metallicTexture.width;
        int height = metallicTexture.height;

        Texture2D packedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        Color[] metallicPixels = metallicTexture.GetPixels();
        Color[] roughnessPixels = roughnessTexture.GetPixels();

        Color[] packedPixels = new Color[metallicPixels.Length];

        for (int i = 0; i < metallicPixels.Length; i++)
        {
            Color m = metallicPixels[i];
            float rough = roughnessPixels[i].r;

            float smoothness = 1.0f - rough;

            packedPixels[i] = new Color(m.r, m.g, m.b, smoothness);
        }

        packedTexture.SetPixels(packedPixels);
        packedTexture.Apply();

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Packed Texture",
            "MetallicSmoothness",
            "png",
            "Choose location to save the packed texture"
        );

        if (!string.IsNullOrEmpty(path))
        {
            byte[] pngData = packedTexture.EncodeToPNG();
            System.IO.File.WriteAllBytes(path, pngData);

            AssetDatabase.Refresh();

            // Set correct import settings automatically
            TextureImporter newImporter = (TextureImporter)AssetImporter.GetAtPath(path);
            newImporter.sRGBTexture = false;
            newImporter.alphaSource = TextureImporterAlphaSource.FromInput;
            newImporter.isReadable = false;
            newImporter.SaveAndReimport();

            EditorUtility.DisplayDialog("Success", "Texture packed successfully!", "OK");
        }

        // Restore original readability
        RestoreReadable(metallicTexture, metallicWasModified);
        RestoreReadable(roughnessTexture, roughnessWasModified);
    }

    private void RestoreReadable(Texture2D texture, bool wasModified)
    {
        if (!wasModified) return;

        string path = AssetDatabase.GetAssetPath(texture);
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

        importer.isReadable = false;
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }
}
