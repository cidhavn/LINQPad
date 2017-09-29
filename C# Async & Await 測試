// C# Async & Await 測試

// 參考資料
// http://www.huanlintalk.com/2016/01/async-and-await.html
// http://studyhost.blogspot.tw/2012/06/metro-style-appnet-45.html
// http://blog.sanc.idv.tw/2013/07/c-asyncawait.html

// Thread.Sleep vs. Task.Delay
// http://slashlook.com/articles_20160201.html
	
void Main()
{
	//TestAsync();
	TestAsyncWithAwait();
}

public void TestAsync()
{
	Console.WriteLine(1);
	Print(2);
	Console.WriteLine(3);
}

public async Task TestAsyncWithAwait()
{
	Console.WriteLine(1);
	Print(2);
	Console.WriteLine(3);
	await Print(4);
	Console.WriteLine(5);
	Print(6);
	await Print(7);
	Console.WriteLine(8);
}

private async Task Print(int value)
{
	await Task.Delay(3000);
	Console.WriteLine(value);
}
