using FunctionApp1.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public static class TodoFetchById
    {
        [FunctionName(nameof(TodoFetchById))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos/{id}")] HttpRequest req,
            string id,
            ILogger log)
        {
            log.LogInformation($"{nameof(TodoFetchById)} function processed a request.");

            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult("'id' is required.");
            }

            Todo todo;

            using var cn = new SqlConnection(
                Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING"));
            {
                // await _tokenProvider.SetTokenAsync(cn);

                var query = "SELECT * FROM Todos WHERE Id = @Id";

                using var cmd = new SqlCommand(query, cn);
                {
                    cn.Open();

                    cmd.Parameters.AddWithValue("@Id", id);

                    var sqlDataReader =
                        await cmd.ExecuteReaderAsync();

                    if (!sqlDataReader.Read())
                    {
                        return new NotFoundResult();
                    }

                    todo =
                        new Todo(
                            sqlDataReader);
                }
            }

            return new OkObjectResult(todo);
        }
    }
}
