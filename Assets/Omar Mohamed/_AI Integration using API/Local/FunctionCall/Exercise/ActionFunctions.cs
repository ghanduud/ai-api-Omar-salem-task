using UnityEngine;

public class ActionFunctions
{
    public static string MoveRight()
    {
        return "MoveRight";
    }

    public static string MoveLeft()
    {
        return "MoveLeft";
    }

    public static string Shoot()
    {
        return "Shoot";
    }

    public static string ShieldOn()
    {
        return "ShieldOn";
    }

    public static string ShieldOff()
    {
        return "ShieldOff";
    }

    public static string Heal()
    {
        return "Heal";
    }

    public static string NoAction()
    {
        return "NoAction"; // Default if no valid action is found
    }
}
