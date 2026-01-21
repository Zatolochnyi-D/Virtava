using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Connector : MonoBehaviour
{
    private const int PORT = 13133;
    private const string SERVER_ADDRESS = "localhost";

    [SerializeField] private Button _connect;
    [SerializeField] private Button _disconnect;
    [SerializeField] private Animat _animator;

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
        var lengthBytes = new byte[sizeof(int)];
        while (true)
        {
            try
            {
                var lengthReadSuccessful = await _stream.ReadExactly(lengthBytes);
                int messageLength = BitConverter.ToInt32(lengthBytes);
                var messageBytes = new byte[messageLength];
                var messageReadSuccessful = await _stream.ReadExactly(messageBytes);
                var list = NormalizedLandmarkPointsList.Parser.ParseFrom(messageBytes);

                if (token.IsCancellationRequested)
                {
                    Debug.Log("We disconnected");
                    break;
                }

                _animator.Animate(list);
            }
            catch (SocketException)
            {
                return;
            }
            // catch (Exception e)
            // {
            //     Debug.Log(e.GetType());
            //     Debug.Log(e.Message);
            //     return;
            // }
        }
    }
}

public static class NetworkStreamExtension
{
    public static async Task<bool> ReadExactly(this NetworkStream stream, byte[] buffer, CancellationToken token = default)
    {
        await Awaitable.BackgroundThreadAsync();
        var index = 0;
        while (index != buffer.Length)
        {
            if (token.IsCancellationRequested)
                return false;
            if (stream.CanRead && stream.DataAvailable)
            {
                var receivedByte = stream.ReadByte();
                if (receivedByte == -1)
                    return false;
                buffer[index] = (byte)receivedByte;
                index++;
            }
        }
        return true;
    }
}