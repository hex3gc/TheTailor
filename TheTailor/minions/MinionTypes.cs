using MegaCrit.Sts2.Core.Models;

namespace TheTailor.Minions;

/// <summary>
///     Extra information handed to a minion when it is summoned.
/// </summary>
public readonly record struct MinionSummonOptions(CardModel? Source = null);