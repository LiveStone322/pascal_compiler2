namespace PascalCompiler2
{
    public class TypeTableEntity : NamedEntity
    {
        public TypeTableTypeEnums Type;
        public BaseTypeParams Params;

        public TypeTableEntity(TypeTableTypeEnums type = TypeTableTypeEnums.SCALAR, string name = "", BaseTypeParams par = null) : base(name)
        {
            Type = type;
            Params = par != null ? par : new BaseTypeParams();
        }

        public bool IsCompatableTo(TypeTableEntity type)
        {
            var basic1 = GetBasicType();
            var basic2 = type.GetBasicType();

            if (type == null) return true;

            if (basic1.Type == TypeTableTypeEnums.SCALAR && basic2.Type == TypeTableTypeEnums.SCALAR)
            {
                if (basic2.Name == basic1.Name) return true;
                return basic1.Name == "real" && basic2.Name == "integer";
            }


            if (basic1.Type == TypeTableTypeEnums.TYPECLASS_REGULAR && basic2.Type == TypeTableTypeEnums.TYPECLASS_REGULAR)
            {
                return basic1.Name == basic2.Name;
            }
            if (basic1.Type == TypeTableTypeEnums.TYPECLASS_LIMITED && basic2.Type == TypeTableTypeEnums.TYPECLASS_REGULAR)
            {
                return basic1.Name == basic2.Name;
            }
            if (basic1.Type == TypeTableTypeEnums.TYPECLASS_REGULAR && basic2.Type == TypeTableTypeEnums.TYPECLASS_LIMITED)
            {
                return basic1.Name == basic2.Name;
            }

            return basic2.Type == basic1.Type || basic2.Name == basic1.Name;
        }

        private TypeTableEntity GetBasicType()
        {
            if (Params is BaseTypeParams)
            {
                return new TypeTableEntity(TypeTableTypeEnums.SCALAR, Name);
            }
            if (Params is RegularTypeParams)
            {
                return ((RegularTypeParams)Params).BaseType;
            }
            if (Params is LimitedTypeParams)
            {
                switch (((LimitedTypeParams)Params).Type)
                {
                    case LimitedTypeParamsTypesEnum.INT:
                        return new TypeTableEntity(TypeTableTypeEnums.SCALAR, "integer");
                    case LimitedTypeParamsTypesEnum.CHAR:
                        return new TypeTableEntity(TypeTableTypeEnums.SCALAR, "char");
                }
            }
            return null;
        }
    }

    public enum TypeTableTypeEnums
    {
        SCALAR,             // [ name, TypeTableTypes ]
        TYPECLASS_LIMITED,  // [ name, TypeTableTypes, min, max, base_Type ]
        TYPECLASS_ENUM,     // [ name, TypeTableTypes, idents[] ]
        TYPECLASS_REGULAR,  // [ name, TypeTableTypes, base_type, dimIndecies[] ]
    }
}
