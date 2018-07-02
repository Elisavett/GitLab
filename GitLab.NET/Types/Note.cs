namespace GitLabDotNet
{
    public partial class GitLab
    {
        /// <summary>
        /// Generic class for comments in various GitLab objects
        /// </summary>
        public class Note
        {
            public int id, noteable_id;
            public string body, created_at, noteable_type;
            public object attachment;
            public User author;
            public bool system, upvote, downvote;
        }
    }
}
