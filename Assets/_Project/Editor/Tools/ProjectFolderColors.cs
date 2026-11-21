using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SeriousGameAR.EditorTools
{
    /// <summary>Colors project folders and their descendants without changing asset icons on disk.</summary>
    [InitializeOnLoad]
    internal static class ProjectFolderColors
    {
        private static readonly Dictionary<string, Color?> ColorCache = new Dictionary<string, Color?>();

        private static readonly KeyValuePair<string, Color>[] Rules =
        {
            Rule("Assets/_Project/Scripts", new Color32(80, 165, 245, 255)),
            Rule("Assets/_Project/Art", new Color32(185, 120, 235, 255)),
            Rule("Assets/_Project/Animations", new Color32(185, 120, 235, 255)),
            Rule("Assets/_Project/Scenes", new Color32(100, 200, 125, 255)),
            Rule("Assets/_Project/Prefabs", new Color32(245, 160, 70, 255)),
            Rule("Assets/_Project/UI", new Color32(240, 210, 85, 255)),
            Rule("Assets/_Project/Audio", new Color32(235, 105, 105, 255)),
            Rule("Assets/_Project/Settings", new Color32(170, 175, 185, 255)),
            Rule("Assets/ThirdParty", new Color32(170, 175, 185, 255))
        };

        static ProjectFolderColors()
        {
            EditorApplication.projectWindowItemOnGUI += DrawFolder;
            EditorApplication.projectChanged += ClearCache;
            EditorApplication.delayCall += EditorApplication.RepaintProjectWindow;
        }

        private static KeyValuePair<string, Color> Rule(string path, Color color)
        {
            return new KeyValuePair<string, Color>(path, color);
        }

        private static void ClearCache()
        {
            ColorCache.Clear();
            EditorApplication.RepaintProjectWindow();
        }

        private static Color? GetFolderColor(string guid)
        {
            if (ColorCache.TryGetValue(guid, out Color? cached))
                return cached;

            string path = AssetDatabase.GUIDToAssetPath(guid);
            Color? color = null;
            if (AssetDatabase.IsValidFolder(path))
            {
                foreach (KeyValuePair<string, Color> rule in Rules)
                {
                    if (path.Equals(rule.Key, StringComparison.Ordinal) ||
                        path.StartsWith(rule.Key + "/", StringComparison.Ordinal))
                    {
                        color = rule.Value;
                        break;
                    }
                }
            }

            ColorCache[guid] = color;
            return color;
        }

        private static void DrawFolder(string guid, Rect selectionRect)
        {
            if (Event.current.type != EventType.Repaint || string.IsNullOrEmpty(guid))
                return;

            Color? color = GetFolderColor(guid);
            if (!color.HasValue)
                return;

            // The list/tree uses a 16px icon; the grid reserves its bottom row for the name.
            bool isGrid = selectionRect.height > 20f;
            float size = isGrid ? Mathf.Min(selectionRect.width, selectionRect.height - 16f) : 16f;
            Rect iconRect = new Rect(
                isGrid ? selectionRect.x + (selectionRect.width - size) * 0.5f : selectionRect.x,
                isGrid ? selectionRect.y : selectionRect.y + (selectionRect.height - size) * 0.5f,
                size, size);

            Texture icon = EditorGUIUtility.IconContent("Folder Icon").image;
            if (icon == null)
                return;

            Color previousColor = GUI.color;
            try
            {
                GUI.color = color.Value;
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit, true);
            }
            finally
            {
                GUI.color = previousColor;
            }
        }
    }
}
