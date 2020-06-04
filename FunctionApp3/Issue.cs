using Newtonsoft.Json;
using System;

namespace FunctionApp3
{
    public class Issue
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string State { get; set; }

        [JsonProperty("closed_at")]
        public DateTime? ClosedOn { get; set; }
    }
}
