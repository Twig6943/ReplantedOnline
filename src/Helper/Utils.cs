using Il2CppReloaded.Gameplay;
using Il2CppTekly.PanelViews;
using MelonLoader;
using ReplantedOnline.Enums;
using ReplantedOnline.Patches.Gameplay.Versus.Networked;
using System.Reflection;
using UnityEngine;

namespace ReplantedOnline.Helper;

internal static class Utils
{
    internal static PanelView GetPanel(this PanelViewContainer panelViewContainer, string panelId)
    {
        foreach (var panel in panelViewContainer.m_panels)
        {
            if (panel.Id != panelId) continue;
            return panel;
        }

        return null;
    }

    /// <summary>
    /// Places a seed (plant or zombie) at the specified grid position with network synchronization support
    /// </summary>
    /// <param name="seedType">Type of seed to plant</param>
    /// <param name="imitaterType">Imitater plant type if applicable</param>
    /// <param name="gridX">X grid coordinate (0-8 for plants, 0-8 for zombies)</param>
    /// <param name="gridY">Y grid coordinate (0-4 for lawn rows)</param>
    /// <param name="spawnOnNetwork">Whether to spawn the object on the network for multiplayer synchronization</param>
    /// <returns>The created game object (plant or zombie)</returns>
    internal static ReloadedObject PlaceSeed(SeedType seedType, SeedType imitaterType, int gridX, int gridY, bool spawnOnNetwork)
    {
        return SeedPacketSyncPatch.PlaceSeed(seedType, imitaterType, gridX, gridY, spawnOnNetwork);
    }

    /// <summary>
    /// Spawns a plant at the specified grid position with optional network synchronization
    /// </summary>
    /// <param name="seedType">Type of plant seed to spawn</param>
    /// <param name="imitaterType">Imitater plant type if the plant is mimicking another plant</param>
    /// <param name="gridX">X grid coordinate (0-8)</param>
    /// <param name="gridY">Y grid coordinate (0-4)</param>
    /// <param name="spawnOnNetwork">Whether to create a network controller for multiplayer sync</param>
    /// <returns>The spawned Plant object</returns>
    internal static Plant SpawnPlant(SeedType seedType, SeedType imitaterType, int gridX, int gridY, bool spawnOnNetwork)
    {
        return SeedPacketSyncPatch.SpawnPlant(seedType, imitaterType, gridX, gridY, spawnOnNetwork);
    }

    /// <summary>
    /// Spawns a zombie at the specified grid position with optional network synchronization
    /// </summary>
    /// <param name="zombieType">Type of zombie to spawn</param>
    /// <param name="gridX">X grid coordinate (0-8)</param>
    /// <param name="gridY">Y grid coordinate (0-4)</param>
    /// <param name="spawnOnNetwork">Whether to create a network controller for multiplayer sync</param>
    /// <param name="shakeBush">If the bush on the row the zombie spawns in shakes</param>
    /// <returns>The spawned Zombie object</returns>
    internal static Zombie SpawnZombie(ZombieType zombieType, int gridX, int gridY, bool shakeBush, bool spawnOnNetwork)
    {
        return SeedPacketSyncPatch.SpawnZombie(zombieType, gridX, gridY, shakeBush, spawnOnNetwork);
    }

    /// <summary>
    /// Gets the opposite team for a given player team.
    /// </summary>
    /// <param name="team">The player team to get the opposite of.</param>
    /// <returns>
    /// The opposite team:
    /// <list type="bullet">
    /// <item><description>Plants → Zombies</description></item>
    /// <item><description>Zombies → Plants</description></item>
    /// <item><description>Any other value → None</description></item>
    /// </list>
    /// </returns>
    internal static PlayerTeam GetOppositeTeam(PlayerTeam team)
    {
        switch (team)
        {
            case PlayerTeam.Plants:
                return PlayerTeam.Zombies;
            case PlayerTeam.Zombies:
                return PlayerTeam.Plants;
            default:
                return PlayerTeam.None;
        }
    }

    /// <summary>
    /// Dictionary for caching loaded sprites to improve performance by avoiding duplicate loads.
    /// </summary>
    private static readonly Dictionary<string, Sprite> _cachedSprites = [];

    /// <summary>
    /// Loads a sprite from the specified resource path with optional pixel density settings.
    /// </summary>
    /// <param name="path">The path to the sprite resource within the assembly's embedded resources.</param>
    /// <param name="pixelsPerUnit">The number of texture pixels that correspond to one unit in world space. Default is 1.</param>
    /// <returns>The loaded Sprite object, or null if loading fails.</returns>
    internal static Sprite LoadSprite(string path, float pixelsPerUnit = 1f)
    {
        try
        {
            var cacheKey = path + pixelsPerUnit;
            if (_cachedSprites.TryGetValue(cacheKey, out var sprite))
                return sprite;

            var texture = LoadTextureFromResources(path);
            if (texture == null)
                return null;

            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
            sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;

            return _cachedSprites[cacheKey] = sprite;
        }
        catch (Exception ex)
        {
            MelonLogger.Error(ex);
            return null;
        }
    }

    /// <summary>
    /// Loads a Texture2D from embedded resources in the executing assembly.
    /// </summary>
    /// <param name="path">The path to the texture resource within the assembly's embedded resources.</param>
    /// <returns>The loaded Texture2D object, or null if loading fails.</returns>
    internal static Texture2D LoadTextureFromResources(string path)
    {
        try
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream == null)
                return null;

            var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                if (!texture.LoadImage(ms.ToArray(), false))
                    return null;
            }

            return texture;
        }
        catch (Exception ex)
        {
            MelonLogger.Error(ex);
            return null;
        }
    }
}
