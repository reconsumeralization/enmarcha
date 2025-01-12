﻿using Encamina.Enmarcha.SemanticKernel.Abstractions;
using Encamina.Enmarcha.SemanticKernel.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Encamina.Enmarcha.SemanticKernel;

/// <summary>
/// A memory store handler that removes collections from memory after a configured time (thus ephemeral) of inactivity.
/// </summary>
internal sealed class EphemeralMemoryStoreHandler : MemoryStoreHandlerBase
{
    private readonly ILogger logger;

    private EphemeralMemoryStoreHandlerOptions options;

    /// <summary>
    /// Initializes a new instance of the <see cref="EphemeralMemoryStoreHandler"/> class.
    /// </summary>
    /// <param name="memoryManager">A valid instance of <see cref="IMemoryManager"/> that manages the memory store handled by this instance.</param>
    /// <param name="sessionManagementOptions">Configuration options for this memory store handler.</param>
    /// <param name="logger">A logger for this memory store handler.</param>
    public EphemeralMemoryStoreHandler(IMemoryManager memoryManager, IOptionsMonitor<EphemeralMemoryStoreHandlerOptions> sessionManagementOptions, ILogger<EphemeralMemoryStoreHandler> logger)
        : base(memoryManager)
    {
        this.logger = logger;

        this.options = sessionManagementOptions.CurrentValue;

        sessionManagementOptions.OnChange(newOptions => this.options = newOptions);

        Task.Run(() => RemoveOutdatedCollectionsAsync());
    }

    /// <inheritdoc/>
    public override string CollectionNamePrefix => @"ephemeral-";

    private async Task RemoveOutdatedCollectionsAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            try
            {
                var date = DateTime.UtcNow.AddMinutes(-1 * options.IdleTimeoutMinutes);

                foreach (var memoryStoreInfo in MemoryStoreCollectionInfo.Where(i => i.Value.LastAccessUtc < date).ToList())
                {
                    MemoryStoreCollectionInfo.Remove(memoryStoreInfo.Key);
                    await MemoryManager.MemoryStore.DeleteCollectionAsync(memoryStoreInfo.Value.CollectionName, cancellationToken);
                }

                Thread.Sleep(TimeSpan.FromMinutes(options.InactivePollingTimeMinutes));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $@"Error removing outdated collections from memory store!");
            }
        }
    }
}
