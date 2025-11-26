using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ExtraTools.Editor
{
    public class SpriteMaterialFixer
    {
        [MenuItem("Extra Tools/Setup/Fix Sprite Materials")]
        private static void FixSpriteMaterials()
        {
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            List<string> fixedObjects = new List<string>();

            // Busca o material correto de Sprite-Lit-Default
            Material defaultSpriteMaterial = null;
            string[] guids = AssetDatabase.FindAssets("Sprite-Lit-Default");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat != null && mat.name == "Sprite-Lit-Default")
                {
                    defaultSpriteMaterial = mat;
                    break;
                }
            }

            if (defaultSpriteMaterial == null)
            {
                Debug.LogError("Não foi possível encontrar o material Sprite-Lit-Default!");
                return;
            }

            foreach (GameObject obj in allObjects)
            {
                SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    if (spriteRenderer.sharedMaterial != null)
                    {
                        // Verifica se está usando material incorreto
                        if (spriteRenderer.sharedMaterial.name.Contains("Lit (Instance)") ||
                            spriteRenderer.sharedMaterial.name == "Lit" ||
                            (spriteRenderer.sharedMaterial.name.Contains("Lit") && !spriteRenderer.sharedMaterial.name.Contains("Sprite")))
                        {
                            spriteRenderer.sharedMaterial = defaultSpriteMaterial;
                            fixedObjects.Add($"{obj.name} -> {spriteRenderer.gameObject.name}");
                            EditorUtility.SetDirty(spriteRenderer);
                            EditorUtility.SetDirty(obj);
                        }
                    }
                    else
                    {
                        // Se não tem material, aplica o padrão
                        spriteRenderer.sharedMaterial = defaultSpriteMaterial;
                        fixedObjects.Add($"{obj.name} -> {spriteRenderer.gameObject.name} (material nulo)");
                        EditorUtility.SetDirty(spriteRenderer);
                        EditorUtility.SetDirty(obj);
                    }
                }
            }

            if (fixedObjects.Count > 0)
            {
                Debug.Log($"<color=green>Materiais de sprite corrigidos:</color>\n" + string.Join("\n", fixedObjects));
            }
            else
            {
                Debug.Log("<color=yellow>Nenhum material de sprite precisou ser corrigido.</color>");
            }
        }

        [MenuItem("Extra Tools/Setup/Detect Wrong Sprite Materials")]
        private static void DetectWrongSpriteMaterials()
        {
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
            List<string> problemObjects = new List<string>();

            foreach (GameObject obj in allObjects)
            {
                SpriteRenderer[] spriteRenderers = obj.GetComponentsInChildren<SpriteRenderer>();

                foreach (SpriteRenderer spriteRenderer in spriteRenderers)
                {
                    if (spriteRenderer.sharedMaterial == null)
                    {
                        problemObjects.Add($"{obj.name} -> {spriteRenderer.gameObject.name}: Material NULO");
                    }
                    else if (spriteRenderer.sharedMaterial.name.Contains("Lit (Instance)") ||
                             spriteRenderer.sharedMaterial.name == "Lit" ||
                             (spriteRenderer.sharedMaterial.name.Contains("Lit") && !spriteRenderer.sharedMaterial.name.Contains("Sprite")))
                    {
                        problemObjects.Add($"{obj.name} -> {spriteRenderer.gameObject.name}: {spriteRenderer.sharedMaterial.name}");
                    }
                }
            }

            if (problemObjects.Count > 0)
            {
                Debug.LogWarning($"<color=red>Materiais de sprite incorretos detectados:</color>\n" + string.Join("\n", problemObjects));
            }
            else
            {
                Debug.Log("<color=green>Todos os materiais de sprite estão corretos!</color>");
            }
        }
    }
}