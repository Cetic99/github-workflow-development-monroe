using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog.Sinks.Http;

namespace CashVault.WebAPI.Extensions
{
    public class SerilogBatchFormatter : IBatchFormatter
    {
        private readonly string _deviceId;

        public SerilogBatchFormatter(string deviceId)
        {
            _deviceId = deviceId;
        }

        public void Format(IEnumerable<string> logEvents, TextWriter output)
        {
            var logsArray = new JArray();

            foreach (var logEvent in logEvents)
            {
                try
                {
                    logsArray.Add(JObject.Parse(logEvent));
                }
                catch (JsonReaderException)
                {
                    logsArray.Add(new JObject { ["raw"] = logEvent });
                }
            }

            var payload = new JObject
            {
                ["deviceId"] = _deviceId,
                ["logs"] = logsArray
            };

            output.Write(payload.ToString(Formatting.None));
        }
    }
}
