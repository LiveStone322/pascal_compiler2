namespace PascalCompiler2
{
    public class IdentTableEntity : NamedEntity
    {
        public IdentUsageType UsageType { get; set; }
        public TypeTableEntity Type { get; set; }
        public bool? BoolVal { get; set; } = null;
        public int? IntVal { get; set; } = null;
        public double? FloatVal { get; set; } = null;
        public string StringVar { get; set; } = null;

        public IdentTableEntity(TypeTableEntity type, IdentUsageType usageType, string name) : base (name)
        {
            Type = type;
            UsageType = usageType;
        }

        // for enums
        public IdentTableEntity(IdentUsageType usageType, string name = "") : base(name)
        {
            Type = new TypeTableEntity();
            UsageType = usageType;
        }
    }

    public enum IdentUsageType
    {
        ROOT,
        CONST,
        FUNCTION,
        PROCEDURE,
        PROGRAM,
        TYPE,
        VAR
    }
}
