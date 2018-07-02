using System.Collections.Generic;

namespace GitLabDotNet
{
    partial class GitLab
    {

        partial class Group 
        {
            /// <summary>
            /// Sets the access level of a group member
            /// </summary>
            /// <param name="_Member"></param>
            /// <param name="_AccessLevel"></param>           

            public class GroupMemberList : List<Member>, IMemberContainer
            {
                private Group _Parent = null;

                public Group Parent
                {
                    get { return _Parent; }
                }

                public GroupMemberList(Group _parent)
                {
                    _Parent = _parent;
                }

                /// <summary>
                /// Refreshes group members as long as Parent GitLab object is not null
                /// </summary>
                public void RefreshMembers()
                {
                    
                    if (Parent != null)
                    {
                        this.Clear();

                        foreach (Member M in ListMembers(Parent.Parent.CurrentConfig, Parent))
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

                public void SetMemberAccessLevel(Member _Member, Member.AccessLevel _AccessLevel)
                {
                    if (this.Parent != null)
                    {
                        if (this.Parent.Parent != null)
                        {
                            Group.UpdateMember(this.Parent.Parent.CurrentConfig, this.Parent, _Member, _AccessLevel);
                            RefreshMembers();
                        }
                        else throw new GitLabStaticAccessException("Unable to set Access Level without Parent Group object");
                    }
                    else throw new GitLabStaticAccessException("Unable to set Access Level without Parent GitLab object");
                }

                new public void Add(Member M)
                {
                    GitLab.Group.AddMember(Parent.Parent.CurrentConfig, this.Parent, M, M.access_level);
                    RefreshMembers();

                }

                new public void Remove(Member M)
                {
                    GitLab.Group.DeleteMember(Parent.Parent.CurrentConfig, Parent, M);
                    RefreshMembers();
                }
            }
        }
    }
}
