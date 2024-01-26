﻿using System.Reflection;

using Encamina.Enmarcha.AI.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Extensions;
using Encamina.Enmarcha.Testing;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace Encamina.Enmarcha.SemanticKernel.Tests;

[Collection(MagicStrings.FixturesCollection)]
public class KernelExtensionsTests : FakerProviderFixturedBase
{
    private const string PluginName = "PluginTest";
    private const string PluginDirectory = "TestUtilities";

    public KernelExtensionsTests(FakerProvider fakerFixture) : base(fakerFixture)
    {
    }

    [Fact]
    public void ImportPromptFunctionsFromAssembly_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();

        // Act...
        var kernelPlugin = kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());

        // Assert..
        var plugin = Assert.Single(kernelPlugin);
        Assert.Equal(PluginName, plugin.Name);

        var function = Assert.Single(plugin.ToList());
        Assert.Equal("DummyEmbedded", function.Name);
    }

    [Fact]
    public async Task GetKernelFunctionPrompt_FromPluginDirectory_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPluginFromPromptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginDirectory, PluginName));
        var function = kernel.Plugins.GetFunction(PluginName, "Dummy");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var prompt = await kernel.GetKernelFunctionPromptAsync(Path.Combine(PluginDirectory, PluginName), function, arguments);

        // Assert..
        Assert.Equal("This is a Prompt function for testing purposes dummy foo value bar value", prompt);
    }

    [Fact]
    public async Task GetKernelFunctionPrompt_FromAssembly_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());
        var function = kernel.Plugins.GetFunction(PluginName, "DummyEmbedded");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var prompt = await kernel.GetKernelFunctionPromptAsync(PluginName, Assembly.GetExecutingAssembly(), function, arguments);

        // Assert..
        Assert.Equal("This is a Prompt function for testing purposes dummy foo value bar value", prompt);
    }

    [Fact]
    public async Task GetKernelPrompt_FromInlinePrompt_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        const string promptTemplate = "This is a Prompt function for testing purposes {{$input}} {{$foo}} {{$bar}}";
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var prompt = await kernel.GetKernelPromptAsync(promptTemplate, arguments);

        // Assert..
        Assert.Equal("This is a Prompt function for testing purposes dummy foo value bar value", prompt);
    }

    [Fact]
    public async Task GetKernelFunctionUsedTokens_FromPluginDirectory_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPluginFromPromptDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PluginDirectory, PluginName));
        var function = kernel.Plugins.GetFunction(PluginName, "Dummy");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };  
          
        // Act...
        var usedTokens = await kernel.GetKernelFunctionUsedTokensAsync(Path.Combine(PluginDirectory, PluginName), function, arguments, ILengthFunctions.LengthByCharacterCount);

        //Assert
        var expectedUsedTokens = "This is a Prompt function for testing purposes dummy foo value bar value".Length + 500; // prompt with arguments + config json max tokens
        Assert.Equal(expectedUsedTokens, usedTokens);
    }

    [Fact]
    public async Task GetKernelFunctionUsedTokens_FromAssembly_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        kernel.ImportPromptFunctionsFromAssembly(Assembly.GetExecutingAssembly());
        var function = kernel.Plugins.GetFunction(PluginName, "DummyEmbedded");
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var usedTokens = await kernel.GetKernelFunctionUsedTokensAsync(PluginName, Assembly.GetExecutingAssembly(), function, arguments, ILengthFunctions.LengthByCharacterCount);

        //Assert
        var expectedUsedTokens = "This is a Prompt function for testing purposes dummy foo value bar value".Length + 500; // prompt with arguments + config json max tokens
        Assert.Equal(expectedUsedTokens, usedTokens);
    }

    [Fact]
    public async Task GetKernelFunctionUsedTokens_FromInlinePromptFunction_Succeeds()
    {
        // Arrange...
        var kernel = GivenAKernel();
        const string promptTemplate = "This is a Prompt function for testing purposes {{$input}} {{$foo}} {{$bar}}";
        var function = kernel.CreateFunctionFromPrompt(promptTemplate, new OpenAIPromptExecutionSettings() { MaxTokens = 500, Temperature = 0.4, TopP = 1 });
        var arguments = new KernelArguments()
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var usedTokens = await kernel.GetKernelFunctionUsedTokensFromPromptAsync(promptTemplate, function, arguments, ILengthFunctions.LengthByCharacterCount);

        //Assert
        var expectedUsedTokens = "This is a Prompt function for testing purposes dummy foo value bar value".Length + 500; // prompt with arguments + config json max tokens
        Assert.Equal(expectedUsedTokens, usedTokens);
    }

    [Fact]
    public async Task GetKernelFunctionUsedTokens_PrioritizesKernelArgumentsMaxTokens_Succeeds()
    {
        // Arrange...
        const int maxTokens = 1892;
        var kernel = GivenAKernel();
        const string promptTemplate = "This is a Prompt function for testing purposes {{$input}} {{$foo}} {{$bar}}";
        var function = kernel.CreateFunctionFromPrompt(promptTemplate, new OpenAIPromptExecutionSettings() { MaxTokens = 500, Temperature = 0.4, TopP = 1 });
        var arguments = new KernelArguments(new OpenAIPromptExecutionSettings()
        {
            MaxTokens = maxTokens
        })
        {
            ["input"] = "dummy",
            ["foo"] = "foo value",
            ["bar"] = "bar value"
        };

        // Act...
        var usedTokens = await kernel.GetKernelFunctionUsedTokensFromPromptAsync(promptTemplate, function, arguments, ILengthFunctions.LengthByCharacterCount);

        //Assert
        var expectedUsedTokens = "This is a Prompt function for testing purposes dummy foo value bar value".Length + maxTokens; // prompt with arguments + max tokens
        Assert.Equal(expectedUsedTokens, usedTokens);
    }

    private Kernel GivenAKernel()
    {
        return Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(FakerProvider.GetFaker().Random.Word(), 
                FakerProvider.GetFaker().Internet.Url(),
                FakerProvider.GetFaker().Random.Guid().ToString()) 
            .Build();
    }
}