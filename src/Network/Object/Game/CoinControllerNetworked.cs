using Il2CppInterop.Runtime.Attributes;
using Il2CppReloaded.Gameplay;
using MelonLoader;
using ReplantedOnline.Modules;
using ReplantedOnline.Network.Online;
using ReplantedOnline.Network.Packet;
using ReplantedOnline.Patches.Versus.NetworkSync;
using System.Collections;
using UnityEngine;

namespace ReplantedOnline.Network.Object.Game;

internal class CoinControllerNetworked : NetworkClass
{
    /// Global dictionary tracking all active coins and their associated network controllers.
    /// Used to find network controllers when coins need to send RPCs or be synchronized.
    internal static Dictionary<Coin, CoinControllerNetworked> NetworkedCoinControllers = [];

    // Local coin instance this controller manages
    internal Coin _Coin;
    // Original spawn position of the coin on the board
    internal Vector2 BoardGridPos;
    // Type of coin (e.g., silver, gold, etc.)
    internal CoinType TheCoinType;
    // Motion behavior of the coin (e.g., falling, bouncing, etc.)
    internal CoinMotion TheCoinMotion;

    public void Update()
    {
        if (AmOwner && !Despawning)
        {
            if ((HasSpawned && _Coin == null) || (_Coin.mDead || _Coin.WasCollected))
            {
                Despawning = true;
                MelonCoroutines.Start(CoDespawn());
            }
        }
    }

    private bool Despawning;
    private IEnumerator CoDespawn()
    {
        // wait for desync
        yield return new WaitForSeconds(3f);
        if (this != null)
        {
            Despawn();
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        if (_Coin != null)
        {
            NetworkedCoinControllers.Remove(_Coin);
        }
    }

    [HideFromIl2Cpp]
    public override void HandleRpc(SteamNetClient sender, byte rpcId, PacketReader packetReader)
    {
        switch (rpcId)
        {
            case 0:
                HandleCollectRpc();
                break;
        }
    }

    internal void SendCollectRpc()
    {
        this.SendRpc(0, null, false);
    }

    private void HandleCollectRpc()
    {
        _Coin.CollectOriginal(1, false);
    }

    [HideFromIl2Cpp]
    public override void Serialize(PacketWriter packetWriter, bool init)
    {
        if (init)
        {
            // Only send full state during initial spawn
            packetWriter.WriteVector2(BoardGridPos);
            packetWriter.WriteByte((byte)TheCoinType);
            packetWriter.WriteByte((byte)TheCoinMotion);
        }
    }

    [HideFromIl2Cpp]
    public override void Deserialize(PacketReader packetReader, bool init)
    {
        if (init)
        {
            // Only process full state during initial spawn
            BoardGridPos = packetReader.ReadVector2();
            TheCoinType = (CoinType)packetReader.ReadByte();
            TheCoinMotion = (CoinMotion)packetReader.ReadByte();

            // Recreate the actual coin object in the game world using the original method
            _Coin = Instances.GameplayActivity.Board.AddCoinOriginal(BoardGridPos.x, BoardGridPos.y, TheCoinType, TheCoinMotion);

            // Register this network controller with the newly created coin
            NetworkedCoinControllers[_Coin] = this;
        }
    }
}