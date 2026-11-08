using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves;
using MinionLib.Commands;
using MinionLib.Minion;
using MinionLib.Utilities;
using TheTailor.Cards.Token;
using TheTailor.Extensions;
using TheTailor.Powers;

namespace TheTailor.Minions;

public static class TailorMinionCmd
{
    public enum MinionTriggerType
    {
        First,
        All,
        Random
    }

    public static bool CanMinionBeAdded(Player owner)
    {
        return GetMinionCount<TailorMinion>(owner) < 3;
    }

    public static int GetMinionCount<T>(Player owner)
    {
        var ret = 0;
        if (owner.Creature.Pets.Count <= 0) return ret;
        foreach (var minion in owner.Creature.Pets)
            if (minion?.Monster is T)
                ret++;

        return ret;
    }

    /// <summary>
    ///     Adds a minion.
    /// </summary>
    public static async Task<bool> AddMinion<T>(PlayerChoiceContext playerChoiceContext, Player owner,
        int maxHpOverride = 0, int withTriggers = 0, bool toFront = false) where T : MinionModel
    {
        if (CanMinionBeAdded(owner))
        {
            var result = await MinionCmd.AddMinion<T>(playerChoiceContext, owner,
                new MinionSummonOptions(Position: MinionPosition.Front));
            if (maxHpOverride > 0 && result != null) await CreatureCmd.SetMaxAndCurrentHp(result, maxHpOverride);

            if (toFront)
            {
                var accessor = new PetsOrderAccessor(owner);
                if (accessor is { Pets: not null } && accessor.Pets.Contains(result) &&
                    accessor.Pets.Count > 1)
                {
                    accessor.Pets.Remove(result);
                    accessor.Pets.Insert(0, result);
                    _ = MinionAnimCmd.Rearrange(duration: 0.5f);
                    await CreatureCmd.TriggerAnim(result, "cast", 0f);
                    accessor.SetManualRearranged();
                    PetOrderSnapshotManager.TakeSnapshot(result.PetOwner);
                }
            }

            await PutOstyAtBack(playerChoiceContext, owner);

            if (withTriggers > 0 && result != null)
                for (var i = 0; i < withTriggers; i++)
                    await TriggerMinionAbility<TailorMinion>(playerChoiceContext, owner, MinionTriggerType.First,
                        result.Monster as TailorMinion);

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Adds a minion. If too many exist, prompts the player to select one for replacing. Returns true if the minion was
    ///     added
    /// </summary>
    public static async Task<bool> AddOrReplaceMinion<T>(PlayerChoiceContext playerChoiceContext, Player owner,
        bool canSkip, int maxHpOverride = 0, int withTriggers = 0) where T : TailorMinion
    {
        if (!CanMinionBeAdded(owner))
        {
            var replaceIndex = await SelectionPromptFromCurrentMinions<T>(playerChoiceContext, owner, canSkip);

            if (replaceIndex < 0) return false;

            if (await ReplaceMinion<T>(playerChoiceContext, owner, replaceIndex, maxHpOverride: maxHpOverride,
                    withTriggers: withTriggers)) return true;
        }
        else
        {
            var result = await MinionCmd.AddMinion<T>(playerChoiceContext, owner,
                new MinionSummonOptions(Position: MinionPosition.Front));
            if (maxHpOverride > 0 && result != null) await CreatureCmd.SetMaxAndCurrentHp(result, maxHpOverride);

            await PutOstyAtBack(playerChoiceContext, owner);

            if (withTriggers <= 0 || result == null) return true;
            for (var i = 0; i < withTriggers; i++)
                await TriggerMinionAbility<TailorMinion>(playerChoiceContext, owner, MinionTriggerType.First,
                    result.Monster as TailorMinion);

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Replaces a minion in their current position. Set convert to true to retain its max HP for the new minion
    /// </summary>
    public static async Task<bool> ReplaceMinion<T>(PlayerChoiceContext playerChoiceContext, Player owner,
        int replaceIndex, bool convert = false, bool first = false, int maxHpOverride = 0, int withTriggers = 0)
        where T : MinionModel
    {
        var accessor = new PetsOrderAccessor(owner);

        if (first)
            foreach (var creature in accessor.Pets)
                if (creature.Monster is TailorMinion)
                {
                    replaceIndex = accessor.Pets.IndexOf(creature);
                    break;
                }

        if (accessor is { Pets: not null } && accessor.Pets[replaceIndex] != null &&
            accessor.Pets[replaceIndex].Monster is TailorMinion)
        {
            var oldMinionMaxHp = accessor.Pets[replaceIndex].MaxHp;

            accessor.Pets[replaceIndex].RemoveAllPowersInternalExcept();

            await CreatureCmd.Kill(accessor.Pets[replaceIndex], true);
            var newMinion = await MinionCmd.AddMinion<T>(playerChoiceContext, owner,
                new MinionSummonOptions(Position: MinionPosition.Front));
            accessor.Pets.Remove(newMinion);
            accessor.Pets.Insert(replaceIndex, newMinion);
            _ = MinionAnimCmd.Rearrange(duration: 0.5f);
            accessor.SetManualRearranged();
            PetOrderSnapshotManager.TakeSnapshot(owner);

            await PutOstyAtBack(playerChoiceContext, owner);

            if (withTriggers > 0)
                for (var i = 0; i < withTriggers; i++)
                    await TriggerMinionAbility<TailorMinion>(playerChoiceContext, owner, MinionTriggerType.First,
                        newMinion.Monster as TailorMinion);

            if (convert)
            {
                await CreatureCmd.SetMaxHp(newMinion, oldMinionMaxHp);
                await CreatureCmd.SetCurrentHp(newMinion, oldMinionMaxHp);
            }

            if (maxHpOverride > 0 && newMinion != null) await CreatureCmd.SetMaxAndCurrentHp(newMinion, maxHpOverride);

            return true;
        }

        return false;
    }

    /// <summary>
    ///     Allows the player to select from cards representing their minions, and returns the index of the minion's index in
    ///     accessor.Pets
    /// </summary>
    public static async Task<int> SelectionPromptFromCurrentMinions<T>(PlayerChoiceContext playerChoiceContext,
        Player owner, bool canSkip, TailorMinion? minionToAdd = null) where T : TailorMinion
    {
        List<CardModel> choices = new();

        var accessor = new PetsOrderAccessor(owner);
        if (accessor is { Pets: not null })
            foreach (var minion in accessor.Pets)
            {
                CardModel? cm = null;
                switch (minion.Monster)
                {
                    case MinionLeather:
                        cm = ModelDb.Card<LeatherMinionToken>().ToMutable();
                        cm.Owner = owner;
                        cm.DynamicVars["ChoiceIndex"].BaseValue = accessor.Pets.IndexOf(minion);
                        cm.DynamicVars["Health"].BaseValue = minion.CurrentHp;
                        cm.DynamicVars["HealthPluralize"].BaseValue = 2;
                        choices.Add(cm);
                        break;
                    case MinionCotton:
                        cm = ModelDb.Card<CottonMinionToken>().ToMutable();
                        cm.Owner = owner;
                        cm.DynamicVars["ChoiceIndex"].BaseValue = accessor.Pets.IndexOf(minion);
                        cm.DynamicVars["Health"].BaseValue = minion.CurrentHp;
                        cm.DynamicVars["HealthPluralize"].BaseValue = 2;
                        choices.Add(cm);
                        break;
                    case MinionDenim:
                        cm = ModelDb.Card<DenimMinionToken>().ToMutable();
                        cm.Owner = owner;
                        cm.DynamicVars["ChoiceIndex"].BaseValue = accessor.Pets.IndexOf(minion);
                        cm.DynamicVars["Health"].BaseValue = minion.CurrentHp;
                        cm.DynamicVars["HealthPluralize"].BaseValue = 2;
                        choices.Add(cm);
                        break;
                    case MinionLinen:
                        cm = ModelDb.Card<LinenMinionToken>().ToMutable();
                        cm.Owner = owner;
                        cm.DynamicVars["ChoiceIndex"].BaseValue = accessor.Pets.IndexOf(minion);
                        cm.DynamicVars["Health"].BaseValue = minion.CurrentHp;
                        cm.DynamicVars["HealthPluralize"].BaseValue = 2;
                        choices.Add(cm);
                        break;
                    case MinionSilk:
                        cm = ModelDb.Card<SilkMinionToken>().ToMutable();
                        cm.Owner = owner;
                        cm.DynamicVars["ChoiceIndex"].BaseValue = accessor.Pets.IndexOf(minion);
                        cm.DynamicVars["Health"].BaseValue = minion.CurrentHp;
                        cm.DynamicVars["HealthPluralize"].BaseValue = 2;
                        choices.Add(cm);
                        break;
                    case MinionWool:
                        cm = ModelDb.Card<WoolMinionToken>().ToMutable();
                        cm.Owner = owner;
                        cm.DynamicVars["ChoiceIndex"].BaseValue = accessor.Pets.IndexOf(minion);
                        cm.DynamicVars["Health"].BaseValue = minion.CurrentHp;
                        cm.DynamicVars["HealthPluralize"].BaseValue = 2;
                        choices.Add(cm);
                        break;
                    case MinionBurlap:
                        cm = ModelDb.Card<BurlapMinionToken>().ToMutable();
                        cm.Owner = owner;
                        cm.DynamicVars["ChoiceIndex"].BaseValue = accessor.Pets.IndexOf(minion);
                        cm.DynamicVars["Health"].BaseValue = minion.CurrentHp;
                        cm.DynamicVars["HealthPluralize"].BaseValue = 2;
                        choices.Add(cm);
                        break;
                }
            }

        if (choices.Count <= 0) return -1;

        choices.Reverse();
        var cardModel = await CustomMinionChooseACardScreen<T>(playerChoiceContext, choices, owner, canSkip);

        if (cardModel == null) return -1;

        return cardModel.DynamicVars["ChoiceIndex"].IntValue;
    }

    /// <summary>
    ///     Allows the player to select from cards representing random minions, returning the token picked
    /// </summary>
    public static async Task<CardModel?> SelectionPromptFromRandomMinions(PlayerChoiceContext playerChoiceContext,
        Player owner, bool canSkip)
    {
        List<CardModel> choices = new()
        {
            ModelDb.Card<LeatherMinionToken>().ToMutable(),
            ModelDb.Card<CottonMinionToken>().ToMutable(),
            ModelDb.Card<DenimMinionToken>().ToMutable(),
            ModelDb.Card<LinenMinionToken>().ToMutable(),
            ModelDb.Card<SilkMinionToken>().ToMutable(),
            ModelDb.Card<WoolMinionToken>().ToMutable()
        };
        List<CardModel> randomChoices = new();

        choices = choices.StableShuffle(owner.PlayerRng.Transformations);

        for (var i = 0; i < 3; i++)
        {
            choices[i].Owner = owner;
            randomChoices.Add(choices[i]);
        }

        var cardModel = await CardSelectCmd.FromChooseACardScreen(playerChoiceContext, randomChoices, owner, canSkip);

        if (cardModel == null) return null;

        return cardModel;
    }

    public static async Task TriggerMinionAbility<T>(PlayerChoiceContext choiceContext, Player player,
        MinionTriggerType minionTriggerType, TailorMinion? specificMinion = null) where T : TailorMinion
    {
        var accessor = new PetsOrderAccessor(player);
        if (accessor is { Pets.Count: > 0 })
        {
            List<Creature> minionList = new(accessor.Pets);

            if (minionTriggerType == MinionTriggerType.Random)
                minionList.UnstableShuffle(player.PlayerRng.Transformations);

            foreach (var creature in minionList.Where(creature => specificMinion == null || creature.Monster == specificMinion))
            {
                if (creature.Monster is not T) continue;

                if (creature.Monster is MinionLinen)
                {
                    var enemy = player.RunState.Rng.CombatTargets.NextItem(player.Creature.CombatState.HittableEnemies);
                    await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, 2, player.Creature, null);
                    await CreatureCmd.TriggerAnim(creature, "cast", 0f);
                    await Cmd.Wait(0.2f);
                    if (minionTriggerType != MinionTriggerType.All) break;
                }
                else if (creature.Monster is MinionCotton)
                {
                    await CreatureCmd.Heal(player.Creature, creature.GetPowerAmount<CottonPower>());
                    await CreatureCmd.TriggerAnim(creature, "cast", 0f);
                    await Cmd.Wait(0.2f);
                    if (minionTriggerType != MinionTriggerType.All) break;
                }
                else if (creature.Monster is MinionDenim)
                {
                    await PowerCmd.Apply<StrengthPower>(choiceContext, player.Creature, 1, creature, null);
                    await CreatureCmd.TriggerAnim(creature, "cast", 0f);
                    await Cmd.Wait(0.2f);
                    if (minionTriggerType != MinionTriggerType.All) break;
                }
                else if (creature.Monster is MinionWool)
                {
                    await PowerCmd.Apply<WoolWeakPower>(choiceContext, player.Creature, 2m, creature, null);
                    await CreatureCmd.TriggerAnim(creature, "cast", 0f);
                    await Cmd.Wait(0.2f);
                    if (minionTriggerType != MinionTriggerType.All) break;
                }
                else if (creature.Monster is MinionSilk)
                {
                    await PowerCmd.Apply<SilkDexterityPower>(choiceContext, player.Creature, 2m, creature, null);
                    await CreatureCmd.TriggerAnim(creature, "cast", 0f);
                    await Cmd.Wait(0.2f);
                    if (minionTriggerType != MinionTriggerType.All) break;
                }
                else if (creature.Monster is MinionBurlap)
                {
                    await PowerCmd.Apply<BurlapStrengthPower>(choiceContext, player.Creature, 2m, creature, null);
                    await CreatureCmd.TriggerAnim(creature, "cast", 0f);
                    await Cmd.Wait(0.2f);
                    if (minionTriggerType != MinionTriggerType.All) break;
                }
            }
        }
    }

    public static async Task GiveMinionHealth<T>(PlayerChoiceContext choiceContext, Player player, int amount,
        MinionTriggerType minionTriggerType)
    {
        var accessor = new PetsOrderAccessor(player);
        if (accessor != null && accessor.Pets != null && accessor.Pets.Count > 0)
        {
            List<Creature> minionList = new(accessor.Pets);

            if (minionTriggerType == MinionTriggerType.Random)
                minionList.UnstableShuffle(player.PlayerRng.Transformations);

            foreach (var creature in minionList)
                if (creature.Monster is T)
                {
                    await CreatureCmd.GainMaxHp(creature, amount);
                    if (minionTriggerType != MinionTriggerType.All) break;
                }
        }
    }

    /// <summary>
    ///     Osty gets hit last to protect his max hp stacking
    /// </summary>
    public static async Task PutOstyAtBack(PlayerChoiceContext choiceContext, Player player)
    {
        var accessor = new PetsOrderAccessor(player);
        if (accessor != null && accessor.Pets != null && accessor.Pets.Count > 0)
        {
            var ostyPos = accessor.Pets.FirstIndex(pet => pet.Monster != null && pet.Monster is Osty);

            if (ostyPos >= 0)
            {
                var osty = accessor.Pets[ostyPos];
                accessor.Pets.Remove(osty);
                accessor.Pets.Add(osty);
                accessor.SetManualRearranged();
                PetOrderSnapshotManager.TakeSnapshot(player);
            }
        }
    }

    public static async Task<CardModel?> CustomMinionChooseACardScreen<T>(PlayerChoiceContext context,
        IReadOnlyList<CardModel> cards, Player player, bool canSkip = false) where T : TailorMinion
    {
        if (cards.Count > 3) throw new ArgumentException("Only works with less than 3 cards", "cards");
        if (cards.Count == 0)
        {
            CardSelectCmd.ReportSoftlock();
            return null;
        }

        CardModel? result;
        if (CardSelectCmd.Selector != null)
        {
            result = (await CardSelectCmd.Selector.GetSelectedCards(cards, 0, 1)).FirstOrDefault();
        }
        else
        {
            var choiceId = RunManager.Instance.PlayerChoiceSynchronizer.ReserveChoiceId(player);
            await context.SignalPlayerChoiceBegun(player, PlayerChoiceOptions.None);
            if (CardSelectCmd.ShouldSelectLocalCard(player))
            {
                if (CardSelectCmd.LocalSelector != null)
                {
                    result = (await CardSelectCmd.LocalSelector.GetSelectedCards(cards, 0, 1)).FirstOrDefault();
                }
                else
                {
                    NPlayerHand.Instance?.CancelAllCardPlay();
                    var nChooseACardSelectionScreen = ShowScreenWithMinionName<T>(cards, canSkip);
                    if (LocalContext.IsMe(player))
                        foreach (var card in cards)
                            SaveManager.Instance.MarkCardAsSeen(card);

                    result = (await nChooseACardSelectionScreen.CardsSelected()).FirstOrDefault();
                    var value = cards.IndexOf(result);
                    var result2 = PlayerChoiceResult.FromIndex(value);
                    RunManager.Instance.PlayerChoiceSynchronizer.SyncLocalChoice(player, choiceId, result2);
                }
            }
            else
            {
                var num = (await RunManager.Instance.PlayerChoiceSynchronizer.WaitForRemoteChoice(player, choiceId))
                    .AsIndex();
                result = num < 0 ? null : cards[num];
            }

            await context.SignalPlayerChoiceEnded();
        }

        CardSelectCmd.LogChoice(player, [result]);
        return result;
    }

    public static NChooseACardSelectionScreen? ShowScreenWithMinionName<T>(IReadOnlyList<CardModel> cards, bool canSkip)
        where T : TailorMinion
    {
        var nChooseACardSelectionScreen = NChooseACardSelectionScreen.ShowScreen(cards, canSkip);
        if (nChooseACardSelectionScreen != null)
            nChooseACardSelectionScreen._banner.label.SetTextAutoSize(string.Format(
                CardSelectorPrefsExtensions.ReplaceMinionSelectionPrompt.GetRawText(), GetMinionNameFromType<T>()));
        // nChooseACardSelectionScreen._banner.label.Size = new Godot.Vector2(nChooseACardSelectionScreen._banner.label.Size.X * 1.4f, nChooseACardSelectionScreen._banner.label.Size.Y * 1.4f);
        return nChooseACardSelectionScreen;
    }

    public static string GetMinionNameFromType<T>() where T : TailorMinion
    {
        var ret = "";


        if (typeof(T) == typeof(MinionLeather))
            ret = new LocString("monsters", "THETAILOR-MINION_LEATHER.name").GetRawText();
        else if (typeof(T) == typeof(MinionLinen))
            ret = new LocString("monsters", "THETAILOR-MINION_LINEN.name").GetRawText();
        else if (typeof(T) == typeof(MinionDenim))
            ret = new LocString("monsters", "THETAILOR-MINION_DENIM.name").GetRawText();
        else if (typeof(T) == typeof(MinionWool))
            ret = new LocString("monsters", "THETAILOR-MINION_WOOL.name").GetRawText();
        else if (typeof(T) == typeof(MinionSilk))
            ret = new LocString("monsters", "THETAILOR-MINION_SILK.name").GetRawText();
        else if (typeof(T) == typeof(MinionCotton))
            ret = new LocString("monsters", "THETAILOR-MINION_COTTON.name").GetRawText();
        else if (typeof(T) == typeof(MinionBurlap))
            ret = new LocString("monsters", "THETAILOR-MINION_BURLAP.name").GetRawText();

        return ret;
    }
}