using UnityEngine;

public class EnableOnGameState : MonoBehaviour {
    [SerializeField] GameState wantedState;
    public bool immediate = false;

    public bool setDisabled = false;

    void Start() {
        CheckState();
    }

    public void CheckState() {
        bool hasState = GlobalController.HasState(wantedState);
        if (setDisabled) gameObject.SetActive(!hasState);
        else gameObject.SetActive(hasState);
    }
}