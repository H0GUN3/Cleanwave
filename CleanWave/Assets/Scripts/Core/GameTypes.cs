namespace CleanWave
{
    public enum TrashType
    {
        Paper,
        Can,
        Plastic,
        Vinyl,
        Food,
        Net,
        Oil
    }

    public enum BinType
    {
        Paper,
        Can,
        Plastic,
        General,
        Special
    }

    public enum ZoneType
    {
        City,
        River,
        Beach
    }

    public enum UpgradeType
    {
        Tongs,
        Bag,
        Shoes
    }

    public enum GameState
    {
        Playing,
        Result
    }

    public static class TrashBinMapping
    {
        public static BinType GetCorrectBin(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.Paper:   return BinType.Paper;
                case TrashType.Can:     return BinType.Can;
                case TrashType.Plastic: return BinType.Plastic;
                case TrashType.Vinyl:   return BinType.General;
                case TrashType.Food:    return BinType.General;
                case TrashType.Net:     return BinType.Special;
                case TrashType.Oil:     return BinType.Special;
                default:                return BinType.General;
            }
        }

        public static float GetPollutionValue(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.Paper:   return 1f;
                case TrashType.Can:     return 1.5f;
                case TrashType.Plastic: return 2f;
                case TrashType.Vinyl:   return 2f;
                case TrashType.Food:    return 1f;
                case TrashType.Net:     return 4f;
                case TrashType.Oil:     return 5f;
                default:                return 1f;
            }
        }

        public static int GetCoinValue(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.Paper:   return 5;
                case TrashType.Can:     return 7;
                case TrashType.Plastic: return 7;
                case TrashType.Vinyl:   return 8;
                case TrashType.Food:    return 5;
                case TrashType.Net:     return 15;
                case TrashType.Oil:     return 20;
                default:                return 5;
            }
        }
    }
}
