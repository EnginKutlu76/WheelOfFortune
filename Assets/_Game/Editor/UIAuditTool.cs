using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace VertigoGames.EditorTools
{
    public static class UIAuditTool
    {
        private static readonly Regex NamePattern =
            new Regex("^ui_[a-z0-9]+(_[a-z0-9]+)*$");

        [MenuItem("Tools/Vertigo/UI Audit")]
        private static void Audit() => Run(false);

        [MenuItem("Tools/Vertigo/UI Audit + Auto Fix")]
        private static void AuditAndFix() => Run(true);

        private static void Run(bool fix)
        {
            var report = new StringBuilder();
            int count = 0;

            foreach (Graphic g in Object.FindObjectsByType<Graphic>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (g.raycastTarget && !NeedsRaycast(g))
                {
                    report.AppendLine($"[Raycast] {g.name}");
                    count++;
                    if (fix)
                    {
                        Undo.RecordObject(g, "Disable Raycast");
                        g.raycastTarget = false;
                    }
                }

                if (g is MaskableGraphic mg && mg.maskable && !HasMask(g))
                {
                    report.AppendLine($"[Maskable] {g.name}");
                    count++;
                    if (fix)
                    {
                        Undo.RecordObject(mg, "Disable Maskable");
                        mg.maskable = false;
                    }
                }

                if (g is Image image && image.sprite != null)
                {
                    bool hasBorder = image.sprite.border != Vector4.zero;

                    if (hasBorder && image.type == Image.Type.Simple)
                    {
                        report.AppendLine($"[Not Sliced] {g.name}");
                        count++;

                        if (fix)
                        {
                            Undo.RecordObject(image, "Set Sliced");
                            image.type = Image.Type.Sliced;
                        }
                    }
                    else if (image.type == Image.Type.Sliced && !hasBorder)
                    {
                        report.AppendLine(
                            $"[Sliced but no sprite border] {g.name} -> In the Sprite Editor, set a border.");
                        count++;
                    }
                }
            }

            foreach (RectTransform rect in Object.FindObjectsByType<RectTransform>(
                         FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (rect.GetComponentInParent<Canvas>(true) == null ||
                    rect.GetComponent<Canvas>() != null)
                    continue;

                if (!NamePattern.IsMatch(rect.name))
                {
                    report.AppendLine($"[Name] {rect.name}");
                    count++;
                }
            }

            foreach (CanvasScaler scaler in Object.FindObjectsByType<CanvasScaler>(
                         FindObjectsSortMode.None))
            {
                if (scaler.screenMatchMode != CanvasScaler.ScreenMatchMode.Expand ||
                    scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    report.AppendLine($"[CanvasScaler] {scaler.name} not Expand");
                    count++;
                }
            }

            if (fix && count > 0)
                EditorSceneManager.MarkAllScenesDirty();

            Debug.Log(count == 0
                ? "UI Audit: no problem"
                : $"UI Audit: {count} bulgu\n{report}");
        }

        private static bool NeedsRaycast(Graphic g)
        {
            Selectable selectable = g.GetComponentInParent<Selectable>(true);

            if (selectable != null && selectable.targetGraphic == g)
                return true;

            ScrollRect scroll = g.GetComponentInParent<ScrollRect>(true);

            if (scroll != null && scroll.viewport == g.rectTransform)
                return true;

            return g.name.EndsWith("_blocker");
        }

        private static bool HasMask(Graphic g) =>
            g.GetComponentInParent<RectMask2D>(true) != null ||
            g.GetComponentInParent<Mask>(true) != null;
    }
}