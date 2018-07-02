using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using unirest_net.http;
using System.Web;
using Newtonsoft.Json.Linq;

namespace GitLabDotNet
{
    partial class GitLab
    {
        public partial class Group
        {
            /// <summary>
            /// Creates a new project group. Available only for users who can create groups.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Name"></param>
            /// <param name="_Path"></param>
            /// <param name="_Description"></param>
            /// <returns></returns>
            public static Group Create(Config _Config, string _Name, string _Path = null, string _Description = null)
            {
                if (_Path == null)
                    _Path = _Name.Trim().Replace(" ", "-").ToLowerInvariant();

                string URI = _Config.APIUrl + "groups?name=" + HttpUtility.UrlEncode(_Name)
                                            + "&path=" + HttpUtility.UrlEncode(_Path);

                if (_Description != null)
                    URI += "&description=" + HttpUtility.UrlEncode(_Description);

                HttpResponse<string> R = Unirest.post(URI)
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }

                Group RetVal = JsonConvert.DeserializeObject<Group>(R.Body);
                return RetVal;

            }

            /// <summary>
            /// Get a list of groups. (As user: my groups, as admin: all groups)
            /// </summary>
            /// <param name="_Config"></param>
            /// <returns></returns>
            public static List<Group> List(Config _Config, string _Search = null)
            {
                List<Group> RetVal = new List<Group>();

                try
                {
                    int page = 1;
                    List<Group> groups = new List<Group>();

                    do
                    {
                        groups.Clear();

                        string URI = (_Config.APIUrl + "groups/"
                                + "?per_page=100"
                                + "&page=" + page.ToString());

                        if (_Search != null)
                            URI += "&search=" + HttpUtility.UrlEncode(_Search);

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
                                    Group G = JsonConvert.DeserializeObject<Group>(Token.ToString());
                                    groups.Add(G);
                                }
                            }
                        }
                        page++;
                        RetVal.AddRange(groups);
                    }
                    while (groups.Count > 0 & page < 100);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }

            /// <summary>
            /// Get a single group description by numeric group ID
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_id"></param>
            /// <returns></returns>
            public static Group Get(Config _Config, int _id)
            {
                Group RetVal = null;

                try
                {
                    string URI = _Config.APIUrl + "groups/" + _id.ToString();

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
                        RetVal = JsonConvert.DeserializeObject<Group>(R.Body.ToString());
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }


            /// <summary>
            /// Removes group with all projects inside.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Group"></param>
            public static void Delete(Config _Config, Group _Group)
            {
                HttpResponse<string> R = Unirest.delete(_Config.APIUrl + "groups/" + _Group.id.ToString())
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }
            }

            /// <summary>
            /// Get a list of group members viewable by the authenticated user.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Group"></param>
            /// <returns></returns>
            public static List<Member> ListMembers(Config _Config, Group _Group)
            {
                List<Member> RetVal = new List<Member>();

                try
                {
                    int page = 1;
                    List<Member> members = new List<Member>();

                    do
                    {
                        members.Clear();

                        string URI = (_Config.APIUrl + "groups/"
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
            /// Adds a user to the list of group members.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Group"></param>
            /// <param name="_User"></param>
            /// <param name="_AccessLevel"></param>
            public static void AddMember(Config _Config, Group _Group, User _User, Member.AccessLevel _AccessLevel)
            {
                string URI = _Config.APIUrl + "groups/" + _Group.id.ToString() + "/members/?user_id=" + _User.id + "&access_level=" + Convert.ToInt64(_AccessLevel);

                HttpResponse<string> R = Unirest.post(URI)
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }
            }

            /// <summary>
            /// Updates a group team member to a specified access level.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Group"></param>
            /// <param name="_User"></param>
            /// <param name="_AccessLevel"></param>
            public static void UpdateMember(Config _Config, Group _Group, User _User, Member.AccessLevel _AccessLevel)
            {
                string URI = _Config.APIUrl + "groups/" + _Group.id.ToString() + "/members/" + _User.id + "&access_level=" + Convert.ToInt64(_AccessLevel);

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
            /// Removes user from group.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Group"></param>
            /// <param name="_User"></param>
            public static void DeleteMember(Config _Config, Group _Group, User _User)
            {
                string URI = _Config.APIUrl + "groups/" + _Group.id.ToString() + "/members/" + _User.id;
                HttpResponse<string> R = Unirest.delete(URI)
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }
            }

            /// <summary>
            /// Transfer a project to the Group namespace. Available only for admin
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_Group">Group to transfer project to</param>
            /// <param name="_Project">Project to be transferred</param>
            public static void TransferProject(Config _Config, Group _Group, Project _Project)
            {
                string URI = _Config.APIUrl + "groups/" + _Group.id.ToString() + "/projects/" + _Project.id;

                HttpResponse<string> R = Unirest.post(URI)
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

