﻿
using StardewModdingAPI;

namespace CustomPictureFrames
{
    public class ModConfig
    {
        public bool EnableMod { get; set; } = true;

        public SButton SwitchFrameKey { get; set; } = SButton.F11;
        public SButton StartFramingKey { get; set; } = SButton.F10;
        public SButton TakePictureKey { get; set; } = SButton.F12;
        public string Message { get; set; } = "Saving framed picture for {0}";
    }
}
