using System.Text.Json;
using GaiaAgent.Services;
using OpenAI.Chat;

namespace GaiaAgent.Agent;

public class Gaia
{
    private readonly GaiaApiService _llm;

    // Memory container — extensible, replaceable, injectable.
    private readonly List<ChatMessage> _conversation = new();

    // Hook points for future memory systems:
    // - TripMemory (last destination, last POIs, last route)
    // - UserPreferencesMemory (likes, dislikes, “avoid highways”, etc.)
    // - SpatialMemory (what GAIA last told the user)
    // - ProfileMemory (recurring behaviors)
    //
    // All optional. We leave the architecture open.
    //
    // Later:
    // private readonly ITripMemory _tripMem;
    // private readonly IUserPreferencesMemory _prefMem;
    // private readonly ISpatialMemory _spatialMem;

    private const string SystemPrompt = @"
You are GAIA, the GPS AI Agent.
You maintain conversational continuity with the user, but you NEVER invent map data.

You ALWAYS rely exclusively on the JSON provided for:
- POIs
- distances
- bearings
- coordinates
- routes
- heading

Conversation context may be remembered:
- user intent
- follow-up questions
- preferences
- ongoing tasks

But geographic facts MUST come only from the JSON in the current turn.

Routing rules:
- You never modify an active Google Maps route.
- To reroute, generate a new Google Maps deep link:

  https://www.google.com/maps/dir/?api=1
      &origin={userLat},{userLon}
      &destination={destLat},{destLon}
      &dir_action=navigate

Keep responses brief, natural, and helpful.
You do not hallucinate.
";

    public Gaia(GaiaApiService llm)
    {
        _llm = llm;

        // Initialize with system instruction
        _conversation.Add(new SystemChatMessage(SystemPrompt));
    }

    // -------------------------------------------------------
    // Ask GAIA (with modular memory)
    // -------------------------------------------------------
    public async Task<string> AskAsync(string userMessage, object navigationContextJson)
    {
        // Navigation JSON gets injected fresh every turn
        string jsonContext = JsonSerializer.Serialize(navigationContextJson);

        string userTurn =
$@"
CONTEXT (JSON):
{jsonContext}

USER MESSAGE:
{userMessage}
";

        // Add the user's message
        _conversation.Add(new UserChatMessage(userTurn));

        // Invoke the LLM with FULL conversation history
        string reply = await _llm.AskGaiaWithHistoryAsync(_conversation);

        // Add GAIA's reply into memory
        _conversation.Add(new AssistantChatMessage(reply));

        return reply;
    }

    // -------------------------------------------------------
    // Memory Management (extensible)
    // -------------------------------------------------------

    public void ResetConversation()
    {
        _conversation.Clear();
        _conversation.Add(new SystemChatMessage(SystemPrompt));
    }

    public IReadOnlyList<ChatMessage> GetConversation() => _conversation.AsReadOnly();
}
