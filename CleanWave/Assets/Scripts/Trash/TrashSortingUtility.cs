using CleanWave.Core;

public static class TrashSortingUtility
{
    public static BinType InferBinType(string objectName, BinType fallback)
    {
        if (Contains(objectName, "Plastic"))
            return BinType.Plastic;
        if (Contains(objectName, "CanMetal") || Contains(objectName, "Can"))
            return BinType.Can;
        if (Contains(objectName, "Special"))
            return BinType.Special;
        if (Contains(objectName, "Paper"))
            return BinType.Paper;
        if (Contains(objectName, "General"))
            return BinType.General;

        return fallback;
    }

    public static TrashType InferTrashType(string objectName, CleanWaveZoneType zoneType, TrashType fallback)
    {
        if (IsUrbanVinylTrash(objectName))
            return TrashType.Vinyl;

        if (TryGetVariantIndex(objectName, out int variantIndex))
        {
            if (zoneType == CleanWaveZoneType.City && variantIndex == 1)
                return TrashType.Vinyl;

            if (zoneType == CleanWaveZoneType.River && variantIndex == 0)
                return TrashType.Plastic;

            if (zoneType == CleanWaveZoneType.Coast)
                return GetCoastTrashByVariant(variantIndex, fallback);

            switch (variantIndex)
            {
                case 0:
                    return TrashType.Paper;
                case 1:
                    return TrashType.Plastic;
                case 2:
                    return TrashType.Can;
                case 3:
                    return GetZoneSpecialTrash(zoneType);
            }
        }

        return fallback;
    }

    public static CleanWaveZoneType InferTrashFamilyZone(string objectName, CleanWaveZoneType fallback)
    {
        if (Contains(objectName, "City_Trash") || Contains(objectName, "Urban_Trash") || Contains(objectName, "Urban"))
            return CleanWaveZoneType.City;
        if (Contains(objectName, "River_Trash") || Contains(objectName, "River"))
            return CleanWaveZoneType.River;
        if (Contains(objectName, "Coast_Trash") || Contains(objectName, "Coast"))
            return CleanWaveZoneType.Coast;

        return fallback;
    }

    public static string GetKoreanTrashName(TrashType trashType)
    {
        switch (trashType)
        {
            case TrashType.Paper:
                return "종이";
            case TrashType.Can:
                return "캔/금속";
            case TrashType.Plastic:
                return "플라스틱";
            case TrashType.Vinyl:
                return "비닐";
            case TrashType.Food:
                return "음식물";
            case TrashType.Net:
                return "어망";
            case TrashType.Rope:
                return "밧줄";
            case TrashType.Buoy:
                return "부표";
            case TrashType.Oil:
                return "기름통";
            default:
                return "쓰레기";
        }
    }

    public static string GetKoreanBinName(BinType binType)
    {
        switch (binType)
        {
            case BinType.Paper:
                return "종이 수거함";
            case BinType.Can:
                return "캔/금속 수거함";
            case BinType.Plastic:
                return "플라스틱 수거함";
            case BinType.Special:
                return "특수 수거함";
            default:
                return "일반 수거함";
        }
    }

    static TrashType GetZoneSpecialTrash(CleanWaveZoneType zoneType)
    {
        switch (zoneType)
        {
            case CleanWaveZoneType.City:
                return TrashType.Food;
            case CleanWaveZoneType.River:
                return TrashType.Vinyl;
            case CleanWaveZoneType.Coast:
                return TrashType.Net;
            default:
                return TrashType.Food;
        }
    }

    static TrashType GetCoastTrashByVariant(int variantIndex, TrashType fallback)
    {
        switch (variantIndex)
        {
            case 0:
                return TrashType.Net;
            case 1:
                return TrashType.Rope;
            case 2:
                return TrashType.Buoy;
            case 3:
                return TrashType.Oil;
            default:
                return fallback;
        }
    }

    static bool TryGetVariantIndex(string objectName, out int variantIndex)
    {
        variantIndex = -1;
        if (string.IsNullOrEmpty(objectName))
            return false;

        int lastUnderscore = objectName.LastIndexOf('_');
        if (lastUnderscore < 0 || lastUnderscore >= objectName.Length - 1)
            return false;

        int digitStart = lastUnderscore + 1;
        int digitEnd = digitStart;
        while (digitEnd < objectName.Length && char.IsDigit(objectName[digitEnd]))
            digitEnd++;

        if (digitEnd == digitStart)
            return false;

        return int.TryParse(objectName.Substring(digitStart, digitEnd - digitStart), out variantIndex);
    }

    static bool IsUrbanVinylTrash(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return false;

        return Contains(objectName, "urban_01")
            || Contains(objectName, "urban 01")
            || Contains(objectName, "urban-01");
    }

    static bool Contains(string value, string token)
    {
        return value != null && value.IndexOf(token, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
