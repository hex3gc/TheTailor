using MegaCrit.Sts2.Core.Localization;

namespace TheTailor.Extensions;

public static class CardSelectorPrefsExtensions
{
    public static LocString StitchSelectionPrompt => new("card_selection", "TO_STITCH");
    public static LocString StitchPatchSelectionPrompt => new("card_selection", "TO_STITCH_PATCH");
    public static LocString StitchSlipperSelectionPrompt => new("card_selection", "TO_STITCH_SLIPPER");
    public static LocString StitchCloneSelectionPrompt => new("card_selection", "TO_STITCH_CLONE");
    public static LocString ReplaceMinionSelectionPrompt => new("card_selection", "TO_REPLACE_MINION");
}