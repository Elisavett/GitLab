using System.Collections.Generic;

namespace GitLabDotNet
{
    partial class GitLab
    {
        public partial class User
        {
            private List<Email> _Emails = null;
            public List<Email> Emails
            {
                get
                {
                    if (_Emails == null)
                        RefreshEmails();
                    return _Emails;
                }
            }

            void RefreshEmails()
            {
                if (Parent != null)
                {
                    if (this.id == Parent.CurrentUser.id)
                    {
                        _Emails = Email.List(Parent.CurrentConfig);
                    }
                    else
                    {
                        _Emails = Email.List(Parent.CurrentConfig, this);
                    }
                }
            }

            void AddEmail(string _Email)
            {
                if(Parent != null)
                {
                    if (this.id == Parent.CurrentUser.id)
                        User.Email.Add(Parent.CurrentConfig, _Email);
                    else
                        User.Email.Add(Parent.CurrentConfig, _Email, this);

                    RefreshEmails();
                }
            }

            void DeleteEmail(Email _Email)
            {
                if (Parent != null)
                {
                    if (this.id == Parent.CurrentUser.id)
                        User.Email.Delete(Parent.CurrentConfig, _Email);
                    else
                        User.Email.Delete(Parent.CurrentConfig, _Email, this);

                    RefreshEmails();
                }
            }

            public partial class Email
            {
                int id;
                string email;
            }
        }
    }
}
