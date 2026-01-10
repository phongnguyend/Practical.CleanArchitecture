using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ClassifiedAds.Infrastructure.AI;

public class ImageAnalysisService
{
    private readonly IConfiguration _configuration;
    private readonly OpenAIOptions _options;
    private readonly ChatClient _chatClient;

    public ImageAnalysisService(IConfiguration configuration)
    {
        _configuration = configuration;
        _options = new OpenAIOptions();
        _configuration.GetSection("ImageAnalysis:OpenAI").Bind(_options);
        _chatClient = _options.CreateOpenAIChatClient();
    }

    public async Task<string> AnalyzeImageAsync(byte[] bytes, string mediaType, CancellationToken cancellationToken = default)
    {
        var textPart = ChatMessageContentPart.CreateTextPart("Describe this picture:");
        var imgPart = ChatMessageContentPart.CreateImagePart(new BinaryData(bytes), mediaType);

        var chatMessages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a helpful assistant."),
            new UserChatMessage(textPart, imgPart)
        };

        ChatCompletion chatCompletion = await _chatClient.CompleteChatAsync(chatMessages, cancellationToken: cancellationToken);

        return chatCompletion.Content[0].Text;
    }
}
