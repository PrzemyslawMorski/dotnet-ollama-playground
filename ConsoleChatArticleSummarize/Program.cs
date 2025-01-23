using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddChatClient(new OllamaChatClient(new Uri("http://localhost:11434"), "tinydolphin"));

var app = builder.Build();

var chatClient = app.Services.GetRequiredService<IChatClient>();


var posts = Directory.GetFiles("posts").Take(5).ToArray();
foreach (var post in posts)
{
    var prompt = $$"""
                   You will receive an input text and the desired output format.
                   You need to analyze the text and produce the desired output format.
                   You not allow to change code, text, or other references.

                   # Desired response

                   Only provide a RFC8259 compliant JSON response following this format without deviation.

                   {
                      "title": "Title pulled from the front matter section",
                      "tags": "Array of tags based on analyzing the article content. Tags should be lowercase."
                   }

                   # Article content:

                   {{await File.ReadAllTextAsync(post)}}
                   """;

    var chatCompletion = await chatClient.CompleteAsync(prompt);
    Console.WriteLine(chatCompletion.Message.Text);
    Console.WriteLine(Environment.NewLine);

    var chatCompletionStructured = await chatClient.CompleteAsync<PostCategory>(prompt);
    Console.WriteLine($"{chatCompletionStructured.Result.Title}. Tags: {string.Join(",", chatCompletionStructured.Result.Tags)}");
}

internal record PostCategory
{
    public string Title { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
}