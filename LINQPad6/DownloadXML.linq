<Query Kind="Program">
  <Namespace>System.Net.Http</Namespace>
</Query>

void Main()
{
	var client = new HttpClient();

    var response = client.GetAsync("https://www.yoco.com.tw/GoogleFeed/Index").Result;
    string strResponse = response.Content.ReadAsStringAsync().Result;
	
	strResponse.Dump();
	
	XmlDocument xdoc = new XmlDocument();
    xdoc.LoadXml(strResponse);
}
