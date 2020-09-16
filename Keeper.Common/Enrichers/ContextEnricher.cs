using Serilog.Core;
using Serilog.Events;
using System.Linq;

namespace Keeper.Common.Enrichers
{
    public class ContextEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            int max = 20;
            string beginning = string.Empty;
            bool empty = true;
            var val = logEvent.Properties.FirstOrDefault(x => x.Key == "SourceContext");
            if (val.Value != null)
            {
                beginning += val.Value.ToString().Replace("\"", string.Empty);
                if (beginning.Length > max - 2)
                {
                    beginning = beginning.Substring(0, max - 2);
                }
                empty = false;
            }

            string newx = string.Empty;
            if (beginning.Length < max)
            {
                int l = (max - beginning.Length) / 2;
                for (int i = 0; i < l; i++)
                {
                    newx += " ";
                }
            }

            if (empty)
                newx = new string(' ', max - 1);
            else
                newx = newx + beginning + newx;

            if (newx.Length >= max)
                newx = newx.Substring(1, newx.Length - 1);

            var eventType = propertyFactory.CreateProperty("SrcContext", newx);
            logEvent.AddPropertyIfAbsent(eventType);
        }
    }
}
