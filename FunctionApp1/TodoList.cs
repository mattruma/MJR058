using ClassLibrary1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public static class TodoList
    {
        [FunctionName(nameof(TodoList))]
        [SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "<Pending>")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"{nameof(TodoList)} function processed a request.");

            if (!int.TryParse(req.Query["page"], out var page)) page = 1;
            if (!int.TryParse(req.Query["pageSize"], out var pageSize)) pageSize = 20;

            var todoList =
                new List<Todo>();

            using var cn = new SqlConnection(
                Environment.GetEnvironmentVariable("SQL_CONNECTIONSTRING"));
            {
                // await _tokenProvider.SetTokenAsync(cn);

                var query = $"SELECT * FROM Todos ORDER BY DueOn OFFSET {(page - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

                using var cmd = new SqlCommand(query, cn);
                {
                    cn.Open();

                    var sqlDataReader = await cmd.ExecuteReaderAsync();

                    while (await sqlDataReader.ReadAsync())
                    {
                        todoList.Add(
                            new Todo(
                                sqlDataReader));
                    }
                }
            }

            return new OkObjectResult(todoList);
        }
    }
}
