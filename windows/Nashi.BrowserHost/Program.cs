using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using Nashi.Core;

const string PipeName="MAGU.Browser.v1";
var input=await ReadNativeMessageAsync(Console.OpenStandardInput());
if(input is null) return 1;
var request=BrowserProtocol.Deserialize<BrowserRequest>(input);
if(request is null||!BrowserProtocol.IsSupportedRequest(request)||!Uri.TryCreate(request.Url,UriKind.Absolute,out var uri)||uri.Host is not ("youtube.com" or "www.youtube.com" or "m.youtube.com" or "music.youtube.com" or "youtu.be"))
{
    await WriteNativeMessageAsync(Console.OpenStandardOutput(),BrowserProtocol.Serialize(new BrowserResponse("error",request?.Id??"",false,"Only public YouTube links are supported by this prototype.")));
    return 2;
}
try
{
    using var pipe=await ConnectAsync();
    await WriteFrameAsync(pipe,input);
    while(true)
    {
        var response=await ReadFrameAsync(pipe);if(response is null)break;
        await WriteNativeMessageAsync(Console.OpenStandardOutput(),response);
        var parsed=BrowserProtocol.Deserialize<BrowserResponse>(response);if(parsed?.Type is "complete" or "error")break;
    }
    return 0;
}
catch(Exception error)
{
    await WriteNativeMessageAsync(Console.OpenStandardOutput(),BrowserProtocol.Serialize(new BrowserResponse("error",request.Id,false,error.Message)));
    return 1;
}

static async Task<NamedPipeClientStream> ConnectAsync()
{
    var pipe=new NamedPipeClientStream(".",PipeName,PipeDirection.InOut,PipeOptions.Asynchronous);
    try{await pipe.ConnectAsync(700);return pipe;}
    catch
    {
        pipe.Dispose();
        var app=Path.Combine(AppContext.BaseDirectory,"Nashi.exe");
        if(File.Exists(app))Process.Start(new ProcessStartInfo(app,"--background"){UseShellExecute=false,CreateNoWindow=true});
        pipe=new NamedPipeClientStream(".",PipeName,PipeDirection.InOut,PipeOptions.Asynchronous);await pipe.ConnectAsync(5000);return pipe;
    }
}
static async Task<string?> ReadNativeMessageAsync(Stream stream)
{
    var header=new byte[4];if(await stream.ReadAsync(header)!=4)return null;var length=BinaryPrimitives.ReadInt32LittleEndian(header);if(length is <1 or >1_000_000)throw new InvalidDataException("Invalid native message length.");var data=new byte[length];await stream.ReadExactlyAsync(data);return Encoding.UTF8.GetString(data);
}
static async Task WriteNativeMessageAsync(Stream stream,string value)
{
    var data=Encoding.UTF8.GetBytes(value);var header=new byte[4];BinaryPrimitives.WriteInt32LittleEndian(header,data.Length);await stream.WriteAsync(header);await stream.WriteAsync(data);await stream.FlushAsync();
}
static async Task WriteFrameAsync(Stream stream,string value)=>await WriteNativeMessageAsync(stream,value);
static async Task<string?> ReadFrameAsync(Stream stream){var header=new byte[4];if(await stream.ReadAsync(header)!=4)return null;var length=BinaryPrimitives.ReadInt32LittleEndian(header);if(length is <1 or >1_000_000)throw new InvalidDataException("Invalid browser response length.");var data=new byte[length];await stream.ReadExactlyAsync(data);return Encoding.UTF8.GetString(data);}
