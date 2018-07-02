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
        public partial class Project
        {
            public class MergeRequest
            {

                private Project Parent;               

                internal void SetParent(Project _Project)
                {
                    Parent = _Project;
                }

                /// <summary>
                /// Accepts the Merge Request.
                /// </summary>
                /// <param name="CommitMessage">The commit message.</param>
                public void Accept(string CommitMessage = null)
                {
                    if (Parent != null)
                    {                        
                        MergeRequest.Accept(Parent.Parent.CurrentConfig, this, CommitMessage);
                    }
                    else
                        throw new GitLabStaticAccessException("Unable to complete operation without parent project");
                }

                public void Update(StateEvent _NewState = StateEvent.None)
                {
                    if (Parent != null)
                    {
                        MergeRequest.Update(Parent.Parent.CurrentConfig, this, this.target_branch, this.title, this.description, this.assignee, labels, _NewState );
                    }
                    else
                        throw new GitLabStaticAccessException("Unable to complete operation without parent project");
                }

                public int id, iid, project_id, source_project_id, target_project_id = -1;
                /// <summary>
                /// With GitLab 8.2 the return fields upvotes and downvotes are deprecated and always return 0 
                /// </summary>
                public int upvotes;
                /// <summary>
                /// With GitLab 8.2 the return fields upvotes and downvotes are deprecated and always return 0
                /// </summary>
                public int downvotes;
                public string target_branch, source_branch, title, state, description;
                public string[] labels;
                public Milestone milestone;
                public Repository.Diff[] files;
                public User author;
                public User assignee;
                public bool work_in_progress;
                public int user_notes_count;

                public enum StateEvent
                {
                    None, Close, Reopen, Merge
                }

                /// <summary>
                /// List all merge requests
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <returns></returns>
                public static List<MergeRequest> List(Config _Config, Project _Project)
                {
                    List<MergeRequest> RetVal = new List<MergeRequest>();

                    try
                    {
                        int page = 1;
                        List<MergeRequest> mergerequests = new List<MergeRequest>();

                        do
                        {
                            mergerequests.Clear();

                            string URI = _Config.APIUrl + "projects/" + _Project.id + "/merge_requests";
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
                                        MergeRequest W = JsonConvert.DeserializeObject<MergeRequest>(Token.ToString());
                                        mergerequests.Add(W);
                                    }
                                   
                                }
                            }
                            page++;
                            RetVal.AddRange(mergerequests);
                        }
                        while (mergerequests.Count > 0 & page < 100);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_ID"></param>
                /// <param name="_WithChanges">Get MergeRequest with associated changes. See https://gitlab.com/help/api/merge_requests.md#get-single-mr-changes</param>
                /// <returns></returns>
                public static MergeRequest Get(Config _Config, Project _Project, int _ID, bool _WithChanges = false)
                {
                    MergeRequest RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "/projects/" + _Project.id + "/merge_requests/" + _ID.ToString();

                        if (_WithChanges)
                            URI += "/changes";

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
                            RetVal = JsonConvert.DeserializeObject<MergeRequest>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Creates a new merge request.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_SourceBranch">The source branch</param>
                /// <param name="_TargetBranch">The target branch</param>
                /// <param name="_Title">Title of MR</param>
                /// <param name="_Description">Description of MR</param>
                /// <param name="_Asignee">Assignee user </param>
                /// <param name="_TargetProject">The target project</param>
                /// <param name="_Labels">Labels for MR as a comma-separated list</param>
                /// <returns></returns>
                public static MergeRequest Create(Config _Config, Project _Project, string _SourceBranch, string _TargetBranch, string _Title
                    , string _Description=null, User _Asignee = null, Project _TargetProject = null, string[] _Labels=null )
                {
                    MergeRequest RetVal;
                    try
                    {
                        string labels = "";
                        bool first = true;

                        foreach (string l in _Labels)
                        {
                            if (!first)
                                labels += ",";

                            labels += l;
                            first = false;
                        }

                        string URI = _Config.APIUrl + "/projects/" + _Project.id + "/merge_requests"
                            + "?source_branch=" + HttpUtility.UrlEncode(_SourceBranch)
                            + "&target_branch=" + HttpUtility.UrlEncode(_TargetBranch)
                            + "&title=" + HttpUtility.UrlEncode(_Title);

                        if (_Asignee != null)
                            URI += "&assignee_id=" + _Asignee.id.ToString();
                        if (_Description != null)
                            URI += "&description=" + HttpUtility.UrlEncode(_Description);
                        if (_TargetProject != null)
                            URI += "&target_project_id=" + _TargetProject.id.ToString();
                        if (_Labels != null)
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
                            RetVal = JsonConvert.DeserializeObject<MergeRequest>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Update Merge Request
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_MergeRequest"></param>
                /// <param name="_TargetBranch"></param>
                /// <param name="_Title"></param>
                /// <param name="_Description"></param>
                /// <param name="_Asignee"></param>
                /// <param name="_Labels"></param>
                /// <returns></returns>
                public static MergeRequest Update(Config _Config, MergeRequest _MergeRequest, string _TargetBranch =null, string _Title=null
                  , string _Description = null, User _Asignee = null, string[] _Labels = null, StateEvent _StateEvent = StateEvent.None)
                {
                    MergeRequest RetVal;
                    try
                    {
                        bool first = true;
                        string ls = "";

                        foreach (string l in _Labels)
                        {
                            if (!first)
                                ls += ",";

                            ls += l;
                            first = false;
                        }

                        string URI = _Config.APIUrl + "/projects/" + _MergeRequest.project_id + "/merge_requests/" + _MergeRequest.id + "?";

                        if (_TargetBranch != null)
                            URI += "&target_branch=" + HttpUtility.UrlEncode(_TargetBranch);
                        if(_Title != null)
                            URI += "&title=" + HttpUtility.UrlEncode(_Title);
                        if (_Asignee != null)
                            URI += "&assignee_id=" + _Asignee.id.ToString();
                        if (_Description != null)
                            URI += "&description=" + HttpUtility.UrlEncode(_Description);                        
                        if (_Labels != null)
                            URI += "&labels=" + HttpUtility.UrlEncode(ls);
                        if (_StateEvent != StateEvent.None)
                            URI += "&state_event=" + _StateEvent.ToString().ToLowerInvariant();

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
                            RetVal = JsonConvert.DeserializeObject<MergeRequest>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Accepts the specified MergeRequest.
                /// </summary>
                /// <param name="_Config">The configuration.</param>
                /// <param name="_MergeRequest">The merge request.</param>
                /// <param name="_CommitMessage">The commit message (Optional).</param>
                /// <returns></returns>
                /// <exception cref="GitLabServerErrorException"></exception>
                public static MergeRequest Accept(Config _Config, MergeRequest _MergeRequest, string _CommitMessage = null)
                {
                    MergeRequest RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "/projects/" + _MergeRequest.project_id + "/merge_requests/" + _MergeRequest.id + "/merge?";

                        if (_CommitMessage != null)
                            URI += "&merge_commit_message=" + HttpUtility.UrlEncode(_CommitMessage);                       

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
                            RetVal = JsonConvert.DeserializeObject<MergeRequest>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }


                /// <summary>
                /// Lists the comments.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_MergeRequest"></param>
                /// <returns></returns>
                public static List<Note> ListComments(Config _Config, MergeRequest _MergeRequest)
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

                            URI += "projects/" + _MergeRequest.project_id.ToString() + "/merge_requests/" + _MergeRequest.id.ToString() + "/notes";


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
                /// Gets a comment by numeric ID.
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_MergeRequest">The _ merge request.</param>
                /// <param name="_ID">The _ identifier.</param>
                /// <returns></returns>
                public static Note GetComment(Config _Config, MergeRequest _MergeRequest, int _ID)
                {
                    string URI = _Config.APIUrl + "projects/" + _MergeRequest.project_id.ToString() + "/merge_requests/" + _MergeRequest.id.ToString() + "/notes/" + _ID.ToString();

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
                /// <param name="_MergeRequest">The _ merge request.</param>
                /// <param name="_Note">The _ note.</param>
                /// <returns></returns>
                public static Note UpdateComment(Config _Config, MergeRequest _MergeRequest, Note _Note)
                {
                    string URI = _Config.APIUrl + "projects/" + _MergeRequest.project_id.ToString() + "/merge_requests/" + _MergeRequest.id + "/notes/" + _Note.id
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
                /// <param name="_MergeRequest">The _ merge request.</param>
                /// <param name="_Body">The _ body.</param>
                /// <returns></returns>
                public static Note AddComment(Config _Config, MergeRequest _MergeRequest, string _Body)
                {
                    string URI = _Config.APIUrl + "projects/" + _MergeRequest.project_id.ToString() + "/merge_requests/" + _MergeRequest.id + "/notes?"
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

            class MergeRequestList: List<MergeRequest>
            {
                private Project Parent;

                public MergeRequestList(Project _Project)
                {
                    Parent = _Project;
                }

                public void RefreshItems()
                {
                 
                    if (Parent != null)
                    {
                        this.Clear();
                        foreach (MergeRequest M in MergeRequest.List(Parent.Parent.CurrentConfig, Parent))
                        {
                            base.Add(M);
                            M.SetParent(Parent);
                        }
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project available for operation.");
                }

                new public void Add(MergeRequest _MergeRequest)
                {
                    if (Parent != null)
                    {
                        Project P = null;

                        if(_MergeRequest.target_project_id != -1)
                        {
                            P = new Project();
                            P.id = _MergeRequest.target_project_id;
                        }

                        MergeRequest.Create(Parent.Parent.CurrentConfig, Parent, _MergeRequest.source_branch
                            , _MergeRequest.target_branch, _MergeRequest.title, _MergeRequest.description, _MergeRequest.assignee, P, _MergeRequest.labels);

                        RefreshItems();
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project available for operation.");
                }
                

                new public void Remove(MergeRequest _MergeRequest)
                {
                    throw new InvalidOperationException("Removing Merge Requests is not supported by the GitLab API.");
                }
            }
        }
    }
}
