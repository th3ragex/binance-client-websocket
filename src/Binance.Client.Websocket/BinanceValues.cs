using System;

namespace Binance.Client.Websocket
{
    /// <summary>
    /// Binance static urls
    /// </summary>
    public static class BinanceValues
    {
        /// <summary>
        /// Main Binance url to websocket API
        /// </summary>
        public static readonly Uri ApiWebsocketUrl = new Uri("wss://stream.binance.com:9443");

        /// <summary>
        /// Binance reference order book URL.
        /// </summary>
        public static readonly Uri ReferenceLimitOrderBookUrl = new Uri("https://www.binance.com/api/v1/depth");
    }
}
