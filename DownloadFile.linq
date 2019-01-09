void Main()
{
	string url = "https://shop.lianhwa.com.tw/gmc_product_feed.php?auth=84ab32ea4aabd841a551846b050a873a";
	
	using (var client = new HttpClient())
    {
        //client.DefaultRequestHeaders.Add("authorization", "");
        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using (var response = client.GetAsync(url).Result)
        {
            string result = response.Content.ReadAsStringAsync().Result;
			
			ReadXml(result);
        }
    }
}

private void ReadXml(string xml)
{
	XmlDocument doc = new XmlDocument();
    doc.LoadXml(xml);
	doc.Dump();
	
	var xmlRows = doc.SelectNodes("/rss/channel/title");
	//xmlRows.Dump();
	
	foreach (XmlNode row in xmlRows)
    {
    	//row.Dump();
    }
}
