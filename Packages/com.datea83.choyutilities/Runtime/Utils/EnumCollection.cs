namespace EugeneC.Utilities {

    public enum ELanguage : byte {

        English = 0,
        SimplifiedChinese = 1,
        TraditionalChinese = 2,
        Malay = 3,
        NotDefined = byte.MaxValue

    }

    public enum EAxis : byte {

        X = 0,
        Y = 1 << 0,
        Z = 1 << 1

    }

    public enum ETwoAxis : byte {

        XY = 0,
        XZ = 1 << 0,
        YZ = 1 << 1

    }

    public enum EControlSchemeEnum : byte {

        Keyboard,
        Gamepad,
        Touchscreen,
        XR,
        NotDefined = byte.MaxValue

    }

    public enum EVisualElements {

        Button,
        DoubleField,
        DropdownField,
        EnumField,
        FloatField,
        Foldout,
        IntegerField,
        Label,
        ListView,
        LongField,
        MinMaxSlider,
        MultiColumnListView,
        MultiColumnTreeView,
        ProgressBar,
        RadioButton,
        RadioButtonGroup,
        RepeatButton,
        Slider,
        SliderInt,
        TemplateContainer,
        TextElement,
        Toggle,
        TreeView,
        UnsignedLongField,
        Vector2Field,
        Vector2IntField,
        Vector3Field,
        Vector3IntField,
        Vector4Field

    }

}