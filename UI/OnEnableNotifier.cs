namespace QList.UI;

using MelonLoader;
using UnityEngine;

[RegisterTypeInIl2Cpp]
public class OnEnableNotifier : MonoBehaviour
{
    public Action<OnEnableNotifier>? OnEnabled;
    public Action<OnEnableNotifier>? OnDisabled;

    private void OnEnable()
    {
        OnEnabled?.Invoke(this);
    }
    private void OnDisable()
    {
        OnDisabled?.Invoke(this);
    }
}