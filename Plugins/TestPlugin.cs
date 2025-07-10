using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Apertadeiras_POC.Plugins;

public class TestPlugin
{
    [KernelFunction("echo")]
    [Description("Echoes the input text.")]
    public string Echo(string text) => text;
}


