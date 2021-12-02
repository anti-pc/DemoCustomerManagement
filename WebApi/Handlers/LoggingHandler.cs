using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;

namespace WebApi.Handlers
{
    public class LoggingHandler
    {
        private readonly RequestDelegate request;
        private readonly ILogger<LoggingHandler> logger;
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;


        public LoggingHandler(RequestDelegate request, ILogger<LoggingHandler> logger)
        {
            this.request = request;
            this.logger = logger;
            this.recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            await LogRequest(httpContext);
            await LogResponse(httpContext);

            //var originalBodyStream = httpContext.Response.Body;

            //logger.LogInformation($"Query : {string.Join(',', httpContext.Request.QueryString)}");
            //logger.LogInformation($"Header : {string.Join(',', httpContext.Request.Headers.Keys)}");

            //var tmpResponseBody = new MemoryStream();
            //httpContext.Response.Body = tmpResponseBody;

            //await request.Invoke(httpContext);

            //tmpResponseBody.Seek(0, System.IO.SeekOrigin.Begin);
            //string responseTxt = await new StreamReader(tmpResponseBody, Encoding.UTF8).ReadToEndAsync();
            //logger.LogInformation($"Response : {responseTxt}");
            //tmpResponseBody.Seek(0, System.IO.SeekOrigin.Begin);

            //await httpContext.Response.Body.CopyToAsync(originalBodyStream);
        }

        private async Task LogRequest(HttpContext httpContext) 
        {
            httpContext.Request.EnableBuffering();
            await using var requestStream = recyclableMemoryStreamManager.GetStream();
            await httpContext.Request.Body.CopyToAsync(requestStream);
            logger.LogInformation($"Http Request Information:{Environment.NewLine}" +
                                   $"Schema:{httpContext.Request.Scheme} " +
                                   $"Host: {httpContext.Request.Host} " +
                                   $"Path: {httpContext.Request.Path} " +
                                   $"QueryString: {httpContext.Request.QueryString} " +
                                   $"Request Body: {ReadStreamInChunks(requestStream)}");
            httpContext.Request.Body.Position = 0;
        }

        private static string ReadStreamInChunks(Stream stream)
        {
            const int readChunkBufferLength = 4096;
            stream.Seek(0, SeekOrigin.Begin);
            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);
            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;
            do
            {
                readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }

        private async Task LogResponse(HttpContext httpContext) 
        {
            var originalBodyStream = httpContext.Response.Body;
            await using var responseBody = recyclableMemoryStreamManager.GetStream();
            httpContext.Response.Body = responseBody;
            await request(httpContext);
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);

            logger.LogInformation($"Http Response Information:{Environment.NewLine}" +
                                   $"Schema:{httpContext.Request.Scheme} " +
                                   $"Host: {httpContext.Request.Host} " +
                                   $"Path: {httpContext.Request.Path} " +
                                   $"QueryString: {httpContext.Request.QueryString} " +
                                   $"Response Body: {text}");

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
