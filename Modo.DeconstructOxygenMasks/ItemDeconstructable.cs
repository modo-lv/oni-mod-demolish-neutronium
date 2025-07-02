using KSerialization;
using Modo.Core.Extensions;
using PeterHan.PLib.Actions;
using STRINGS;
using static Modo.DeconstructOxygenMasks.Main;

namespace Modo.DeconstructOxygenMasks;

/// <summary>Implements the deconstruction functionality for Oxygen Masks.</summary>
/// <remarks>Based on game's built-in <see cref="Deconstructable"/> component.</remarks>
[SerializationConfig(MemberSerialization.OptIn)]
public class ItemDeconstructable : Workable {
  /// <summary>Active chore for deconstructing the OM.</summary>
  private Chore? _chore;
  /// <summary>A flag indicating and tracking whether a deconstruction is currently requested.</summary>
  [Serialize] private Boolean _markedForDecon;
  /// <summary>Stores the "real" unit-mass of the OM, set according to the recipe (normally 50 kg).</summary>
  [Serialize] public Single RealMassPerUnit;

  protected override void OnPrefabInit() {
    base.OnPrefabInit();
    this.Subscribe((Int32) GameHashes.RefreshUserMenu, _ => {
      Log.Debug($"Menu refreshed, (re)adding the Deconstruct button on {this.goInstance()}.");
      Game.Instance.userMenu?.AddButton(
        go: this.gameObject,
        button: new KIconButtonMenu.ButtonInfo(
          iconName: "action_power",
          text: this._markedForDecon
            ? UI.USERMENUACTIONS.DECONSTRUCT.NAME_OFF
            : UI.USERMENUACTIONS.DECONSTRUCT.NAME,
          on_click: this.OnDeconClick,
          shortcutKey: PAction.MaxAction,
          on_refresh: null,
          on_create: null,
          texture: null,
          tooltipText: this._markedForDecon
            ? UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP_OFF
            : UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP
        )
      );
    });
  }

  protected override void OnCleanUp() {
    this.Unsubscribe((Int32) GameHashes.RefreshUserMenu);
    base.OnCleanUp();
  }

  protected override void OnSpawn() {
    base.OnSpawn();
    if (this._markedForDecon)
      this.CreateChore();
    this.overrideAnims = [Assets.GetAnim("anim_interacts_craftingstation_kanim")];
    this.workAnims = ["working_loop"];
    this.synchronizeAnims = false;
    // Half the speed of constructing
    this.SetWorkTime((Single) TUNING.EQUIPMENT.SUITS.OXYMASK_FABTIME / 2);
    // On game load, OMs get spawned with their default unit-mass, so override with recipe specifics. 
    if (this.RealMassPerUnit >= PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT) {
      this.PrimaryElement().MassPerUnit = this.RealMassPerUnit;
      Log.Debug(
        $"Restored real unit-mass {this.RealMassPerUnit} for {this.goInstance()}."
      );
    } else {
      Log.Debug(
        $"Real unit-mass for {this.goInstance()} not available, " +
        $"leaving it with game's default ({this.PrimaryElement().MassPerUnit})."
      );
    }
  }

  /// <summary>Handles "Deconstruct" button clicks.</summary>
  public void OnDeconClick() {
    if (this._markedForDecon) {
      this._markedForDecon = false;
      this._chore!.Cancel("Deconstruction canceled!");
      Prioritizable.RemoveRef(this.gameObject);
      this._chore = null;
      this.ShowProgressBar(false);
    } else if (DebugHandler.InstantBuildMode) {
      this.OnCompleteWork(null);
    } else {
      this._markedForDecon = true;
      this.CreateChore();
    }
  }

  /// <summary>Creates the chore to make duplicants go and deconstruct the OM.</summary>
  private void CreateChore() {
    if (this._chore != null) return;

    Prioritizable.AddRef(this.gameObject);
    this._chore = new WorkChore<ItemDeconstructable>(
      chore_type: Db.Get().ChoreTypes.Deconstruct,
      target: this,
      ignore_building_assignment: true
    );
  }

  // ReSharper disable once ParameterHidesMember
  /// <summary>Processes the actual deconstruction.</summary>
  protected override void OnCompleteWork(WorkerBase? worker) {
    this._markedForDecon = false;
    this._chore = null;
    Prioritizable.RemoveRef(this.gameObject);
    // Drop the oxygen from non-worn OMs if present
    this.GetComponent<Storage>()?.DropAll();
    // Return the primary element of the OM to the world  
    GameUtil.KInstantiate(
      original: Assets.GetPrefab(this.PrimaryElement().Element.tag),
      sceneLayer: Grid.SceneLayer.Ore
    ).Also(recovered => {
      recovered.PrimaryElement().SetMassTemperature(
        mass: this.PrimaryElement().Mass,
        temperature: this.PrimaryElement().Temperature
      );
      recovered.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore));
      recovered.SetActive(true);
    });
    // All done, erase the OM. 
    this.DeleteObject();
  }
}