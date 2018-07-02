using System;
using System.Collections.Generic;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class Project
        {
            private List<Branch> _Branches = null;
            public List<Branch> Branches
            {
                get
                { 
                    if(_Branches == null)
                    {
                         //TODO: Implement get branches and store to _Branches;
                    }

                    return _Branches;
                }
            }
            /// <summary>
            /// Gitlab Project Branch object
            /// </summary>
            public class Branch
            {
                public Repository.Commit Commit;
                public string Name;
                public bool Protected;
            }

            void CreateBranch(string _name, bool _protected = false)
            {               
                throw new NotImplementedException();
            }
        }
    }
}
