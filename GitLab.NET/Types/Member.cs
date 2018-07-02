namespace GitLabDotNet
{
    /// <summary>
    /// Descriptor class for members of Groups or Projects
    /// </summary>
    public class Member : GitLab.User
    {

        IMemberContainer Parent
        {
            get { return _Parent; }
        }
        private IMemberContainer _Parent;

        internal void SetParent(IMemberContainer _parent)
        {
            _Parent = _parent;
        }
        
        public AccessLevel access_level;

        public enum AccessLevel
        {
            GUEST = 10
                   , REPORTER = 20
                   , DEVELOPER = 30
                   , MASTER = 40
                   , OWNER = 50
        }

        public void SetAccessLevel(AccessLevel _AccessLevel)
        {
            if (Parent != null)
            {
                Parent.SetMemberAccessLevel(this, _AccessLevel);
            }
            else
                throw new GitLabStaticAccessException("Unable to set access level with missing Parent");
        }
    }

    
    public interface IMemberContainer
    {
        /// <summary>
        /// Sets the Access level on a member
        /// </summary>
        /// <param name="_Member"></param>
        /// <param name="_AccessLevel"></param>
        void SetMemberAccessLevel(Member _Member, Member.AccessLevel _AccessLevel);
    }


}
