using Il2CppInterop.Runtime.Attributes;
using Il2CppReloaded.Gameplay;
using MelonLoader;
using ReplantedOnline.Helper;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.Packet;
using ReplantedOnline.Patches.Versus.NetworkSync;
using System.Collections;
using UnityEngine;

namespace ReplantedOnline.Network.Object.Game;

/// <summary>
/// Represents a networked zombie entity in the game world, handling synchronization of zombie state
/// across connected clients including health, position, and follower relationships.
/// </summary>
internal class ZombieNetworked : NetworkClass
{
    /// <summary>
    /// Dictionary mapping zombie instances to their networked counterparts for easy lookup.
    /// </summary>
    internal static Dictionary<Zombie, ZombieNetworked> NetworkedZombies = [];

    /// <summary>
    /// The underlying zombie instance that this networked object represents.
    /// </summary>
    internal Zombie _Zombie;

    /// <summary>
    /// The type of zombie this networked object represents when spawning.
    /// </summary>
    internal ZombieType ZombieType;

    /// <summary>
    /// The unique identifier for this zombie instance when spawning.
    /// </summary>
    internal ZombieID ZombieID;

    /// <summary>
    /// The grid X coordinate where this zombie is located when spawning.
    /// </summary>
    internal int GridX;

    /// <summary>
    /// The grid Y coordinate where this zombie is located when spawning.
    /// </summary>
    internal int GridY;

    /// <summary>
    /// Called when the zombie is destroyed, cleans up the zombie from the networked zombies dictionary.
    /// </summary>
    public void OnDestroy()
    {
        if (_Zombie != null)
        {
            NetworkedZombies.Remove(_Zombie);
        }
    }

    /// <summary>
    /// Cooldown timer for synchronization to prevent excessive network traffic.
    /// </summary>
    private static float syncCooldown = 2f;

    /// <summary>
    /// Updates the zombie state and handles periodic synchronization.
    /// </summary>
    public void Update()
    {
        if (AmOwner)
        {
            if (_Zombie?.mDead != true)
            {
                if (syncCooldown <= 0f)
                {
                    MarkDirty();
                    syncCooldown = 2f;
                }
                syncCooldown -= Time.deltaTime;
            }
            else if (!IsDespawning)
            {
                DespawnAndDestroyWithDelay(5f);
            }
        }
    }

    [HideFromIl2Cpp]
    public override void HandleRpc(SteamNetClient sender, byte rpcId, PacketReader packetReader)
    {
        if (sender.SteamId != OwnerId) return;

        switch (rpcId)
        {
            case 0:
                HandleSetFollowerZombieIdRpc(packetReader);
                break;
            case 1:
                HandleDeathRpc(packetReader);
                break;
        }
    }

    internal void SendSetFollowerZombieIdRpc(int index, ZombieID zombieID)
    {
        var writer = PacketWriter.Get();
        writer.WriteInt(index);
        writer.WriteInt((int)zombieID);
        this.SendRpc(0, writer, false);
    }

    [HideFromIl2Cpp]
    private void HandleSetFollowerZombieIdRpc(PacketReader packetReader)
    {
        var index = packetReader.ReadInt();
        var zombieId = (ZombieID)packetReader.ReadInt();
        _Zombie?.mFollowerZombieID[index] = zombieId;
    }

    internal void SendDeathRpc(DamageFlags damageFlags)
    {
        var writer = PacketWriter.Get();
        writer.WriteByte((byte)damageFlags);
        this.SendRpc(1, writer, false);
    }

    [HideFromIl2Cpp]
    private void HandleDeathRpc(PacketReader packetReader)
    {
        var damageFlags = (DamageFlags)packetReader.ReadByte();
        _Zombie.PlayDeathAnimOriginal(damageFlags);
    }

    [HideFromIl2Cpp]
    public override void Serialize(PacketWriter packetWriter, bool init)
    {
        if (init)
        {
            // Set spawn info
            packetWriter.WriteInt(GridX);
            packetWriter.WriteInt(GridY);
            packetWriter.WriteInt((int)ZombieID);
            packetWriter.WriteByte((byte)ZombieType);
            return;
        }

        packetWriter.WriteInt(_Zombie.mBodyHealth);
        packetWriter.WriteInt(_Zombie.mFlyingHealth);
        packetWriter.WriteInt(_Zombie.mHelmHealth);
        packetWriter.WriteInt(_Zombie.mShieldHealth);
        packetWriter.WriteBool(_Zombie.IsMovingAtChilledSpeed());
        packetWriter.WriteFloat(_Zombie.mPosX);

        ClearDirtyBits();
    }

    [HideFromIl2Cpp]
    public override void Deserialize(PacketReader packetReader, bool init)
    {
        if (init)
        {
            // Read spawn info
            GridX = packetReader.ReadInt();
            GridY = packetReader.ReadInt();
            ZombieID = (ZombieID)packetReader.ReadInt();
            ZombieType = (ZombieType)packetReader.ReadByte();

            _Zombie = Utils.SpawnZombie(ZombieType, GridX, GridY, false);
            _Zombie.DataID = ZombieID;

            NetworkedZombies[_Zombie] = this;
            return;
        }

        _Zombie?.mBodyHealth = packetReader.ReadInt();
        _Zombie?.mFlyingHealth = packetReader.ReadInt();
        _Zombie?.mHelmHealth = packetReader.ReadInt();
        _Zombie?.mShieldHealth = packetReader.ReadInt();
        var isMovingAtChilledSpeed = packetReader.ReadBool();
        var posX = packetReader.ReadFloat();
        LarpPos(posX, isMovingAtChilledSpeed);

        ClearDirtyBits();
    }

    /// <summary>
    /// Token used to track and manage position interpolation coroutines.
    /// </summary>
    private object larpToken;

    /// <summary>
    /// Smoothly interpolates the zombie's position to the target position when distance threshold is exceeded.
    /// </summary>
    /// <param name="posX">The target X position to interpolate to</param>
    /// <param name="isMovingAtChilledSpeed">If the zombie is slow</param>
    private void LarpPos(float posX, bool isMovingAtChilledSpeed)
    {
        if (_Zombie == null) return;

        float currentX = _Zombie.mPosX;
        float distance = Mathf.Abs(currentX - posX);

        float threshold = !isMovingAtChilledSpeed ? 35f : 15f;

        if (distance > threshold)
        {
            // Stop existing interpolation
            if (larpToken != null)
            {
                MelonCoroutines.Stop(larpToken);
            }

            if (distance < 100f)
            {
                larpToken = MelonCoroutines.Start(CoLarpPos(posX, isMovingAtChilledSpeed));
            }
            else
            {
                _Zombie.mPosX = posX;
            }
        }
    }

    /// <summary>
    /// Coroutine that smoothly interpolates the zombie's position over time.
    /// </summary>
    /// <param name="targetX">The target X position to reach</param>
    /// <param name="isMovingAtChilledSpeed">If the zombie is slow</param>
    [HideFromIl2Cpp]
    private IEnumerator CoLarpPos(float targetX, bool isMovingAtChilledSpeed)
    {
        if (this == null || _Zombie == null) yield break;

        float startX = _Zombie.mPosX;
        float distance = Mathf.Abs(targetX - startX);

        float speed = !isMovingAtChilledSpeed ? 25f : 15f;
        float duration = Mathf.Clamp(distance / speed, 0.1f, 2f);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (this == null || _Zombie == null) yield break;

            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            t = SmoothStep(t);

            _Zombie.mPosX = Mathf.Lerp(startX, targetX, t);
            yield return null;
        }

        // Ensure final position is exact
        _Zombie?.mPosX = targetX;

        larpToken = null;
    }

    // Smoother interpolation curves
    private static float SmoothStep(float t)
    {
        return t * t * (3f - 2f * t);
    }
}