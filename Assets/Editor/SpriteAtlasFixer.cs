using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteAtlasFixer : MonoBehaviour
{
    [MenuItem("Tools/Fix All Sprites For Atlas")]
    public static void FixAll()
    {
        // 1. Находим все текстуры
        string[] guids = AssetDatabase.FindAssets("t:Texture2D");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Только png в папке Sprite
            if (!path.Contains("/Sprite/")) continue;

            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null) continue;

            // Меняем настройки
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;

            // Убираем компрессию
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;

            // Point filter для пиксель-арта
            importer.filterMode = FilterMode.Point;

            // Сохраняем
            importer.SaveAndReimport();
        }

        // 2. Чистим SpriteAtlas и создаём заново
        string atlasPath = "Assets/Atlas/MainAtlas.spriteatlas";
        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);

        if (atlas == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Atlas"))
                AssetDatabase.CreateFolder("Assets", "Atlas");

            atlas = new SpriteAtlas();
            AssetDatabase.CreateAsset(atlas, atlasPath);
        }

        // Очищаем
        atlas.Remove(atlas.GetPackables());

        // Добавляем папку со спрайтами
        Object folder = AssetDatabase.LoadAssetAtPath<Object>("Assets/Source/Prefab/Cafe1.1/Sprite");
        if (folder != null)
        {
            atlas.Add(new Object[] { folder });
        }

        EditorUtility.SetDirty(atlas);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Готово! Все текстуры переведены в uncompressed, атлас обновлён.");
    }
}