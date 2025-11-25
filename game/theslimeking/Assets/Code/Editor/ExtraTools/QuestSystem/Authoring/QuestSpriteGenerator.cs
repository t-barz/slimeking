using UnityEngine;
using UnityEditor;
using System.IO;

namespace ExtraTools.QuestSystem
{
    /// <summary>
    /// Editor tool para gerar sprites de indicadores de quest.
    /// </summary>
    public class QuestSpriteGenerator : EditorWindow
    {
        [MenuItem("Extra Tools/Quest System/Authoring/🎨 Generate UI Sprites")]
        public static void GenerateQuestSprites()
        {
            string spritePath = "Assets/Art/Sprites/UI";

            // Garante que diretório existe
            if (!Directory.Exists(spritePath))
            {
                Directory.CreateDirectory(spritePath);
            }

            // Cria sprite amarelo (quest disponível)
            CreateExclamationSprite(spritePath + "/quest_indicator_available.png", new Color(1f, 0.9f, 0f, 1f));

            // Cria sprite dourado (quest pronta)
            CreateExclamationSprite(spritePath + "/quest_indicator_ready.png", new Color(1f, 0.7f, 0f, 1f));

            AssetDatabase.Refresh();

            UnityEngine.Debug.Log("[QuestSpriteGenerator] Sprites de indicadores criados com sucesso!");
            EditorUtility.DisplayDialog("Quest Sprites", "Sprites de indicadores criados com sucesso em:\n" + spritePath, "OK");
        }

        private static void CreateExclamationSprite(string path, Color color)
        {
            int width = 32;
            int height = 32;

            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;

            // Preenche com transparente
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }

            // Desenha exclamação simples (!)
            DrawExclamation(pixels, width, height, color);

            texture.SetPixels(pixels);
            texture.Apply();

            // Salva PNG
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);

            DestroyImmediate(texture);
        }

        private static void DrawExclamation(Color[] pixels, int width, int height, Color color)
        {
            // Desenha corpo da exclamação (vertical)
            for (int y = 10; y < 22; y++)
            {
                for (int x = 14; x < 18; x++)
                {
                    SetPixel(pixels, x, y, width, height, color);
                }
            }

            // Desenha ponto da exclamação
            for (int y = 6; y < 9; y++)
            {
                for (int x = 14; x < 18; x++)
                {
                    SetPixel(pixels, x, y, width, height, color);
                }
            }
        }

        private static void SetPixel(Color[] pixels, int x, int y, int width, int height, Color color)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
            {
                pixels[y * width + x] = color;
            }
        }
    }
}
