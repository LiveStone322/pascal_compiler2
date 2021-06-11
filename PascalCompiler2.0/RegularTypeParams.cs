namespace PascalCompiler2
{
    public class RegularTypeParams : BaseTypeParams
    {
        public TypeTableEntity[] Params { get; set; }
        public TypeTableEntity BaseType { get; set; }

        public RegularTypeParams(TypeTableEntity type, TypeTableEntity[] par)
        {
            BaseType = type;
            Params = par;
        }
    }
}
