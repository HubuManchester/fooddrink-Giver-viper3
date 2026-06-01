namespace FoodAndDrink.Services
{
    public class FontScaleService
    {
        private const string PreferenceKey = "FontScale";
        private const double DefaultScale = 1.0;
        private const double MinScale = 0.8;
        private const double MaxScale = 1.4;

        // Base font sizes (before scaling)
        private static readonly Dictionary<string, double> BaseSizes = new()
        {
            ["HeroFontSize"] = 32,
            ["TitleFontSize"] = 24,
            ["SectionFontSize"] = 20,
            ["LargeFontSize"] = 16,
            ["BodyFontSize"] = 15,
            ["MediumFontSize"] = 14,
            ["SmallFontSize"] = 12,
        };

        private double _scale = DefaultScale;

        public double Scale
        {
            get => _scale;
            private set
            {
                if (value < MinScale || value > MaxScale)
                    return;
                if (Math.Abs(_scale - value) < 0.001)
                    return;

                _scale = value;
                Preferences.Set(PreferenceKey, _scale);
                UpdateGlobalResources();
                ScaleChanged?.Invoke(this, _scale);
            }
        }

        public double Min => MinScale;
        public double Max => MaxScale;

        public event EventHandler<double>? ScaleChanged;

        public FontScaleService()
        {
            _scale = Preferences.Get(PreferenceKey, DefaultScale);
            UpdateGlobalResources();
        }

        public static void InitResources()
        {
            var saved = Preferences.Get(PreferenceKey, DefaultScale);
            SetResources(saved);
        }

        private void UpdateGlobalResources()
        {
            SetResources(_scale);
        }

        private static void SetResources(double scale)
        {
            if (Application.Current == null)
                return;

            foreach (var kv in BaseSizes)
            {
                Application.Current.Resources[kv.Key] = Math.Round(kv.Value * scale, 1);
            }
        }

        public void SetScale(double scale)
        {
            Scale = Math.Clamp(scale, MinScale, MaxScale);
        }

        public double GetFontSize(double baseSize)
        {
            return Math.Round(baseSize * Scale, 1);
        }

        public string GetScaleLabel()
        {
            if (Scale < 0.95)
                return "Small";
            if (Scale > 1.15)
                return "Large";
            return "Medium";
        }

        public double GetSliderValue()
        {
            return (Scale - MinScale) / (MaxScale - MinScale);
        }

        public void SetFromSlider(double sliderValue)
        {
            var clamped = Math.Clamp(sliderValue, 0.0, 1.0);
            SetScale(MinScale + clamped * (MaxScale - MinScale));
        }
    }
}
