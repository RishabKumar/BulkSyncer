using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BulkSyncer.RateLimiter
{
    class TokenBucket
    {
        private Queue<DateTime> _tokens;
        private DateTime _lastRefillTime;
        private int _maxTokens;
        private readonly Queue<DateTime> dateTimes;
        private int _refillCount;
        private int _refillFrequency = 5;
        public bool HasTokens => _tokens.Count > 0 || dateTimes.Count > 0;

        public TokenBucket(int maxTokens, Queue<DateTime> dateTimes, bool preventBoundary = false)
        {
            _tokens = new Queue<DateTime>();
            _lastRefillTime = DateTime.Now;
            _maxTokens = maxTokens;
            this.dateTimes = dateTimes;
            if (preventBoundary && _maxTokens * _refillFrequency % 60 == 0)
            {
                maxTokens--;
            }
            _refillCount = (int)((maxTokens / 60f) * _refillFrequency);   // refill in 1 second * 5
            
            for (var i = 0; i < _refillCount; i++)
            {
                _tokens.Enqueue(this.dateTimes.Dequeue());
            }
        }

        public async Task Refill()
        {
            var elapsedSeconds = (int)(DateTime.Now - _lastRefillTime).TotalSeconds;
            var refillCount = Math.Min((elapsedSeconds / _refillFrequency * _refillCount), dateTimes.Count);
            var remainingTokens = _maxTokens - _tokens.Count;
            refillCount = Math.Min(refillCount, remainingTokens);
            if (refillCount > 0)
            {
                for (var i = 0; i < refillCount; i++)
                {
                    _tokens.Enqueue(dateTimes.Dequeue());
                }
                _lastRefillTime = DateTime.Now;
            }
            else
            {
                await Task.Delay(500);
            }
        }

        public IEnumerable<DateTime> ConsumeToken()
        {
            Console.WriteLine(_tokens.Count());
            while (_tokens.Count > 0)
            {
                yield return _tokens.Dequeue();
            }
        }
    }
}