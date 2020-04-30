using UnityEngine;
using System.Collections;

public class AcademyValue : MonoBehaviour
{
    // For global accessed player count
    public static int playerCount;

    // For global accessed minimum and maximum
    public static float maximumX = 0;
    public static float minimumX = 0;
    public static float maximumY = 0;
    public static float minimumY = 0;

    public static bool gameDone = false;

    public static int[] numberOfShoots;
    public static int[] numberOfHits;

    public static int[] numberOfKills;
    public static int[] numberOfDeath;
}
