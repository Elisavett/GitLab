using System;
using System.Collections.Generic;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        /// <summary>
        /// Class for managing GitLab System Hooks
        /// </summary>
        public class SystemHook
        {
            public int id;
            public string url, created_at;

            public static void Add(Config _Config, string url)
            {

            }

            public static List<SystemHook> List(Config _Config)
            {
                List<SystemHook> RetVal = new List<SystemHook>();
                throw new NotImplementedException();
                return RetVal;
            }

        }
    }
}
