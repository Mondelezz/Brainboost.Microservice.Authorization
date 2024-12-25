using Serilog;
using System.Diagnostics;

namespace API.Extensions;

// Обработчик позволяет перехватывать трафик между клиентом и службой аутентификации
public class BackChannelListener : DelegatingHandler
{
    public BackChannelListener() : base(new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // HACK: *** NOT PRODUCTION ***
    }) => Log.Logger.ForContext("SourceContext", "BackChannelListener")
                .Information("### BackChannelListener: Constructor called");

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Log.Logger.ForContext("SourceContext", "BackChannelListener")
                .Information("### BackChannelListener: SendAsync method called");

        // HACK: *** NOT PRODUCTION ***
        try
        {
            Stopwatch sw = new();
            sw.Start();

            HttpResponseMessage result = await base.SendAsync(request, cancellationToken);
            sw.Stop();

            Log.Logger.ForContext("SourceContext", "BackChannelListener")
                .Information($"### BackChannel request to {request?.RequestUri?.AbsoluteUri} took {sw.ElapsedMilliseconds} ms");

            if (result.Content != null)
            {
                string responseBody = await result.Content.ReadAsStringAsync(cancellationToken);
                Log.Logger.ForContext("SourceContext", "BackChannelListener")
                    .Information($"### Response Body: {responseBody}");
            }

            return result;
        }
        catch (Exception ex)
        {
            Log.Logger.ForContext("SourceContext", "BackChannelListener")
                .Error("### BackChannelListener: Error occurred: " + ex.Message);
            throw;
        }
    }
}
