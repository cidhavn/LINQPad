<Query Kind="Program" />

void Main()
{
	string value = "https://graph.facebook.com:443/v8.0/act_193925888646040/campaigns?fields=insights.limit(1000).default_summary(True).time_range(%7b%27since%27%3a%272021-01-15%27%2c%27until%27%3a%272021-01-21%27%7d).time_increment(1)%7bactions%2caction_values%2cclicks%2cdate_start%2cdate_stop%2cimpressions%2creach%2cspend%7d%2cid%2cname%2cobjective&limit=120&filtering=%5b%7bfield%3a%27impressions%27%2coperator%3a%27GREATER_THAN%27%2cvalue%3a%270%27%7d%5d&time_range=%7b%27since%27%3a%272021-01-15%27%2c%27until%27%3a%272021-01-21%27%7d&time_increment=all_days";
	
	System.Net.WebUtility.UrlDecode(value).Dump();
	//System.Web.HttpUtility.UrlDecode(value).Dump();
}

