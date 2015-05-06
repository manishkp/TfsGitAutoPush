// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TfsGitSession.cs" company="">
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
    /// The tfs git session.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly",
        Justification = "Reviewed. Suppression is OK here.")]
    internal class TfsGitSession : IDisposable
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
        /// Initializes a new instance of the <see cref="TfsGitSession"/> class.
        /// </summary>
        public TfsGitSession()
        {
            // this.httpsDefinition = new SmartSubtransportDefinition<TfsSmartSubtransport>("https://", 2);
            
            // this.httpDefinition = new SmartSubtransportDefinition<TfsSmartSubtransport>("http://", 2);
            var tfsSmartSubTransportType =
                Type.GetType(
                    "Microsoft.TeamFoundation.Git.CoreServices.TfsSmartSubtransport, Microsoft.TeamFoundation.Git.CoreServices, Version=12.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                    true);
            var smartSubtransportDefinitionType =
                typeof(SmartSubtransportDefinition<>).MakeGenericType(tfsSmartSubTransportType);

            this.httpsDefinition = (IDisposable)Activator.CreateInstance(smartSubtransportDefinitionType, "https://", 2);
            this.httpDefinition = (IDisposable)Activator.CreateInstance(smartSubtransportDefinitionType, "https://", 2);
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
