namespace PascalCompiler2
{
    public class EnumTypeParams : BaseTypeParams
    {
        IdentTableEntity[] Enums { get; set; }

        public EnumTypeParams(IdentTableEntity[] enums)
        {
            Enums = enums;
        }
    }
}
