using SiraUtil.Logging;
using System;
using Zenject;
using static BeatSaber.Settings.QualitySettings;

namespace QuickMirrorToggle
{
    internal class MirrorManager : IInitializable, IDisposable
    {
        private readonly SettingsManager _settingsManager;
        private readonly SettingsApplicatorSO _settingsApplicator;
        private readonly GameScenesManager _gameScenesManager;
        private readonly QMTConfig _config;
        private readonly SiraLog _logger;

        public MirrorManager(SettingsManager settingsManager, SettingsApplicatorSO settingsApplicator,
            GameScenesManager gameScenesManager, QMTConfig config, SiraLog logger)
        {
            _settingsManager = settingsManager;
            _settingsApplicator = settingsApplicator;
            _gameScenesManager = gameScenesManager;
            _config = config;
            _logger = logger;
        }

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
            _settingsManager.settings = settings;
            _settingsApplicator.ApplyGraphicSettings(settings, SceneType.Menu);
            _settingsApplicator.ApplyGraphicSettings(settings, SceneType.Game);
        }

        public void Dispose()
        {
            _config.OnChanged -= Config_OnChanged;
            _gameScenesManager.transitionDidFinishEvent -= GameScenesManager_transitionDidFinishEvent;
        }
    }
}
