using System;
using Newtonsoft.Json;
using unirest_net.http;
using System.Web;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GitLabDotNet
{
    partial class GitLab
    {
        partial class Project
        {
            /// <summary>
            /// Creates a new project
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Name">The name of the new project</param>
            /// <param name="_Description"> Optional Description, can be null</param>
            /// <param name="_Namespace">Namespace to create the project in</param>
            /// <param name="_VisibilityLevel">New project's visibility level</param>
            /// <returns></returns>
            public static Project Create(Config _Config, string _Name, string _Description, Namespace _Namespace, VisibilityLevel _VisibilityLevel)
            {
                HttpResponse<string> R = Unirest.post(_Config.APIUrl + "projects?name=" + HttpUtility.UrlEncode(_Name)
                                        + "&namespace_id=" + _Namespace.id.ToString()
                                        + "&description=" + HttpUtility.UrlEncode(_Description)
                                        + "&visibility_level=" + Convert.ToInt64(_VisibilityLevel).ToString())
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }

                Project RetVal = JsonConvert.DeserializeObject<Project>(R.Body);
                return RetVal;
            }

            /// <summary>
            /// Update any project
            /// </summary>
            /// <param name="_Config">Gitlab Config Object</param>
            /// <param name="_Project">Project object</param>
            /// <returns></returns>
            public static Project Update(Config _Config, Project _Project)
            {

                string URI = _Config.APIUrl + "projects/" + _Project.id.ToString()
                                        + "?name=" + HttpUtility.UrlEncode(_Project.name)
                                        + "&path=" + HttpUtility.UrlEncode(_Project.path)
                                        + "&description=" + HttpUtility.UrlEncode(_Project.description);

                if (!String.IsNullOrWhiteSpace(_Project.default_branch))
                    URI +=                "&default_branch=" + HttpUtility.UrlEncode(_Project.default_branch);

                URI +=                    "&issues_enabled=" + Convert.ToInt32(_Project.issues_enabled).ToString()
                                        + "&merge_requests_enabled=" + Convert.ToInt32(_Project.merge_requests_enabled).ToString()
                                        + "&builds_enabled=" + Convert.ToInt32(_Project.builds_enabled).ToString()
                                        + "&wiki_enabled=" + Convert.ToInt32(_Project.wiki_enabled).ToString()
                                        + "&snippets_enabled=" + Convert.ToInt32(_Project.snippets_enabled).ToString()
                                        + "&visibility_level=" + Convert.ToInt64(_Project.visibility_level).ToString();

                HttpResponse<string> R = Unirest.put(URI)
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body + " / " + URI, R.Code);
                }

                Project RetVal = JsonConvert.DeserializeObject<Project>(R.Body);
                return RetVal;
            }

            /// <summary>
            /// Deletes a project
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Project"></param>
            public static void Delete(Config _Config, Project _Project)
            {
                HttpResponse<string> R = Unirest.delete(_Config.APIUrl + "projects/" + _Project.id.ToString())
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }
            }

            /// <summary>
            /// List all projects
            /// </summary>
            /// <param name="_Config"></param>
            /// <returns></returns>
            public static List<Project> List(Config _Config)
            {
                List<Project> RetVal = new List<Project>();

                try
                {
                    int page = 1;
                    List<Project> projects = new List<Project>();

                    do
                    {
                        projects.Clear();

                        HttpResponse<string> R = Unirest.get
                                (_Config.APIUrl + "projects?per_page=100"
                                + "&page=" + page.ToString())
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                        if (R.Code < 200 || R.Code >= 300)
                        {
                            throw new GitLabServerErrorException(R.Body, R.Code);
                        }
                        else
                        {
                            dynamic Result = JsonConvert.DeserializeObject(R.Body);
                            if (Result is JArray)
                            {
                                JArray ResultArray = (JArray)Result;
                                foreach (JToken Token in ResultArray)
                                {
                                    //   Console.WriteLine(Token.ToString());
                                    Project P = JsonConvert.DeserializeObject<Project>(Token.ToString());
                                    projects.Add(P);
                                }
                            }
                        }
                        page++;
                        RetVal.AddRange(projects);
                    }
                    while (projects.Count > 0 & page < 100);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }

            /// <summary>
            /// Get single project by numeric ID
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_id"></param>
            /// <returns></returns>
            public static Project Get(Config _Config, int _id)
            {
                Project RetVal = null;

                try
                {
                    string URI = _Config.APIUrl + "projects/" + _id.ToString();

                    HttpResponse<string> R = Unirest.get(URI)
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                    if (R.Code < 200 || R.Code >= 300)
                    {
                        throw new GitLabServerErrorException(R.Body, R.Code);
                    }
                    else
                    {
                        RetVal = JsonConvert.DeserializeObject<Project>(R.Body.ToString());
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }


            /// <summary>
            /// Query projects by name
            /// </summary>
            /// <param name="_Config"></param>
            /// <returns></returns>
            public static List<Project> Search(Config _Config, string Query)
            {
                List<Project> RetVal = new List<Project>();

                try
                {
                    int page = 1;
                    List<Project> projects = new List<Project>();

                    do
                    {
                        projects.Clear();

                        HttpResponse<string> R = Unirest.get
                                (_Config.APIUrl + "projects/search/" + HttpUtility.UrlEncode(Query)
                                + "?per_page=100"
                                + "&page=" + page.ToString())
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                        if (R.Code < 200 || R.Code >= 300)
                        {
                            throw new GitLabServerErrorException(R.Body, R.Code);
                        }
                        else
                        {
                            dynamic Result = JsonConvert.DeserializeObject(R.Body);
                            if (Result is JArray)
                            {
                                JArray ResultArray = (JArray)Result;
                                foreach (JToken Token in ResultArray)
                                {
                                    Project P = JsonConvert.DeserializeObject<Project>(Token.ToString());
                                    projects.Add(P);
                                }
                            }
                        }
                        page++;
                        RetVal.AddRange(projects);
                    }
                    while (projects.Count > 0 & page < 100);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }

            /// <summary>
            /// Get a list of project members viewable by the authenticated user.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Project"></param>
            /// <returns></returns>
            public static List<Member> ListMembers(Config _Config, Project _Project)
            {
                List<Member> RetVal = new List<Member>();

                try
                {
                    int page = 1;
                    List<Member> members = new List<Member>();

                    do
                    {
                        members.Clear();

                        string URI = (_Config.APIUrl + "projects/" + _Project.id.ToString() + "/members"
                                + "?per_page=100"
                                + "&page=" + page.ToString());


                        HttpResponse<string> R = Unirest.get(URI)
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                        if (R.Code < 200 || R.Code >= 300)
                        {
                            throw new GitLabServerErrorException(R.Body, R.Code);
                        }
                        else
                        {
                            dynamic Result = JsonConvert.DeserializeObject(R.Body);
                            if (Result is JArray)
                            {
                                JArray ResultArray = (JArray)Result;
                                foreach (JToken Token in ResultArray)
                                {
                                    Member M = JsonConvert.DeserializeObject<Member>(Token.ToString());
                                    members.Add(M);
                                }
                            }
                        }
                        page++;
                        RetVal.AddRange(members);
                    }
                    while (members.Count > 0 & page < 100);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }

            /// <summary>
            /// Adds a user to the list of project members.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Project"></param>
            /// <param name="_User"></param>
            /// <param name="_AccessLevel"></param>
            public static void AddMember(Config _Config, Project _Project, User _User, Member.AccessLevel _AccessLevel)
            {
                //OWNER access level only valid for groups according to https://docs.gitlab.com/ce/api/access_requests.html
                if (_AccessLevel > Member.AccessLevel.MASTER)
                    _AccessLevel = Member.AccessLevel.MASTER;

                string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/members/?user_id=" + _User.id + "&access_level=" + Convert.ToInt64(_AccessLevel);

                HttpResponse<string> R = Unirest.post(URI)
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException("Exception performing action " + URI + ": " +R.Body, R.Code);
                }
            }

            /// <summary>
            /// Updates a project team member to a specified access level.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Project"></param>
            /// <param name="_User"></param>
            /// <param name="_AccessLevel"></param>
            public static void UpdateMember(Config _Config, Project _Project, User _User, Member.AccessLevel _AccessLevel)
            {
                //OWNER access level only valid for groups according to https://docs.gitlab.com/ce/api/access_requests.html
                if (_AccessLevel > Member.AccessLevel.MASTER)
                    _AccessLevel = Member.AccessLevel.MASTER;

                string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/members/" + _User.id + "&access_level=" + Convert.ToInt64(_AccessLevel);

                HttpResponse<string> R = Unirest.put(URI)
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }
            }

            /// <summary>
            /// Removes a user from a project team.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Project"></param>
            /// <param name="_User"></param>
            public static void DeleteMember(Config _Config, Project _Project, User _User)
            {
                string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/members/" + _User.id;

                HttpResponse<string> R = Unirest.delete(URI)
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }
            }
        }
    }
}