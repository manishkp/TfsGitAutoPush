// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HtppClientSmartSubtransportStream.cs" company="">
//   https://github.com/gasparnagy/Sample_NtlmGitTest.git
// </copyright>
// <summary>
//   The htpp client smart subtransport stream.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsGitPush
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net.Http;

    using LibGit2Sharp;

    /// <summary>
    /// The http client smart subtransport stream.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class HttpClientSmartSubtransportStream : SmartSubtransportStream
    {
        /// <summary>
        /// The http client.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// The service uri.
        /// </summary>
        private readonly Uri serviceUri;

        /// <summary>
        /// The post content type.
        /// </summary>
        private readonly string postContentType;

        /// <summary>
        /// The response stream.
        /// </summary>
        private Stream responseStream;

        /// <summary>
        /// The request stream.
        /// </summary>
        private MemoryStream requestStream = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientSmartSubtransportStream"/> class.
        /// </summary>
        /// <param name="smartSubtransport">
        /// The smart subtransport.
        /// </param>
        /// <param name="httpClient">
        /// The http client.
        /// </param>
        /// <param name="serviceUri">
        /// The service uri.
        /// </param>
        /// <param name="postContentType">
        /// The post content type.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public HttpClientSmartSubtransportStream(SmartSubtransport smartSubtransport, HttpClient httpClient, Uri serviceUri, string postContentType)
            : base(smartSubtransport)
        {
            this.httpClient = httpClient;
            this.serviceUri = serviceUri;
            this.postContentType = postContentType;
        }
    
        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="dataStream">
        /// The data stream.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="bytesRead">
        /// The bytes read.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Read(Stream dataStream, long length, out long bytesRead)
        {
            bytesRead = 0L;
            var buffer = new byte[65536];
            this.EnsureResponseStream();

            int count;
            while (length > 0 && (count = this.responseStream.Read(buffer, 0, (int)Math.Min(buffer.Length, length))) > 0)
            {
                dataStream.Write(buffer, 0, count);
                bytesRead += (long)count;
                length -= (long)count;
            }

            return 0;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="dataStream">
        /// The data stream.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int Write(Stream dataStream, long length)
        {
            if (this.requestStream == null)
            {
                this.requestStream = new MemoryStream();
            }

            var buffer = new byte[65536];
            int count;
            while (length > 0 && (count = dataStream.Read(buffer, 0, (int)Math.Min(buffer.Length, length))) > 0)
            {
                this.requestStream.Write(buffer, 0, count);
                length -= (long)count;
            }

            return 0;
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        protected override void Dispose()
        {
            if (this.responseStream != null)
            {
                this.responseStream.Dispose();
            }

            if (this.requestStream != null)
            {
                this.requestStream.Dispose();
            }

            base.Dispose();
        }

        /// <summary>
        /// The ensure response stream.
        /// </summary>
        private void EnsureResponseStream()
        {
            if (this.responseStream != null)
            {
                return;
            }

            HttpResponseMessage result;

            // we also have something to send
            if (this.requestStream != null)
            {
                this.requestStream.Seek(0, SeekOrigin.Begin);
                var streamContent = new StreamContent(this.requestStream);

                var request = new HttpRequestMessage(HttpMethod.Post, this.serviceUri);
                if (!string.IsNullOrEmpty(this.postContentType))
                {
                    streamContent.Headers.Add("Content-Type", this.postContentType);
                }

                request.Content = streamContent;
                result = this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).Result;
            }
            else
            {
                result = this.httpClient.GetAsync(this.serviceUri, HttpCompletionOption.ResponseHeadersRead).Result;
            }

            this.responseStream = result.EnsureSuccessStatusCode().Content.ReadAsStreamAsync().Result;
        }
    }
}