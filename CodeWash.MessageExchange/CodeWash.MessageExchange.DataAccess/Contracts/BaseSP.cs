using Microsoft.Data.SqlClient;

namespace CodeWash.MessageExchange.DataAccess.Contracts;

public abstract class BaseSP
{
    public abstract string ProcedureName { get; }
    public abstract Dictionary<string, object> Parameters { get; }

    protected void SetParameters(SqlCommand command)
    {
        foreach (var parameter in Parameters ?? [])
        {
            command.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }
    }
}
