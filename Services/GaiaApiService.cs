using OpenAI;
using OpenAI.Chat;
using OpenAI.Audio;

namespace GaiaAgent.Services;

public class GaiaApiService
{
    private readonly ChatClient _chat;
    private readonly AudioClient _tts;
    private readonly AudioClient _stt;

    public GaiaApiService(string apiKey)
    {
        var client = new OpenAIClient(apiKey);

        // Correct clients EXACTLY as documented in 2.7.0
        _chat = client.GetChatClient("gpt-4o-mini");
        _tts = client.GetAudioClient("gpt-4o-mini-tts");
        _stt = client.GetAudioClient("whisper-1");
    }

    // =====================================================
    // 1. CHAT COMPLETIONS
    // =====================================================
    public async Task<string> AskGaiaAsync(string message)
    {
        ChatCompletion completion = await _chat.CompleteChatAsync(message);
        return completion.Content[0].Text;
    }

    // =====================================================
    // 2. TEXT → SPEECH
    // =====================================================
    public async Task<byte[]> SynthesizeAsync(string text)
    {
        SpeechGenerationOptions opts = new()
        { 
            ResponseFormat = GeneratedSpeechFormat.Mp3
        };
         
        var audio = await _tts.GenerateSpeechAsync(
            text,
            GeneratedSpeechVoice.Alloy,
            opts
        );

        return audio.Value.ToArray();  
    }

    // =====================================================
    // 3. SPEECH → TEXT
    // =====================================================
    public async Task<string> TranscribeAsync(string filePath)
    {
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Text
        };

        AudioTranscription transcription =
            await _stt.TranscribeAudioAsync(filePath, options);

        return transcription.Text;
    }

    public async Task<string> AskGaiaWithHistoryAsync(List<ChatMessage> history)
    {
        ChatCompletion completion =
            await _chat.CompleteChatAsync(history);

        return completion.Content.FirstOrDefault()?.Text ?? "";
    }
}
