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

            IRestResponse response = client.Execute(request);
            var content = response.Content; 
        }
    }
}
