using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace ExtraTools.Editor
{
    /// <summary>
    /// Creates the Grid hierarchy used in InitialCave with identical configuration.
    /// </summary>
    public static class InitialCaveGridCreator
    {
        private const string MenuPath = "Extra Tools/Setup/Create InitialCave Grid Rig";
        private const string RootName = "Grid";
        private const string SpriteLitMaterialPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat";
        private const float RootPosX = -2.89f;
        private const float RootPosY = 3.14f;
        private const int FallbackTileLayerIndex = 6;

        private static readonly string[] TileLayerCandidates = { "Ground", "Tiles", "Tilemap" };

        private static readonly TilemapLayerConfig[] TilemapConfigs =
        {
            new TilemapLayerConfig("GroundGrid", "Ground"),
            new TilemapLayerConfig("GroundPropsGrid", "GroundProps"),
            new TilemapLayerConfig("GroundProps2XGrid", "GroundProps"),
            new TilemapLayerConfig("WallsGrid", "Walls"),
            new TilemapLayerConfig("WallsPropsGrid", "WallsProps")
        };

        private static Material spriteLitMaterial;

        [MenuItem(MenuPath)]
        public static void CreateGridRig()
        {
            if (GameObject.Find(RootName) != null)
            {
                EditorUtility.DisplayDialog("InitialCave Grid", "A GameObject named 'Grid' already exists in the scene. Delete or rename it before running this tool.", "OK");
                return;
            }

            var activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid())
            {
                EditorUtility.DisplayDialog("InitialCave Grid", "No active scene detected.", "OK");
                return;
            }

            Undo.IncrementCurrentGroup();
            var undoGroup = Undo.GetCurrentGroup();

            var root = new GameObject(RootName);
            Undo.RegisterCreatedObjectUndo(root, "Create InitialCave Grid Rig");
            ConfigureRoot(root);

            foreach (var config in TilemapConfigs)
            {
                CreateTilemapChild(root.transform, config);
            }

            Selection.activeGameObject = root;
            EditorSceneManager.MarkSceneDirty(activeScene);
            Undo.CollapseUndoOperations(undoGroup);

            EditorUtility.DisplayDialog("InitialCave Grid", "Grid hierarchy created successfully.", "OK");
        }

        private static void ConfigureRoot(GameObject root)
        {
            root.transform.position = new Vector3(RootPosX, RootPosY, 0f);
            root.transform.rotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            var grid = Undo.AddComponent<Grid>(root);
            grid.cellSize = new Vector3(1f, 1f, 0f);
            grid.cellGap = Vector3.zero;
            grid.cellLayout = GridLayout.CellLayout.Rectangle;
            grid.cellSwizzle = GridLayout.CellSwizzle.XYZ;
        }

        private static void CreateTilemapChild(Transform parent, TilemapLayerConfig config)
        {
            var go = new GameObject(config.Name);
            Undo.RegisterCreatedObjectUndo(go, "Create Tilemap Layer");
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.layer = ResolveTilemapLayerIndex();

            var tilemap = Undo.AddComponent<Tilemap>(go);
            ConfigureTilemap(tilemap);

            var renderer = Undo.AddComponent<TilemapRenderer>(go);
            ConfigureRenderer(renderer, config.SortingLayerName, config.SortingOrder);
        }

        private static void ConfigureTilemap(Tilemap tilemap)
        {
            tilemap.color = Color.white;
            tilemap.animationFrameRate = 1f;
            tilemap.tileAnchor = new Vector3(0.5f, 0.5f, 0f);
            tilemap.orientation = Tilemap.Orientation.XY;
            tilemap.orientationMatrix = Matrix4x4.identity;
        }

        private static void ConfigureRenderer(TilemapRenderer renderer, string sortingLayerName, int sortingOrder)
        {
            renderer.sortingOrder = sortingOrder;

            var sortingLayerId = SortingLayer.NameToID(sortingLayerName);
            if (sortingLayerId != 0 || sortingLayerName == SortingLayer.layers[0].name)
            {
                renderer.sortingLayerID = sortingLayerId;
            }
            else
            {
                Debug.LogWarning($"Sorting layer '{sortingLayerName}' was not found. Using existing sorting layer '{SortingLayer.layers[0].name}'.");
            }

            var material = GetSpriteLitMaterial();
            if (material != null)
            {
                renderer.sharedMaterial = material;
            }
        }

        private static Material GetSpriteLitMaterial()
        {
            if (spriteLitMaterial != null)
            {
                return spriteLitMaterial;
            }

            spriteLitMaterial = AssetDatabase.LoadAssetAtPath<Material>(SpriteLitMaterialPath);
            if (spriteLitMaterial == null)
            {
                Debug.LogWarning("Sprite-Lit Default material not found. Tilemaps will use their default material.");
            }

            return spriteLitMaterial;
        }

        private static int ResolveTilemapLayerIndex()
        {
            foreach (var candidate in TileLayerCandidates)
            {
                var index = LayerMask.NameToLayer(candidate);
                if (index >= 0)
                {
                    return index;
                }
            }

            return Mathf.Clamp(FallbackTileLayerIndex, 0, 31);
        }

        private readonly struct TilemapLayerConfig
        {
            public string Name { get; }
            public string SortingLayerName { get; }
            public int SortingOrder { get; }

            public TilemapLayerConfig(string name, string sortingLayerName, int sortingOrder = 0)
            {
                Name = name;
                SortingLayerName = sortingLayerName;
                SortingOrder = sortingOrder;
            }
        }
    }
}
