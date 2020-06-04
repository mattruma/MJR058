using ClassLibrary1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public static class TodoAdd
    {
        // https://docs.microsoft.com/en-us/azure/azure-functions/manage-connections
        // https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?tabs=csharp

        [FunctionName(nameof(TodoAdd))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todos")] TodoAddOptions todoAddOptions,
            ILogger log)
        {
            log.LogInformation($"{nameof(TodoAdd)} function processed a request.");

            if (string.IsNullOrWhiteSpace(todoAddOptions.Status))
            {
                return new BadRequestObjectResult("'status' is required.");
            }

            if (string.IsNullOrWhiteSpace(todoAddOptions.Description))
            {
                return new BadRequestObjectResult("'description' is required.");
            }

            var todo =
                new Todo
                {
                    Status = todoAddOptions.Status,
                    Description = todoAddOptions.Description,
                    GitHubId = todoAddOptions.GitHubId,
                    DueOn = todoAddOptions.DueOn
                };

            using var cn = new SqlConnection(
                Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING"));
            {
                // await _tokenProvider.SetTokenAsync(cn);

                var query = "INSERT INTO Todos VALUES (@Id, @Status, @Description, @GitHubId, @DueOn, @CreatedOn)";

                using var cmd = new SqlCommand(query, cn);
                {
                    cn.Open();

                    cmd.Parameters.AddWithValue("@Id", todo.Id);
                    cmd.Parameters.AddWithValue("@Status", todo.Status);
                    cmd.Parameters.AddWithValue("@GitHubId", todo.GitHubId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", todo.Description);
                    cmd.Parameters.AddWithValue("@DueOn", todo.DueOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", todo.CreatedOn);

                    await cmd.ExecuteNonQueryAsync();
                }
            }

            return new OkObjectResult(todo);
        }
    }
}
