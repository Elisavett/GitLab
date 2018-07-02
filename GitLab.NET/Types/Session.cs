using System;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        /// <summary>
        /// A GitLab Login Session object
        /// </summary>
        /// <remarks>
        /// Mainly used to get the Private Token from a username / password combination
        /// See: https://gitlab.com/help/api/session.md
        /// </remarks>
        class Session
        {
            public int id
                , theme_id;
            public bool blocked
                , dark_scheme
                , is_admin
                , can_create_group
                , can_create_team
                , can_create_project;
            public string username
                , email
                , name
                , private_token
                , created_at
                , bio
                , skype
                , linkedin
                , twitter
                , website_url;

            /// <summary>
            /// Gets a session object from the server using a Username or Email and Password combination
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Password"></param>
            /// <param name="_UserName"></param>
            /// <param name="_Email"></param>
            /// <returns></returns>
            public static Session Login(Config _Config, string _Password, string _UserName = null, string _Email = null)
            {
                if (_UserName == null && _Password == null)
                    throw new InvalidOperationException("Username or Email is required");
                else
                {
                    //TODO: Do all the other function stuff here and remove the Not Implemented throw at the end.
                }
                throw new NotImplementedException();
            }

        }
    }   
}
