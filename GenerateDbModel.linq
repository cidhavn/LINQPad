void Main()
{
	DumpClass(this.Connection, "Category").Dump();
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
    typeof(DateTime)
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

    return builder.ToString();
}
