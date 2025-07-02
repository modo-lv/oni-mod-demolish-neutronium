using UnityEngine;

namespace Modo.Core.Extensions;

public static class KleiExtensions {
  public static PrimaryElement PrimaryElement(this KMonoBehaviour go) => go.Component<PrimaryElement>();
  public static PrimaryElement PrimaryElement(this GameObject go) => go.Component<PrimaryElement>();

  public static T Component<T>(this KMonoBehaviour component) where T : KMonoBehaviour =>
    component.gameObject.Component<T>();

  public static T Component<T>(this GameObject go) where T : KMonoBehaviour =>
    go.GetComponent<T>()
    ?? throw new NullReferenceException(
      $"Game object {go} does not have a [{nameof(T)}] component."
    );


  public static GameObject Prefab(this String tag) =>
    Assets.GetPrefab(tag)
    ?? throw new NullReferenceException(
      $"There is no [{tag}] prefab."
    );
}