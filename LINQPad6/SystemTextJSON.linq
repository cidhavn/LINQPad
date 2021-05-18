<Query Kind="Program">
  <Namespace>System.Text.Json</Namespace>
  <Namespace>System.Text.Json.Serialization</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

void Main()
{
	//Test1();
	Test2();
}

private void Test1()
{
	//string value = "{\"172285523330146\":[{\"type\":\"ads_insights\",\"call_count\":1,\"total_cputime\":1,\"total_time\":1,\"estimated_time_to_regain_access\":0}]}";
	//JsonSerializer.Deserialize<Dictionary<string, List<BusinessUseCaseUsageHeaderResponse>>>(value).Dump();
	
	var dic = new Dictionary<string, byte>();
	var makeup = new List<byte>();
	DateTime today = DateTime.Now.Date;
	for (var i = 0; i < 730; i++)
	{
		dic.Add(today.AddDays(i).ToString("yyyy-MM-dd"), 100);
		makeup.Add(100);
	}
	string json = JsonSerializer.Serialize(dic);
	string json2 = JsonSerializer.Serialize(makeup);
	json.Length.Dump();
	json2.Length.Dump();
}

private void Test2()
{
	var options = new JsonSerializerOptions()
    {
        // 處理中文會被 Encode 的問題
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        // 排板顯示縮排和空白字元
        WriteIndented = true,
        IgnoreNullValues = true
    };
	
	JsonSerializer.Serialize(new BusinessUseCaseUsageHeaderResponse(), options).Dump();
}

internal class BusinessUseCaseUsageHeaderResponse
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("call_count")]
    public int CallCount { get; set; }

    [JsonPropertyName("total_cputime")]
    public int TotalCpuTime { get; set; }

    [JsonPropertyName("total_time")]
    public int TotalTime { get; set; }

    [JsonPropertyName("estimated_time_to_regain_access")]
    public int EstimatedTimeToRegainAccess { get; set; }
}
