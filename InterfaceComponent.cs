using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    struct InterfaceComponent
    {
        public GameObject MainCanvas;
        public Slider StrengthBarSlider;
        public SlowMotionPanelMB SlowMotionPanelMB;
        public FloatingJoystick FloatingJoystick;
        public WinPanelMB WinPanelMB;
        public StorePanelMB StorePanelMB;
        public LosePanelMB LosePanelMB;
        public StagePanelMB StagePanelMB;
        public MainMenuMB MainMenuMB;

        public StrengthBarMB StrengthBarMB;
        public Transform TargetStrengthBar;

        public ComboSystemMB ComboSystemMB;
        public ResourcesMB ResourcesMB;
        public CanvasTMPro CanvasTMPro;
        public SettingsPanelMB SettingsPanelMB;
        public PlayPanelMB PlayPanelMB;

        public Transform PlayPanel;

        public EventSystem EventSystem;
        public GraphicRaycaster GraphicRaycaster;
    }
}