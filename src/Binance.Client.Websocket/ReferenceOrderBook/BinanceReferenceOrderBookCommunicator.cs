using Binance.Client.Websocket.Communicator;
using Binance.Client.Websocket.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace Binance.Client.Websocket.ReferenceOrderBook
{
    public class BinanceReferenceOrderBookCommunicator : IBinanceReferenceOrderBookCommunicator
    {
        private static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        private readonly HttpClient _client = new HttpClient();
        private Subject<ResponseMessage> _messageReceivedSubject = new Subject<ResponseMessage>();
        private CancellationTokenSource _cts;
        private Task _task;
        private bool disposedValue;

        public string SubscribedReferenceOrderBookSymbol { get; private set; }
        public ushort LobLimit { get; } = 5000; // Only specific values are allowed: 5, 10, 20, 50, 100, 500, 1000, 5000
        public TimeSpan PollingIntervall { get; private set; }

        public Uri BaseUrl { get; }

        public bool IsRunning { get; private set; }

        public string Name { get; set; }

        public IObservable<ResponseMessage> MessageReceived => _messageReceivedSubject.AsObservable();

        public BinanceReferenceOrderBookCommunicator(Uri baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public async Task Start()
        {
            if (string.IsNullOrWhiteSpace(SubscribedReferenceOrderBookSymbol))
                throw new Exception("No symbol set. Please subscribe first.");

            if (IsRunning)
                return;
            IsRunning = true;

            _cts = new CancellationTokenSource();

            _task = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                var token = _cts.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var referenceLob = await LoadReferenceOrderBook(SubscribedReferenceOrderBookSymbol, LobLimit);
                        _messageReceivedSubject.OnNext(ResponseMessage.TextMessage(referenceLob));
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Loading reference orderbook failed.");
                    }

                    await Task.Delay(PollingIntervall, token);
                }
            });
        }

        public async Task<bool> Stop()
        {
            if (!IsRunning)
                return true;

            IsRunning = false;

            _cts?.Cancel();
            try
            {
                await _task;
            }
            catch { /* don't care for cancelled exceptions  */ };

            return true;
        }

        public void SubscribeToReferenceOrderBook(string symbol, TimeSpan intervall)
        {
            SubscribedReferenceOrderBookSymbol = symbol;
            PollingIntervall = intervall;
        }

        private async Task<string> LoadReferenceOrderBook(string symbol, ushort limit)
        {
            var url = $"{BaseUrl}?symbol={symbol.ToUpper()}&limit={limit}";

            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", "DataOctopus");

            var referenceOrderBookJson = await _client.GetStringAsync(url);
            var m = $"{{\"rt\": {DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()},\"stream\": \"{symbol}@referencedepth\",\"s\":\"{symbol}\",\"data\":{referenceOrderBookJson}}}";
            return m;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _cts?.Cancel();
                    _cts?.Dispose();
                    _client?.Dispose();
                    _messageReceivedSubject?.Dispose();
                }

                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ReferenceOrderBookCommunicator()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
