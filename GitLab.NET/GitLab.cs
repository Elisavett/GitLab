using System;
using unirest_net.http;
using System.Web;
using Newtonsoft.Json;

namespace GitLabDotNet
{
    /// <summary>
    /// Class for interfacing with a GitLab API via .NET
    /// See https://gitlab.com/help/api/README.md for API Reference
    /// </summary>
    public partial class GitLab
    {
        public Config CurrentConfig = null;

        public GitLab(Config _config)
        {
            CurrentConfig = _config;
            Refresh();   
        }

        /// <summary>
        /// Refreshes Everything for this GitLab object;
        /// </summary>
        public void Refresh()
        {
            RefreshProjects();
            RefreshGroups();
            RefreshUsers();
            RefreshCurrentUser();
        }

        /// <summary>
        /// Login to the specified GitLab URI.
        /// </summary>
        /// <param name="_URI">The _ URI.</param>
        /// <param name="_Login">The _ login.</param>
        /// <param name="_Password">The _ password.</param>
        /// <param name="_LoginIsEmail">if set to <c>true</c> [_ login is email].</param>
        /// <returns>GitLab object with a config set up for the logged in user.</returns>
        /// <exception cref="GitLab.GitLabServerErrorException"></exception>
        public static GitLab Login(string _URI, string _Login, string _Password, bool _LoginIsEmail = false)
        {
            Config C = new Config();
            C.GitLabURI = _URI;          


            try
            {
                 string URI = C.APIUrl + "session"                    
                    + "?password=" + HttpUtility.UrlEncode(_Password);

                if (!_LoginIsEmail)
                    URI += "&login=" + HttpUtility.UrlEncode(_Login);
                else
                    URI += "&email=" + HttpUtility.UrlEncode(_Login);

                HttpResponse<string> R = Unirest.post(URI)
                            .header("accept", "application/json")
                            .header("PRIVATE-TOKEN", C.APIKey)
                            .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }
                else
                {
                    User U = JsonConvert.DeserializeObject<User>(R.Body.ToString());
                    C.APIKey = U.private_token;
                    return new GitLab(C);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
