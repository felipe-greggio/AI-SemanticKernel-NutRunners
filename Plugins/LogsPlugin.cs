using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Apertadeiras_POC.Plugins;

public class LogsPlugin
{
    
    // Mock data for the lights
    private readonly List<NutRunnersModel> nutRunners = new()
   {
      new NutRunnersModel { Id = 1, Name = "NutRunner 1", IpAddress = "10.236.170.146", IsOpen = true, ErrorLog = null },
      new NutRunnersModel { Id = 2, Name = "NutRunner 2", IpAddress = "192.168.1.2", IsOpen = true, ErrorLog = null },
      new NutRunnersModel { Id = 3, Name = "NutRunner 3", IpAddress = "192.168.1.3", IsOpen = true, ErrorLog = null },
      new NutRunnersModel { Id = 4, Name = "NutRunner 4", IpAddress = "192.168.1.4", IsOpen = true, ErrorLog = null }
   };

    [KernelFunction("get_nutrunners_state")]
    [Description("Gets a list of nutrunners and their current state")]
    public async Task<List<NutRunnersModel>> GetNutRunnersAsync()
    {
        return nutRunners;
    }

    [KernelFunction("change_state")]
    [Description("Changes the state of the nutRunner")]
    public async Task<NutRunnersModel?> ChangeStateAsync(string ipAddress, bool isOpen, string? errorLog, string? solution)
    {
        var nutRunner = nutRunners.FirstOrDefault(nutRunner => nutRunner.IpAddress == ipAddress);

        if (nutRunner == null)
        {
            return null;
        }

        nutRunner.IsOpen = isOpen;
        nutRunner.ErrorLog = errorLog;
        nutRunner.Solution = solution;

        return nutRunner;
    }

}

public class NutRunnersModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("ip_address")]
    public string IpAddress { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("is_open")]
    [Description("Indicates whether the nutrunner channel is currently open  (true) or closed (false).")]
    public bool? IsOpen { get; set; }

    [JsonPropertyName("error_log")]
    public string? ErrorLog { get; set; }

    [JsonPropertyName("solution")]
    public string? Solution{ get; set; }

}