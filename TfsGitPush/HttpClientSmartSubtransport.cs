// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientSmartSubtransport.cs">
//   https://github.com/gasparnagy/Sample_NtlmGitTest.git
// </copyright>
// <summary>
//   The http client smart subtransport.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsGitPush
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;

    using LibGit2Sharp;

    /// <summary>
    /// The http client smart subtransport.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    [RpcSmartSubtransport]
    public class HttpClientSmartSubtransport : SmartSubtransport
    {
        /// <summary>
        /// The http client.
        /// </summary>
        private HttpClient httpClient = null;

        /// <summary>
        /// The action override
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <param name="action">
        /// The action.
        /// </param>
        /// <returns>
        /// The <see cref="SmartSubtransportStream"/> smart stream
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Unsupported Git action
        /// </exception>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        protected override SmartSubtransportStream Action(string url, GitSmartSubtransportAction action)
        {
            string postContentType = null;
            Uri serviceUri;
            switch (action)
            {
                case GitSmartSubtransportAction.UploadPackList:
                    serviceUri = new Uri(url + "/info/refs?service=git-upload-pack");
                    break;
                case GitSmartSubtransportAction.UploadPack:
                    serviceUri = new Uri(url + "/git-upload-pack");
                    postContentType = "application/x-git-upload-pack-request";
                    break;
                case GitSmartSubtransportAction.ReceivePackList:
                    serviceUri = new Uri(url + "/info/refs?service=git-receive-pack");
                    break;
                case GitSmartSubtransportAction.ReceivePack:
                    serviceUri = new Uri(url + "/git-receive-pack");
                    postContentType = "application/x-git-receive-pack-request";
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (this.httpClient == null)
            {
                this.httpClient = BuildHttpClientForUri();
            }

            return new HttpClientSmartSubtransportStream(this, this.httpClient, serviceUri, postContentType);
        }

        /// <summary>
        /// The build http client for uri.
        /// </summary>
        /// <returns>
        /// The <see cref="HttpClient"/>.
        /// </returns>
        private static HttpClient BuildHttpClientForUri()
        {
            var handler = new HttpClientHandler();

            if (!string.IsNullOrWhiteSpace(Config.Instance.UserName))
            {
                handler.Credentials = new NetworkCredential(Config.Instance.UserName, Config.Instance.Password);
            }
            else
            {
                handler.UseDefaultCredentials = true;
            }                           

            var client = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(5.0) };
            return client;
        }
    }
}
