namespace PascalCompiler2
{
    public enum LimitedTypeParamsTypesEnum
    {
        INT,
        CHAR
    }
    public class LimitedTypeParams : BaseTypeParams
    {
        public object Min { get; set; }
        public object Max { get; set; }
        public LimitedTypeParamsTypesEnum Type { get; set; }

        public LimitedTypeParams(int min, int max)
        {
            Min = min;
            Max = max;
            Type = LimitedTypeParamsTypesEnum.INT;
        }

        public LimitedTypeParams(char min, char max)
        {
            Min = min;
            Max = max;
            Type = LimitedTypeParamsTypesEnum.CHAR;
        }
    }
}
