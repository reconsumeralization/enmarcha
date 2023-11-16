﻿using CommunityToolkit.Diagnostics;

using Encamina.Enmarcha.Data.Abstractions;
using Encamina.Enmarcha.Data.Cosmos;

using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Plugins.Chat.Plugins;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;

namespace Encamina.Enmarcha.SemanticKernel.Plugins.Chat;

/// <summary>
/// Extension methods on <see cref="IKernel"/> to import and configure plugins.
/// </summary>
public static class IKernelExtensions
{
    /// <summary>
    /// Imports the «Chat with History» plugin into the kernel.
    /// </summary>
    /// <remarks>
    /// This extension method uses a «Service Location» pattern provided by the <see cref="IServiceProvider"/> to resolve the following dependencies:
    /// <list type="bullet">
    ///     <item>
    ///         <term>SemanticKernelOptions</term>
    ///         <description>
    ///             A required dependency of type <see cref="SemanticKernelOptions"/> used to retrieve the configurations for the <see cref="IKernel"/>. This dependency
    ///             should be added using any of the <see cref="OptionsServiceCollectionExtensions.AddOptions"/> extension method.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ChatWithHistoryPluginOptions</term>
    ///         <description>
    ///             A required dependency of type <see cref="ChatWithHistoryPluginOptions"/> used to retrieve the configuration options for this plugin. This dependency should
    ///             be added using any of the <see cref="OptionsServiceCollectionExtensions.AddOptions"/> extension method.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>ICosmosRepositoryFactory</term>
    ///         <description>
    ///             A required dependency of type <see cref="ICosmosRepositoryFactory"/> used to create a <see cref="ICosmosRepository{T}"/> (which
    ///             inherits from <see cref="IAsyncRepository{T}"/>) and manage chat history messages. Use the <c>AddCosmos</c> extension method to add this dependency.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <param name="kernel">The <see cref="IKernel"/> instance to add this plugin.</param>
    /// <param name="serviceProvider">A <see cref="IServiceProvider"/> to resolve the dependencies.</param>
    /// <param name="cosmosContainer">The name of the Cosmos DB container to store the chat history messages.</param>
    /// <param name="tokensLengthFunction">
    /// A function to calculate the length by tokens of the chat messages. These functions are usually available in the «mixin» interface <see cref="ILengthFunctions"/>.
    /// </param>
    /// <returns>A list of all the functions found in this plugin, indexed by function name.</returns>
    public static IDictionary<string, ISKFunction> ImportChatWithHistoryPluginUsingCosmosDb(this IKernel kernel, IServiceProvider serviceProvider, string cosmosContainer, Func<string, int> tokensLengthFunction)
    {
        Guard.IsNotNull(serviceProvider);
        Guard.IsNotNull(tokensLengthFunction);
        Guard.IsNotNullOrWhiteSpace(cosmosContainer);

        var semanticKernelOptions = serviceProvider.GetRequiredService<IOptionsMonitor<SemanticKernelOptions>>().CurrentValue;
        var chatWithHistoryPluginOptions = serviceProvider.GetRequiredService<IOptionsMonitor<ChatWithHistoryPluginOptions>>();
        var chatMessagesHistoryRepository = serviceProvider.GetRequiredService<ICosmosRepositoryFactory>().Create<ChatMessageHistoryRecord>(cosmosContainer);

        var chatWithHistoryPlugin = new ChatWithHistoryPlugin(kernel, semanticKernelOptions.ChatModelName, tokensLengthFunction, chatMessagesHistoryRepository, chatWithHistoryPluginOptions);

        return kernel.ImportSkill(chatWithHistoryPlugin, PluginsInfo.ChatWithHistoryPlugin.Name);
    }
}
