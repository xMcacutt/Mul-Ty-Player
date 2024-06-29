using System;

namespace MulTyPlayerClient.Classes.Utility;

public class BloomSettings
{
    private bool _state;
    private float _redThreshold;
    private float _greenThreshold;
    private float _blueThreshold;
    private float _hue;
    private float _saturation;
    private float _value;
    public bool State
    {
        get => _state;
        set
        {
            _state = value;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x2895C5, BitConverter.GetBytes(value));
        }
    }
    public float RedThreshold
    {
        get => _redThreshold;
        set
        {
            _redThreshold = value;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x288F6C, BitConverter.GetBytes(value));
        }
    }
    public float GreenThreshold
    {
        get => _greenThreshold;
        set
        {
            _greenThreshold = value;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x288F70, BitConverter.GetBytes(value));
        }
    }
    public float BlueThreshold
    {
        get => _blueThreshold;
        set
        {
            _blueThreshold = value;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x288F74, BitConverter.GetBytes(value));
        }
    }
    public float Hue
    {
        get => _hue;
        set
        {
            _hue = value;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x288F7C, BitConverter.GetBytes(value));
        }
    }
    public float Saturation
    {
        get => _saturation;
        set
        {
            _saturation = value;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x288F80, BitConverter.GetBytes(value));
        }
    }
    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            ProcessHandler.WriteData((int)TyProcess.BaseAddress + 0x288F84, BitConverter.GetBytes(value));
        }
    }

    private BloomSettings _originalBloomSettings;

    private BloomSettings() { }

    public static BloomSettings Create()
    {
        var settings = new BloomSettings();
        settings.Initialize();
        settings._originalBloomSettings = settings.DeepCopy();
        return settings;
    }

    private void Initialize()
    {
        if (Client.HGameState.IsOnMainMenuOrLoading)
            return;
        ProcessHandler.TryRead(0x2895C5, out _state, true, "BloomState");
        ProcessHandler.TryRead(0x288F6C, out _redThreshold, true, "BloomRed");
        ProcessHandler.TryRead(0x288F70, out _greenThreshold, true, "BloomGreen");
        ProcessHandler.TryRead(0x288F74, out _blueThreshold, true, "BloomBlue");
        ProcessHandler.TryRead(0x288F7C, out _hue, true, "BloomHue");
        ProcessHandler.TryRead(0x288F80, out _saturation, true, "BloomSat");
        ProcessHandler.TryRead(0x288F84, out _value, true, "BloomVal");
    }

    private BloomSettings DeepCopy()
    {
        return new BloomSettings
        {
            _state = this._state,
            _redThreshold = this._redThreshold,
            _greenThreshold = this._greenThreshold,
            _blueThreshold = this._blueThreshold,
            _hue = this._hue,
            _saturation = this._saturation,
            _value = this._value
        };
    }

    public void RevertToOriginal()
    {
        State = _originalBloomSettings.State;
        RedThreshold = _originalBloomSettings.RedThreshold;
        GreenThreshold = _originalBloomSettings.GreenThreshold;
        BlueThreshold = _originalBloomSettings.BlueThreshold;
        Hue = _originalBloomSettings.Hue;
        Saturation = _originalBloomSettings.Saturation;
        Value = _originalBloomSettings.Value;
    }
}