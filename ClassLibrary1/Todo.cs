using System;
using System.Data.SqlClient;

namespace ClassLibrary1
{
    public class Todo
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public DateTime? DueOn { get; set; }

        public string GitHubId { get; set; }

        public DateTime? CompletedOn { get; set; }

        public DateTime CreatedOn { get; set; }

        public Todo()
        {
            this.Id = Guid.NewGuid();
            this.CreatedOn = DateTime.UtcNow;
        }

        public Todo(
            SqlDataReader sqlDataReader)
        {
            this.Id = sqlDataReader.GetGuid(sqlDataReader.GetOrdinal("Id"));
            this.Status = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Status"));
            this.Description = sqlDataReader.GetString(sqlDataReader.GetOrdinal("Description"));

            if (!sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("DueOn")))
            {
                this.DueOn = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("DueOn"));
            }

            if (!sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("GitHubId")))
            {
                this.GitHubId = sqlDataReader.GetString(sqlDataReader.GetOrdinal("GitHubId"));
            }

            if (!sqlDataReader.IsDBNull(sqlDataReader.GetOrdinal("CompletedOn")))
            {
                this.CompletedOn = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("CompletedOn"));
            }

            this.CreatedOn = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("CreatedOn"));
        }
    }
}
