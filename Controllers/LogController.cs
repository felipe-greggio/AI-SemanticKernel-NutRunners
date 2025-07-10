using Apertadeiras_POC.Plugins;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI.Chat;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatMessageContent = Microsoft.SemanticKernel.ChatMessageContent;

namespace Apertadeiras_POC.Controllers;


[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{

    private readonly ILogger<LogController> _logger;
    private readonly ChatCompletionAgent _agent;

    public LogController(ILogger<LogController> logger)
    {
        _logger = logger;
    }



    [HttpPost("test-echo-kernel-only")]
    public async Task<IActionResult> TestEchoKernelOnly(
    [FromServices] Kernel kernel,
    [FromServices] ILogger<LogController> logger)
    {
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings executionSettings = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Please call the echo function with the text 'hello world'.");
        logger.LogInformation("--- Step 1: Initial user prompt sent ---");

        var decisionResponse = await chatCompletionService.GetChatMessageContentAsync(
            chatHistory,
            executionSettings,
            kernel);

        Console.WriteLine("Assistant > " + decisionResponse);

        chatHistory.AddMessage(decisionResponse.Role, decisionResponse.Content ?? string.Empty);

        return Ok(decisionResponse.Content);
    }

    [HttpPost("upload-log")]
    public async Task<IActionResult> UploadLog(IFormFile logFile, [FromServices] Kernel kernel)
    {
        if (logFile == null || logFile.Length == 0)
            return BadRequest("No file uploaded.");

        using var reader = new StreamReader(logFile.OpenReadStream());
        var logContent = await reader.ReadToEndAsync();


        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var chatHistory = new ChatHistory();

        chatHistory.AddSystemMessage(this.GetPrompt());

        chatHistory.AddUserMessage(logContent);

        var result = await chatCompletionService.GetChatMessageContentAsync(
        chatHistory,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel);

        return Ok(result.Content);
    }


    [HttpPost("upload-log-with-agent")]
    public async Task<IActionResult> UploadLogWithAgent(IFormFile logFile, [FromServices] Kernel kernel)
    {
        if (logFile == null || logFile.Length == 0)
            return BadRequest("No file uploaded.");

        using var reader = new StreamReader(logFile.OpenReadStream());
        var logContent = await reader.ReadToEndAsync();


        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        ChatCompletionAgent agent = new()
        {
            Name = "Carlito",
            Instructions = this.GetPrompt(),
            Kernel = kernel,
            Arguments = new KernelArguments(openAIPromptExecutionSettings)
        };

        var chatHistory = new ChatHistory();

        chatHistory.AddUserMessage(logContent);

        var result = await agent.InvokeAsync(chatHistory).FirstAsync();

        return Ok(result.Message.Content);
    }

    [HttpPost("Agent-chat-about-log")]
    public async Task<IActionResult> AgentChatWithAIAboutLog(IFormFile logFile, [FromServices] Kernel kernel)
    {
        if (logFile == null || logFile.Length == 0)
            return BadRequest("No file uploaded.");

        using var reader = new StreamReader(logFile.OpenReadStream());
        var logContent = await reader.ReadToEndAsync();


        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        ChatCompletionAgent agent = new()
        {
            Name = "UPLOG",
            Instructions = this.GetPrompt(),
            Kernel = kernel,
            Arguments = new KernelArguments(openAIPromptExecutionSettings)
        };

        var chatHistory = new ChatHistory();

        chatHistory.AddUserMessage(logContent);

        string? userInput;

        var firstAnalysis = await agent.InvokeAsync(chatHistory).FirstAsync();
        var thread = firstAnalysis.Thread;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Assistant > " + firstAnalysis.Message.Content);
        Console.ResetColor();
        do
        {
            // Collect user input

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("User > ");
            userInput = Console.ReadLine();
            Console.ResetColor();

            // Add user input
            chatHistory.AddUserMessage(userInput);

            // Get the response from the AI
            var result = await agent.InvokeAsync(chatHistory, thread).FirstAsync();

            // Print the results

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Assistant > " + result.Message.Content);
            Console.ResetColor();

            // Add the message from the agent to the chat history
            // chatHistory.AddMessage(result.Role, result.Content ?? string.Empty);
        } while (userInput is not null);

        return Ok();
    }

    [HttpPost("chat-about-log")]
    public async Task<IActionResult> ChatWithAIAboutLog(IFormFile logFile, [FromServices] Kernel kernel)
    {
        if (logFile == null || logFile.Length == 0)
            return BadRequest("No file uploaded.");

        using var reader = new StreamReader(logFile.OpenReadStream());
        var logContent = await reader.ReadToEndAsync();


        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        var chatHistory = new ChatHistory();

        chatHistory.AddSystemMessage(this.GetPrompt());

        chatHistory.AddUserMessage(logContent);

        string? userInput;

        var firstAnalysis = await chatCompletionService.GetChatMessageContentAsync(
               chatHistory,
               executionSettings: openAIPromptExecutionSettings,
               kernel: kernel);

        Console.WriteLine("Incident Analyst > " + firstAnalysis);
        do
        {
            // Collect user input
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("User > ");
            Console.ResetColor();
            userInput = Console.ReadLine();

            // Add user input
            chatHistory.AddUserMessage(userInput);

            // Get the response from the AI
            var result = await chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings: openAIPromptExecutionSettings,
                kernel: kernel);

            // Print the results
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Incident Analyst > " + result);
            Console.ResetColor();

            // Add the message from the agent to the chat history
            chatHistory.AddMessage(result.Role, result.Content ?? string.Empty);
        } while (userInput is not null);

        return Ok();
    }


    [HttpPost("test-echo")]
    public async Task<IActionResult> TestEcho([FromServices] Kernel kernel)
    {

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };


        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        ChatCompletionAgent agent = new()
        {
            Name = "TestAgent",
            Instructions = "Call the echo function with the text 'hello world'.",
            Kernel = kernel,
            Arguments = new KernelArguments(openAIPromptExecutionSettings)
        };

        var chatHistory = new ChatHistory();
        chatHistory.AddUserMessage("Please call the echo function with the text 'hello world'.");

       // var agentResponse = await agent.InvokeAsync(chatHistory).FirstAsync();


        var test = await agent.InvokeAsync(chatHistory).LastAsync();


        return Ok(test.Message.Content);

    }



    private string GetPrompt()
    {
        return """
Você é um engenheiro de redes sênior com 30 anos de experiência e especialização técnica na área de dispositivos embarcados de fábrica, com foco em 'apertadeiras'. Sua função é monitorar e analisar logs para identificar problemas de rede relacionados a essas apertadeiras, determinando seu status operacional e oferecendo soluções.



Propósito e Metas:


* Analisar logs de rede para 'apertadeiras' e identificar seu status operacional (operacional, com problemas de conexão, etc.).

* Em caso de problemas, identificar o erro específico descrito no log da rede.

* Oferecer uma possível solução para o problema de rede detectado.

* Fornecer informações técnicas precisas e compreensíveis.

* IMPORTANTE: Você que determina o estado atual das apertadeiras, conforme os logs, se a última informação do log referente àquela apertadeira diz que ela está ativa/inativa agora, você tem que mudar o estado da apertadeira para refletir isso




Comportamentos e Regras:



1)  Inquérito Inicial:

    a)  Comece com uma saudação profissional e apresente-se como um engenheiro de redes sênior especializado em 'apertadeiras'.

    b)  Peça ao usuário para fornecer o log da 'apertadeira' que precisa de análise.

    c)  Se o log não for fornecido, peça detalhes sobre o problema ou sintoma que o usuário está observando.



2)  Análise de Logs e Diagnóstico:

    a)  Ao receber o log, simule uma análise detalhada, focando em padrões ou mensagens de erro.

    b)  Se a 'apertadeira' estiver operacional, declare claramente: 'A apertadeira está operacional, sem problemas de conexão identificados nos logs fornecidos.'

    c)  Se houver um problema, identifique o erro específico na rede conforme descrito no log.

    d)  Apresente uma explicação clara e concisa do erro.



3)  Sugestão de Solução:

    a)  Para cada erro identificado, ofereça uma ou mais possíveis soluções técnicas.

    b)  As soluções devem ser práticas e relacionadas a problemas de rede em dispositivos embarcados (ex: verificar cabos, configurações IP, firmware, interferência, etc.).

    c)  Evite jargões excessivos sem explicação e use uma linguagem que possa ser compreendida por um técnico.



4)  Interação Contínua:

    a)  Mantenha uma comunicação concisa e direta, com foco nos aspectos técnicos.

    b)  Convide o usuário a fornecer logs adicionais ou detalhes para uma análise mais aprofundada, se necessário.

    c)  Converse em um estilo natural, com um máximo de 3 frases por turno de diálogo.



Tom Geral:



* Profissional e técnico.

* Claro, direto e objetivo.

* Prestativo e analítico, como um engenheiro de redes experiente.

* Demonstre expertise na área de dispositivos embarcados de fábrica, especialmente 'apertadeiras'.

FUNCTION USAGE:
- ALWAYS make sure to call the appropriate functions that correspond to the users prompt.
- When it comes to the nutrunners, first call the `get_nutrunners_state` function to understand the current status of all nutrunners.
- Analyze the provided log file content.
- If you identify an error in the log for a specific nutrunner (identified by its IP address), you MUST call the `change_state` function.
- If detected an error, when calling `change_state`, you must provide the `ipAddress`, set `isOpen` to `false`, provide the relevant `errorLog` snippet in the function, and provide a technical `solution` for the problem in function.
- If it was detected that a nutrunner that has a problem, has been fixed, you  MUST call the `change_state` function. In that case, you must provide the `ipAddress`, set `isOpen` to `true`, and clean the `errorLog` and `solution` in the state, by setting them to a null value.
- After successfully calling the functions, provide a final summary to the user explaining which nutrunner had an error, what the error was, and what the proposed solution is.If a nutrunner was fixed, explain that as well.
""";

    }
}

public class JsonFunctionCall
{
    [JsonPropertyName("name")]
    public string FunctionName { get; set; }

    [JsonPropertyName("arguments")]
    public string FunctionArguments { get; set; }
}
