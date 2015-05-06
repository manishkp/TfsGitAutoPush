namespace TfsGitPush
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// The config.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// The instance.
        /// </summary>
        private static Config instance;

        /// <summary>
        /// Prevents a default instance of the <see cref="Config"/> class from being created.
        /// </summary>
        private Config()
        {
            XDocument xdoc = XDocument.Load(".\\config.xml");
            var repositoryNode = xdoc.Descendants("Repository").FirstOrDefault();

            if (repositoryNode == null)
            {
                throw new FileNotFoundException("config.xml not found");
            }

            this.RepositoryPath = repositoryNode.Attribute("Path").Value;
            this.UserName = repositoryNode.Attribute("UserName").Value;
            this.Password = repositoryNode.Attribute("Password").Value;
            this.Email = repositoryNode.Attribute("Email").Value;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static Config Instance
        {
            get
            {
                return instance ?? (instance = new Config());
            }
        }

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Gets or sets the user email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; } 
    }
}
