using System;
using System.Threading.Tasks;
using Websocket.Client;

namespace Binance.Client.Websocket.Communicator
{
    public interface IBinanceReferenceOrderBookCommunicator : IDisposable
    {
        string SubscribedReferenceOrderBookSymbol { get; }
        ushort LobLimit { get; }
        TimeSpan PollingIntervall { get; }

        void SubscribeToReferenceOrderBook(string symbol, TimeSpan intervall);

        Uri BaseUrl { get; }
        
        bool IsRunning { get; }
        
        string Name { get; set; }
        
        IObservable<ResponseMessage> MessageReceived { get; }
        
        Task Start();

        Task<bool> Stop();
    }
}
