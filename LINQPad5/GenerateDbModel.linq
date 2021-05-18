<Query Kind="Program">
  <Connection>
    <ID>0f764c55-8ed0-4186-ab5d-603b9923feb7</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <SqlSecurity>true</SqlSecurity>
    <Database>AdvertiseManagement</Database>
    <UserName>sa</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAv9BorW4o6kivG5/C+u8YegAAAAACAAAAAAAQZgAAAAEAACAAAADgdwbz8hxYSpeqcvYE13z2SwsIVMOyZQysWHk+AtB5CAAAAAAOgAAAAAIAACAAAADxCN9vyuZzKdNwRXhah9RK3nhhNqWh0qBnt1ZHKVQNPxAAAACMZbAEK5wBte8LDt/RbSDvQAAAAM2mqocXEM/505FxQ8qTLyaYvU2pNDAbx40CEqWWNw5j+BjJwyLwjgkcxWJJTaxFyIdYAVciZZv+mWbQwUgV8mM=</Password>
    <NoPluralization>true</NoPluralization>
  </Connection>
</Query>

void Main()
{
	string tableName = "FacebookAccountPerformance";
	
	DumpClass(this.Connection, tableName).Dump();
	GenerateInsertSql(this.Connection, tableName).Dump();
	GenerateUpdateSql(this.Connection, tableName).Dump();
}

private static readonly Dictionary<Type, string> TypeAliases = new Dictionary<Type, string> {
    { typeof(int), "int" },
    { typeof(short), "short" },
    { typeof(byte), "byte" },
    { typeof(byte[]), "byte[]" },
    { typeof(long), "long" },
    { typeof(double), "double" },
    { typeof(decimal), "decimal" },
    { typeof(float), "float" },
    { typeof(bool), "bool" },
    { typeof(string), "string" }
};

private static readonly HashSet<Type> NullableTypes = new HashSet<Type> {
    typeof(int),
    typeof(short),
    typeof(long),
    typeof(double),
    typeof(decimal),
    typeof(float),
    typeof(bool),
    typeof(DateTime),
	typeof(byte),
	typeof(DateTimeOffset),
};

private static Dictionary<string, string> ColumnDesc(IDbConnection conn, string tableName)
{
    var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT objname AS ColumnName, value AS [Description]";
    cmd.CommandText += $"FROM fn_listextendedproperty(NULL, 'schema', 'dbo', 'table', '{tableName}', 'column', default) ";
    cmd.CommandText += "WHERE name = 'MS_Description' AND objtype = 'COLUMN'";

    var reader = cmd.ExecuteReader();
    var result = new Dictionary<string, string>();

    while(reader.Read())
    {
        result.Add(reader.GetString(0), reader.GetString(1));
    }
	
	reader.Close();
	
    return result;
}

private static string DumpClass(IDbConnection conn, string tableName)
{
    if (conn.State != ConnectionState.Open)
    {
        conn.Open();
    }

	// 欄位描述
    Dictionary<string, string> columnDesc = ColumnDesc(conn, tableName);
	
    var cmd = conn.CreateCommand();
    cmd.CommandText = $"SELECT TOP 1 * FROM {tableName}";
    var reader = cmd.ExecuteReader();

    var builder = new StringBuilder();
    do
    {
        if (reader.FieldCount <= 1) continue;

        builder.AppendLine($"public partial class {tableName}");
        builder.AppendLine("{");
        var schema = reader.GetSchemaTable();

        foreach (DataRow row in schema.Rows)
        {
            Type dataType = (Type)row["DataType"];
            string parameterType = TypeAliases.ContainsKey(dataType) ? TypeAliases[dataType] : dataType.Name;
            bool isNullable = (bool)row["AllowDBNull"] && NullableTypes.Contains(dataType);
            string columnName = (string)row["ColumnName"];
			
            if (columnDesc.ContainsKey(columnName))
            {
                builder.AppendLine($"    /// <summary>{columnDesc[columnName]}</summary>");
            }

            builder.AppendLine($"    public {parameterType}{(isNullable ? "?" : string.Empty)} {columnName} {{ get; set; }}");
            builder.AppendLine();
		}

        builder.AppendLine("}");
        builder.AppendLine();
    }
    while (reader.NextResult());
	
	reader.Close();

    return builder.ToString();
}

private static string GenerateInsertSql(IDbConnection conn, string tableName)
{
    if (conn.State != ConnectionState.Open)
    {
        conn.Open();
    }

    var cmd = conn.CreateCommand();
    cmd.CommandText = $"SELECT TOP 1 * FROM {tableName}";
    var reader = cmd.ExecuteReader();

    var builder = new StringBuilder();
    do
    {
        if (reader.FieldCount <= 1) continue;

		var schema = reader.GetSchemaTable();
		var columnNames = schema.Rows.Cast<DataRow>().Select(x => (string)x["ColumnName"]).ToArray();
	
		builder.AppendLine($"INSERT INTO {tableName} (");
		
		for (var i = 0; i < columnNames.Length; i++)
		{
			builder.AppendLine($"    {(i > 0 ? "," : "")}{columnNames[i]}");
		}
		
       	builder.AppendLine($") VALUES (");
		
		for (var i = 0; i < columnNames.Length; i++)
		{
			builder.AppendLine($"    {(i > 0 ? "," : "")}@{columnNames[i]}");
		}
		
		builder.AppendLine($")");
    }
    while (reader.NextResult());

	reader.Close();

    return builder.ToString();
}

private static string GenerateUpdateSql(IDbConnection conn, string tableName)
{
    if (conn.State != ConnectionState.Open)
    {
        conn.Open();
    }

    var cmd = conn.CreateCommand();
    cmd.CommandText = $"SELECT TOP 1 * FROM {tableName}";
    var reader = cmd.ExecuteReader();

    var builder = new StringBuilder();
    do
    {
        if (reader.FieldCount <= 1) continue;

		var schema = reader.GetSchemaTable();
		
		builder.AppendLine($"UPDATE {tableName}");
		builder.Append("SET ");
		
		int count = schema.Rows.Count;
		int index = 0;
		foreach (DataRow row in schema.Rows)
		{
			string columnName = (string)row["ColumnName"];
			
			if (index > 0) {
				builder.Append("    ,");
			}
			
			builder.AppendLine($"{columnName} = @{columnName}");
			index++;
		}
    }
    while (reader.NextResult());

	reader.Close();

    return builder.ToString();
}