using Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Core.CrossCuttingConcerns.Logging.Configurations;

public class PostgreSqlConfiguration : ILogConfiguration
{
    public string ConnectionString { get; set; }
    public string TableName { get; set; }
    public bool NeedAutoCreateTable { get; set; }

    public PostgreSqlConfiguration()
    {
        ConnectionString = string.Empty;
        TableName = string.Empty;
    }

    public PostgreSqlConfiguration(string connectionString, string tableName, bool needAutoCreateTable)
    {
        ConnectionString = connectionString;
        TableName = tableName;
        NeedAutoCreateTable = needAutoCreateTable;
    }
}