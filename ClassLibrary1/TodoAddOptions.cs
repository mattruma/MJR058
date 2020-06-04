using System;

namespace ClassLibrary1
{
    public class TodoAddOptions
    {
        public string Status { get; set; }

        public string Description { get; set; }

        public string GitHubId { get; set; }

        public DateTime? DueOn { get; set; }
    }
}
