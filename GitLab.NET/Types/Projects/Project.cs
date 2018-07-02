using System.Collections.Generic;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        private List<Project> _Projects = null;

        public List<Project> Projects
        {
            get
            {
                if (_Projects == null)
                {
                    RefreshProjects();
                }
                return _Projects;
            }
        }

        /// <summary>
        /// Refreshes Project List for this GitLab object
        /// </summary>
        public void RefreshProjects()
        {
            _Projects = Project.List(this.CurrentConfig);

            foreach (Project p in _Projects)
            {
                p.SetParent(this);
            }
        }
        
        public Project CreateProject(string _Name, string _Description, Namespace _Namespace, Project.VisibilityLevel _VisibilityLevel)
        {
            Project RetVal = Project.Create(CurrentConfig, _Name, _Description, _Namespace, _VisibilityLevel);
            this.RefreshProjects();
            return RetVal;
        }

        /// <summary>
        /// Non-Static Delete Project function
        /// </summary>
        /// <param name="_Project"></param>
        public void DeleteProject(Project _Project)
        {
            GitLab.Project.Delete(CurrentConfig, _Project);
            this.RefreshProjects();
        }

        /// <summary>
        /// GitLab Project Class
        /// </summary>
        /// <remarks>
        /// See https://gitlab.com/help/api/projects.md for reference
        /// </remarks>
        public  partial class Project : object
        {
            public string name
                , name_with_namespace
                , path
                , path_with_namespace
                , description
                , default_branch
                , import_url
                , ssh_url_to_repo
                , http_url_to_repo
                , web_url
                , created_at
                , last_activity_at
                , avatar_url;
            public string[] tag_list;
            public int id, namespace_id, visibility_level, creator_id, star_count, forks_count;
            public bool issues_enabled, merge_requests_enabled, wiki_enabled, snippets_enabled , builds_enabled, archived;
            public Owner owner;
            public Namespace NameSpace;

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

            /// <summary>
            /// Sets the parent GitLab object of this Project.
            /// </summary>
            /// <param name="_parent"></param>
            internal void SetParent(GitLab _parent)
            {
                _Parent = _parent;
            }

            /// <summary>
            /// Project Visibility Level as enum for ease of use;
            /// </summary>
            public enum VisibilityLevel
            {
                Undefined = -1
                , Private = 0
                , Internal = 10
                , Public = 20
            }

            /// <summary>
            /// Project owner sub-class so that owner can be parsed from JSON
            /// </summary>
            public class Owner
            {
                public int id;
                public string name, created_at;
            }

            new public string ToString()
            {
                return this.name_with_namespace;
            }

            }
    }
}
