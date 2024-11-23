using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

using Flk_API = Flakkari4Unity.API;
using CurrentProtocol = Flakkari4Unity.Protocol.V1;

public class NetworkClient : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint serverEndpoint;
    private Flakkari4Unity.Synchronizer synchronizer;
    private GameObject tmpPlayer;
    private readonly float keepAliveInterval = 3;
    private string serverIP;
    private int serverPort;
    private string gameName;
    private bool enable = false;

    public bool Enable
    {
        get => enable;
    }

    public void Create(string serverIP, int serverPort, string gameName)
    {
        this.serverIP = serverIP;
        this.serverPort = serverPort;
        this.gameName = gameName;
        enable = true;

        udpClient = new UdpClient();
        serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        synchronizer = gameObject.AddComponent<Flakkari4Unity.Synchronizer>();

        udpClient.BeginReceive(OnReceive, null);

        byte[] packet = Flk_API.APIClient.ReqConnect(gameName);
        udpClient.Send(packet, packet.Length, serverEndpoint);
        InvokeRepeating(nameof(ReqKeepAlive), keepAliveInterval, keepAliveInterval);

        tmpPlayer = Instantiate(Resources.Load<GameObject>("Player"));
        tmpPlayer.name = "Player_tmp";

        Player playerScript = tmpPlayer.GetComponent<Player>();
        playerScript.SetupCameraViewport(new Rect(0, 0, 1, 1));
        playerScript.SetupNetworkClient(this);
    }

    internal void Send(byte[] packet)
    {
        if (udpClient != null)
        {
            udpClient.Send(packet, packet.Length, serverEndpoint);
        }
        else
        {
            Debug.LogError("UDP client is not initialized.");
        }
    }

    private void ReqKeepAlive()
    {
        byte[] packet = Flk_API.APIClient.ReqKeepAlive();
        udpClient.Send(packet, packet.Length, serverEndpoint);
    }

    private void OnReceive(IAsyncResult result)
    {
        if (!enable)
            return;

            byte[] receivedData = udpClient.EndReceive(result, ref serverEndpoint);
        Flk_API.APIClient.Reply(receivedData, out List<CurrentProtocol.CommandId> commandId, out List<ulong> sequenceNumber, out List<byte[]> payloads);
        byte[] payload;

            for (int i = 0; i < commandId.Count; i++)
            {
                switch (commandId[i])
                {
                    case CurrentProtocol.CommandId.REP_CONNECT:
                    payload = Flk_API.APIClient.ResConnect(payloads[i], out ulong userId, out string userTemplate);

                    Flakkari4Unity.Synchronizer.Enqueue(() =>
                    {
                        tmpPlayer.name = userTemplate + "_" + userId;
                        synchronizer.AddEntity(userId, tmpPlayer.GetComponent<Flakkari4Unity.ECS.Entity>(), payload);
                    });
                        break;

                    case CurrentProtocol.CommandId.REQ_ENTITY_SPAWN:
                    payload = Flk_API.APIClient.ReqEntitySpawn(payloads[i], out ulong entityId, out string templateName);

                    Flakkari4Unity.Synchronizer.Enqueue(() =>
                    {
                        GameObject entity = Instantiate(Resources.Load<GameObject>(templateName));
                        entity.name = templateName + "_" + entityId;
                        synchronizer.AddEntity(entityId, entity.GetComponent<Flakkari4Unity.ECS.Entity>(), payload);
                    });
                        break;

                    case CurrentProtocol.CommandId.REQ_ENTITY_UPDATE:
                    Flk_API.APIClient.ReqEntityUpdate(payloads[i], ref synchronizer);
                        break;

                    case CurrentProtocol.CommandId.REQ_ENTITY_DESTROY:
                    Flk_API.APIClient.ReqEntityDestroy(payloads[i], ref synchronizer);
                        break;

                    case CurrentProtocol.CommandId.REQ_ENTITY_MOVED:
                    Flk_API.APIClient.ReqEntityMoved(payloads[i], ref synchronizer);
                    break;

                case CurrentProtocol.CommandId.REP_HEARTBEAT:
                        break;

                    default:
                        Debug.LogWarning("Unknown command ID received from the server.");
                        break;
                }
            }

        udpClient.BeginReceive(OnReceive, null);
    }

    private void OnApplicationQuit()
    {
        if (!enable)
            return;
        enable = false;

        udpClient.Close();
        Debug.Log("UDP client closed.");
    }
}
