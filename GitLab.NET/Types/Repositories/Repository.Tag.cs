namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class Repository
        {
            public class Tag
            {
                public Repository.Commit commit;
                public string name, message;
                public ReleaseNote release;

                public class ReleaseNote
                {
                    public string tag_name, description;
                }
            }            
        }
    }
}
