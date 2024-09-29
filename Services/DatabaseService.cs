using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Data;

namespace LibraryManagementSystem.Services
{
    public class DatabaseService
    {
        private readonly LibraryContext _context;

        public DatabaseService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<DataTable> RunStoredProcedureAsync(string storedProcName, Dictionary<string, object> parameters)
        {
            var dataTable = new DataTable();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcName;
                command.CommandType = CommandType.StoredProcedure;

                // Add input parameters to the command
                foreach (var param in parameters)
                {
                    var sqlParam = new SqlParameter(param.Key, param.Value ?? DBNull.Value);
                    command.Parameters.Add(sqlParam);
                }

                // Open connection
                await _context.Database.OpenConnectionAsync();

                // Execute the stored procedure and fill the result into a DataTable
                using (var reader = await command.ExecuteReaderAsync())
                {
                    dataTable.Load(reader);
                }
            }

            return dataTable; // Return the entire DataTable
        }

        public IActionResult CreateErrorResponse(int statusCode, string message, string details = "")
        {
            var errorResponse = new
            {
                error = new
                {
                    code = statusCode,
                    message = message,
                    details = details
                }
            };

            return new ObjectResult(errorResponse)
            {
                StatusCode = statusCode
            };
        }
    }
}
