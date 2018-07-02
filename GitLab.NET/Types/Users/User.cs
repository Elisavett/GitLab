using System.Collections.Generic;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        private List<User> _Users = null;
        public List<User> Users
        {
            get
            {
                if (_Users == null)
                    RefreshUsers();

                return _Users;
            }
        }

        public void RefreshUsers()
        {
            _Users = User.List(CurrentConfig, this);
        }

        private User _CurrentUser;
        public User CurrentUser
        {
            get
            {
                if (_CurrentUser == null)
                    RefreshCurrentUser();

                return _CurrentUser;
            }
        }

        public void RefreshCurrentUser()
        {
            _CurrentUser = User.Get(CurrentConfig);
        }

        /// <summary>
        /// Creates a new user on the GitLab server
        /// </summary>
        /// <param name="_UserName"></param>
        /// <param name="_Email"></param>
        /// <param name="_Password"></param>
        /// <param name="_DisplayName"></param>
        /// <param name="Confirm"></param>
        /// <returns></returns>
        public User CreateUser(string _UserName, string _Email, string _Password, string _DisplayName = null, bool Confirm = true)
        {
            User U = new User();
            U.username = _UserName;
            U.email = _Email;
            U.name = _DisplayName;

            if (U.name == null)
                U.name = _UserName;
            
            RefreshUsers();
            return User.Create(CurrentConfig, U, _Password, Confirm);
        }

        /// <summary>
        ///  GitLab User Object
        /// </summary>
        /// <remarks>
        /// See https://gitlab.com/help/api/users.md for API Reference
        /// </remarks>
        public partial class User
        {
            public string name
                , username
                , state
                , email
                , created_at
                , bio
                , skype
                , linkedin
                , twitter
                , web_url
                , website_url
                , extern_uid
                , provider
                , avatar_url
                , current_sign_in_at
                , private_token;

            public int id = -1, color_scheme_id = -1, theme_id = -1, projects_limit = -1;
            public bool two_factor_enabled = false, can_create_group = false, is_admin = false;

            public GitLab Parent
            {
                get
                {
                    return _Parent;
                }
            }

            private GitLab _Parent = null;

            internal void SetParent(GitLab _parent)
            {
                _Parent = _parent;
            }
            
            /// <summary>
            /// Blocks the user from login
            /// </summary>
            public void Block()
            {
                if (Parent != null)
                {
                    User.Block(Parent.CurrentConfig, this);
                }
                else
                {
                    throw new GitLabStaticAccessException("Unable to block user without parent GitLab");
                }
            }

            /// <summary>
            /// UnBlocks the user from login
            /// </summary>
            public void UnBlock()
            {
                if (Parent != null)
                {
                    User.UnBlock(Parent.CurrentConfig, this);
                }
                else
                {
                    throw new GitLabStaticAccessException("Unable to unblock user without parent GitLab");
                }
            }

            public void Delete()
            {
                if (Parent != null)
                {
                    User.Delete (Parent.CurrentConfig, this);
                }
                else
                {
                    throw new GitLabStaticAccessException("Unable to delete user without parent GitLab");
                }
            }

            /// <summary>
            /// Saves changes made to this user object to server.
            /// </summary>
            public void Update()
            {
                if (Parent != null)
                {
                    User.Update(Parent.CurrentConfig, this);
                }
                else
                {
                    throw new GitLabStaticAccessException("Unable to update user without parent GitLab");
                }
            }

            /// <summary>
            /// Resets password on user account. This will also save any other changes made to user object to server.
            /// </summary>
            /// <param name="_Password"></param>
            public void ResetPassword(string _Password)
            {
                if (Parent != null)
                {
                    User.Update(Parent.CurrentConfig, this, _Password);
                }
                else
                {
                    throw new GitLabStaticAccessException("Unable to update user without parent GitLab");
                }
            }
        }
    }
}
