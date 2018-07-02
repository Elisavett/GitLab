using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class Repository
        {
            public class Diff
            {
                public string old_path, new_path, a_mode, b_mode, diff;
                public bool new_file, renamed_file, deleted_file;
            }

        }
    }
}
