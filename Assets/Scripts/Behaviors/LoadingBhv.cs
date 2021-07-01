using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBhv : MonoBehaviour
{
    private Instantiator _instantiator;

    void Start()
    {
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>().Instantiator;
        Invoke(nameof(DestroyAfterTimeOut), Constants.ServerCallTimeout);
    }

    private void DestroyAfterTimeOut()
    {
        _instantiator.NewPopupYesNo("Timeout", "looks like the server is taking too long to respond, please try again later.", null, "Damn...", null);
        Helper.ResumeLoading();
    }

    public void ForceDestroy()
    {
        Destroy(this.gameObject);
    }
}
