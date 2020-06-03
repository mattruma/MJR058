using Microsoft.Azure.Services.AppAuthentication;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FunctionApp1.Helpers
{
    public class TokenProvider : ITokenProvider
    {
        public static string _accessToken;

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrWhiteSpace(_accessToken))
            {
                return _accessToken;
            }

            return await this.RefreshTokenAsync();
        }

        public async Task SetTokenAsync(
            SqlConnection connection)
        {
            var sqlConnectionStringBuilder =
                new SqlConnectionStringBuilder(connection.ConnectionString);

            if (string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.UserID)
                && string.IsNullOrWhiteSpace(sqlConnectionStringBuilder.Password))
            {
                connection.AccessToken = await GetTokenAsync();
            }
        }

        public async Task<string> RefreshTokenAsync()
        {
            var tokenProvider =
                new AzureServiceTokenProvider();

            _accessToken =
                await tokenProvider.GetAccessTokenAsync("https://database.windows.net/");

            return _accessToken;
        }
    }
}
