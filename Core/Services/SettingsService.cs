using DemolishNeutronium;
using Modo.Core.Extensions;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace Modo.Core.Services;

/// <summary>Functionality for working with mod's configuration.</summary>
public class SettingsService<TSettings> where TSettings : class, new() {
  private readonly LogService _logger;

  public Lazy<TSettings> Cached = null!;

  public SettingsService(LogService logger) {
    this._logger = logger;
    this.ClearCache();
  }

  public void ClearCache() => this.Cached = new Lazy<TSettings>();

  /// <summary>Load mod settings, creating if necessary.</summary>
  public TSettings Load() =>
    POptions.ReadSettings<TSettings>()
      .Also(it => this._logger.Info($"Settings loaded: {JsonConvert.SerializeObject(it)}"))
      .OrIfNull(() =>
        new TSettings().Also(it => {
          POptions.WriteSettings(it);
          this._logger.Info(
            $"Existing settings not found, using defaults: {JsonConvert.SerializeObject(it)}"
          );
        })
      );
}