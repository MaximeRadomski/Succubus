using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class Helper
{
    public static bool IsSpriteRendererVisible(this GameObject gameObject, GameObject mask = null)
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer.maskInteraction == SpriteMaskInteraction.VisibleInsideMask && mask != null)
        {
            var maskBox2D = mask.GetComponent<BoxCollider2D>();
            return spriteRenderer.isVisible
                && gameObject.transform.position.x >= mask.transform.position.x - (maskBox2D.size.x / 2) && gameObject.transform.position.x <= mask.transform.position.x + (maskBox2D.size.x / 2)
                && gameObject.transform.position.y >= mask.transform.position.y - (maskBox2D.size.y / 2) && gameObject.transform.position.y <= mask.transform.position.y + (maskBox2D.size.y / 2);
        }
        return spriteRenderer.isVisible;
    }

    public static bool IsInsideCamera(Camera camera, Vector3 position)
    {
        var halfHeight = camera.orthographicSize;
        var halfWidth = camera.orthographicSize * camera.aspect;
        return position.x >= camera.transform.position.x - halfWidth && position.x <= camera.transform.position.x + halfWidth
            && position.y >= camera.transform.position.y - halfHeight && position.y <= camera.transform.position.y + halfHeight;
    }

    public static Camera GetMainCamera()
    {
        return Camera.allCameras.FirstOrDefault(c => c.name.ToLower().Contains("main"));
    }

    public static Opponent UpgradeOpponentToUpperType(Opponent opponent, OpponentType opponentType)
    {
        var tmpOpponent = opponent.Clone();
        var typeDifference = opponentType.GetHashCode() - opponent.Type.GetHashCode();
        tmpOpponent.Cooldown -= (int)(opponent.Cooldown * (typeDifference * Constants.OpponentUpgradeReducedCooldown));
        tmpOpponent.HpMax += (int)(opponent.HpMax * (typeDifference * Constants.OpponentUpgradeAddedLife));
        tmpOpponent.Type = opponentType;
        return tmpOpponent;
    }

    public static Loot GetLootFromTypeAndId(LootType lootType, int id)
    {
        if (lootType == LootType.Character)
        {
            return CharactersData.Characters[id];
        }
        else if (lootType == LootType.Item)
        {
            return ItemsData.GetItemFromName(ItemsData.Items[id]);
        }
        else if (lootType == LootType.Resource)
        {
            return ResourcesData.GetResourceFromName(ResourcesData.Resources[id]);
        }
        else if (lootType == LootType.Tattoo)
        {
            return TattoosData.GetTattooFromName(TattoosData.Tattoos[id]);
        }
        return null;
    }

    public static Vector3 TransformFromStepCoordinates(int x, int y)
    {
        return new Vector3((x - 50) * Constants.Pixel * 27, (y - 50) * Constants.Pixel * 25, 0.0f);
    }

    public static string ToHex(this Color color, bool withoutHash = false)
    {
        var tmp = withoutHash ? "" : "#";
        tmp += ((int)(color.r * 255.0f)).ToString("X");
        tmp += ((int)(color.g * 255.0f)).ToString("X");
        tmp += ((int)(color.b * 255.0f)).ToString("X");
        return tmp;
    }

    public static bool IsSuperiorByRealm(Realm subjectRealm, Realm targetRealm)
    {
        if (subjectRealm == Realm.Hell)
        {
            return targetRealm == Realm.Earth;
        }
        else if (subjectRealm == Realm.Earth)
        {
            return targetRealm == Realm.Heaven;
        }
        else if (subjectRealm == Realm.Heaven)
        {
            return targetRealm == Realm.Hell;
        }
        return false;
    }

    public static Realm GetSuperiorFrom(Realm realm)
    {
        if (realm == Realm.Hell)
            return Realm.Heaven;
        else if (realm == Realm.Earth)
            return Realm.Hell;
        else if (realm == Realm.Heaven)
            return Realm.Earth;
        return Realm.None;
    }

    public static Realm GetInferiorFrom(Realm realm)
    {
        if (realm == Realm.Hell)
            return Realm.Earth;
        else if (realm == Realm.Earth)
            return Realm.Heaven;
        else if (realm == Realm.Heaven)
            return Realm.Hell;
        return Realm.None;
    }

    public static int DoesListContainsSameFromName(List<GameObject> list, string name)
    {
        var count = 0;
        foreach (GameObject gm in list)
        {
            if (gm.name.Contains(name))
                ++count;
        }
        return count;
    }

    public static string ReplaceChar(this string str, int id, char character)
    {
        str = str.Remove(id, 1);
        return str = str.Insert(id, character.ToString());
    }
    public static char GameplayButtonToLetter(string gameplayButton)
    {
        if (gameplayButton == Constants.GoButtonLeftName)
            return 'L';
        else if (gameplayButton == Constants.GoButtonRightName)
            return 'R';
        else if (gameplayButton == Constants.GoButtonDownName)
            return 'd';
        else if (gameplayButton == Constants.GoButtonDropName)
            return 'D';
        else if (gameplayButton == Constants.GoButtonHoldName)
            return 'H';
        else if (gameplayButton == Constants.GoButtonAntiClockName)
            return 'A';
        else if (gameplayButton == Constants.GoButtonClockName)
            return 'C';
        else if (gameplayButton == Constants.GoButtonItemName)
            return 'I';
        else if (gameplayButton == Constants.GoButtonSpecialName)
            return 'S';
        else if (gameplayButton == Constants.GoButton180)
            return '1';
        return '0';
    }

    public static string LetterToGameplayButton(char letter)
    {
        if (letter == 'L')
            return Constants.GoButtonLeftName;
        else if (letter == 'R')
            return Constants.GoButtonRightName;
        else if (letter == 'd')
            return Constants.GoButtonDownName;
        else if (letter == 'D')
            return Constants.GoButtonDropName;
        else if (letter == 'H')
            return Constants.GoButtonHoldName;
        else if (letter == 'A')
            return Constants.GoButtonAntiClockName;
        else if (letter == 'C')
            return Constants.GoButtonClockName;
        else if (letter == 'I')
            return Constants.GoButtonItemName;
        else if (letter == 'S')
            return Constants.GoButtonSpecialName;
        else if (letter == '1')
            return Constants.GoButton180;
        return null;
    }

    public static float GetAngleFromTwoPositions(Vector3 start, Vector3 end)
    {
        float x = Mathf.Abs(start.x - end.x);
        float y = Mathf.Abs(start.y - end.y);
        float angle = 0.0f;
        if (FloatEqualsPrecision(end.x, start.x, 0.01f) && end.y > start.y) //UP
            angle = 0.0f;
        else if (FloatEqualsPrecision(end.y, start.y, 0.01f) && end.x > start.x) //RIGHT
            angle = 90.0f;
        else if (FloatEqualsPrecision(end.x, start.x, 0.01f) && end.y < start.y) //DOWN
            angle = 180.0f;
        else if (FloatEqualsPrecision(end.y, start.y, 0.01f) && end.x < start.x) //LEFT
            angle = 270.0f;
        else //DIAGONALS
        {
            
            if (end.y > start.y && end.x > start.x) //UP-RIGHT
            {
                angle = Mathf.Atan(x / y) * Mathf.Rad2Deg;
                angle += 0.0f;
            }
            else if (end.y < start.y && end.x > start.x) //DOWN-RIGHT
            {
                angle = Mathf.Atan(y / x) * Mathf.Rad2Deg;
                angle += 90.0f;
            }
            else if (end.y < start.y && end.x < start.x) //DOWN-LEFT
            {
                angle = Mathf.Atan(x / y) * Mathf.Rad2Deg;
                angle += 180.0f;
            }
            else if (end.y > start.y && end.x < start.x) //UP-LEFT
            {
                angle = Mathf.Atan(y / x) * Mathf.Rad2Deg;
                angle += 270.0f;
            }
        }
        return -angle;
    }

    public static string GetOrdinal(int number)
    {
        string str = number.ToString();
        var suffix = "th";
        if (str.Length == 1)
        {
            if (number == 1)
                suffix = "st";
            else if (number == 2)
                suffix = "nd";
            if (number == 3)
                suffix = "rd";
        }
        else
        {
            var last = str.Substring(str.Length - 1);
            var secondLast = str.Substring(str.Length - 2, 1);
            if (secondLast != "1")
            {
                if (last == "1")
                    suffix = "st";
                else if (last == "2")
                    suffix = "nd";
                if (last == "3")
                    suffix = "rd";
            }
        }
        return number + suffix;
    }

    public static IEnumerator ExecuteAfterDelay(float delay, Func<object> func, bool lockInputWhile = true)
    {
        if (lockInputWhile)
            Constants.InputLocked = true;
        yield return new WaitForSeconds(delay);
        func.Invoke();
        if (lockInputWhile)
            Constants.InputLocked = false;
    }

    public static bool RandomDice100(int target)
    {
        return UnityEngine.Random.Range(0, 100) < target;
    }

    public static int RandomIntMultipleOf(int min, int max, int multiple)
    {
        var unRoundedMinutes = UnityEngine.Random.Range(min, max + multiple);
        return unRoundedMinutes - unRoundedMinutes % multiple;
    }

    public static int CharacterAfterString(string str, string subStr)
    {
        if (!str.Contains(subStr))
            return -1;
        return str.IndexOf(subStr) + subStr.Length;
    }

    public static Sprite GetSpriteFromSpriteSheet(string path)
    {
        var separatorId = path.IndexOf('_');
        var spriteSheetPath = path.Substring(0, separatorId);
        var spriteSheet = Resources.LoadAll<Sprite>(spriteSheetPath);
        var spriteId = int.Parse(path.Substring(separatorId + 1));
        if (spriteId >= spriteSheet.Length)
            return null;
        else
            return spriteSheet[spriteId];
    }
    public static IEnumerable<T> OrEmptyIfNull<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static int EnumCount<EnumType>()
    {
        return System.Enum.GetNames(typeof(EnumType)).Length;
    }

    public static string GetAttribute<T>(this Enum value) where T : CustomAttribute
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var attribute = value.ToString();
        var fieldInfo = value.GetType().GetRuntimeField(attribute);

        if (fieldInfo == null)
            return string.Empty;
        var attributes = (T[])fieldInfo.GetCustomAttributes(typeof(T), false);

        if (attributes.Length > 0)
        {
            attribute = ((CustomAttribute)attributes[0]).Attribute;
        }

        if (attribute != null)
            return attribute;
        return string.Empty;
    }

    public static string GetDescription(this Enum value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var description = value.ToString();
        var fieldInfo = value.GetType().GetRuntimeField(description);

        if (fieldInfo == null)
            return string.Empty;
        var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributes.Length > 0)
        {
            description = attributes[0].Description;
        }

        return description;
    }

    public static float MultiplierFromPercent(float root, int percent)
    {
        return root + ((float)percent / 100.0f);
    }

    public static int RoundToNextDecade(int value)
    {
        return value + (value % 10 == 0 ? 0 : 10 - value % 10);
    }

    public static int RoundToClosestTable(int value, int table)
    {
        var superior = value % table == 0 ? 0 : table - value % table;
        var inferior = value - superior;
        if (superior == 0)
            return value;
        else if (superior < inferior)
            return value + superior;
        else
            return value - inferior;
    }

    public static bool FloatEqualsPrecision(float float1, float float2, float precision)
    {
        return float1 >= float2 - precision && float1 <= float2 + precision;
    }

    public static bool VectorEqualsPrecision(Vector3 vector1, Vector3 vector2, float precision)
    {
        return (vector1.x >= vector2.x - precision && vector1.x <= vector2.x + precision)
            && (vector1.y >= vector2.y - precision && vector1.y <= vector2.y + precision)
            && (vector1.z >= vector2.z - precision && vector1.z <= vector2.z + precision);
    }

    public static bool ColorEqualsPrecision(Color color1, Color color2, float precision)
    {
        return (color1.r >= color2.r - precision && color1.r <= color2.r + precision)
            && (color1.g >= color2.g - precision && color1.g <= color2.g + precision)
            && (color1.b >= color2.b - precision && color1.b <= color2.b + precision)
            && (color1.a >= color2.a - precision && color1.a <= color2.a + precision);
    }

    public static void ApplyDifficulty(List<Opponent> opponents, Difficulty difficulty)
    {
        var realmTree = PlayerPrefsHelper.GetRealmTree();
        foreach (var opponent in opponents)
        {
            opponent.Cooldown += (realmTree.CooldownBrake * 0.666f);
            if (difficulty == Difficulty.Easy)
            {
                opponent.Cooldown = (int)(opponent.Cooldown * 3.0f);
                opponent.HpMax = (int)(opponent.HpMax * 0.5f);
                opponent.GravityLevel -= 8;
            }
            else if (difficulty == Difficulty.Hard)
            {
                opponent.Cooldown = (int)(opponent.Cooldown * 0.666f);
                opponent.HpMax = (int)(opponent.HpMax * 1.5f);
                opponent.GravityLevel += 8;
            }
            else if (difficulty == Difficulty.Infernal)
            {
                opponent.Cooldown = (int)(opponent.Cooldown * 0.333f);
                opponent.HpMax = (int)(opponent.HpMax * 2.0f);
                opponent.GravityLevel += 16;
            }
            if (opponent.Cooldown < 1.0f)
                opponent.Cooldown = 1.0f;
            if (difficulty != Difficulty.Normal)
                opponent.HpMax = RoundToClosestTable(opponent.HpMax, 5);
            if (opponent.GravityLevel < 1)
                opponent.GravityLevel = 1;
            else if (opponent.GravityLevel > 25)
                opponent.GravityLevel = 25;
        }
    }

    public static void ReloadScene()
    {
        NavigationService.ReloadScene();
    }

    public static void ResumeLoading()
    {
        GameObject.Destroy(GameObject.Find(Constants.GoLoading));
    }

    public static string CalculateMD5Hash(string input)
    {
        // To calculate MD5 hash from an input string
        MD5 md5 = MD5.Create();
        byte[] inputBytes = Encoding.ASCII.GetBytes(input);

        byte[] hash = md5.ComputeHash(inputBytes);

        // convert byte array to hex string
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < hash.Length; i++)
        {
            //to make hex string use lower case instead of uppercase add parameter "X2"
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }
}
