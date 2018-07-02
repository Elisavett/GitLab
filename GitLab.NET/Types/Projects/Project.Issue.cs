using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using unirest_net.http;
using System.Web;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace GitLabDotNet
{
    public partial class GitLab
    {
        public partial class Project
        {
            public class Issue
            {
                public int id, iid, project_id;
                public string[] labels;
                public Milestone milestone;
                public User assignee;
                public User author;
                public string title, description, state, updated_at, created_at, due_date;

                public enum StateEvent
                {
                    None, Close, Reopen
                }

                /// <summary>
                /// Lists all Issues for a given project or the current user if _Project is null
                /// </summary>
                /// <param name="_Config">The configuration.</param>
                /// <param name="_Project">The project.</param>
                /// <returns></returns>
                public static List<Issue> List(Config _Config, Project _Project = null, Milestone _Milestone = null)
                {
                    List<Issue> RetVal = new List<Issue>();

                    int page = 0
                        ;
                    List<Issue> issues = new List<Issue>();

                    do
                    {
                        issues.Clear();

                        string URI = _Config.APIUrl;
                        if(_Milestone != null)
                            URI += "/projects/" + _Milestone.project_id.ToString() + "/milestones/" +_Milestone.id+ "/issues";
                        else if (_Project != null)
                            URI += "/projects/" + _Project.id.ToString() + "/issues";
                        else
                            URI += "/issues";

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
                                    var v = Token.ToString();
                                    Issue I = JsonConvert.DeserializeObject<Issue>(v);
                                    issues.Add(I);
                                }
                            }
                        }

                        page++;
                        RetVal.AddRange(issues);
                    }
                    while (issues.Count > 0 & page < 100);

                    return RetVal.GroupBy(x=>x.id).Select(x=>x.First()).ToList();
                }

                /// <summary>
                /// Gets the specified Issue by numeric ID.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_ID">The _ identifier.</param>
                /// <returns></returns>
                public static Issue Get(Config _Config, Project _Project, int _ID)
                {
                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/issues/" + _ID.ToString();

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
                        return JsonConvert.DeserializeObject<Issue>(R.Body);
                    }
                }

                public static Issue Add(Config _Config, Project _Project, Issue _Issue)
                {
                    string labels = null;
                    bool first = true;

                    if (_Issue.labels != null)
                    {
                        labels = "";

                        foreach (string l in _Issue.labels)
                        {
                            if (!first)
                                labels += ",";

                            labels += l;
                            first = false;
                        }
                    }

                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/issues?"
                        + "title=" + HttpUtility.UrlEncode(_Issue.title)
                        + "&description=" + HttpUtility.UrlEncode(_Issue.description);

                    if (_Issue.assignee != null)
                        URI += "&assignee_id=" + _Issue.assignee.id;
                    if (_Issue.milestone != null)
                        URI += "&milestone_id=" + _Issue.milestone.id;
                    if (_Issue.labels != null)
                        URI += "&labels=" + HttpUtility.UrlEncode(labels);

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
                        return JsonConvert.DeserializeObject<Issue>(R.Body);
                    }
                }


                /// <summary>
                /// Saves the specified Issue on the GitLab server
                /// </summary>
                /// <param name="_Config">The configuration.</param>
                /// <param name="_Issue">The issue.</param>
                /// <param name="_StateEvent"></param>
                /// <returns></returns>
                public static Issue Update(Config _Config, Issue _Issue, StateEvent _StateEvent = StateEvent.None)
                {
                    string labels = null;
                    bool first = true;

                    if (_Issue.labels != null)
                    {
                        labels = "";

                        foreach (string l in _Issue.labels)
                        {
                            if (!first)
                                labels += ",";

                            labels += l;
                            first = false;
                        }
                    }

                    string URI = _Config.APIUrl + "projects/" + _Issue.project_id.ToString() + "/issues/" + _Issue.id.ToString()
                        + "?title=" + HttpUtility.UrlEncode(_Issue.title)
                        + "&description=" + HttpUtility.UrlEncode(_Issue.description);
                    if (_Issue.assignee != null)
                        URI += "&assignee_id=" + _Issue.assignee.id;
                    if (_Issue.milestone != null)
                        URI += "&milestone_id=" + _Issue.milestone.id;
                    if (_Issue.labels != null)
                        URI += "&labels=" + HttpUtility.UrlEncode(labels);
                    if (_StateEvent != StateEvent.None)
                        URI += "&state_event=" + HttpUtility.UrlEncode(_StateEvent.ToString().ToLowerInvariant());

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
                        return JsonConvert.DeserializeObject<Issue>(R.Body);
                    }
                }

                /// <summary>
                /// Lists the comments.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Issue">The _ issue.</param>
                /// <returns></returns>
                public static List<Note> ListComments(Config _Config, Issue _Issue)
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

                            URI += "projects/" + _Issue.project_id.ToString() + "/issues/" + _Issue.id.ToString() + "/notes";


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
                /// <param name="_Issue">The _ issue.</param>
                /// <param name="_ID">The _ identifier.</param>
                /// <returns></returns>
                public static Note GetComment(Config _Config, Issue _Issue, int _ID)
                {
                    string URI = _Config.APIUrl + "projects/" + _Issue.project_id.ToString() + "/issues/" + _Issue.id.ToString() + "/notes/" + _ID.ToString();

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
                /// <param name="_Issue">The _ issue.</param>
                /// <param name="_Note">The _ note.</param>
                /// <returns></returns>
                public static Note UpdateComment(Config _Config, Issue _Issue, Note _Note)
                {
                    string URI = _Config.APIUrl + "projects/" + _Issue.project_id.ToString() + "/issues/" + _Issue.id + "/notes/" + _Note.id
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
                /// Adds a comment.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Issue">The _ issue.</param>
                /// <param name="_Body">The _ body.</param>
                /// <returns></returns>
                public static Note AddComment(Config _Config, Issue _Issue, string _Body)
                {
                    string URI = _Config.APIUrl + "projects/" + _Issue.project_id.ToString() + "/issues/" + _Issue.id + "/notes?"
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
