using ClassLibrary1;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FunctionApp2
{
    public static class TodoAdd
    {
        [FunctionName(nameof(TodoAdd))]
        public async static Task Run(
            [QueueTrigger("%QUEUESTORAGE_TODOADDQUEUENAME%", Connection = "QUEUESTORAGE_CONNECTIONSTRING")] TodoAddOptions todoAddOptions,
            ILogger log,
            [Queue("%QUEUESTORAGE_TODOADDEDQUEUENAME%", Connection = "QUEUESTORAGE_CONNECTIONSTRING")] IAsyncCollector<Todo> todoAysncCollector)
        {
            log.LogInformation($"{nameof(TodoAdd)} function processed a request.");

            if (string.IsNullOrWhiteSpace(todoAddOptions.Status))
            {
                throw new ValidationException("'status' is required.");
            }

            if (string.IsNullOrWhiteSpace(todoAddOptions.Description))
            {
                throw new ValidationException("'description' is required.");
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

                var query = "INSERT INTO Todos VALUES (@Id, @Status, @Description, @GitHubId, @DueOn, @CompletedOn, @CreatedOn)";

                using var cmd = new SqlCommand(query, cn);
                {
                    cn.Open();

                    cmd.Parameters.AddWithValue("@Id", todo.Id);
                    cmd.Parameters.AddWithValue("@Status", todo.Status);
                    cmd.Parameters.AddWithValue("@Description", todo.Description);
                    cmd.Parameters.AddWithValue("@GitHubId", todo.GitHubId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DueOn", todo.DueOn ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompletedOn", (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedOn", todo.CreatedOn);

                    await cmd.ExecuteNonQueryAsync();
                }

                await todoAysncCollector.AddAsync(todo);
            }
        }
    }
}
