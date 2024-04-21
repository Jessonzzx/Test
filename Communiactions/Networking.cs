using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Communications { 
    public class Networking : INetworking
    {
        private TcpClient _tcpClient;
        private TcpListener _tcpListener;
        private ILogger _logger;
        private readonly ReportConnectionEstablished _onConnect;
        private readonly ReportDisconnect _onDisconnect;
        private readonly ReportMessageArrived _onMessage;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWaitingForClients;

        public string ID { get; set; }
        public bool IsConnected => _tcpClient?.Connected ?? false;

        public bool IsWaitingForClients => _isWaitingForClients;

        public string RemoteAddressPort => _tcpClient?.Client?.RemoteEndPoint?.ToString() ?? "Disconnected";

        public string LocalAddressPort => _tcpClient?.Client?.LocalEndPoint?.ToString() ?? "Disconnected";

        public string ClientName { get; set; }
        /// <summary>
        /// The constructor for networking
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="onConnect"></param>
        /// <param name="onDisconnect"></param>
        /// <param name="onMessage"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Networking(ILogger logger,
                          ReportConnectionEstablished onConnect,
                          ReportDisconnect onDisconnect,
                          ReportMessageArrived onMessage)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _onConnect = onConnect ?? throw new ArgumentNullException(nameof(onConnect));
            _onDisconnect = onDisconnect ?? throw new ArgumentNullException(nameof(onDisconnect));
            _onMessage = onMessage ?? throw new ArgumentNullException(nameof(onMessage));
            _isWaitingForClients = false;
            _tcpClient = new TcpClient();
            _cancellationTokenSource = new CancellationTokenSource();
        }
        public Networking(TcpClient client, ILogger logger, ReportConnectionEstablished onConnect, ReportDisconnect onDisconnect, ReportMessageArrived onMessage)
        : this(logger, onConnect, onDisconnect, onMessage) 
        {
            _tcpClient = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _onConnect = onConnect ?? throw new ArgumentNullException(nameof(onConnect));
            _onDisconnect = onDisconnect ?? throw new ArgumentNullException(nameof(onDisconnect));
            _onMessage = onMessage ?? throw new ArgumentNullException(nameof(onMessage));
            _isWaitingForClients = false;
            _cancellationTokenSource = new CancellationTokenSource();
        }
     
        /// <summary>
        /// Initiates an asynchronous connection to a remote host.
        /// </summary>
        /// <param name="host">The host name of the remote server.</param>
        /// <param name="port">The port number of the remote server.</param>
        /// <exception cref="Exception">Throws if the connection attempt fails.</exception>
        public async Task ConnectAsync(string host, int port)
        {
            if (_tcpClient == null)
            {
                _tcpClient = new TcpClient();
            }

            if (IsConnected)
            {
                _logger.LogInformation("Already connected to the server.");
                return;
            }

            if (!IsConnected)
            {
                try
                {
                    await _tcpClient.ConnectAsync(host, port);
                    ID = _tcpClient.Client.RemoteEndPoint?.ToString();
                    _onConnect?.Invoke(this);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Connection failed: {ex.Message}");
                    throw; // Re-throw the exception to signal failure to the caller
                }
            }
        }
        /// <summary>
        /// Disconnects the client from the server and cleans up resources.
        /// </summary>
        public void Disconnect()
        {
            if (IsConnected)
            {
                _tcpClient?.Close();
                _onDisconnect?.Invoke(this); // Notify that the disconnection happened
            }

            _tcpClient = null;

            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }
        /// <summary>
        /// Reads incoming data from the connected TcpClient.
        /// </summary>
        /// <param name="infinite"></param>
        /// <returns></returns>
        public async Task HandleIncomingDataAsync(bool infinite = true)
        {
            if (_tcpClient == null || !_tcpClient.Connected)
            {
                _logger.LogError("HandleIncomingDataAsync called when the client is not connected.");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            NetworkStream networkStream = _tcpClient.GetStream();
            byte[] buffer = new byte[4096]; // Adjust the buffer size based on your needs
            StringBuilder messageBuilder = new StringBuilder();

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // Read data from the stream
                    int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token);
                    if (bytesRead == 0)
                    {
                        // The client has closed the connection
                        _onDisconnect?.Invoke(this);
                        break;
                    }

                    // Convert bytes to string
                    string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    // Process the received data and invoke the message received event
                    _onMessage?.Invoke(this, receivedText);

                    if (!infinite)
                    {
                        break; // Exit the loop if not set to run infinitely
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("The read operation was canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occurred while reading data: {ex}");
                _onDisconnect?.Invoke(this); // Notify about the disconnection due to exception
            }
            
        }
        /// <summary>
        /// Waits  for clients to connect on the specified port.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="infinite"></param>
        /// <returns></returns>
        public async Task WaitForClientsAsync(int port, bool infinite)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Start();
            _isWaitingForClients = true;

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    // Accept a new client connection
                    TcpClient client = await _tcpListener.AcceptTcpClientAsync();
                    _onConnect?.Invoke(this); // Invoke the connection established delegate

                    if (!infinite)
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Stopped waiting for clients.");
            }
            finally
            {
                _tcpListener.Stop();
                _isWaitingForClients = false;
            }
        }
        /// <summary>
        /// Stops waiting for clients to connect.
        /// </summary>
        public void StopWaitingForClients()
        {
            if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel();
                _logger.LogInformation("Waiting for clients has been stopped.");
            }
        }

        /// <summary>
        /// Stops waiting for messages from the connected client.
        /// </summary>
        public void StopWaitingForMessages()
       {
            StopWaitingForClients();
        }

        /// <summary>
        /// Sends the provided text to the server
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SendAsync(string text)
        {
            if (!IsConnected)
            {
                _logger.LogWarning("Send called when not connected");
                return;
            }

            try
            {
                // Prepare the data to be sent
                byte[] buffer = Encoding.UTF8.GetBytes(text + "\n"); 
                await _tcpClient.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send failed: {ex.Message}");
                Disconnect(); // Disconnect on sending error
            }
        }


    }
}
