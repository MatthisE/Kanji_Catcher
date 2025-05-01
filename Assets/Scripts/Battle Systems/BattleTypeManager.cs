[System.Serializable] // this class can be used anywhere

// define a battle scenario, BattleInstigator of a zone has a list of them
public class BattleTypeManager // should not be attached to an object, is just source of information (framework)
{
    public string[] enemies;
    public int rewardXP;
    public ItemsManager[] rewardItems;
}