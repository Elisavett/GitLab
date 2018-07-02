using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using unirest_net.http;
using System.Web;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class Project
        {
            public class Milestone
            {
                public int id, iid, project_id;
                public string title, description, due_date, state, updated_at, created_at;
                public enum StateEvent
                {
                    NONE, ACTIVATE, CLOSE
                }

                /// <summary>
                /// Lists Milestones for the specified Project.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <returns></returns>
                /// <exception cref="GitLabServerErrorException"></exception>
                public static List<Milestone> List(Config _Config, Project _Project)
                {
                    List<Milestone> RetVal = new List<Milestone>();

                    try
                    {
                        int page = 1;
                        List<Milestone> milestones = new List<Milestone>();

                        do
                        {
                            milestones.Clear();

                            string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/milestones";

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
                                        Milestone M = JsonConvert.DeserializeObject<Milestone>(Token.ToString());
                                        milestones.Add(M);
                                    }
                                }
                            }
                            page++;
                            RetVal.AddRange(milestones);
                        }
                        while (milestones.Count > 0 & page < 100);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Gets the specified Milestone by ID.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_ID">The _ identifier.</param>
                /// <returns></returns>
                /// <exception cref="GitLabServerErrorException"></exception>
                public static Milestone Get(Config _Config, Project _Project, int _ID)
                {
                    Milestone RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/milestones/" + _ID.ToString();

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
                            RetVal = JsonConvert.DeserializeObject<Milestone>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Creates a new Milestone
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_Title">The _ title.</param>
                /// <param name="_Description">The _ description.</param>
                /// <param name="_DueDate">The _ due date.</param>
                /// <returns></returns>
                /// <exception cref="GitLabServerErrorException"></exception>
                public static Milestone Create(Config _Config, Project _Project, string _Title, string _Description = null, DateTime _DueDate = new DateTime())
                {
                    Milestone RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/milestones/"
                                + "?title=" + HttpUtility.UrlEncode(_Title);
                        if (_Description != null)
                            URI += "&description=" + HttpUtility.UrlEncode(_Description);

                        if (_DueDate != new DateTime())
                            URI += "&due_date=" + HttpUtility.UrlEncode(_DueDate.ToString("o"));
                               
                        HttpResponse<string> R = Unirest.post(URI)
                                    .header("accept", "application/json")
                                    .header("PRIVATE-TOKEN", _Config.APIKey)
                                    .asString();

                        if (R.Code < 200 || R.Code >= 300)
                        {
                            throw new GitLabServerErrorException(R.Body, R.Code);
                        }
                        else
                        {
                            RetVal = JsonConvert.DeserializeObject<Milestone>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Updates the Milestone.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_Milestone">The _ milestone.</param>
                /// <param name="_StateEvent">The _ state event.</param>
                /// <exception cref="GitLabServerErrorException"></exception>
                public static void Update(Config _Config, Project _Project, Milestone _Milestone, StateEvent _StateEvent = StateEvent.NONE)
                {
                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/milestones/" + _Milestone.id.ToString()
                                + "?title=" + HttpUtility.UrlEncode(_Milestone.title)
                                + "&description=" + HttpUtility.UrlEncode(_Milestone.description)
                                + "&due_date=" + HttpUtility.UrlEncode(_Milestone.due_date);

                        if(_StateEvent != StateEvent.NONE)
                            URI += "&state_event=" + HttpUtility.UrlEncode(_StateEvent.ToString().ToLowerInvariant());
                        
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


                /// <summary>
                /// Lists the comments associated with the Milestone
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_Snippet">The _ snippet.</param>
                /// <returns></returns>
                public static List<Note> ListComments(Config _Config, Project _Project, Milestone _Milestone)
                {
                    List<Note> RetVal = new List<Note>();

                    try
                    {
                        int page = 1;
                        List<Note> notes = new List<Note>();

                        do
                        {
                            notes.Clear();

                            string URI = _Config.APIUrl;

                            URI += "projects/" + _Milestone.project_id.ToString() + "/milestones/" + _Milestone.id.ToString() + "/notes";


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
                                        Note N = JsonConvert.DeserializeObject<Note>(Token.ToString());
                                        notes.Add(N);
                                    }
                                }
                            }

                            page++;
                            RetVal.AddRange(notes);
                        }
                        while (notes.Count > 0 & page < 100);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;

                }

                /// <summary>
                /// Gets a comment by ID.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_Milestone">The milestone.</param>
                /// <param name="_ID">The _ identifier.</param>
                /// <returns></returns>
                public static Note GetComment(Config _Config, Milestone _Milestone, int _ID)
                {
                    string URI = _Config.APIUrl + "projects/" + _Milestone.project_id.ToString() + "/milestones/" + _Milestone.id.ToString() + "/notes/" + _ID.ToString();

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
                        return JsonConvert.DeserializeObject<Note>(R.Body);
                    }
                }

                /// <summary>
                /// Updates a comment.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Milestone">The _ milestone.</param>
                /// <param name="_Note">The _ note.</param>
                /// <returns></returns>
                public static Note UpdateComment(Config _Config, Milestone _Milestone, Note _Note)
                {
                    string URI = _Config.APIUrl + "projects/" + _Milestone.project_id.ToString() + "/milestones/" + _Milestone.id + "/notes/" + _Note.id
                        + "?body=" + HttpUtility.UrlEncode(_Note.body);


                    HttpResponse<string> R = Unirest.put(URI)
                                            .header("accept", "application/json")
                                            .header("PRIVATE-TOKEN", _Config.APIKey)
                                            .asString();

                    if (R.Code < 200 || R.Code >= 300)
                    {
                        throw new GitLabServerErrorException(R.Body, R.Code);
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<Note>(R.Body);
                    }
                }

                /// <summary>
                /// Adds a comment to the Milestone.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Milestone">The _ milestone.</param>
                /// <param name="_Body">The _ body.</param>
                /// <returns></returns>
                public static Note AddComment(Config _Config, Milestone _Milestone, string _Body)
                {
                    string URI = _Config.APIUrl + "projects/" + _Milestone.project_id.ToString() + "/milestones/" + _Milestone.id.ToString() + "/notes?"
                        + "body=" + HttpUtility.UrlEncode(_Body);


                    HttpResponse<string> R = Unirest.post(URI)
                                            .header("accept", "application/json")
                                            .header("PRIVATE-TOKEN", _Config.APIKey)
                                            .asString();

                    if (R.Code < 200 || R.Code >= 300)
                    {
                        throw new GitLabServerErrorException(R.Body, R.Code);
                    }
                    else
                    {
                        return JsonConvert.DeserializeObject<Note>(R.Body);
                    }
                }

            }
        }
    }
}
