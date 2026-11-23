using System.Text.Json;

namespace Nashi.Core;

public sealed record BrowserRequest(string Type,string Id,string Url,string? Kind=null,int? MaximumHeight=null);
public sealed record BrowserResponse(string Type,string Id,bool Ok=true,string? Error=null,string? Title=null,int[]? Heights=null,string[]? Formats=null,double? Fraction=null,string? Label=null,string[]? Outputs=null);

public static class BrowserProtocol
{
    public static string Serialize<T>(T value)=>JsonSerializer.Serialize(value);
    public static T? Deserialize<T>(string json)=>JsonSerializer.Deserialize<T>(json);
    public static bool IsSupportedRequest(BrowserRequest request)=>request.Type is "inspect" or "download"&&request.Id.Length is >0 and <=128&&request.Url.Length is >0 and <=32768;
}
