using System.Collections.Generic;

namespace GitLabDotNet
{
    public partial class GitLab
    {

        private List<Group> _Groups = null;

        /// <summary>
        /// Groups current user has access to in this GitLab;
        /// </summary>
        public List<Group> Groups
        {
            get
            {
                if (_Groups == null)
                {
                    RefreshGroups();
                }
                return _Groups;
            }
        }

        /// <summary>
        /// Refreshes Group list for this GitLab object
        /// </summary>
        public void RefreshGroups()
        {
            _Groups = Group.List(this.CurrentConfig);
            foreach(Group g in _Groups)
            {
                g.SetParent(this);
            }
        }     

        public partial class Group
        {
            public int id;
            public string name,
                          path,
                          description;

            private List<Member> _Members = null;
            public List<Member> Members
            {
                get
                {
                    return _Members;
                }
            }
            
            /// <summary>
            /// Refreshes group members as long as Parent GitLab object is not null
            /// </summary>
            public void RefreshMembers()
            {
                if (Parent != null)
                {
                    _Members = Group.ListMembers(Parent.CurrentConfig, this);
                }
                else
                {
                    throw new GitLabStaticAccessException("Unable to retrieve members without Parent Configuration.");
                }
            }

            /// <summary>
            /// GitLab object that this group belongs to. NULL if retrieved using static functions;
            /// </summary>
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
        }
    }
}
