namespace ReplantedOnline.Enums;

/// <summary>
/// Identifies the type of network packet for proper routing and handling.
/// Used to distinguish between different packet categories in the networking system.
/// </summary>
internal enum PacketTag
{
    /// <summary>
    /// No specific tag or unhandled packet type.
    /// </summary>
    None,

    /// <summary>
    /// Packet used for P2P session establishment and maintenance.
    /// </summary>
    P2P,

    /// <summary>
    /// Packet used for P2P session closing.
    /// </summary>
    P2PClose,

    /// <summary>
    /// Remote Procedure Call packet for executing methods on remote clients.
    /// </summary>
    Rpc,

    /// <summary>
    /// Packet used for spawning a network class.
    /// </summary>
    NetworkClassSpawn,

    /// <summary>
    /// Packet used for despawning a network class.
    /// </summary>
    NetworkClassDespawn,

    /// <summary>
    /// Packet used for syncing  a network class.
    /// </summary>
    NetworkClassSync,

    /// <summary>
    /// Packet used for P2P session establishment and maintenance on a network class.
    /// </summary>
    NetworkClassRpc,
}