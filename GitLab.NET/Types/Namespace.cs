using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using unirest_net.http;
using System.Web;



namespace GitLabDotNet
{
    public partial class GitLab
    {
        public class Namespace : object
        {
            public int id;
            public string path, kind;

            public override string ToString()
            {
                return path;
            }

            public static List<Namespace> List(Config _Config, bool _OnlyOwned = true, string _Search = null)
            {

                List<Namespace> RetVal = new List<Namespace>();

                try
                {
                    int page = 1;                    

                    User Me = JsonConvert.DeserializeObject<User>(Unirest.get(_Config.APIUrl + "user")
                        .header("accept", "application/json")
                        .header("PRIVATE-TOKEN", _Config.APIKey)
                        .asString().Body);

                    List<Namespace> namespaces = new List<Namespace>();

                    do
                    {
                        namespaces.Clear();

                        string URI = _Config.APIUrl + "/namespaces?per_page=100"
                                + "&page=" + page.ToString();

                        if (_Search != null)
                            URI += "%&search=" + HttpUtility.UrlEncode(_Search);

                        HttpResponse <string> R = Unirest.get(URI)
                                .header("accept", "application/json")
                                .header("PRIVATE-TOKEN", _Config.APIKey)
                                .asString();

                        if (R.Code < 200 || R.Code >= 300)
                        {
                            throw new GitLabServerErrorException(R.Body, R.Code);
                        }

                        namespaces = JsonConvert.DeserializeObject<List<Namespace>>(R.Body);

                        foreach (Namespace NS in namespaces)
                        {
                            if ((Me.username.ToUpperInvariant() == NS.path.ToUpperInvariant()
                                    && NS.kind.ToUpperInvariant() == "USER")
                                    || NS.kind.ToUpperInvariant() == "GROUP"
                                    || !_OnlyOwned
                                    )
                            {
                                RetVal.Add(NS);
                            }
                        }

                        page++;
                        RetVal.AddRange(namespaces);
                    }
                    while (namespaces.Count > 0 & page < 100);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RetVal;
            }
        }
    }
}

