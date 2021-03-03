using System;
using UnityEngine;
using KSP.Localization;

namespace WingProcedural
{
    public static class UIUtility
    {
        public static Rect uiRectWindowEditor = new Rect(305, 95, 300, 67);

        public static GUIStyle uiStyleWindow = new GUIStyle();
        public static GUIStyle uiStyleLabelMedium = new GUIStyle();
        public static GUIStyle uiStyleLabelHint = new GUIStyle();
        public static GUIStyle uiStyleButton = new GUIStyle();
        public static GUIStyle uiStyleSlider = new GUIStyle();
        public static GUIStyle uiStyleSliderThumb = new GUIStyle();
        public static GUIStyle uiStyleToggle = new GUIStyle();
        public static GUIStyle uiStyleInputField = new GUIStyle();
        public static bool uiStyleConfigured = false;

        public static Font uiFont = null;
        private static readonly float alphaNormal = 0.5f;
        private static readonly float alphaHover = 0.35f;
        private static readonly float alphaActive = 0.75f;

        public static bool numericInput = false;
        public static bool angleChangd = false;

        public static void ConfigureStyles()
        {
            if (uiFont == null)
            {
                uiFont = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            }
            if (uiFont != null)
            {
                uiStyleWindow = new GUIStyle(HighLogic.Skin.window)
                {
                    fixedWidth = 300f,
                    wordWrap = true
                };
                uiStyleWindow.normal.textColor = Color.white;
                uiStyleWindow.font = uiFont;
                uiStyleWindow.fontStyle = FontStyle.Normal;
                uiStyleWindow.fontSize = 13;
                uiStyleWindow.alignment = TextAnchor.UpperLeft;

                uiStyleLabelMedium = new GUIStyle(HighLogic.Skin.label)
                {
                    stretchWidth = true,
                    font = uiFont,
                    fontStyle = FontStyle.Normal,
                    fontSize = 13
                };
                uiStyleLabelMedium.normal.textColor = Color.white;

                uiStyleLabelHint = new GUIStyle(HighLogic.Skin.label)
                {
                    stretchWidth = true,
                    font = uiFont,
                    fontStyle = FontStyle.Normal,
                    fontSize = 11
                };
                uiStyleLabelHint.normal.textColor = Color.white;

                uiStyleButton = new GUIStyle(HighLogic.Skin.button);
                AssignTexturesToStyle(uiStyleButton);
                uiStyleButton.padding = new RectOffset(0, 0, 0, 0);
                uiStyleButton.overflow = new RectOffset(0, 0, 0, 0);
                uiStyleButton.font = uiFont;
                uiStyleButton.fontStyle = FontStyle.Normal;
                uiStyleButton.fontSize = 11;
                uiStyleButton.fixedHeight = 16;

                uiStyleSlider = new GUIStyle(HighLogic.Skin.horizontalSlider);
                AssignTexturesToStyle(uiStyleSlider);
                uiStyleSlider.border = new RectOffset(0, 0, 0, 0);
                uiStyleSlider.margin = new RectOffset(4, 4, 4, 4);
                uiStyleSlider.padding = new RectOffset(0, 0, 0, 0);
                uiStyleSlider.overflow = new RectOffset(0, 0, 0, 0);
                uiStyleSlider.fixedHeight = 16;

                uiStyleSliderThumb = new GUIStyle(HighLogic.Skin.horizontalSliderThumb);
                AssignTexturesToStyle(uiStyleSlider);
                uiStyleSliderThumb.border = new RectOffset(0, 0, 0, 0);
                uiStyleSliderThumb.margin = new RectOffset(4, 4, 4, 4);
                uiStyleSliderThumb.padding = new RectOffset(0, 0, 0, 0);
                uiStyleSliderThumb.overflow = new RectOffset(0, 0, 0, 0);
                uiStyleSliderThumb.normal.background = Color.black.WithAlpha(0).GetTexture2D();
                uiStyleSliderThumb.hover.background = Color.black.WithAlpha(0).GetTexture2D();
                uiStyleSliderThumb.active.background = Color.black.WithAlpha(0).GetTexture2D();
                uiStyleSliderThumb.onNormal.background = Color.black.WithAlpha(0).GetTexture2D();
                uiStyleSliderThumb.onHover.background = Color.black.WithAlpha(0).GetTexture2D();
                uiStyleSliderThumb.onActive.background = Color.black.WithAlpha(0).GetTexture2D();
                uiStyleSliderThumb.fixedWidth = 0f;
                uiStyleSliderThumb.fixedHeight = 16;

                uiStyleToggle = new GUIStyle(HighLogic.Skin.toggle)
                {
                    font = uiFont,
                    fontStyle = FontStyle.Normal,
                    fontSize = 11
                };
                uiStyleToggle.normal.textColor = Color.white;
                uiStyleToggle.padding = new RectOffset(4, 4, 4, 4);
                uiStyleToggle.margin = new RectOffset(4, 4, 4, 4);

                uiStyleInputField = new GUIStyle(HighLogic.Skin.textField)
                {
                    stretchWidth = true,
                    font = uiFont,
                    fontStyle = FontStyle.Normal,
                    fontSize = 11
                };
                uiStyleInputField.normal.textColor = Color.white;

                uiStyleConfigured = true;
            }
        }

        private static void AssignTexturesToStyle(GUIStyle s)
        {
            s.normal.textColor = s.onNormal.textColor = Color.white;
            s.hover.textColor = s.onHover.textColor = Color.white;
            s.active.textColor = s.onActive.textColor = Color.white;

            s.normal.background = Color.black.WithAlpha(alphaNormal).GetTexture2D();
            s.hover.background = Color.black.WithAlpha(alphaHover).GetTexture2D();
            s.active.background = Color.black.WithAlpha(alphaActive).GetTexture2D();
            s.onNormal.background = Color.black.WithAlpha(alphaNormal).GetTexture2D();
            s.onHover.background = Color.black.WithAlpha(alphaHover).GetTexture2D();
            s.onActive.background = Color.black.WithAlpha(alphaActive).GetTexture2D();
            uiStyleButton.border = new RectOffset(0, 0, 0, 0);
        }

        public static float FieldSlider(float value, float increment, float range, string name, out bool changed, Color backgroundColor, int valueType, ref int delta, bool allowFine = true)
        {
            if (!UIUtility.uiStyleConfigured)
            {
                UIUtility.ConfigureStyles();
            }

            GUILayout.BeginHorizontal();
            int newDelta = (int)(value / range);
            if (newDelta != delta | newDelta != delta + 1)
                delta = newDelta;
            double value01 = (value - delta * range) / range;
            double increment01 = increment / range;
            double valueOld = value01;
            const float buttonWidth = 12, spaceWidth = 3;

            GUILayout.Label(string.Empty, UIUtility.uiStyleLabelHint);
            Rect rectLast = GUILayoutUtility.GetLastRect();
            Rect rectSlider = new Rect(rectLast.xMin + buttonWidth + spaceWidth, rectLast.yMin, rectLast.width - 2 * (buttonWidth + spaceWidth), rectLast.height);
            Rect rectSliderValue = new Rect(rectSlider.xMin, rectSlider.yMin, rectSlider.width * (float)value01, rectSlider.height - 3f);
            Rect rectButtonL = new Rect(rectLast.xMin, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectButtonR = new Rect(rectLast.xMin + rectLast.width - buttonWidth, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectLabelValue = new Rect(rectSlider.xMin + rectSlider.width * 0.75f, rectSlider.yMin, rectSlider.width * 0.25f, rectSlider.height);

            bool buttonAdjust = false;

            if (GUI.Button(rectButtonL, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    if (delta == 0)
                        value01 = 0;
                    else
                        delta -= 1;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 -= increment01;
                }
                else buttonAdjust = false;
            }
            if (GUI.Button(rectButtonR, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    if (value01 != 1)
                        value01 = 1;
                    else
                        delta += 1;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 += increment01;
                }
                else buttonAdjust = false;
            }

            if (!numericInput)
            {
                if (rectLast.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) // right click drag doesn't work properly without the event check
                        && Event.current.type != EventType.MouseUp) // drag event covers this, but don't want it to
                {
                    value01 = GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);

                    if (valueOld != value01)
                    {
                        if (Input.GetMouseButton(0) || !allowFine) // normal control
                        {
                            double excess = value01 / increment01;
                            value01 -= (excess - Math.Round(excess)) * increment01;
                        }
                        else if (Input.GetMouseButton(1) && allowFine) // fine control
                        {
                            double excess = valueOld / increment01;
                            value01 = (valueOld - (excess - Math.Round(excess)) * increment01) + Math.Min(value01 - 0.5, 0.4999) * increment01;
                        }
                    }
                }
                else
                {
                    GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);
                }
            }


            GUI.DrawTexture(rectSliderValue, backgroundColor.GetTexture2D()); // slider filled area
            GUI.Label(rectSlider, $"  {name}", UIUtility.uiStyleLabelHint); // slider name
            if (!numericInput)
            {
                value = (float)((value01 + delta) * range);
                GUI.Label(rectLabelValue, GetValueTranslation(value, valueType), UIUtility.uiStyleLabelHint); // slider value
                value = Mathf.Clamp(value, 0.0f, float.PositiveInfinity);
            }
            else
            {
                if (float.TryParse(GUI.TextField(rectLabelValue, value.ToString("F3"), UIUtility.uiStyleInputField), out var temp)) // Add optional numeric input
                {
                    if (!buttonAdjust)
                    {
                        value = temp;
                        value01 = (value - delta * range) / range;
                    }
                    else
                        value = (float)((value01 + delta) * range);
                }
                value = Mathf.Clamp(value, 0.0f, float.PositiveInfinity);
            }
            changed = valueOld != value01;
            GUILayout.EndHorizontal();
            return value;
        }

        public static float OffsetSlider(float value, float increment, float range, string name, out bool changed, Color backgroundColor, int valueType, ref int delta, bool allowFine = true)
        {
            if (!UIUtility.uiStyleConfigured)
            {
                UIUtility.ConfigureStyles();
            }

            GUILayout.BeginHorizontal();
            value += range / 2;
            int newDelta = (int)(value / range);
            if (newDelta != delta & newDelta != delta + 1)
                delta = newDelta;
            double value01 = (value - delta * range) / range;
            double increment01 = increment / range;
            double valueOld = value01;
            const float buttonWidth = 12, spaceWidth = 3;

            GUILayout.Label(string.Empty, UIUtility.uiStyleLabelHint);
            Rect rectLast = GUILayoutUtility.GetLastRect();
            Rect rectSlider = new Rect(rectLast.xMin + buttonWidth + spaceWidth, rectLast.yMin, rectLast.width - 2 * (buttonWidth + spaceWidth), rectLast.height);
            Rect rectSliderValue = new Rect(rectSlider.xMin, rectSlider.yMin, rectSlider.width * (float)value01, rectSlider.height - 3f);
            Rect rectButtonL = new Rect(rectLast.xMin, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectButtonR = new Rect(rectLast.xMin + rectLast.width - buttonWidth, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectLabelValue = new Rect(rectSlider.xMin + rectSlider.width * 0.75f, rectSlider.yMin, rectSlider.width * 0.25f, rectSlider.height);

            bool buttonAdjust = false;

            if (GUI.Button(rectButtonL, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    if (delta == 0 & value01 > 0.5) value01 = 0.5;
                    else if (value01 == 0) delta -= 1;
                    else value01 = 0;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 -= increment01;
                }
                else buttonAdjust = false;
            }
            if (GUI.Button(rectButtonR, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    if (delta == 0 & value01 < 0.5) value01 = 0.5;
                    else if (value01 == 1) delta += 1;
                    else value01 = 1;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 += increment01;
                }
                else buttonAdjust = false;
            }

            if (!numericInput)
            {
                if (rectLast.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) // right click drag doesn't work properly without the event check
                        && Event.current.type != EventType.MouseUp) // drag event covers this, but don't want it to
                {
                    value01 = GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);

                    if (valueOld != value01)
                    {
                        if (Input.GetMouseButton(0) || !allowFine) // normal control
                        {
                            double excess = value01 / increment01;
                            value01 -= (excess - Math.Round(excess)) * increment01;
                        }
                        else if (Input.GetMouseButton(1) && allowFine) // fine control
                        {
                            double excess = valueOld / increment01;
                            value01 = (valueOld - (excess - Math.Round(excess)) * increment01) + Math.Min(value01 - 0.5, 0.4999) * increment01;
                        }
                    }
                }
                else
                {
                    GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);
                }
            }


            GUI.DrawTexture(rectSliderValue, backgroundColor.GetTexture2D()); // slider filled area
            GUI.Label(rectSlider, $"  {name}", UIUtility.uiStyleLabelHint); // slider name
            if (!numericInput)
            {
                value = (float)(value01 * range + range * delta - range / 2);
                GUI.Label(rectLabelValue, GetValueTranslation(value, valueType), UIUtility.uiStyleLabelHint); // slider value
            }
            else
            {
                value -= range / 2;
                if (float.TryParse(GUI.TextField(rectLabelValue, value.ToString("F3"), UIUtility.uiStyleInputField), out var temp)) // Add optional numeric input
                {
                    if (!buttonAdjust)
                    {
                        value = temp;
                        value01 = (value - delta * range) / range;
                    }
                    else
                        value = (float)(value01 * range + range * delta - range / 2);
                }
                value = Mathf.Clamp(value, float.NegativeInfinity, float.PositiveInfinity);
            }
            changed = valueOld != value01;
            GUILayout.EndHorizontal();
            return value;
        }

        public static float LimitedSlider(float value, float increment, float incrementLarge, Vector2 limits, string name, out bool changed, Color backgroundColor, int valueType, bool allowFine = true)
        {
            if (!UIUtility.uiStyleConfigured)
            {
                UIUtility.ConfigureStyles();
            }

            GUILayout.BeginHorizontal();
            double range = limits.y - limits.x;
            double value01 = (value - limits.x) / range; // rescaling value to be <0-1> of range for convenience
            double increment01 = increment / range;
            double valueOld = value01;
            const float buttonWidth = 12, spaceWidth = 3;

            GUILayout.Label(string.Empty, UIUtility.uiStyleLabelHint);
            Rect rectLast = GUILayoutUtility.GetLastRect();
            Rect rectSlider = new Rect(rectLast.xMin + buttonWidth + spaceWidth, rectLast.yMin, rectLast.width - 2 * (buttonWidth + spaceWidth), rectLast.height);
            Rect rectSliderValue = new Rect(rectSlider.xMin, rectSlider.yMin, rectSlider.width * (float)value01, rectSlider.height - 3f);
            Rect rectButtonL = new Rect(rectLast.xMin, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectButtonR = new Rect(rectLast.xMin + rectLast.width - buttonWidth, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectLabelValue = new Rect(rectSlider.xMin + rectSlider.width * 0.75f, rectSlider.yMin, rectSlider.width * 0.25f, rectSlider.height);

            bool buttonAdjust = false;

            if (GUI.Button(rectButtonL, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    value01 -= incrementLarge / range;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 -= increment01;
                }
                else buttonAdjust = false;
            }
            if (GUI.Button(rectButtonR, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    value01 += incrementLarge / range;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 += increment01;
                }
                else buttonAdjust = false;
            }

            if (!numericInput)
            {
                if (rectLast.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) // right click drag doesn't work properly without the event check
                        && Event.current.type != EventType.MouseUp) // drag event covers this, but don't want it to
                {
                    value01 = GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);

                    if (valueOld != value01)
                    {
                        if (Input.GetMouseButton(0) || !allowFine) // normal control
                        {
                            double excess = value01 / increment01;
                            value01 -= (excess - Math.Round(excess)) * increment01;
                        }
                        else if (Input.GetMouseButton(1) && allowFine) // fine control
                        {
                            double excess = valueOld / increment01;
                            value01 = (valueOld - (excess - Math.Round(excess)) * increment01) + Math.Min(value01 - 0.5, 0.4999) * increment01;
                        }
                    }
                }
                else
                {
                    GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);
                }
            }


            GUI.DrawTexture(rectSliderValue, backgroundColor.GetTexture2D()); // slider filled area
            GUI.Label(rectSlider, $"  {name}", UIUtility.uiStyleLabelHint); // slider name
            if (!numericInput)
            {
                value = Mathf.Clamp((float)(value01 * range + limits.x), limits.x, limits.y);

                // merge conflict here
                //value = Mathf.Clamp((float)(value01 * range + limits.x), Mathf.Min((float)(limits.x * 0.5), limits.x), limits.y); // lower limit is halved so the fine control can reduce it further but the normal tweak still snaps. Min makes -ve values work


                GUI.Label(rectLabelValue, GetValueTranslation(value, valueType), UIUtility.uiStyleLabelHint); // slider value
            }
            else
            {
                if (float.TryParse(GUI.TextField(rectLabelValue, value.ToString("F3"), UIUtility.uiStyleInputField), out var temp)) // Add optional numeric input
                {
                    if (!buttonAdjust)
                    {
                        value = temp;
                        value01 = (value - limits.x) / range;
                    }
                    else

                        value = Mathf.Clamp((float)(value01 * range + limits.x), limits.x, limits.y);
                  
                         // merge conflict here
                        value = Mathf.Clamp((float)(value01 * range + limits.x), Mathf.Min((float)(limits.x * 0.5), limits.x), limits.y); // lower limit is halved so the fine control can reduce it further but the normal tweak still snaps. Min makes -ve values work
                }
                value = Mathf.Clamp(value, limits.x, limits.y);
            }
            changed = valueOld != value01;
            GUILayout.EndHorizontal();
            return value;
        }

        public static float IntegerSlider(float value, float incrementLarge, int min, int max, string name, out bool changed, Color backgroundColor, int valueType, bool allowFine = true)
        {
            if (!UIUtility.uiStyleConfigured)
            {
                UIUtility.ConfigureStyles();
            }

            GUILayout.BeginHorizontal();
            int range = max - min;
            double value01 = (value - min) / range;
            double increment01 = 1 / range;
            double valueOld = value01;
            const float buttonWidth = 12, spaceWidth = 3;

            GUILayout.Label(string.Empty, UIUtility.uiStyleLabelHint);
            Rect rectLast = GUILayoutUtility.GetLastRect();
            Rect rectSlider = new Rect(rectLast.xMin + buttonWidth + spaceWidth, rectLast.yMin, rectLast.width - 2 * (buttonWidth + spaceWidth), rectLast.height);
            Rect rectSliderValue = new Rect(rectSlider.xMin, rectSlider.yMin, rectSlider.width * (float)value01, rectSlider.height - 3f);
            Rect rectButtonL = new Rect(rectLast.xMin, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectButtonR = new Rect(rectLast.xMin + rectLast.width - buttonWidth, rectLast.yMin, buttonWidth, rectLast.height);
            Rect rectLabelValue = new Rect(rectSlider.xMin + rectSlider.width * 0.75f, rectSlider.yMin, rectSlider.width * 0.25f, rectSlider.height);

            bool buttonAdjust = false;

            if (GUI.Button(rectButtonL, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    value01 -= incrementLarge / range;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 -= increment01;
                }
                else buttonAdjust = false;
            }
            if (GUI.Button(rectButtonR, string.Empty, UIUtility.uiStyleButton))
            {
                buttonAdjust = true;
                if (Input.GetMouseButtonUp(0) || !allowFine)
                {
                    value01 += incrementLarge / range;
                }
                else if (Input.GetMouseButtonUp(1) && allowFine)
                {
                    value01 += increment01;
                }
                else buttonAdjust = false;
            }

            if (!numericInput)
            {
                if (rectLast.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown) // right click drag doesn't work properly without the event check
                        && Event.current.type != EventType.MouseUp) // drag event covers this, but don't want it to
                {
                    value01 = GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);
                }
                else
                {
                    GUI.HorizontalSlider(rectSlider, (float)value01, 0f, 1f, UIUtility.uiStyleSlider, UIUtility.uiStyleSliderThumb);
                }
            }


            GUI.DrawTexture(rectSliderValue, backgroundColor.GetTexture2D()); // slider filled area
            GUI.Label(rectSlider, $"  {name}", UIUtility.uiStyleLabelHint); // slider name
            if (!numericInput)
            {
                value = Mathf.Round((float)value01 * range) + min;
                value = Mathf.Clamp(value, min, max);

                GUI.Label(rectLabelValue, GetValueTranslation(value, valueType), UIUtility.uiStyleLabelHint); // slider value
            }
            else
            {
                if (float.TryParse(GUI.TextField(rectLabelValue, value.ToString("F3"), UIUtility.uiStyleInputField), out var temp)) // Add optional numeric input
                {
                    if (!buttonAdjust)
                    {
                        value = temp;
                        value01 = (value - min) / range;
                    }
                    else
                        value = Mathf.Round((float)value01 * range) + min;
                }
                value = Mathf.Clamp(value, min, max);
            }
            changed = valueOld != value01;
            GUILayout.EndHorizontal();
            return value;
        }

        public static Rect ClampToScreen(Rect window)
        {
            window.x = Mathf.Clamp(window.x, -window.width + 20, Screen.width - 20);
            window.y = Mathf.Clamp(window.y, -window.height + 20, Screen.height - 20);

            return window;
        }

        public static Rect SetToScreenCenter(this Rect r)
        {
            if (r.width > 0 && r.height > 0)
            {
                r.x = Screen.width / 2f - r.width / 2f;
                r.y = Screen.height / 2f - r.height / 2f;
            }
            return r;
        }

        public static Rect SetToScreenCenterAlways(this Rect r)
        {
            r.x = Screen.width / 2f - r.width / 2f;
            r.y = Screen.height / 2f - r.height / 2f;
            return r;
        }

        public static double TextEntryForDouble(string label, int labelWidth, double prevValue)
        {
            string valString = prevValue.ToString();
            TextEntryField(label, labelWidth, ref valString);

            return
                !double.TryParse(valString, out double temp)
                    ? prevValue
                    : temp
            ;
        }

        public static void TextEntryField(string label, int labelWidth, ref string inputOutput)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(labelWidth));
            inputOutput = GUILayout.TextField(inputOutput);
            GUILayout.EndHorizontal();
        }

        public static Vector3 GetMousePos()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.y = Screen.height - mousePos.y;
            return mousePos;
        }

        public static Vector3 GetMouseWindowPos(Rect windowRect)
        {
            Vector3 mousepos = GetMousePos();
            mousepos.x -= windowRect.x;
            mousepos.y -= windowRect.y;
            return mousepos;
        }

        public static string GetValueTranslation(float value, int type)
        {
            if (type == 1)
            {
                if (value == 0f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000133");		// #autoLOC_B9_Aerospace_WingStuff_1000133 = Uniform
                else if (value == 1f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000134");		// #autoLOC_B9_Aerospace_WingStuff_1000134 = Standard
                else if (value == 2f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000135");		// #autoLOC_B9_Aerospace_WingStuff_1000135 = Reinforced
                else if (value == 3f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000136");		// #autoLOC_B9_Aerospace_WingStuff_1000136 = LRSI
                else if (value == 4f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000137");		// #autoLOC_B9_Aerospace_WingStuff_1000137 = HRSI
                else return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000138");		// #autoLOC_B9_Aerospace_WingStuff_1000138 = Unknown material
            }
            else if (type == 2)
            {
                if (value == 1f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000139");		// #autoLOC_B9_Aerospace_WingStuff_1000139 = No edge
                else if (value == 2f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000140");		// #autoLOC_B9_Aerospace_WingStuff_1000140 = Rounded
                else if (value == 3f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000141");		// #autoLOC_B9_Aerospace_WingStuff_1000141 = Biconvex
                else if (value == 4f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000142");		// #autoLOC_B9_Aerospace_WingStuff_1000142 = Triangular
                else return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000143");		// #autoLOC_B9_Aerospace_WingStuff_1000143 = Unknown
            }
            else if (type == 3)
            {
                if (value == 1f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000144");		// #autoLOC_B9_Aerospace_WingStuff_1000144 = Rounded
                else if (value == 2f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000145");		// #autoLOC_B9_Aerospace_WingStuff_1000145 = Biconvex
                else if (value == 3f) return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000146");		// #autoLOC_B9_Aerospace_WingStuff_1000146 = Triangular
                else return Localizer.Format("#autoLOC_B9_Aerospace_WingStuff_1000147");		// #autoLOC_B9_Aerospace_WingStuff_1000147 = Unknown
            }
            else return value.ToString("F3");
        }
        public static bool CheckBox(string desc, string choice1, string choice2, bool value, out bool changed)
        {
            float buttonWidth = 50;
            //float spaceWidth = 3;
            GUILayout.BeginHorizontal();
            GUILayout.Label("", UIUtility.uiStyleLabelHint);
            Rect rectLast = GUILayoutUtility.GetLastRect();
            Rect rectButton = new Rect(rectLast.x + rectLast.width - 53, rectLast.y, buttonWidth, rectLast.height);
            //Rect rectChoice = new Rect(rectButton.xMin, rectButton.yMin, rectButton.width, rectButton.height); 
            Rect rectDesc = new Rect(rectLast.x, rectLast.y, rectLast.width - 53, rectLast.height);
            string choice;
            changed = false;
            choice = GetChoice(choice1, choice2, value);
            if (GUI.Button(rectButton, choice, UIUtility.uiStyleButton))
            {
                value = !value;
                changed = true;
            }
            GUI.Label(rectDesc, "  " + desc, UIUtility.uiStyleLabelHint);


            GUILayout.EndHorizontal();
            return value;
        }
        public static string GetChoice(string choice1, string choice2, bool state)
        {
            string choice;
            if (!state)
                choice = choice1;
            else
                choice = choice2;
            return choice;
        }
    }
}