namespace Encamina.Enmarcha.AI.QuestionsAnswering.Abstractions;

/// <summary>
/// Represents a logical operation like &apos;OR&apos; or &apos;AND&apos;.
/// </summary>
[Obsolete("This type is obsolete and will be removed in future versions. Use 'Encamina.Enmarcha.Core.Abstractions.LogicalOperation' instead.")]
public enum LogicalOperation
{
    /// <summary>
    /// "AND" logical operation.
    /// </summary>
    And,

    /// <summary>
    /// "OR" logical operation.
    /// </summary>
    Or,
}
