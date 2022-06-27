using System.Text;
using Newtonsoft.Json;

namespace RunaTiming.Shared.Upload;

public class UploadService
{
    private static readonly HttpClient _httpClient = new();

    public static async Task<bool> UploadResults(string serviceUrl, List<ResultItem> results)
    {
        var url = new Uri(new Uri(serviceUrl), "/api/UploadResult");
        var json = JsonConvert.SerializeObject(results);

        //var jsonBytes = Encoding.UTF8.GetBytes(json);
        //using var ms = new MemoryStream();
        //await using var gzip = new GZipStream(ms, CompressionMode.Compress);
        //await gzip.WriteAsync(jsonBytes);
        //gzip.Flush();
        //ms.Position = 0;
        //using var content = new StreamContent(ms);
        //content.Headers.ContentEncoding.Add("gzip");
        //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        return response.IsSuccessStatusCode;
    }
}