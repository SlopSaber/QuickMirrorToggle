using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.ViewControllers;
using SiraUtil.Logging;
using System.Linq;
using UnityEngine;
using Zenject;
using static BeatSaber.Settings.QualitySettings;

#pragma warning disable IDE0052 // Remove unread private members
namespace QuickMirrorToggle
{
    internal class QMTUI : BSMLAutomaticViewController, IInitializable
    {
        private const string QMT_TOGGLE = "" +
            "<bg id='qmt-root'>" +
            "<horizontal anchor-pos-x='-80' anchor-pos-y='65' preferred-width='40' horizontal-fit='PreferredSize' ignore-layout='true'>" +
            "<toggle-setting id='qmt-toggle' hover-hint='His name is Teddy ;3' text='~qmt-toggle-text' size-delta-x='1' size-delta-y='1' bind-value='true' apply-on-change='true' value='qmt-toggle-value'/>" +
            "</horizontal>" +
            "</bg>";

        [UIValue("qmt-mirror-setting-options")]
        private readonly string[] _mirrorSettings = new string[] { "Low", "Medium", "High" };

        [Inject] private QMTConfig _config { get; set; }
        [Inject] private SiraLog _logger { get; set; }
        private string _qmtText = "Disable Mirror";
        private bool _qmtToggleValue = false;

        [UIComponent("qmt-root")]
        private RectTransform _qmtRectTransform { get; set; }

        [UIValue("qmt-toggle-text")]
        public string QMTText
        {
            get => _qmtText;
            set
            {
                _qmtText = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("qmt-toggle-value")]
        public bool QMTToggleValue
        {
            get => _qmtToggleValue;
            set
            {
                _logger.Info("QMTToggleValue set to " + value);
                _qmtToggleValue = value;
                NotifyPropertyChanged();
                if (_config.GameMirrorSetting == MirrorQuality.Off)
                {
                    _config.MirrorState = value ? _config.QMTMirrorSetting : MirrorQuality.Off;
                }
                else
                {
                    _config.MirrorState = value ? MirrorQuality.Off : _config.GameMirrorSetting;
                }
                _config.Changed();
            }
        }

        [UIValue("qmt-mirror-setting-value")]
        public string SetValue
        {
            get => _config.QMTMirrorSetting.ToString();
            set
            {
                _config.QMTMirrorSetting = (MirrorQuality)System.Enum.Parse(typeof(MirrorQuality), value);
                if (_config.GameMirrorSetting == MirrorQuality.Off && _config.MirrorState != MirrorQuality.Off)
                {
                    _config.MirrorState = _config.QMTMirrorSetting;
                }
                _config.Changed();
            }
        }

        public void Initialize()
        {
            GameplaySetup.Instance.AddTab("Quick Mirror Toggle", "QuickMirrorToggle.SettingsUI.bsml", this);
            BSMLParser.Instance.Parse(QMT_TOGGLE, Resources.FindObjectsOfTypeAll<LevelSelectionNavigationController>().First().gameObject, this);
            _qmtRectTransform.localScale *= 0.6f;
            QMTText = _config.GameMirrorSetting == MirrorQuality.Off ? "Enable Mirror" : "Disable Mirror";
            _config.OnChanged += Config_OnChanged;
            QMTToggleValue = _config.GameMirrorSetting == MirrorQuality.Off ? (_config.MirrorState != MirrorQuality.Off) : (_config.MirrorState == MirrorQuality.Off);
        }

        private void Config_OnChanged(QMTConfig newConfig)
        {
            switch (newConfig.GameMirrorSetting)
            {
                case MirrorQuality.Off:
                    QMTText = "Enable Mirror";
                    break;
                case MirrorQuality.Low:
                case MirrorQuality.Medium:
                case MirrorQuality.High:
                    QMTText = "Disable Mirror";
                    break;
                default:
                    break;
            }
        }
    }
}
