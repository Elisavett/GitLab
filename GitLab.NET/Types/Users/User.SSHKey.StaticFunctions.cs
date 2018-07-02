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
        partial class SSHKey
        {

            /// <summary>
            /// Gets list of SSH Keys for a user. Gets keys for the current user if _User is null.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_User"></param>
            /// <returns></returns>
            public static List<SSHKey> List(Config _Config, User _User = null)
            {
                List<SSHKey> RetVal = new List<SSHKey>();

                try
                {
                    int page = 1;
                    List<SSHKey> keys = new List<SSHKey>();

                    do
                    {
                        keys.Clear();

                        string URI = _Config.APIUrl;

                        if (_User == null)
                            URI += "user/keys";
                        else
                            URI += "users/" + _User.id.ToString() + "/keys";

                        URI += "?per_page=100" + "&page=" + page.ToString();

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
                                    SSHKey K = JsonConvert.DeserializeObject<SSHKey>(Token.ToString());
                                    keys.Add(K);
                                }
                            }
                        }
                        page++;
                        RetVal.AddRange(keys);
                    }
                    while (keys.Count > 0 & page < 100);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }

            /// <summary>
            /// Get SSHKey by numeric ID.
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_User"></param>
            /// <param name="_id"></param>
            /// <returns></returns>
            public static SSHKey Get(Config _Config, User _User, int _id)
            {
                SSHKey RetVal;
                try
                {
                    string URI = _Config.APIUrl;

                    if (_User == null)
                        URI += "user/keys/";
                    else
                        URI += "users/" + _User.id.ToString() + "/keys/";

                    URI +=  _id.ToString();
                    
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
                        RetVal = JsonConvert.DeserializeObject<SSHKey>(R.Body.ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }

            /// <summary>
            /// Adds SSH key to user account. If _User is null adds for current user. 
            /// </summary>            
            /// <param name="_Config"></param>
            /// <param name="_SSHKey"></param>
            /// <param name="_User"></param>
            public static void Add(Config _Config, SSHKey _SSHKey, User _User = null)
            {
                try
                {
                    string URI = _Config.APIUrl;

                    if (_User == null)
                        URI += "user/keys";
                    else
                        URI += "users/" + _User.id.ToString() + "/keys";

                    URI += "?title=" + HttpUtility.UrlEncode(_SSHKey.title)
                     + "&key=" + HttpUtility.UrlEncode(_SSHKey.key);

                    HttpResponse<string> R = Unirest.post(URI)
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
            /// Deletes SSH key to user account. If _User is null adds for current user. 
            /// </summary>
            /// <param name="_Config"></param>
            /// <param name="_SSHKey"></param>
            /// <param name="_User"></param>
            public static void Delete(Config _Config, SSHKey _SSHKey, User _User = null)
            {
                try
                {
                    string URI = _Config.APIUrl;

                    if (_User == null)
                        URI += "user/keys/" + _SSHKey.id.ToString();
                    else
                        URI += "users/" + _User.id.ToString() + "/keys/" + _SSHKey.id.ToString();

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

        }
    }
}
