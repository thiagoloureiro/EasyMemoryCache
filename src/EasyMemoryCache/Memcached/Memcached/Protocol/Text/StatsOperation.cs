using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EasyMemoryCache.Memcached.Logging;
using EasyMemoryCache.Memcached.Memcached.Results;
using EasyMemoryCache.Memcached.Memcached.Results.Extensions;

namespace EasyMemoryCache.Memcached.Memcached.Protocol.Text
{
    public class StatsOperation : Operation, IStatsOperation
    {
        private static ILog log = LogManager.GetLogger(typeof(StatsOperation));

        private readonly string type;
        private Dictionary<string, string> result;

        public StatsOperation(string type)
        {
            this.type = type;
        }

        protected internal override IList<ArraySegment<byte>> GetBuffer()
        {
            var command = String.IsNullOrEmpty(this.type)
                            ? "stats" + TextSocketHelper.CommandTerminator
                            : "stats " + this.type + TextSocketHelper.CommandTerminator;

            return TextSocketHelper.GetCommandBuffer(command);
        }

        protected internal override IOperationResult ReadResponse(PooledSocket socket)
        {
            var serverData = new Dictionary<string, string>();

            while (true)
            {
                string line = TextSocketHelper.ReadResponse(socket);

                // stat values are terminated by END
                if (String.Compare(line, "END", StringComparison.Ordinal) == 0)
                    break;

                // expected response is STAT item_name item_value
                if (line.Length < 6 || String.Compare(line, 0, "STAT ", 0, 5, StringComparison.Ordinal) != 0)
                {
                    if (log.IsWarnEnabled)
                        log.Warn("Unknow response: " + line);

                    continue;
                }

                // get the key&value
                string[] parts = line.Remove(0, 5).Split(' ');
                if (parts.Length != 2)
                {
                    if (log.IsWarnEnabled)
                        log.Warn("Unknow response: " + line);

                    continue;
                }

                // store the stat item
                serverData[parts[0]] = parts[1];
            }

            this.result = serverData;

            return new TextOperationResult().Pass();
        }

        Dictionary<string, string> IStatsOperation.Result
        {
            get { return result; }
        }

        protected internal override ValueTask<IOperationResult> ReadResponseAsync(PooledSocket socket)
        {
            var serverData = new Dictionary<string, string>();

            while (true)
            {
                string line = TextSocketHelper.ReadResponse(socket);

                // stat values are terminated by END
                if (String.Compare(line, "END", StringComparison.Ordinal) == 0)
                    break;

                // expected response is STAT item_name item_value
                if (line.Length < 6 || String.Compare(line, 0, "STAT ", 0, 5, StringComparison.Ordinal) != 0)
                {
                    if (log.IsWarnEnabled)
                        log.Warn("Unknow response: " + line);

                    continue;
                }

                // get the key&value
                string[] parts = line.Remove(0, 5).Split(' ');
                if (parts.Length != 2)
                {
                    if (log.IsWarnEnabled)
                        log.Warn("Unknow response: " + line);

                    continue;
                }

                // store the stat item
                serverData[parts[0]] = parts[1];
            }

            this.result = serverData;

            return new ValueTask<IOperationResult>(new TextOperationResult().Pass());
        }

        protected internal override Task<bool> ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
        {
            throw new System.NotSupportedException();
        }
    }
}