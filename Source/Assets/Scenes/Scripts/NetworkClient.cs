using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

using Flk_API = Flakkari4Unity.API;
using CurrentProtocol = Flakkari4Unity.Protocol.V1;

public class NetworkClient : MonoBehaviour
{
    private UdpClient udpClient;
    private IPEndPoint serverEndpoint;
    private readonly float keepAliveInterval = 4;
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private int serverPort = 54000;
    [SerializeField] private string gameName = "R-Type";
    [SerializeField] private bool enable;

    void Start()
    {
        if (!enable)
        {
            Destroy(this);
            return;
        }

        udpClient = new UdpClient();
        serverEndpoint = new IPEndPoint(IPAddress.Parse(serverIP), serverPort);

        udpClient.BeginReceive(OnReceive, null);

        byte[] packet = Flk_API.APIClient.ReqConnect(gameName);
        udpClient.Send(packet, packet.Length, serverEndpoint);
        InvokeRepeating(nameof(ReqKeepAlive), keepAliveInterval, keepAliveInterval);
    }

    public bool Enable
    {
        get => enable;
        set => enable = value;
    }

    internal void Send(byte[] packet)
    {
        if (udpClient != null)
            udpClient.Send(packet, packet.Length, serverEndpoint);
        else
            Debug.LogError("UDP client is not initialized.");
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

        try
        {
            byte[] receivedData = udpClient.EndReceive(result, ref serverEndpoint);
            Flk_API.APIClient.Reply(receivedData, out CurrentProtocol.CommandId commandId, out uint sequenceNumber, out byte[] payload);
        }
        catch (Exception e)
        {
            Debug.LogError("Error in OnReceive: " + e.Message);
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
