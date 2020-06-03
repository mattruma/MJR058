using System;

namespace FunctionApp2
{
    public class TodoAddOptions
    {
        public string Status { get; set; }

        public string Description { get; set; }

        public DateTime? DueOn { get; set; }
    }
}
