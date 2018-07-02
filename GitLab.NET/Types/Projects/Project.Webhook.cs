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
        partial class Project
        {
            /// <summary>
            /// The webhooks that belong to this project.
            /// </summary>
            private WebhookList _Webhooks;
                
            public WebhookList Webhooks
            {
                get
                {
                    if (_Webhooks == null)
                        _Webhooks = new WebhookList(this);
                    return _Webhooks;                    
                }
            }                      
         
            public partial class Webhook
            {
                public int id, project_id;
                public string url, created_at;
                public bool push_events, tag_push_events, issues_events, merge_requests_events, note_events, enable_ssl_verification;

                public Project Parent
                {
                    get
                    {
                        return _Parent;
                    }
                }

                internal Project _Parent;

                /// <summary>
                /// Updates this webhook on the server.
                /// </summary>
                public void Update()
                {
                    if (Parent != null)
                    {
                        Webhook.Update(Parent.Parent.CurrentConfig, this, Parent);
                    }
                    else
                    {
                        throw new GitLabStaticAccessException("Unable to save Webhook without parent project");
                    }
                }

                /// <summary>
                /// List all webhooks for a project
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <returns></returns>
                public static List<Webhook> List(Config _Config, Project _Project)
                {
                    List<Webhook> RetVal = new List<Webhook>();

                    try
                    {
                        int page = 1;
                        List<Webhook> webhooks = new List<Webhook>();

                        do
                        {
                            webhooks.Clear();

                            string URI = _Config.APIUrl + "projects/" + _Project.id + "/hooks";
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
                                        Webhook W = JsonConvert.DeserializeObject<Webhook>(Token.ToString());
                                        webhooks.Add(W);
                                    }
                                }
                            }
                            page++;
                            RetVal.AddRange(webhooks);
                        }
                        while (webhooks.Count > 0 & page < 100);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Get Project Webhook by numerif ID
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_ID"></param>
                /// <returns></returns>
                public static Webhook Get(Config _Config, Project _Project, int _ID)
                {
                    Webhook RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "/projects/" + _Project.id + "/hooks/" + _ID.ToString();

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
                            RetVal = JsonConvert.DeserializeObject<Webhook>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Adds a new webhook to a project
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Webhook"></param>
                /// <param name="_Project"></param>
                public static void Add(Config _Config, Webhook _Webhook, Project _Project)
                {
                    try
                    {
                        string URI = _Config.APIUrl + "/projects/"+_Project.id.ToString()+"/hooks";

                        URI += "?url=" + HttpUtility.UrlEncode(_Webhook.url)
                            + "&push_events=" + Convert.ToInt32(_Webhook.push_events).ToString()
                            + "&issues_events=" + Convert.ToInt32(_Webhook.issues_events).ToString()
                            + "&merge_requests_events=" + Convert.ToInt32(_Webhook.merge_requests_events).ToString()
                            + "&tag_push_events=" + Convert.ToInt32(_Webhook.tag_push_events).ToString()
                            + "&note_events=" + Convert.ToInt32(_Webhook.note_events).ToString()
                            + "&enable_ssl_verification=" + Convert.ToInt32(_Webhook.enable_ssl_verification).ToString();
                        
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
                /// Updates a project webhook on the GitLab server.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Webhook"></param>
                /// <param name="_Project"></param>
                public static void Update(Config _Config, Webhook _Webhook, Project _Project)
                {
                    try
                    {
                        string URI = _Config.APIUrl + "/projects/" + _Project.id.ToString() + "/hooks/" + _Webhook.id;

                        URI += "?url=" + HttpUtility.UrlEncode(_Webhook.url)
                            + "&push_events=" + Convert.ToInt32(_Webhook.push_events).ToString()
                            + "&issues_events=" + Convert.ToInt32(_Webhook.issues_events).ToString()
                            + "&merge_requests_events=" + Convert.ToInt32(_Webhook.merge_requests_events).ToString()
                            + "&tag_push_events=" + Convert.ToInt32(_Webhook.tag_push_events).ToString()
                            + "&note_events=" + Convert.ToInt32(_Webhook.note_events).ToString()
                            + "&enable_ssl_verification=" + Convert.ToInt32(_Webhook.enable_ssl_verification).ToString();

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
                /// Deletes a project webhook from the server. 
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Webhook"></param>
                /// <param name="_Project"></param>
                public static void Delete(Config _Config, Webhook _Webhook, Project _Project)
                {
                    try
                    {
                        string URI = _Config.APIUrl + "/projects/" + _Project.id.ToString() + "/hooks" + _Webhook.id;

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

            public class WebhookList : List<Webhook>
            {
                Project Parent;

                public WebhookList(Project _Parent)
                {
                    Parent = _Parent;
                    RefreshItems();
                }

                public void RefreshItems()
                {
                    if (Parent != null)
                    {
                        this.Clear();

                        foreach (Webhook W in Webhook.List(Parent.Parent.CurrentConfig, Parent))
                        {
                            base.Add(W);
                        }
                    }
                    else
                        throw new GitLabStaticAccessException("No Parent Project for operation");
                }

                new public void Add(Webhook _Webhook)
                {
                    if (Parent != null)
                    {
                        Webhook.Add(Parent.Parent.CurrentConfig, _Webhook, Parent);
                        RefreshItems();
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project available for operation.");
                }

                new public void Remove(Webhook _Webhook)
                {
                    if (Parent != null)
                    {
                        Webhook.Delete(Parent.Parent.CurrentConfig, _Webhook, Parent);
                        RefreshItems();
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project available for operation.");
                }
            }
        }
    }
}
