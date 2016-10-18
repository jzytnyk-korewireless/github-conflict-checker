using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
using RestSharp;

namespace GitHubConflictChecker
{
    class Program
    {
        private static string getApiKey()
        {
            var apiKey = File.ReadAllText("ApiKey.txt");
            return apiKey;
        }

        private static string prUnMergeableComment = "This PR is unmergeable!";

        static void Main(string[] args)
        {
            var apiKey = getApiKey();
            var client = new RestClient(@"https://api.github.com");

            var request = new RestRequest("/repos/{owner}/{repo}/pulls", Method.GET);
            request.AddUrlSegment("owner", "dalpert-korewireless"); // replaces matching token in request.Resource
            request.AddUrlSegment("repo", "github-conflict-checker"); // replaces matching token in request.Resource

            IRestResponse<List<Pulls>> response = client.Execute<List<Pulls>>(request);
            var content = response.Content;

            var pullRequestDetails = response.Data.Select(pr =>
            {
                var request2 = new RestRequest("/repos/{owner}/{repo}/pulls/{number}");

                request2.AddUrlSegment("owner", "dalpert-korewireless"); // replaces matching token in request.Resource
                request2.AddUrlSegment("repo", "github-conflict-checker"); // replaces matching token in request.Resource
                request2.AddUrlSegment("number", pr.Number.ToString());

                var pullRequestDetail = client.Execute<PullRequestDetail>(request2).Data;

                if (pullRequestDetail.Mergeable == false)
                {
                    var getComments = new RestRequest("/repos/{owner}/{repo}/pulls/{number}/comments", Method.GET);
                    getComments.AddParameter("Authorization", "token " + apiKey, ParameterType.HttpHeader);
                    getComments.AddUrlSegment("owner", "dalpert-korewireless"); // replaces matching token in request.Resource
                    getComments.AddUrlSegment("repo", "github-conflict-checker"); // replaces matching token in request.Resource
                    getComments.AddUrlSegment("number", pr.Number.ToString());
                    getComments.RequestFormat = DataFormat.Json;
                    var comments = client.Execute(getComments);

                    if (false)
                    {
                        var request3 = new RestRequest("/repos/{owner}/{repo}/issues/{number}/comments", Method.POST);
                        request3.AddParameter("Authorization", "token " + apiKey, ParameterType.HttpHeader);
                        request3.AddUrlSegment("owner", "dalpert-korewireless"); // replaces matching token in request.Resource
                        request3.AddUrlSegment("repo", "github-conflict-checker"); // replaces matching token in request.Resource
                        request3.AddUrlSegment("number", pr.Number.ToString());
                        request3.RequestFormat = DataFormat.Json;
                        request3.AddBody(new { body = string.Format("{0} @{1}", prUnMergeableComment, pullRequestDetail.User.Login) });
                        var response3 = client.Execute(request3);            
                    }


                }

                return pullRequestDetail;
            })
                .ToList();
        }
        //private static bool NeedToBeNotified(PullRequestDetail prDetail, Pulls pr)
        //{
        //    var apiKey = getApiKey();
        //    var request = new RestRequest("/repos/{owner}/{repo}/issues/{number}/comments", Method.GET);
        //    request.AddParameter("Authorization", "token " + apiKey, ParameterType.HttpHeader);
        //    request.AddUrlSegment("owner", "dalpert-korewireless"); // replaces matching token in request.Resource
        //    request.AddUrlSegment("repo", "github-conflict-checker"); // replaces matching token in request.Resource
        //    request.AddUrlSegment("number", pr.Number.ToString());
        //    request.RequestFormat = DataFormat.Json;
        //    var response = client.Execute(request);  

        //    return false;
        //}
    }


    internal class User
    {
        public string Login{ get; set; }
    }
    internal class PullRequestDetail
    {
        public bool Mergeable { get; set; }
        public User User { get; set; }
    }

    internal class Comment
    {
        public int Id { get; set; }
    }
    public class Pulls
    {
        public int Id { get; set; }
        public int Number { get; set; }
    }
}
