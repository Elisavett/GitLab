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
            public class Snippet
            {
                public int id;
                public string title, file_name, expires_at, updated_at, created_at;
                public User author;

                /// <summary>
                ///  Get a list of project snippets.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <returns></returns>
                public static List<Snippet> List(Config _Config, Project _Project)
                {
                    List<Snippet> RetVal = new List<Snippet>();

                    try
                    {
                        int page = 1;
                        List<Snippet> snippets = new List<Snippet>();

                        do
                        {
                            snippets.Clear();

                            string URI = _Config.APIUrl +  "projects/" + _Project.id.ToString() + "/snippets" ; 
                            
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
                                        Snippet S = JsonConvert.DeserializeObject<Snippet>(Token.ToString());
                                        snippets.Add(S);
                                    }
                                }
                            }
                            page++;
                            RetVal.AddRange(snippets);
                        }
                        while (snippets.Count > 0 & page < 100);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Get a single project snippet
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_ID"></param>
                /// <returns></returns>
                public static Snippet Get(Config _Config, Project _Project, int _ID)
                {
                    Snippet RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/" + _ID.ToString();

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
                            RetVal = JsonConvert.DeserializeObject<Snippet>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Gets the raw content of a snippet.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project">Project Descriptor object that snippet belongs to</param>
                /// <param name="_ID">Snippet ID</param>
                /// <returns></returns>
                public static Stream GetContent(Config _Config, Project _Project, int _ID)
                {
                    Stream RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/" + _ID.ToString() + "/raw";

                        HttpResponse<Stream> R = Unirest.get(URI)                                    
                                    .header("PRIVATE-TOKEN", _Config.APIKey)
                                    .asBinary();

                        if (R.Code < 200 || R.Code >= 300)
                        {
                            throw new GitLabServerErrorException(new StreamReader(R.Body).ReadToEnd(), R.Code);
                        }
                        else
                        {
                            RetVal = R.Body;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                public static Snippet Create(Config _Config, Project _Project, string _Title, string _FileName, string _Code, VisibilityLevel _VisibilityLevel)
                {
                    Snippet RetVal;
                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/"
                                + "?title=" + HttpUtility.UrlEncode(_Title)
                                + "&file_name=" + HttpUtility.UrlEncode(_FileName)
                                + "&code=" + HttpUtility.UrlEncode(_Code)
                                + "&visibility_level=" + Convert.ToInt64(_VisibilityLevel).ToString();

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
                            RetVal = JsonConvert.DeserializeObject<Snippet>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    return RetVal;
                }

                /// <summary>
                /// Delete snippet from a project.
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_Snippet"></param>
                /// <returns></returns>
                public static Snippet Delete(Config _Config, Project _Project, Snippet _Snippet)
                {
                    Snippet RetVal;

                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/" + _Snippet.id.ToString();

                        HttpResponse<string> R = Unirest.delete(URI)
                                    .header("accept", "application/json")
                                    .header("PRIVATE-TOKEN", _Config.APIKey)
                                    .asString();

                        if (R.Code < 200 || R.Code >= 300)
                        {
                            throw new GitLabServerErrorException(R.Body, R.Code);
                        }
                        else
                        {
                            RetVal = JsonConvert.DeserializeObject<Snippet>(R.Body.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    return RetVal;
                }

                /// <summary>
                /// Update the properties of a project snippet
                /// </summary>
                /// <param name="_Config"></param>
                /// <param name="_Project"></param>
                /// <param name="_Snippet"></param>
                /// <param name="_NewCode"></param>
                /// <param name="_NewVisibilityLevel"></param>
                public static void Update(Config _Config, Project _Project, Snippet _Snippet, string _NewCode = null, VisibilityLevel _NewVisibilityLevel = VisibilityLevel.Undefined)
                {                   
                    try
                    {
                        string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/" + _Snippet.id.ToString()
                                + "?title=" + HttpUtility.UrlEncode(_Snippet.title)
                                + "&file_name=" + HttpUtility.UrlEncode(_Snippet.file_name);

                        if (_NewCode != null)
                            URI += "&code=" + HttpUtility.UrlEncode(_NewCode);

                        if(_NewVisibilityLevel != VisibilityLevel.Undefined)
                            URI += "&visibility_level=" + Convert.ToInt64(_NewVisibilityLevel).ToString();

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
                /// Lists the comments associated with the Snippet
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_Snippet">The _ snippet.</param>
                /// <returns></returns>
                public static List<Note> ListComments(Config _Config, Project _Project, Snippet _Snippet)
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

                            URI += "projects/" + _Project.ToString() + "/snippets/" + _Snippet.id.ToString() + "/notes";


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
                /// Gets a specific comment from the Snippet by Note ID
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ projet.</param>
                /// <param name="_Snippet">The _ snippet.</param>
                /// <param name="_ID">The _ identifier.</param>
                /// <returns></returns>
                public static Note GetComment(Config _Config, Project _Project, Snippet _Snippet, int _ID)
                {
                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/" + _Snippet.id.ToString() + "/notes/" + _ID.ToString();

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
                /// <param name="_Project">The _ project.</param>
                /// <param name="_Snippet">The _ snippet.</param>
                /// <param name="_Note">The _ note.</param>
                /// <returns></returns>
                public static Note UpdateComment(Config _Config, Project _Project, Snippet _Snippet, Note _Note)
                {
                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/" + _Snippet.id + "/notes/" + _Note.id
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
                /// Adds a comment to the Snippet
                /// </summary>
                /// <param name="_Config">The _ configuration.</param>
                /// <param name="_Project">The _ project.</param>
                /// <param name="_Snippet">The _ snippet.</param>
                /// <param name="_Body">The _ body.</param>
                /// <returns></returns>
                public static Note AddComment(Config _Config, Project _Project, Snippet _Snippet, string _Body)
                {
                    string URI = _Config.APIUrl + "projects/" + _Project.id.ToString() + "/snippets/" + _Snippet.id.ToString() + "/notes?"
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
