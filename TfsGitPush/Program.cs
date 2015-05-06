// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsGitPush
{
    using System;
    using System.IO;

    using LibGit2Sharp;

    /// <summary>
    /// The program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <exception cref="Exception">
        /// </exception>
        static void Main(string[] args)
        {
            using (new GitSession())
            {
                var anyfileToChanged = false;
                Console.WriteLine("Current dir is {0}", System.Environment.CurrentDirectory);

                var fullRepositoryPath = Config.Instance.RepositoryPath;

                if (!Path.IsPathRooted(fullRepositoryPath))
                {
                   fullRepositoryPath = Path.Combine(System.Environment.CurrentDirectory, fullRepositoryPath);
                }

                if (!Directory.Exists(fullRepositoryPath))
                {
                    throw new FileNotFoundException("Repository Not found at {0}", fullRepositoryPath);
                }

                using (var repo = new Repository(fullRepositoryPath))
                {                   
                    foreach (var fileIndex in repo.Index)
                    {                        
                        var oldFileIndex = fileIndex.Id; 
                        var fullFilePath = Path.Combine(fullRepositoryPath, fileIndex.Path);
                        repo.Index.Stage(fullFilePath);
                        var newFileIndex = repo.Index[fileIndex.Path].Id;
                        if (newFileIndex != oldFileIndex)
                        {
                            Console.WriteLine("Staging File :{0}", fullFilePath);
                            anyfileToChanged = true;
                        }
                    }


                    if (!anyfileToChanged)
                    {
                        Console.WriteLine("No file changed, hence not checkin in");
                        return;
                    }

                    // Create the committer's signature and commit
                    var author = new Signature(Config.Instance.UserName, Config.Instance.Email, DateTime.Now);
                    var committer = author;

                    // Commit to the repository
                    Commit commit = repo.Commit("From Build => checking in files", author, committer);

                    var remote = repo.Network.Remotes["origin"];
                    var master = repo.Branches["master"];
                    
                    Console.WriteLine("Remote origin url {0}", remote.Url);

                    string pushRefSpec = string.Format("HEAD:{0}", master.CanonicalName);
                    Console.WriteLine("push ref {0}", pushRefSpec);
                    var pushOptions = new PushOptions() { OnPushStatusError = (e) => { throw new Exception(e.Message + e.Reference); } };

                    repo.Network.Push(remote, pushRefSpec, pushOptions);                    
                }
            }
        }
    }
}
