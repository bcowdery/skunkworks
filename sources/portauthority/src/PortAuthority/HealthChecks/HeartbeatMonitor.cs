using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace PortAuthority.HealthChecks
{
    /// <summary>
    /// Monitors heartbeats of worker services 
    /// </summary>
    public class HeartbeatMonitor
        : IHeartbeatMonitor
    {
        private readonly ILogger<HeartbeatMonitor> _logger;
        private readonly ConcurrentDictionary<Uri, DateTimeOffset> _heartbeats = new ConcurrentDictionary<Uri, DateTimeOffset>();

        public HeartbeatMonitor(ILogger<HeartbeatMonitor> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns true if all workers are healthy and have reported a heartbeat within the given threshold.
        /// </summary>
        /// <param name="threshold"></param>
        /// <param name="maxAge"></param>
        /// <param name="heartbeats"></param>
        /// <returns></returns>
        public bool CheckHeartbeats(TimeSpan threshold, TimeSpan maxAge, out List<HeartbeatStatus> heartbeats)
        {
            heartbeats = new List<HeartbeatStatus>();
            
            if (_heartbeats.Count == 0)
            {
                _logger.LogWarning("No heartbeats recorded from any worker!");
                return false;
            }
         
            bool isHealthy = true;
            var expired = new List<Uri>();
            var now = DateTimeOffset.UtcNow;
            
            foreach (var entry in _heartbeats)
            {
                var sourceAddress = entry.Key;
                var timestamp = entry.Value;
                var age = (now - timestamp);
                var isAlive = age < threshold;
                var isExpired = age > maxAge;
                
                _logger.LogDebug("Worker {SourceAddress} Age = {Age}, IsAlive = {IsAlive}, IsExpired = {IsExpired}",
                sourceAddress,
                    age,
                    isAlive,
                    isExpired);
                
                if (isExpired)
                {
                    expired.Add(sourceAddress);
                    continue;
                }
                
                isHealthy = isAlive && isHealthy;
                heartbeats.Add(new HeartbeatStatus(sourceAddress, age, isAlive));
            }

            foreach (var sourceAddress in expired)
            {
                _logger.LogDebug("Worker {SourceAddress} has expired and is being removed.", sourceAddress);
                _heartbeats.TryRemove(sourceAddress, out var _);
            }
            
            _logger.LogDebug("Worker status is {Status}", isHealthy ? "Healthy" : "Degraded");
            
            return isHealthy;
        }
        
        /// <summary>
        /// Adds a heartbeat for a service endpoint
        /// </summary>
        /// <param name="sourceAddress"></param>
        /// <param name="heartbeat"></param>
        public void AddHeartbeat(Uri sourceAddress, DateTimeOffset heartbeat)
        {
            _logger.LogDebug("Adding heartbeat for {SourceAddress} {Timestamp}", sourceAddress, heartbeat);
            _heartbeats[sourceAddress] = heartbeat;
        }

        /// <summary>
        /// Removes a heartbeat for a service endpoint
        /// </summary>
        /// <param name="sourceAddress"></param>
        public void RemoveHeartbeat(Uri sourceAddress)
        {
            _logger.LogDebug("Removing heartbeat monitoring for {SourceAddress}", sourceAddress);
            _heartbeats.Remove(sourceAddress, out _);
        }
    }
    
    /// <summary>
    /// Heartbeat status
    /// </summary>
    public struct HeartbeatStatus
    {
        public HeartbeatStatus(Uri sourceAddress, TimeSpan age, bool isAlive)
        {
            SourceAddress = sourceAddress;
            Age = age;
            IsAlive = isAlive;
        }

        /// <summary>
        /// Source address of the heartbeat
        /// </summary>
        public Uri SourceAddress { get; }
        
        /// <summary>
        /// Age of the entry, how long ago was the last heartbeat recorded.
        /// </summary>
        public TimeSpan Age { get; }
        
        /// <summary>
        /// True if endpoint is alive, false if heartbeat is outside of the configured threshold
        /// </summary>
        public bool IsAlive { get; }
    }
}
