namespace Binance.Client.Websocket.Subscriptions
{
    /// <summary>
    /// Order book difference subscription.
    /// It will return only difference, you need to load snapshot in advance. 
    /// </summary>
    public class OrderBookDiffSubscription : SimpleSubscriptionBase
    {
        /// <summary>
        /// Diff order book subscription, provide symbol (ethbtc, bnbbtc, etc)
        /// </summary>
        public OrderBookDiffSubscription(string symbol, bool receiveFast = false) : base(symbol)
        {
            ReceiveFast = receiveFast;
        }

        /// <summary>
        /// Switch to enable 100ms update interval
        /// </summary>
        public bool ReceiveFast { get; }

        /// <inheritdoc />
        public override string Channel => "depth";

        /// <inheritdoc />
        public override string StreamName => $"{Symbol}@{Channel}{(ReceiveFast ? "@100ms" : "")}";

    }
}
