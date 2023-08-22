using System.Net;
using Org.BouncyCastle.Tls;
using StackExchange.Redis;

namespace NRedisStack.Tests
{
    public class RedisFixture : IDisposable
    {
        public RedisFixture()
        {
            // Set the enviroment variable to specify your own alternet host and port:
            var redisStandalone = Environment.GetEnvironmentVariable("REDIS") ?? "localhost:6379";
            var redisCluster = Environment.GetEnvironmentVariable("REDIS_CLUSTER");
            var numRedisClusterNodesEnv = Environment.GetEnvironmentVariable("NUM_REDIS_CLUSTER_NODES");
            // bool redisClusterMode = false; // TODO: check if this is needed
            // Redis Cluster
            if (redisCluster != null && numRedisClusterNodesEnv != null)
            {
                // Split to host and port
                string[] parts = redisCluster.Split(':');
                string host = parts[0];
                int startPort = int.Parse(parts[1]);

                var endpoints = new EndPointCollection();
                int numRedisClusterNodes = int.Parse(numRedisClusterNodesEnv);
                for (int i = 0; i < numRedisClusterNodes; i++)
                {
                    endpoints.Add(host, startPort + i);
                }


                ConfigurationOptions clusterConfig = new ConfigurationOptions
                {
                    EndPoints = endpoints
                };
                // redisClusterMode = true;
                Redis = ConnectionMultiplexer.Connect(clusterConfig);
            }

            // Redis Standalone
            else
                Redis = ConnectionMultiplexer.Connect($"{redisStandalone}");
        }

        public void Dispose()
        {
            Redis.Close();
        }

        public ConnectionMultiplexer Redis { get; private set; }
    }
}