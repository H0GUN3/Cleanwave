namespace CleanWave.Core
{
    public enum ZoneId
    {
        City,
        River,
        Coast
    }

    public enum TrashType
    {
        Paper,
        Can,
        Plastic,
        Vinyl,
        Food,
        Net,
        Rope,
        Buoy,
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

    public enum EquipmentType
    {
        Tongs,
        Bag,
        Shoes
    }

    public static class CleanWaveGameConstants
    {
        public const int ZoneWidthTiles = 32;
        public const int ZoneHeightTiles = 24;
        public const int PurificationPercentToUnlockNextZone = 90;
        public const int PurificationPercentToRevealEnvironmentScore = 100;

        public const int CityTrashCount = 30;
        public const int RiverTrashCount = 40;
        public const int CoastTrashCount = 50;

        public const int StartingGameScore = 100;
        public const int CorrectSortCoinReward = 10;
        public const int WrongSortScorePenalty = -1;

        public const int OneStarPurificationPercent = 70;
        public const int TwoStarPurificationPercent = 80;
        public const int ThreeStarPurificationPercent = 95;

        public const int TongsLevel1RangeTiles = 1;
        public const float TongsLevel2RangeTiles = 1.5f;
        public const int TongsLevel3RangeTiles = 2;

        public const int BagLevel1Capacity = 5;
        public const int BagLevel2Capacity = 8;
        public const int BagLevel3Capacity = 12;

        public const float ShoesLevel1SpeedMultiplier = 1f;
        public const float ShoesLevel2SpeedMultiplier = 1.15f;
        public const float ShoesLevel3SpeedMultiplier = 1.3f;

        public const int Level2UpgradeCostCoins = 50;
        public const int Level3UpgradeCostCoins = 100;

        public const int PaperPollution = 1;
        public const int CanPollution = 2;
        public const int PlasticPollution = 3;
        public const int VinylPollution = 3;
        public const int FoodPollution = 3;
        public const int NetPollution = 5;
        public const int RopePollution = 3;
        public const int BuoyPollution = 5;
        public const int OilPollution = 5;

        public const int PaperCoinReward = 1;
        public const int CanCoinReward = 2;
        public const int PlasticCoinReward = 3;
        public const int VinylCoinReward = 3;
        public const int FoodCoinReward = 3;
        public const int NetCoinReward = 5;
        public const int RopeCoinReward = 3;
        public const int BuoyCoinReward = 5;
        public const int OilCoinReward = 5;
    }
}
