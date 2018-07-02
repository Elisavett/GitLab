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
        partial class User
        {
            partial class Email
            {

                public static List<Email> List(Config _Config, User _User = null)
                {
                    List<Email> RetVal = new List<Email>();

                    try
                    {
                        int page = 1;
                        List<Email> emails = new List<Email>();

                        do
                        {
                            emails.Clear();

                            string URI = _Config.APIUrl;

                            if (_User == null)
                                URI += "user/emails/" + _User.id.ToString();
                            else
                                URI += "users/" + _User.id.ToString() + "/emails/" + _User.id.ToString();

                            URI += "?per_page=100"
                                    + "&page=" + page.ToString();

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
                                        Email E = JsonConvert.DeserializeObject<Email>(Token.ToString());
                                        emails.Add(E);
                                    }
                                }
                            }
                            page++;
                            RetVal.AddRange(emails);
                        }
                        while (emails.Count > 0 & page < 100);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Adds an email address to a user account. If _User is null adds to current user.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Email"></param>
                /// <param name="_User"></param>
                public static void Add(Config _Config, string _Email, User _User = null)
                {
                    try
                    {
                        string URI = _Config.APIUrl;

                        if (_User == null)
                            URI += "user/emails/";
                        else
                            URI += "users/" + _User.id.ToString() + "/emails/";

                        URI += "?email=" + HttpUtility.UrlEncode(_Email);

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
                /// Deletes an email address from a user account. If _User is null, deletes for current user.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Email"></param>
                /// <param name="_User"></param>
                public static void Delete(Config _Config, Email _Email, User _User = null)
                {
                    try
                    {
                        string URI = _Config.APIUrl;

                        if (_User == null)
                            URI += "user/emails/" + _Email.id.ToString();
                        else
                            URI += "users/" + _User.id.ToString() + "/emails/" + _Email.id.ToString();

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

}
