using UnityEngine;

namespace ReplantedOnline.Helper;

internal static class Utils
{
    internal static GameObject FindInactive(string path, string sceneName = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("[FindInactive] Path is null or empty");
            return null;
        }

        string[] parts = path.Split('/');
        if (parts.Length == 0)
        {
            Debug.LogError("[FindInactive] Path has no valid segments");
            return null;
        }

        // Search in specific scene if provided, otherwise check all scenes
        bool searchAllScenes = string.IsNullOrEmpty(sceneName);
        Transform? parent = null;

        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

            // Skip if we're looking for a specific scene and this isn't it
            if (!searchAllScenes && !scene.name.Equals(sceneName))
                continue;

            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name.Equals(parts[0]))
                {
                    parent = root.transform;
                    break;
                }
            }

            if (parent != null) break; // Found our starting point
        }

        if (parent == null)
        {
            return null;
        }

        // Traverse the remaining path
        for (int i = 1; i < parts.Length; i++)
        {
            parent = parent.Find(parts[i]);
            if (parent == null)
            {
                Debug.LogError($"[FindInactive] Child '{parts[i]}' not found under '{parts[i - 1]}'");
                return null;
            }
        }

        return parent.gameObject;
    }
}
