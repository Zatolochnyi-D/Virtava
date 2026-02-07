// using System;
// using System.IO;
// using System.Linq;
// using System.Net.Sockets;
// using System.Threading;
// using System.Threading.Tasks;
// using Google.Protobuf;
// using UnityEngine;
// using UnityEngine.UI;
// using NetMQ.Sockets;
// using NetMQ;

// public class Connector : MonoBehaviour
// {
//     [SerializeField] private string _serverAddress = "localhost";
//     [SerializeField] private int _port = 13133;
//     [SerializeField] private float _waitBeforeTryToReconnect = 5f;
//     [SerializeField] private Button _connect;
//     [SerializeField] private Button _disconnect;
//     [SerializeField] private Animat _animator;

//     private TcpClient _client;
//     private CancellationTokenSource _cancellation;
//     private NetworkStream _stream;

//     async Task Listen()
//     {
//         using var socket = new SubscriberSocket();
//         socket.Connect($"tcp://{_serverAddress}:{_port}");
//         while (!destroyCancellationToken.IsCancellationRequested)
//         {
//             var (bytes, next) = await socket.ReceiveFrameBytesAsync(destroyCancellationToken);
//             Debug.Log(next);
//             Debug.Log(NormalizedLandmarkPointsList.Parser.ParseFrom(bytes).Points[0].X);
//             Debug.Log("=====");
//         }
//     }

//     void Awake()
//     {
//         // using var socket = new SubscriberSocket();
//         // socket.Connect($"tcp://{_serverAddress}:{_port}");
//         // while (!destroyCancellationToken.IsCancellationRequested)
//         // {
//         //     var message = new Msg();
//         //     message.InitEmpty();
//         //     socket.Receive(ref message);
//         //     // var (bytes, next) = await socket.ReceiveFrameBytesAsync(destroyCancellationToken);
//         //     // Debug.Log(next);
//         //     Debug.Log(NormalizedLandmarkPointsList.Parser.ParseFrom(message.Slice()).Points[0].X);
//         //     Debug.Log("=====");
//         // }
//     //     using var runtime = new NetMQRuntime();
//     //     runtime.Run(Listen());

//         // _client = new TcpClient();
//         // _cancellation = new();
//         // ConnectToServerAsync(destroyCancellationToken);

//         // _connect.onClick.AddListener(() =>
//         // {
//         //     _client.Connect(SERVER_ADDRESS, PORT);
//         //     Debug.Log("Client connected.");
//         //     _stream = _client.GetStream();
//         //     _cancellation = new();
//         //     ReadMessages(_cancellation.Token);
//         // });
//         // _disconnect.onClick.AddListener(() =>
//         // {
//         //     Debug.Log("Disconnectiong...");
//         //     _cancellation.Cancel();
//         //     _client.Close();
//         //     _client = null;
//         //     _stream = null;
//         //     _cancellation = null;
//         //     Debug.Log("Disconnected.");
//         // });
//     }

//     void OnDestroy()
//     {
//         // Debug.Log("Closed");
//         _cancellation?.Cancel();
//         // _client?.Client.Shutdown(SocketShutdown.Both);
//         _stream?.Close();
//         _client?.Close();
//     }

//     private async void ConnectToServerAsync(CancellationToken token)
//     {
//         try
//         {
//             while (!_client.Connected)
//             {
//                 token.ThrowIfCancellationRequested();
//                 try
//                 {
//                     await _client.ConnectAsync(_serverAddress, _port);
//                     _stream = _client.GetStream();
//                 }
//                 catch (SocketException e)
//                 {
//                     if (e.SocketErrorCode == SocketError.ConnectionRefused)
//                     {
//                         Debug.Log("Connection failed - server is down. Trying again after some delay.");
//                         await Awaitable.WaitForSecondsAsync(_waitBeforeTryToReconnect, token);
//                     }
//                 }
//             }
//         }
//         catch (OperationCanceledException)
//         {
//             Debug.Log("During trying to connect, operation was cancelled.");
//             return;
//         }

//         Debug.Log("Client connected");

//         while (true)
//         {
//             try
//             {
//                 if (token.IsCancellationRequested)
//                     break;
//                 var result = await _stream.ReadProtoObject(NormalizedLandmarkPointsList.Parser, token);
//                 result.ApplyElse(x =>
//                 {
//                     if (x.Points.Any())
//                         Debug.Log(x.Points[0]);
//                     else
//                         Debug.Log("Nothing");
//                 }, x =>
//                 {
//                     Debug.Log($"Failed: {x}");
//                 });
//                 if (result.IsRight)
//                     break;
//             }
//             catch (Exception)
//             {
//                 throw;
//             }
//         }

//         Debug.Log("Connection method end");

//         // Set up proper listening.
//     }
// }

// public static class NetworkStreamExtension
// {
//     public static async ValueTask<Status> ReadExactlyAsync(this NetworkStream stream, byte[] buffer, CancellationToken token = default)
//     {
//         var offset = 0;
//         var amountToRead = buffer.Length;
//         try
//         {
//             while (amountToRead != 0)
//             {
//                 var bytesRead = await stream.ReadAsync(buffer, offset, amountToRead, token);
//                 if (token.IsCancellationRequested)
//                     return Status.OperationCancelled;
//                 if (bytesRead == 0)
//                     return Status.ServerDisconnected;
//                 offset += bytesRead;
//                 amountToRead -= bytesRead;
//             }
//         }
//         catch (IOException exception)
//         {
//             if (exception.InnerException is SocketException socketException)
//                 if (socketException.SocketErrorCode == SocketError.OperationAborted)
//                     return Status.OperationCancelled;
//         }
//         catch (ObjectDisposedException)
//         {
//             return Status.OperationCancelled;
//         }
//         return Status.Success;
//     }

//     public static async ValueTask<Either<T, Status>> ReadProtoObject<T>(this NetworkStream stream, MessageParser<T> parser, CancellationToken token = default) where T : IMessage<T>
//     {
//         var messageLengthBytes = new byte[sizeof(int)];
//         var status = await stream.ReadExactlyAsync(messageLengthBytes, token);
//         if (status != Status.Success)
//             return Either.Right<T, Status>(status);

//         int messageLength = BitConverter.ToInt32(messageLengthBytes);
//         var messageBytes = new byte[messageLength];
//         status = await stream.ReadExactlyAsync(messageBytes, token);
//         if (status != Status.Success)
//             return Either.Right<T, Status>(status);

//         return Either.Left<T, Status>(parser.ParseFrom(messageBytes));
//     }
// }

// public enum Status
// {
//     Success,
//     OperationCancelled,
//     ServerDisconnected,
// }