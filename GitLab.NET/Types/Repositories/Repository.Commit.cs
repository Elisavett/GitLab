namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class Repository
        {
            /// <summary>
            /// Gitlab Repository Commit descriptor object
            /// </summary>
            public class Commit
            {
                public string id
                    , short_id
                    , title                   
                    , created_at
                    , message
                    , committer_name
                    , committer_email
                    , authored_date
                    , committed_date;
                public string[] parent_ids;
                public bool allow_failure;

                /// <summary>
                /// Gets or sets the author_name. This is a passthrough to the author object to address inconsistencies in how the API describes commits.
                /// </summary>
                /// <value>
                /// The author_name.
                /// </value>
                public string author_name
                {
                    get
                    {
                        return author.name;
                    }
                    set
                    {
                        author.name = value;
                    }
                }

                /// <summary>
                /// Gets or sets the author_email. This is a passthrough to the author object to address inconsistencies in how the API describes commits.
                /// </summary>
                /// <value>
                /// The author_email.
                /// </value>
                public string author_email
                {
                    get
                    {
                        return author.email;
                    }
                    set
                    {
                        author.email = value;
                    }
                }

                public Contributor author = new Contributor();

                public class Contributor
                {
                    public string name, email;
                    public int commits, additions, deletions;
                }
            }
        }
    }
}
