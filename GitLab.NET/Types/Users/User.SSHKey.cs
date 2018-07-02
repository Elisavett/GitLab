using System.Collections.Generic;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class SSHKey
        {
            public int id;
            public string title, key, created_at;
            public User user;
        }

        public partial class User
        {
            private List<SSHKey> _SSHKeys = null;
            public List<SSHKey> SSHKeys
            {
                get
                {
                    if (_SSHKeys == null)
                        RefreshSSHKeys();

                    return _SSHKeys;
                }
            }

            void RefreshSSHKeys()
            {
                if (this.Parent != null)
                {
                    if (this.id == Parent.CurrentUser.id)
                        _SSHKeys = SSHKey.List(Parent.CurrentConfig);
                    else
                        _SSHKeys = SSHKey.List(Parent.CurrentConfig, this);

                    foreach (SSHKey k in _SSHKeys)
                        k.user = this;
                }

            }

            void AddSSHKey(SSHKey _SSHKey)
            {
                if (Parent != null)
                {
                    if (this.id == Parent.CurrentUser.id)
                    {
                        SSHKey.Add(Parent.CurrentConfig, _SSHKey);
                    }
                    else
                    {
                        SSHKey.Add(Parent.CurrentConfig, _SSHKey, this);
                    }
                }

                this.RefreshSSHKeys();
            }

            void DeleteSSHKey(SSHKey _SSHKey)
            {
                if (Parent != null)
                {
                    if (this.id == Parent.CurrentUser.id)
                    {
                        SSHKey.Delete(Parent.CurrentConfig, _SSHKey);
                    }
                    else
                    {
                        SSHKey.Delete(Parent.CurrentConfig, _SSHKey, this);
                    }

                    this.RefreshSSHKeys();
                }
            }
        }
    }
}
