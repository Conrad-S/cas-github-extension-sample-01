<a href="https://" target="_blank">
    <img src="https://aka.ms/deploytoazurebutton" alt="Deploy to Azure">
</a>

### Development-Status
```json
{
  "version": "Alpha",
  "working": "No (Calls to Octokit fails as unauthorized)",
  "developer-required": "Yes"
}
```

**Project description**

This is a GitHub Copilot extension. When deployed, the extension exposes an API that can be called to interact with GitHub Copilot and your own API's (consider this the GitHub Copilot version of the retrieve, augment, generate or RAG pattern).

Note: GitHub Copilot Extensions are in preview.

**Implementation**

The GitHub Copilot extension is developed in C# and deployed as an API in an Azure App Service.

Once deployed, users call the API to interact with the extension.

**Configuration instructions**

1. Deploy a GitHub application to a GitHub enterprise or GitHub personal acccount by following these instructions (TODO: LINK).
2. Deploy the extension to an Azure App Service.
3. Call the App Service's API to interact with the extension.
      
**Hello GitHub Copilot! -- How to modify the extension to call your own API -- (TECHNICAL ICON HERE)**

Once deployed, calling the extension returns: "Hello GitHub Copilot!".

To call your own API prior to calling GitHub Copilot:
 - Call your API
 - Use the data returned from your API to add a new or modify an existing message to GitHub Copilot.
 - Send the message to GitHub Copilot (HTTP request to the GitHub Copilot chat completions endpoint).

Below is an exaplanation of the source code, along with instructions on where to insert your call to your API:

```csharp
app.MapPost("/agent", async (
    [FromHeader(Name = "X-GitHub-Token")] string githubToken, 
    [FromBody] Request userRequest) =>
{

    // CALL YOUR API HERE
    string myAPIReturn = callingMyAPIHere();

    userRequest.Messages.Insert(0, new Message
    {
        Role = "system",
        Content = "You are a helpful assistant that replies to user messages as if you are Blackbeard the Pirate."
    });

    userRequest.Messages.Insert(1, new Message
    {
        Role = "user",
        Content = myAPIReturn;
    });

```csharp
   
