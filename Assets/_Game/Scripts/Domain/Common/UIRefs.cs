using UnityEngine;

namespace VertigoGames.Common
{
    public static class UIRefs
    {
        public static void Assign<T>(ref T field, string objectName)
            where T : Component
        {
            if (field != null || Application.isPlaying)
                return;

            T[] all = Object.FindObjectsByType<T>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            foreach (T component in all)
            {
                if (component.gameObject.name == objectName)
                {
                    field = component;
                    return;
                }
            }
        }

        public static void AssignFirst<T>(ref T field) where T : Component
        {
            if (field != null || Application.isPlaying)
                return;

            field = Object.FindFirstObjectByType<T>(
                FindObjectsInactive.Include);
        }
        
        public static void AssignChild<T>(
    Component owner, ref T field, string objectName) where T : Component
        {
            if (field != null)
                return;

            foreach (T child in owner.GetComponentsInChildren<T>(true))
            {
                if (child.gameObject.name == objectName)
                {
                    field = child;
                    return;
                }
            }
        }
    }
}