using RestSharp;

namespace GitHubConflictChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient(@"https://api.github.com");

            var request = new RestRequest("/repos/{owner}/{repo}/pulls", Method.POST);
            request.AddUrlSegment("owner", "RacoWireless"); // replaces matching token in request.Resource
            request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
        }
    }
}
