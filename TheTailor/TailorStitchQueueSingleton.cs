using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;

public class TailorStitchQueueSingleton() : CustomSingletonModel(HookType.Combat)
{
    public static readonly List<CardModel> BeingPlayed = [];
}