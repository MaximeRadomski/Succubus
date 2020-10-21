using UnityEditor;
using UnityEngine;

public class Instantiator : MonoBehaviour
{
    void Start()
    {
        Init();
    }

    public void Init()
    {
    }

    public void NewAttackLine(Vector3 source, Vector3 target, Realm realm)
    {
        var attackLine = new GameObject("AttackLine");
        attackLine.AddComponent<AttackLineBhv>();
        attackLine.GetComponent<AttackLineBhv>().Init(source, target, realm, this);
    }

    public GameObject NewStepsContainer()
    {
        var tmpStepsContainerInstance = Instantiate(new GameObject(), new Vector3(0.0f, 0.0f, 0.0f), new Quaternion());
        tmpStepsContainerInstance.name = "StepsContainer";
        return tmpStepsContainerInstance;
    }

    public GameObject NewStepInstance(Step step)
    {
        var tmpStepObject = Resources.Load<GameObject>("Prefabs/Step");
        var tmpStepInstance = Instantiate(tmpStepObject, Helper.TransformFromStepCoordinates(step.X, step.Y), tmpStepObject.transform.rotation);
        tmpStepInstance.GetComponent<StepInstanceBhv>().UpdateVisual(step);
        tmpStepInstance.transform.name = step.X + "_" + step.Y;
        return tmpStepInstance;
    }

    public GameObject NewDrillTarget(Realm realm, Vector3 position)
    {
        var tmpDrillTargetObject = Resources.Load<GameObject>("Prefabs/DrillTarget");
        var tmpDrillTargetInstance = Instantiate(tmpDrillTargetObject, position, tmpDrillTargetObject.transform.rotation);
        tmpDrillTargetInstance.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromRealm(realm, 4);
        tmpDrillTargetInstance.name = Constants.GoDrillTarget;
        return tmpDrillTargetInstance;
    }

    public GameObject NewPiece(string pieceLetter, string realm, Vector3 spawnerPosition, bool keepSpawnerX = false)
    {
        var tmpPieceObject = Resources.Load<GameObject>("Prefabs/" + pieceLetter + "-" + realm);
        var pieceModel = tmpPieceObject.GetComponent<Piece>();
        GameObject tmpPieceInstance = null;
        if (pieceModel != null)
            tmpPieceInstance = Instantiate(tmpPieceObject, spawnerPosition + new Vector3(keepSpawnerX ? 0.0f : pieceModel.XFromSpawn, pieceModel.YFromSpawn, 0.0f), tmpPieceObject.transform.rotation); 
        else
            tmpPieceInstance = Instantiate(tmpPieceObject, spawnerPosition, tmpPieceObject.transform.rotation);
        return tmpPieceInstance;
    }

    public GameObject NewPieceBlock(Realm realm, Vector3 spawnPosition, Transform parent)
    {
        var tmpPieceBlockObject = Resources.Load<GameObject>("Prefabs/PieceBlock-" + realm);
        GameObject tmpPieceBlockInstance = Instantiate(tmpPieceBlockObject, spawnPosition, tmpPieceBlockObject.transform.rotation);
        tmpPieceBlockInstance.transform.SetParent(parent);
        return tmpPieceBlockInstance;
    }

    public void NewGravitySquare(GameObject parent, string realm)
    {
        var tmpSquareObject = Resources.Load<GameObject>("Prefabs/GravitySquare-" + realm);
        var tmpSquareInstance = Instantiate(tmpSquareObject, parent.transform.position, tmpSquareObject.transform.rotation);
        tmpSquareInstance.transform.SetParent(parent.transform);
    }

    public GameObject NewFadeBlock(Realm realm, Vector3 position, int startColor, int endColor)
    {
        var tmpBlockObject = Resources.Load<GameObject>("Prefabs/FadeBlock" + realm);
        var tmpPieceInstance = Instantiate(tmpBlockObject, position, tmpBlockObject.transform.rotation);
        tmpPieceInstance.GetComponent<FadeBlockBhv>().Init(startColor, endColor, realm);
        return tmpPieceInstance;
    }

    public void EditViaKeyboard()
    {
        var target = GameObject.Find(Constants.LastEndActionClickedName);
        if (target == null)
            return;
        ShowKeyboard(target.GetComponent<TMPro.TextMeshPro>(), target.GetComponent<BoxCollider2D>().size.x);
    }

    public void ShowKeyboard(TMPro.TextMeshPro target, float maxWidth = -1)
    {
        var tmpKeyboardObject = Resources.Load<GameObject>("Prefabs/Keyboard");
        var tmpKeyboardInstance = Instantiate(tmpKeyboardObject, tmpKeyboardObject.transform.position, tmpKeyboardObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpKeyboardInstance.name);
        for (int i = 0; i < tmpKeyboardInstance.transform.childCount; ++i)
        {
            var inputKeyBhv = tmpKeyboardInstance.transform.GetChild(i).GetComponent<InputKeyBhv>();
            if (inputKeyBhv != null)
                inputKeyBhv.SetPrivates(target, maxWidth);
        }
        tmpKeyboardInstance.transform.Find("InputKeyLayout" + PlayerPrefs.GetInt(Constants.PpFavKeyboardLayout, Constants.PpFavKeyboardLayoutDefault)).GetComponent<InputKeyBhv>().ChangeLayout();
        if (target.transform.position.y < -Camera.main.orthographicSize + Constants.KeyboardHeight)
            Camera.main.gameObject.GetComponent<CameraBhv>().FocusY(target.transform.position.y + (Camera.main.orthographicSize - Constants.KeyboardHeight));
    }

    public void NewPopupYesNo(string title, string content, string negative, string positive,
        System.Func<bool, object> resultAction, Sprite sprite = null)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupYesNo");
        var tmpPopupInstance = Instantiate(tmpPopupObject, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f), tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupYesNoBhv>().Init(title, content, negative, positive, resultAction, sprite);
    }

    public void NewPopupGameplayButtons(System.Func<bool, object> resultAction)
    {
        var tmpPopupObject = Resources.Load<GameObject>("Prefabs/PopupGameplayButtons");
        var tmpPopupInstance = Instantiate(tmpPopupObject, tmpPopupObject.transform.position, tmpPopupObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPopupInstance.name);
        tmpPopupInstance.GetComponent<PopupGameplayButtons>().Init(resultAction);
    }

    public GameObject NewGameplayButton(string name, Vector3 position)
    {
        var tmpButtonObject = Resources.Load<GameObject>("Prefabs/" + name);
        var tmpButtonInstance = Instantiate(tmpButtonObject, position, tmpButtonObject.transform.rotation);
        tmpButtonInstance.name = tmpButtonInstance.name.Replace("(Clone)", "");
        return tmpButtonInstance;
    }

    public void NewOverBlend(OverBlendType overBlendType, string message, float? constantLoadingSpeed,
        System.Func<bool, object> resultAction, bool reverse = false)
    {
        var tmpOverBlendObject = Resources.Load<GameObject>("Prefabs/OverBlend");
        var tmpOverBlendInstance = Instantiate(tmpOverBlendObject, tmpOverBlendObject.transform.position, tmpOverBlendObject.transform.rotation);
        tmpOverBlendInstance.GetComponent<OverBlendBhv>().SetPrivates(overBlendType, message, constantLoadingSpeed, resultAction, reverse);
    }

    public void NewSnack(string content, float duration = 2.0f)
    {
        var tmpSnackObject = Resources.Load<GameObject>("Prefabs/Snack");
        var tmpSnackInstance = Instantiate(tmpSnackObject, tmpSnackObject.transform.position, tmpSnackObject.transform.rotation);
        tmpSnackInstance.GetComponent<SnackBhv>().SetPrivates(content, duration);
    }

    public GameObject NewPauseMenu(System.Func<bool, object> resumeAction)
    {
        var tmpPauseMenuObject = Resources.Load<GameObject>("Prefabs/PauseMenu");
        var tmpPauseMeuInstance = Instantiate(tmpPauseMenuObject, tmpPauseMenuObject.transform.position, tmpPauseMenuObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpPauseMeuInstance.name);
        tmpPauseMeuInstance.GetComponent<PauseMenuBhv>().Init(this, resumeAction);
        return tmpPauseMeuInstance;
    }

    public GameObject NewInfoMenu(System.Func<bool, object> resumeAction, Character character, Opponent opponent)
    {
        var tmpInfoMenuObject = Resources.Load<GameObject>("Prefabs/InfoMenu");
        var tmpInfoMeuInstance = Instantiate(tmpInfoMenuObject, tmpInfoMenuObject.transform.position, tmpInfoMenuObject.transform.rotation);
        Constants.IncreaseInputLayer(tmpInfoMeuInstance.name);
        tmpInfoMeuInstance.GetComponent<InfoMenuBhv>().Init(this, resumeAction, character, opponent);
        return tmpInfoMeuInstance;
    }

    public GameObject PopText(string text, Vector2 position, string color = "#FFFFFF", float floatingTime = 0.0f)
    {
        var tmpPoppingTextObject = Resources.Load<GameObject>("Prefabs/PoppingText");
        var tmpPoppingTextInstance = Instantiate(tmpPoppingTextObject, position, tmpPoppingTextObject.transform.rotation);
        tmpPoppingTextInstance.GetComponent<PoppingTextBhv>().Init(text, position, color, floatingTime);
        return tmpPoppingTextInstance;
    }

    public GameObject NewLightRowText(Vector2 position)
    {
        var tmpTextObject = Resources.Load<GameObject>("Prefabs/LightRowText");
        var tmpTextInstance = Instantiate(tmpTextObject, position, tmpTextObject.transform.rotation);
        return tmpTextInstance;
    }

    public GameObject NewVisionBlock(Vector2 position, int nbRows, int nbSeconds, Realm opponentRealm)
    {
        var tmpVisionBlockObject = Resources.Load<GameObject>("Prefabs/VisionBlock");
        var tmpVisionBlockInstance = Instantiate(tmpVisionBlockObject, position, tmpVisionBlockObject.transform.rotation);
        tmpVisionBlockInstance.GetComponent<VisionBlockBhv>().Init(nbRows, nbSeconds, opponentRealm);
        return tmpVisionBlockInstance;
    }

    public void PopIcon(Sprite sprite, Vector2 position)
    {
        var tmpPoppingIconObject = Resources.Load<GameObject>("Prefabs/PoppingIcon");
        var tmpPoppingIconInstance = Instantiate(tmpPoppingIconObject, position, tmpPoppingIconObject.transform.rotation);
        tmpPoppingIconInstance.GetComponent<PoppingIconBhv>().SetPrivates(sprite, position + new Vector2(0.0f, +0.3f));
    }
}
