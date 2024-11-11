using System;
using System.Collections.Generic;
using UnityEngine;

using CurrentProtocol = Flakkari4Unity.Protocol.V1;

namespace Flakkari4Unity.API {
public class APIClient : MonoBehaviour
{
    /// <summary>
    /// Creates a REQ_CONNECT message to connect to the server.
    /// </summary>
    /// <param name="gameName">The name of the game to connect to.</param>
    /// <returns>A byte array representing the serialized REQ_CONNECT message.</returns>
    public static byte[] ReqConnect(string gameName)
    {
        Debug.Log("REQ_CONNECT message sent to the server.");
        return CurrentProtocol.Packet.Serialize(
            CurrentProtocol.Priority.HIGH,
            CurrentProtocol.CommandId.REQ_CONNECT,
            System.Text.Encoding.UTF8.GetBytes(gameName)
        );
    }

    /// <summary>
    /// Creates a REQ_HEARTBEAT message to keep the connection alive.
    /// </summary>
    /// <returns>A byte array representing the serialized REQ_HEARTBEAT message.</returns>
    /// <remarks>
    /// This message is sent to the server at regular intervals to keep the connection alive.
    /// </remarks>
    public static byte[] ReqKeepAlive()
    {
        return CurrentProtocol.Packet.Serialize(
            CurrentProtocol.Priority.LOW,
            CurrentProtocol.CommandId.REQ_HEARTBEAT
        );
    }

    public static byte[] ReqUserUpdates(List<CurrentProtocol.Event> events)
    {
        Debug.Log("REQ_USER_UPDATE message sent to the server.");
        return CurrentProtocol.Packet.Serialize(
            CurrentProtocol.Priority.HIGH,
            CurrentProtocol.CommandId.REQ_USER_UPDATES,
            CurrentProtocol.Event.Serialize(events)
        );
    }

    public static byte[] ReqUserUpdate(CurrentProtocol.EventId id, CurrentProtocol.EventState state)
    {
        CurrentProtocol.Event _event = new()
        {
            id = id,
            state = state
        };

        Debug.Log("REQ_USER_UPDATE message sent to the server.");
        return CurrentProtocol.Packet.Serialize(
            CurrentProtocol.Priority.HIGH,
            CurrentProtocol.CommandId.REQ_USER_UPDATE,
            CurrentProtocol.Event.Serialize(_event)
        );
    }

    /// <summary>
    /// Processes the received data and extracts the command ID, sequence number, and message.
    /// </summary>
    /// <param name="receivedData">The byte array containing the received data.</param>
    /// <param name="commandId">The extracted command ID from the received data.</param>
    /// <param name="sequenceNumber">The extracted sequence number from the received data.</param>
    /// <param name="message">The extracted message from the received data, decoded as a UTF-8 string.</param>
    public static void Reply(byte[] receivedData, out CurrentProtocol.CommandId commandId, out uint sequenceNumber, out byte[] payload)
    {
        CurrentProtocol.Packet.Deserialize(receivedData, out commandId, out sequenceNumber, out payload);

        if (payload == null)
        {
            Debug.Log("[RECEIVED] CommandId: " + commandId + ", delay: " + (sequenceNumber - (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) + " ms");
            return;
        }

        Debug.Log("[RECEIVED] CommandId: " + commandId + ", delay: " + (sequenceNumber - (uint)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) + " ms, payload: " + payload);
    }
}
} // namespace Flakkari4Unity.API
