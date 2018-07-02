using System.Collections.Generic;

namespace GitLabDotNet
{
    partial class GitLab
    {
        partial class Project
        {

            public class ProjectMemberList: List<Member> , IMemberContainer
            {
                public void SetMemberAccessLevel(Member _Member, Member.AccessLevel _AccessLevel)
                {
                    if (this.Parent != null)
                    {
                        Project.UpdateMember(this.Parent.Parent.CurrentConfig, this.Parent, _Member, _AccessLevel);
                        RefreshItems();
                    }
                    else throw new GitLabStaticAccessException("Unable to set Access Level without Parent GitLab object");
                }

                private Project _Parent = null;

                public Project Parent
                {
                    get { return _Parent; }
                }

                public ProjectMemberList(Project _parent)
                {
                    _Parent = _parent;
                    RefreshItems();
                }                

                /// <summary>
                /// Refreshes project members as long as Parent GitLab object is not null
                /// </summary>
                public void RefreshItems()
                {
                    if (Parent != null)
                    {
                        this.Clear();

                        foreach ( Member M in ListMembers(Parent.Parent.CurrentConfig, Parent))
                        {
                            M.SetParent(this);
                            base.Add(M);
                        }
                    }
                    else
                    {
                        throw new GitLabStaticAccessException("Unable to retrieve members without Parent Configuration.");
                    }
                }

                new public void Add(Member M)
                {
                    GitLab.Project.AddMember(Parent.Parent.CurrentConfig, this.Parent, M, M.access_level);
                    RefreshItems();
                    
                }

                new public void Remove(Member M)
                {
                    GitLab.Project.DeleteMember(Parent.Parent.CurrentConfig, Parent, M);
                    RefreshItems();
                }
            }

            /// <summary>
            /// List of project members. Will always be null if Project retrieved via static methods.
            /// </summary>
            public ProjectMemberList Members
            {
                get
                {
                    if (_Members == null & Parent != null)
                        _Members = new ProjectMemberList(this);

                    return _Members;
                }
            }

            private ProjectMemberList _Members = null;

            
        }
    }
}
