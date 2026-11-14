using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace TheTailor;

public static class Keywords
{
    [CustomEnum("Stitch")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Stitch;

    [CustomEnum("Stitched")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Stitched;

    [CustomEnum("Delicate")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Delicate;

    [CustomEnum("Convert")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Convert;

    [CustomEnum("Premium")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Premium;

    [CustomEnum("LeatherMinion")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword LeatherMinion;

    [CustomEnum("LinenMinion")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword LinenMinion;

    [CustomEnum("SilkMinion")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword SilkMinion;

    [CustomEnum("WoolMinion")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword WoolMinion;

    [CustomEnum("DenimMinion")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword DenimMinion;

    [CustomEnum("CottonMinion")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword CottonMinion;

    [CustomEnum("BurlapMinion")] [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword BurlapMinion;
}