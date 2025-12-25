using UnityEngine;

public interface ISaveableState
{
    string SaveDataFileName { get; }
    object SaveState();
    void LoadState(object data);
    void ResetState();
}
