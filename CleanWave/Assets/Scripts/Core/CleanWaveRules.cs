namespace CleanWave.Core
{
    public static class CleanWaveRules
    {
        public static BinType GetExpectedBin(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.Paper:
                case TrashType.Vinyl:
                case TrashType.Rope:
                    return BinType.General;
                case TrashType.Can:
                    return BinType.Can;
                case TrashType.Plastic:
                    return BinType.Plastic;
                case TrashType.Food:
                case TrashType.Net:
                case TrashType.Buoy:
                case TrashType.Oil:
                    return BinType.Special;
                default:
                    return BinType.General;
            }
        }

        public static int GetPollution(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.Paper:
                    return CleanWaveGameConstants.PaperPollution;
                case TrashType.Can:
                    return CleanWaveGameConstants.CanPollution;
                case TrashType.Plastic:
                    return CleanWaveGameConstants.PlasticPollution;
                case TrashType.Vinyl:
                    return CleanWaveGameConstants.VinylPollution;
                case TrashType.Food:
                    return CleanWaveGameConstants.FoodPollution;
                case TrashType.Net:
                    return CleanWaveGameConstants.NetPollution;
                case TrashType.Rope:
                    return CleanWaveGameConstants.RopePollution;
                case TrashType.Buoy:
                    return CleanWaveGameConstants.BuoyPollution;
                case TrashType.Oil:
                    return CleanWaveGameConstants.OilPollution;
                default:
                    return 0;
            }
        }

        public static int GetCoinReward(TrashType trashType)
        {
            switch (trashType)
            {
                case TrashType.Paper:
                    return CleanWaveGameConstants.PaperCoinReward;
                case TrashType.Can:
                    return CleanWaveGameConstants.CanCoinReward;
                case TrashType.Plastic:
                    return CleanWaveGameConstants.PlasticCoinReward;
                case TrashType.Vinyl:
                    return CleanWaveGameConstants.VinylCoinReward;
                case TrashType.Food:
                    return CleanWaveGameConstants.FoodCoinReward;
                case TrashType.Net:
                    return CleanWaveGameConstants.NetCoinReward;
                case TrashType.Rope:
                    return CleanWaveGameConstants.RopeCoinReward;
                case TrashType.Buoy:
                    return CleanWaveGameConstants.BuoyCoinReward;
                case TrashType.Oil:
                    return CleanWaveGameConstants.OilCoinReward;
                default:
                    return 0;
            }
        }

        public static int GetTrashCount(ZoneId zoneId)
        {
            switch (zoneId)
            {
                case ZoneId.City:
                    return CleanWaveGameConstants.CityTrashCount;
                case ZoneId.River:
                    return CleanWaveGameConstants.RiverTrashCount;
                case ZoneId.Coast:
                    return CleanWaveGameConstants.CoastTrashCount;
                default:
                    return 0;
            }
        }
    }
}
