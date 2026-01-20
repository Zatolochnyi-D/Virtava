using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Connector : MonoBehaviour
{
    private const int PORT = 13133;
    private const string SERVER_ADDRESS = "localhost";

    [SerializeField] private Button _connect;
    [SerializeField] private Button _disconnect;

    private TcpClient _client;
    private NetworkStream _stream;
    private CancellationTokenSource _cancellation;

    void Awake()
    {
        _connect.onClick.AddListener(() =>
        {
            _client = new TcpClient();
            Debug.Log("Client created.");
            _client.Connect(SERVER_ADDRESS, PORT);
            Debug.Log("Client connected.");
            _stream = _client.GetStream();
            _cancellation = new();
            ReadMessages(_cancellation.Token);
        });
        _disconnect.onClick.AddListener(() =>
        {
            Debug.Log("Disconnectiong...");
            _cancellation.Cancel();
            _client.Close();
            _client = null;
            _stream = null;
            _cancellation = null;
            Debug.Log("Disconnected.");
        });
    }

    void OnDestroy()
    {
        _cancellation?.Cancel();
        _client?.Close();
    }

    private async void ReadMessages(CancellationToken token)
    {
        byte[] buffer = new byte[sizeof(double)];
        while (true)
        {
            try
            {
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length, token);
                if (token.IsCancellationRequested)
                {
                    Debug.Log("We disconnected");
                    break;
                }
                if (bytesRead == 0)
                {
                    Debug.Log("Server disconnected");
                    break;
                }
            }
            catch (SocketException)
            {
                return;
            }
            catch (Exception e)
            {
                Debug.Log(e.GetType());
                return;
            }

            double value = BitConverter.ToDouble(buffer);
            Debug.Log($"Reveived: {value}");
        }
    }
}