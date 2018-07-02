using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using unirest_net.http;
using System.Web;
using Newtonsoft.Json.Linq;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class User
        {
            /// <summary>
            /// List all users that the current user can see 
            /// </summary>
            /// <param name="_Config">A GitLab.NET Configuration object</param>
            /// <returns></returns>
            public static List<User> List(Config _Config, GitLab _Parent = null)
            {
                List<User> RetVal = new List<User>();

                try
                {
                    int page = 1;
                    List<User> users = new List<User>();

                    do
                    {
                        users.Clear();

                        string URI = _Config.APIUrl + "users?per_page=100" + "&page=" + page.ToString();

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
                                    User U = JsonConvert.DeserializeObject<User>(Token.ToString());
                                    U.SetParent(_Parent);                               
                                    users.Add(U);
                                }
                            }
                        }
                        page++;
                        RetVal.AddRange(users);
                    }
                    while (users.Count > 0 & page < 100);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;

            }

            /// <summary>
            /// Get user object by numeric ID
            /// </summary>
            /// <param name="_Config">A GitLab.NET Configuration object</param>
            /// <param name="_id">Numeric ID of the user to retrieve. Returns current user if less than 0.</param>
            /// <returns></returns>
            public static User Get(Config _Config, int _id = -1)
            {
                User RetVal = null;

                try
                {
                    string URI = _Config.APIUrl;

                    if (_id > 0)
                        URI += "users/" + _id.ToString();
                    else
                        URI += "user/";

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
                        RetVal = JsonConvert.DeserializeObject<User>(R.Body.ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }

            /// <summary>
            /// Create a new user on the GitLab server
            /// </summary>
            /// <param name="_Config">A GitLab.NET Configuration object</param>
            /// <param name="_User">User object to create on the server</param>
            public static User Create(Config _Config, User _User, string Password, bool RequireConfirmation = true)
            {
                User RetVal = null;

                string URI = _Config.APIUrl + "users?"
                     + "email=" + HttpUtility.UrlEncode(_User.email)
                     + "&password=" + HttpUtility.UrlEncode(Password)
                     + "&username=" + HttpUtility.UrlEncode(_User.username)
                     + "&name=" + HttpUtility.UrlEncode(_User.name)
                     + "&confirm=" + Convert.ToInt64(RequireConfirmation).ToString();

                if (!String.IsNullOrWhiteSpace(_User.skype))
                    URI += "&skype=" + HttpUtility.UrlEncode(_User.skype);
                if (!String.IsNullOrWhiteSpace(_User.linkedin))
                    URI += "&linkedin=" + HttpUtility.UrlEncode(_User.linkedin);
                if (!String.IsNullOrWhiteSpace(_User.twitter))
                    URI += "&twitter=" + HttpUtility.UrlEncode(_User.twitter);
                if (!String.IsNullOrWhiteSpace(_User.web_url))
                    URI += "&web_url=" + HttpUtility.UrlEncode(_User.web_url);
                if (!String.IsNullOrWhiteSpace(_User.website_url))
                    URI += "&website_url=" + HttpUtility.UrlEncode(_User.website_url);
                if (_User.projects_limit >= 0)
                    URI += "&projects_limit=" + _User.projects_limit.ToString();
                if (!String.IsNullOrWhiteSpace(_User.extern_uid))
                    URI += "&extern_uid=" + HttpUtility.UrlEncode(_User.extern_uid);
                if (!String.IsNullOrWhiteSpace(_User.provider))
                    URI += "&provider=" + HttpUtility.UrlEncode(_User.provider);
                if (!String.IsNullOrWhiteSpace(_User.bio))
                    URI += "&bio=" + HttpUtility.UrlEncode(_User.bio);

                HttpResponse < string> R = Unirest.post(URI)
                                        .header("accept", "application/json")
                                        .header("PRIVATE-TOKEN", _Config.APIKey)
                                        .asString();

                if (R.Code < 200 || R.Code >= 300)
                {
                    throw new GitLabServerErrorException(R.Body, R.Code);
                }               
                else
                {
                    RetVal = JsonConvert.DeserializeObject<User>(R.Body);
                }

                return RetVal;
            }

            /// <summary>
            /// Modifies an existing user. Only administrators can change attributes of a user.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_User"></param>
            /// <param name="_Password"></param>
            public static void Update(Config _Config, User _User, string _Password = null)
            {

                string URI = _Config.APIUrl + "users/"+_User.id.ToString()
                     + "?email=" + HttpUtility.UrlEncode(_User.email)         
                     + "&username=" + HttpUtility.UrlEncode(_User.username)
                     + "&name=" + HttpUtility.UrlEncode(_User.name);

                if (!String.IsNullOrWhiteSpace(_Password))
                    URI += "&password=" + HttpUtility.UrlEncode(_Password);
                if (!String.IsNullOrWhiteSpace(_User.skype))
                    URI += "&skype=" + HttpUtility.UrlEncode(_User.skype);
                if (!String.IsNullOrWhiteSpace(_User.linkedin))
                    URI += "&linkedin=" + HttpUtility.UrlEncode(_User.linkedin);
                if (!String.IsNullOrWhiteSpace(_User.twitter))
                    URI += "&twitter=" + HttpUtility.UrlEncode(_User.twitter);
                if (!String.IsNullOrWhiteSpace(_User.web_url))
                    URI += "&web_url=" + HttpUtility.UrlEncode(_User.web_url);
                if (!String.IsNullOrWhiteSpace(_User.website_url))
                    URI += "&website_url=" + HttpUtility.UrlEncode(_User.website_url);
                if (_User.projects_limit >= 0)
                    URI += "&projects_limit=" + _User.projects_limit.ToString();
                if (!String.IsNullOrWhiteSpace(_User.extern_uid))
                    URI += "&extern_uid=" + HttpUtility.UrlEncode(_User.extern_uid);
                if (!String.IsNullOrWhiteSpace(_User.provider))
                    URI += "&provider=" + HttpUtility.UrlEncode(_User.provider);
                if (!String.IsNullOrWhiteSpace(_User.bio))
                    URI += "&bio=" + HttpUtility.UrlEncode(_User.bio);
                
                URI += "&admin=" + Convert.ToInt32(_User.is_admin).ToString();
                URI += "&can_create_group=" + Convert.ToInt32(_User.can_create_group).ToString();

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
            /// Deletes a user.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_User"></param>
			public static void Delete(Config _Config, User _User)
            {
                try
                {
                    string URI = _Config.APIUrl + "users/";
                    URI += _User.id.ToString();

                    HttpResponse<string> R = Unirest.delete(URI)
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                    if (R.Code < 200 || R.Code >= 300)
                    {
                        throw new GitLabServerErrorException(R.Body, R.Code);
                    }                    
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
			
			/// <summary>
            /// Blocks a user. Admin only function
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_User"></param>
			public static void Block(Config _Config, User _User)
            {
                try
                {
                    string URI = _Config.APIUrl + "users/" + _User.id.ToString() + "/block";

                    HttpResponse<string> R = Unirest.put(URI)
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                    if (R.Code < 200 || R.Code >= 300)
                    {
                        throw new GitLabServerErrorException(R.Body, R.Code);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static void UnBlock(Config _Config, User _User)
            {
                try
                {
                    string URI = _Config.APIUrl + "users/" + _User.id.ToString() + "/unblock";

                    HttpResponse<string> R = Unirest.put(URI)
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                    if (R.Code < 200 || R.Code >= 300)
                    {
                        throw new GitLabServerErrorException(R.Body, R.Code);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
    }
}