// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GitSession.cs" company="">
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

    using LibGit2Sharp;

    /// <summary>
    /// The git session.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    internal class GitSession : IDisposable
    {
        /// <summary>
        /// The https definition.
        /// </summary>
        private readonly IDisposable httpsDefinition;

        /// <summary>
        /// The https definition.
        /// </summary>
        private readonly IDisposable httpDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitSession"/> class.
        /// </summary>
        public GitSession()
        {
            this.httpsDefinition = new SmartSubtransportDefinition<HttpClientSmartSubtransport>("https://", 2);
            this.httpDefinition = new SmartSubtransportDefinition<HttpClientSmartSubtransport>("http://", 2);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.httpsDefinition.Dispose();
            this.httpDefinition.Dispose();
        }
    }
}
