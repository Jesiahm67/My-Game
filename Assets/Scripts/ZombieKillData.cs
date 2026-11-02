using UnityEngine;

public class ZombieKillData
{
    
}
public static class GameData
{
    public static int Level1Kills = 0;
    public static int Level2Kills = 0;
    public static int Level3Kills = 0;
    public static int SecretLevelKills = 0;
    internal static bool ReturningFromSecretLevel;

    public static void ResetData()
        {
        Level1Kills = 0;
        Level2Kills = 0;
        Level3Kills = 0;
        SecretLevelKills = 0;

    }

}
