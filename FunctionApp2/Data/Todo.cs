using System;
using System.Data.SqlClient;

namespace FunctionApp2.Data
{
    public class Todo
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public DateTime? DueOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public Todo()
        {
            this.Id = Guid.NewGuid();
            this.CreatedOn = DateTime.UtcNow;
        }

        public Todo(
            SqlDataReader sqlDataReader)
        {
            this.Id = sqlDataReader.GetGuid(0);
            this.Status = sqlDataReader.GetString(1);
            this.Description = sqlDataReader.GetString(2);

            if (!sqlDataReader.IsDBNull(3)) this.DueOn = sqlDataReader.GetDateTime(3);

            this.CreatedOn = sqlDataReader.GetDateTime(4);
        }
    }
}
