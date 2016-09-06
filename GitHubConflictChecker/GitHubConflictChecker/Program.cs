using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using RestSharp;

namespace GitHubConflictChecker
{
    class Program
    {
        static void Main(string[] args)
        {
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

                var detailsResponse = client.Execute<PullRequestDetail>(request2).Data;

                if (detailsResponse.Mergeable == false)
                {
                    var request3 = new RestRequest("/repos/{owner}/{repo}/issues/{number}/comments", Method.POST);
                    request3.AddUrlSegment("owner", "dalpert-korewireless"); // replaces matching token in request.Resource
                    request3.AddUrlSegment("repo", "github-conflict-checker"); // replaces matching token in request.Resource
                    request3.AddUrlSegment("number", pr.Number.ToString());
                    request3.RequestFormat = DataFormat.Json;
                    request3.AddBody(new { body = "test" });
                    client.Execute(request3);
                }

                return detailsResponse;
            })
                .ToList();
        }
    }

    internal class PullRequestDetail
    {
        public bool Mergeable { get; set; }
    }

    public class Pulls
    {
        public int Id { get; set; }
        public int Number { get; set; }
    }
}
