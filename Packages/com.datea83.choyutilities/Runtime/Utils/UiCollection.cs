using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace EugeneC.Utilities {

    public static partial class HelperCollection {

        public static void ActivateDisplay() {
            for (var i = 1; i < Display.displays.Length; i++) Display.displays[i].Activate();
        }

        public static (float, float) GetBoundingBoxSize(this RectTransform rectTransform) {
            var rect = rectTransform.rect;
            var center = rect.center;

            var topLeftRel = new float2(rect.xMin - center.x, rect.yMin - center.y);
            var topRightRel = new float2(rect.xMax - center.x, rect.yMin - center.y);

            // Rotate in 2D using Z (RectTransform is effectively 2D around Z)
            var zRad = math.radians(rectTransform.localEulerAngles.z);
            var sin = math.sin(zRad);
            var cos = math.cos(zRad);

            var rotatedTopLeftRel = Rotate(topLeftRel);
            var rotatedTopRightRel = Rotate(topRightRel);

            var wMax = math.max(math.abs(rotatedTopLeftRel.x), math.abs(rotatedTopRightRel.x));
            var hMax = math.max(math.abs(rotatedTopLeftRel.y), math.abs(rotatedTopRightRel.y));

            return (2f * wMax, 2f * hMax);

            float2 Rotate(float2 rel) {
                return new float2(cos * rel.x - sin * rel.y, sin * rel.x + cos * rel.y);
            }
        }

        public static string GetLocaleCode(this ELanguage language) {
            return language switch {
                ELanguage.NotDefined => throw new ArgumentOutOfRangeException(nameof(language), language,
                    "Not defined language"),
                ELanguage.English => "en",
                ELanguage.SimplifiedChinese => "zh-Hans",
                ELanguage.TraditionalChinese => "zh-TW",
                ELanguage.Malay => "ms",
                _ => throw new ArgumentOutOfRangeException(nameof(language), language, "" +
                    "Not defined language")
            };
        }

        public static VisualElement GetVisualElement(this EVisualElements type) {
            return type switch {
                EVisualElements.Button => new Button(),
                EVisualElements.DoubleField => new DoubleField(),
                EVisualElements.DropdownField => new DropdownField(),
                EVisualElements.EnumField => new EnumField(),
                EVisualElements.FloatField => new FloatField(),
                EVisualElements.Foldout => new Foldout(),
                EVisualElements.IntegerField => new IntegerField(),
                EVisualElements.Label => new Label(),
                EVisualElements.ListView => new ListView(),
                EVisualElements.LongField => new LongField(),
                EVisualElements.MinMaxSlider => new MinMaxSlider(),
                EVisualElements.MultiColumnListView => new MultiColumnListView(),
                EVisualElements.MultiColumnTreeView => new MultiColumnTreeView(),
                EVisualElements.ProgressBar => new ProgressBar(),
                EVisualElements.RadioButton => new RadioButton(),
                EVisualElements.RadioButtonGroup => new RadioButtonGroup(),
                EVisualElements.RepeatButton => new RepeatButton(),
                EVisualElements.Slider => new Slider(),
                EVisualElements.SliderInt => new SliderInt(),
                EVisualElements.TemplateContainer => new TemplateContainer(),
                EVisualElements.TextElement => new TextElement(),
                EVisualElements.Toggle => new Toggle(),
                EVisualElements.TreeView => new TreeView(),
                EVisualElements.UnsignedLongField => new UnsignedLongField(),
                EVisualElements.Vector2Field => new Vector2Field(),
                EVisualElements.Vector2IntField => new Vector2IntField(),
                EVisualElements.Vector3Field => new Vector3Field(),
                EVisualElements.Vector3IntField => new Vector3IntField(),
                EVisualElements.Vector4Field => new Vector4Field(),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type,
                    "This enum value is runtime-only or unsupported at editor.")
            };
        }

    }

}