using System.IO.Compression;
using System.Net.Http.Headers;
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

        var jsonBytes = Encoding.UTF8.GetBytes(json);
        using var ms = new MemoryStream();
        await using var gzip = new GZipStream(ms, CompressionMode.Compress, true);
        gzip.Write(jsonBytes, 0, jsonBytes.Length);
        ms.Seek(0, SeekOrigin.Begin);
        using var content = new StreamContent(ms);
        content.Headers.ContentEncoding.Add("gzip");
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync(url, content);
        return response.IsSuccessStatusCode;
    }
}