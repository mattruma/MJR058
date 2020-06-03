using System.Data.SqlClient;
using System.Threading.Tasks;

namespace FunctionApp1.Helpers
{
    public interface ITokenProvider

    {
        Task<string> GetTokenAsync();

        Task SetTokenAsync(
            SqlConnection connection);

        Task<string> RefreshTokenAsync();

    }
}
