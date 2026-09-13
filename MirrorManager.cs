using SiraUtil.Logging;
using System;
using Zenject;
using static BeatSaber.Settings.QualitySettings;

namespace QuickMirrorToggle
{
    internal class MirrorManager : IInitializable, IDisposable
    {
        [Inject] private readonly SettingsManager _settingsManager;
        [Inject] private readonly SettingsApplicatorSO _settingsApplicator;
        [Inject] private readonly GameScenesManager _gameScenesManager;
        [Inject] private readonly IFileStorage _fileStorage;
        [Inject] private readonly QMTConfig _config;
        [Inject] private readonly SiraLog _logger;

        public void Initialize()
        {
            _logger.Info("Initializing MirrorManager");
            _config.GameMirrorSetting = _settingsManager.settings.quality.mirror;
            SetMirrorState(_config.MirrorState);

            _config.OnChanged += Config_OnChanged;
            _gameScenesManager.transitionDidFinishEvent += GameScenesManager_transitionDidFinishEvent;
        }

        private void GameScenesManager_transitionDidFinishEvent(GameScenesManager.SceneTransitionType arg1, ScenesTransitionSetupData sceneSetupData, DiContainer arg3)
        {
            SetMirrorState(_config.MirrorState);
        }

        private void Config_OnChanged(QMTConfig newConfig)
        {
            SetMirrorState(newConfig.MirrorState);
        }

        public void SetMirrorState(MirrorQuality state)
        {
            _logger.Info($"Setting mirror to {state}");
            var settings = _settingsManager.settings;
            settings.quality.mirror = state;
            //await SettingsIO.SaveAsync(_fileStorage, settings);
            _settingsManager.settings = settings;
            _settingsApplicator.ApplyGraphicSettings(settings, SceneType.Menu);
            _settingsApplicator.ApplyGraphicSettings(settings, SceneType.Game);
        }

        public void Dispose()
        {
            _config.OnChanged -= Config_OnChanged;
        }
    }
}
