using UnityEditor;
using UnityEngine;

namespace EugeneC.Editor {

    internal static class EditorUtils {

        public const string UtilityWindow = "ChoyUtilities/";

    }

    internal static class EditorBackgroundColor {

        public static readonly Color DefaultLightColor = new(0.7843f, 0.7843f, 0.7843f);
        public static readonly Color DefaultDarkColor = new(0.2196f, 0.2196f, 0.2196f);

        public static readonly Color SelectedLightColor = new(0.22745f, 0.447f, 0.6902f);
        public static readonly Color SelectedDarkColor = new(0.1725f, 0.3647f, 0.5294f);

        public static readonly Color SelectedUnfocusedLightColor = new(0.68f, 0.68f, 0.68f);
        public static readonly Color SelectedUnfocusedDarkColor = new(0.3f, 0.3f, 0.3f);

        public static readonly Color HoverLightColor = new(0.698f, 0.698f, 0.698f);
        public static readonly Color HoverDarkColor = new(0.2706f, 0.2706f, 0.2706f);

        public static Color GetColor(bool isSelected, bool isHovered, bool isWindowsFocused) {
            if (isSelected) {
                if (isWindowsFocused)
                    return EditorGUIUtility.isProSkin ? SelectedDarkColor : SelectedLightColor;

                return EditorGUIUtility.isProSkin ? SelectedUnfocusedDarkColor : SelectedUnfocusedLightColor;
            }

            if (isHovered) return EditorGUIUtility.isProSkin ? HoverDarkColor : HoverLightColor;

            return EditorGUIUtility.isProSkin ? DefaultDarkColor : DefaultLightColor;
        }

    }

}