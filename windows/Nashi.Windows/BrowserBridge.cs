using System.Buffers.Binary;
using System.IO.Pipes;
using System.Text;
using Nashi.Core;

namespace Nashi.Windows;

internal sealed class BrowserBridge:IDisposable
{
    private const string PipeName="MAGU.Browser.v1";
    private readonly MainWindow window;private readonly CancellationTokenSource lifetime=new();private readonly Task loop;
    internal BrowserBridge(MainWindow window){this.window=window;loop=Task.Run(ListenAsync);}
    private async Task ListenAsync()
    {
        while(!lifetime.IsCancellationRequested)
        {
            try
            {
                using var server=new NamedPipeServerStream(PipeName,PipeDirection.InOut,1,PipeTransmissionMode.Byte,PipeOptions.Asynchronous|PipeOptions.CurrentUserOnly);
                await server.WaitForConnectionAsync(lifetime.Token);var raw=await ReadFrameAsync(server,lifetime.Token);var request=raw is null?null:BrowserProtocol.Deserialize<BrowserRequest>(raw);
                using var sendLock=new SemaphoreSlim(1,1);
                if(request is null||!BrowserProtocol.IsSupportedRequest(request)){await SendAsync(server,BrowserProtocol.Serialize(new BrowserResponse("error",request?.Id??"",false,"Invalid browser request.")),sendLock,lifetime.Token);continue;}
                var progress=new Progress<OperationProgress>(value=>_=SendAsync(server,BrowserProtocol.Serialize(new BrowserResponse("progress",request.Id,Label:value.Label,Fraction:value.Fraction)),sendLock,lifetime.Token));
                var response=await window.Dispatcher.InvokeAsync(()=>window.HandleBrowserRequestAsync(request,progress)).Task.Unwrap();await SendAsync(server,BrowserProtocol.Serialize(response),sendLock,lifetime.Token);
            }
            catch(OperationCanceledException){break;}
            catch(Exception error)when(error is IOException or UnauthorizedAccessException){if(!lifetime.IsCancellationRequested)await Task.Delay(100,lifetime.Token);}
        }
    }
    private static async Task<string?> ReadFrameAsync(Stream stream,CancellationToken token){var header=new byte[4];if(await stream.ReadAsync(header,token)!=4)return null;var length=BinaryPrimitives.ReadInt32LittleEndian(header);if(length is <1 or >1_000_000)throw new InvalidDataException("Invalid browser message length.");var data=new byte[length];await stream.ReadExactlyAsync(data,token);return Encoding.UTF8.GetString(data);}
    private static async Task SendAsync(Stream stream,string value,SemaphoreSlim sendLock,CancellationToken token){await sendLock.WaitAsync(token);try{var data=Encoding.UTF8.GetBytes(value);var header=new byte[4];BinaryPrimitives.WriteInt32LittleEndian(header,data.Length);await stream.WriteAsync(header,token);await stream.WriteAsync(data,token);await stream.FlushAsync(token);}finally{sendLock.Release();}}
    public void Dispose(){lifetime.Cancel();}
}
