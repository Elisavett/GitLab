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
            private LabelList _Labels;

            public LabelList Labels
            {
                get
                {
                    if (_Labels == null)
                        _Labels = new LabelList(this);

                    return _Labels;
                }

            }
            
            public class Label
            {
                public string name, color;

                private Project Parent;

                /// <summary>
                /// Sets the parent project.
                /// </summary>
                /// <param name="_Project">The project.</param>
                internal void SetParent(Project _Project)
                {
                    Parent = _Project;
                }

                /// <summary>
                /// Updates this instance.
                /// </summary>
                /// <exception cref="GitLabStaticAccessException">No parent project for operation</exception>
                public void Update()
                {
                    if (Parent != null)
                    {
                        Label.Update(Parent.Parent.CurrentConfig, Parent, this, this.name, this.color);
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project for operation");
                }

                /// <summary>
                /// List all labels associated with a project
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <returns></returns>
                public static List<Label> List(Config _Config, Project _Project)
                {
                    List<Label> RetVal = new List<Label>();

                    try
                    {
                        int page = 1;
                        List<Label> labels = new List<Label>();

                        do
                        {
                            labels.Clear();

                            string URI = (_Config.APIUrl + "projects/" + _Project.id.ToString() + "/labels"
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
                                        Label L = JsonConvert.DeserializeObject<Label>(Token.ToString());
                                        labels.Add(L);
                                    }
                                }
                            }

                            page++;
                            RetVal.AddRange(labels);
                        }
                        while (labels.Count > 0 & page < 100);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Add a label to a project.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_Name"></param>
                /// <param name="_Color">A String representing a color in #FFFFFF format</param>
                /// <returns></returns>
                public static Label Add(Config _Config, Project _Project, string _Name, string _Color)
                {
                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/labels/"
                        + "?name=" + HttpUtility.UrlEncode(_Name)
                        + "&color=" + HttpUtility.UrlEncode(_Color);

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
                        return JsonConvert.DeserializeObject<Label>(R.Body);
                    }
                }

                /// <summary>
                /// Update a Project Label. At least one of NewName or NewColor must be specified
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_Label"></param>
                /// <param name="_NewName"></param>
                /// <param name="_NewColor"></param>
                /// <returns></returns>
                public static Label Update(Config _Config, Project _Project, Label _Label, string _NewName = null, string _NewColor = null)
                {
                    if (_NewName == null && _NewColor == null)
                        throw new ArgumentException("Color or New Name must be specified.");

                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/labels/"
                        + "?name=" + HttpUtility.UrlEncode(_Label.name);

                    if (_NewName != null)
                        URI += "&new_name=" + HttpUtility.UrlEncode(_NewName);
                    
                    if(_NewColor != null )
                        URI += "&color=" + HttpUtility.UrlEncode(_NewColor);

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
                        return JsonConvert.DeserializeObject<Label>(R.Body);
                    }
                }

                /// <summary>
                /// Deletes a label from a project. API Function request is by name, label has no ID field.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_Label"></param>
                public static void Delete(Config _Config, Project _Project, Label _Label)
                {
                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/labels/"
                        + "?name=" + HttpUtility.UrlEncode(_Label.name) ;

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

            public class LabelList: List<Label>
            {
                private Project Parent;

                public LabelList(Project _Parent)
                {
                    Parent = _Parent;
                }

                /// <summary>
                /// Refreshes the items in this list from the server
                /// </summary>
                /// <exception cref="GitLabStaticAccessException">No parent project available for operation.</exception>
                public void RefreshItems()
                {
                    if (Parent != null)
                    {
                        this.Clear();
                        foreach (Label L in Label.List(Parent.Parent.CurrentConfig, Parent))
                        {
                            base.Add(L);
                            L.SetParent(Parent);
                        }
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project available for operation.");
                }

                /// <summary>
                /// Adds the specified label to the parent project.
                /// </summary>
                /// <param name="_Label">The label.</param>
                /// <exception cref="GitLabStaticAccessException">No parent project available for operation.</exception>
                new public void Add(Label _Label)
                {
                    if (Parent != null)
                    {
                        Label.Add(Parent.Parent.CurrentConfig, Parent, _Label.name, _Label.color);
                        RefreshItems();
                        
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project available for operation.");
                }

                /// <summary>
                /// Removes the specified label.
                /// </summary>
                /// <param name="_Label">The label.</param>
                /// <exception cref="GitLabStaticAccessException">No parent project available for operation.</exception>
                new public void Remove(Label _Label)
                {
                    if (Parent != null)
                    {
                        Label.Delete(Parent.Parent.CurrentConfig, Parent, _Label);
                        RefreshItems();
                    }
                    else
                        throw new GitLabStaticAccessException("No parent project available for operation.");
                }
            }
        }
    }
}
