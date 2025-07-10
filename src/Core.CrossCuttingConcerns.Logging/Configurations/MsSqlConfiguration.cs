using Core.CrossCuttingConcerns.Logging.Abstraction;

namespace Core.CrossCuttingConcerns.Logging.Configurations;

public class MsSqlConfiguration : ILogConfiguration
{
    public string ConnectionString { get; set; }
    public string TableName { get; set; }
    public bool AutoCreateSqlTable { get; set; }
    public List<MsSqlColumnDefinition> AdditionalColumns { get; set; }

    public MsSqlConfiguration()
    {
        ConnectionString = string.Empty;
        TableName = string.Empty;
        AdditionalColumns = [];
    }

    public MsSqlConfiguration(string connectionString, string tableName, bool autoCreateSqlTable, List<MsSqlColumnDefinition> additionalColumns)
    {
        ConnectionString = connectionString;
        TableName = tableName;
        AutoCreateSqlTable = autoCreateSqlTable;
        AdditionalColumns = additionalColumns;       
    }
}

public class MsSqlColumnDefinition
{
    public string Name { get; set; } = string.Empty;
    public string SqlDbType { get; set; } = "NVarChar"; // default
    public int? Length { get; set; } = null;
}