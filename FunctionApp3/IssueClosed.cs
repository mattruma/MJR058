using ClassLibrary1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp3
{
    public static class IssueClosed
    {
        [FunctionName("IssueClosed")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(IssueClosed)} function processed a request.");

            var issueClosedOptions =
                JsonConvert.DeserializeObject<IssueClosedOptions>(
                    await new StreamReader(req.Body).ReadToEndAsync());

            if (issueClosedOptions.Action != "closed")
            {
                return new OkResult();
            }

            Todo todo;

            using var cn = new SqlConnection(
                Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING"));
            {
                SqlCommand cmd;

                var query = "SELECT * FROM Todos WHERE GitHubId = @GitHubId";

                cmd = new SqlCommand(query, cn);

                cn.Open();

                cmd.Parameters.AddWithValue("@GitHubId", issueClosedOptions.Issue.Id);

                var sqlDataReader =
                    await cmd.ExecuteReaderAsync();

                if (!sqlDataReader.Read())
                {
                    return new NotFoundResult();
                }

                todo =
                    new Todo(
                        sqlDataReader);

                todo.Status = "completed";
                todo.CompletedOn = issueClosedOptions.Issue.ClosedOn;

                cn.Close();

                query = "UPDATE Todos SET Status = @Status, CompletedOn = @CompletedOn WHERE Id = @Id";

                cmd = new SqlCommand(query, cn);

                cn.Open();

                cmd.Parameters.AddWithValue("@Id", todo.Id);
                cmd.Parameters.AddWithValue("@Status", todo.Status);
                cmd.Parameters.AddWithValue("@CompletedOn", todo.CompletedOn);

                await cmd.ExecuteNonQueryAsync();

                cn.Close();
            }

            return new OkObjectResult(issueClosedOptions);
        }
    }
}
