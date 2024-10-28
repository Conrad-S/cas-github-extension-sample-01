using Microsoft.AspNetCore.Mvc;
using Octokit;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello Copilot!");

string yourGitHubAppName = "cas-github-extension-01";
string githubCopilotCompletionsUrl = "https://api.githubcopilot.com/chat/completions";

app.MapPost("/agent", async (
    [FromHeader(Name = "X-GitHub-Token")] string githubToken, 
    [FromBody] Request userRequest) =>
{

    var octokitClient = new GitHubClient(
        new Octokit.ProductHeaderValue(yourGitHubAppName))
    {
        Credentials = new Credentials(githubToken)
    };

    //var user = await octokitClient.User.Current();

        userRequest.Messages.Insert(0, new Message
    {
        Role = "system",
        Content = $"Start every response with Hello There!"
    });

    userRequest.Messages.Insert(0, new Message
    {
        Role = "system",
        Content = "You are a helpful assistant that replies to user messages as if you were Blackbeard the Pirate."
    });

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", githubToken.Trim());

var jsonRequest = JsonSerializer.Serialize(userRequest);
Console.WriteLine($"Serialized User Request: {jsonRequest}");

var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

var copilotLLMResponse = await httpClient.PostAsync(githubCopilotCompletionsUrl, content);

    if (copilotLLMResponse.IsSuccessStatusCode)
    {
        var responseStream = await copilotLLMResponse.Content.ReadAsStreamAsync();     
        return Results.Stream(responseStream, "application/json");
    }
    else
    {
        var errorContent = await copilotLLMResponse.Content.ReadAsStringAsync();
        Console.WriteLine($"Error: {copilotLLMResponse.StatusCode} - {errorContent}");
        return Results.BadRequest(new { message = "Failed to get a valid response from Copilot API", details = errorContent });
    }

    //return Results.Stream(responseStream, "application/json");

    // Dummy response for testing
    //return Results.Json(new { message = "Hello, this is a dummy response!" });
});

app.MapGet("/callback", () => 
    "You may close this tab and return to GitHub.com (where you should refresh the page and start a fresh chat). If you're using VS Code or Visual Studio, return there.");

app.Run();

// Define the Request and Message classes here
public record Message
{
    public required string Role { get; set; }
    public required string Content { get; set; }
}
public record Request
{
    public bool Stream { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
}
