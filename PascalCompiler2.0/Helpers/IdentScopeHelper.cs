using System;
using System.Collections.Generic;
using System.Text;

namespace PascalCompiler2.Helpers
{
    public class IdentScopeTree
    {
        public List<IdentScopeTree> Children { get; set; } = new List<IdentScopeTree>();
        public IdentScopeTree Parent { get; set; } = null;
        public List<TypeTableEntity> Types = new List<TypeTableEntity>();
        public List<IdentTableEntity> Idents = new List<IdentTableEntity>();

        ErrorController errorController;

        public IdentScopeTree(ErrorController ec)
        {
            this.errorController = ec;
        }

        public IdentTableEntity AddIdent(string name, IdentUsageType usageType = IdentUsageType.CONST, TypeTableEntity type = null)
        {
            if (Idents.Find(t => t.Name == name) != null)
            {
                errorController.AddError(-1, -1, Models.ErrorCodes.NAME_EXISTS, "", name);
                return null;
            }

            var ident = new IdentTableEntity(type, usageType, name);
            Idents.Add(ident);
            return ident;
        }

        public IdentTableEntity AddIdent(string name, IdentUsageType usageType, string type )
        {
            var typeEntity = FindType(type);

            if (typeEntity == null)
            {
                errorController.AddError(-1, -1, Models.ErrorCodes.UNKNOWN_TYPE, "", name);
                AddType(TypeTableTypeEnums.SCALAR, "", null);
                return null;
            }
            if (FindIdent(name) != null)
            {
                errorController.AddError(-1, -1, Models.ErrorCodes.NAME_EXISTS, "", name);
                return null;
            }

            var ident = new IdentTableEntity(typeEntity, usageType, name);
            Idents.Add(ident);
            return ident;
        }


        public TypeTableEntity AddType(TypeTableTypeEnums typeType = TypeTableTypeEnums.SCALAR, string name = "", BaseTypeParams par = null)
        {
            if (FindType(name) != null)
            {
                errorController.AddError(-1, -1, Models.ErrorCodes.TYPE_EXISTS, "", name);
                return null;
            }

            var type = new TypeTableEntity(typeType, name, par);
            Types.Add(type);
            return type;
        }

        public TypeTableEntity AddType(string name, TypeTableEntity type)
        {
            if (FindType(name) != null)
            {
                errorController.AddError(-1, -1, Models.ErrorCodes.TYPE_EXISTS, "", name);
                return null;
            }
            if (type.Name == "")
            {
                type.Name = name;
                Types.Add(type);
                return type;
            }
            var newType = new TypeTableEntity(type.Type, name, type.Params);
            Types.Add(newType);
            return newType;
        }

        public TypeTableEntity AddType(TypeTableEntity type)
        {
            if (FindType(type.Name) != null)
            {
                errorController.AddError(-1, -1, Models.ErrorCodes.TYPE_EXISTS, "", type.Name);
                return null;
            }
            Types.Add(type);
            return type;
        }

        private TypeTableEntity CheckUpperScopesForType(IdentScopeTree node, string name)
        {
            if (node == null) return null;
            var result = Array.Find(node.Types.ToArray(), t => t.Name == name);
            if (result != null) return result;
            return CheckUpperScopesForType(node.Parent, name);
        }

        private IdentTableEntity CheckUpperScopesForIdent(IdentScopeTree node, string name)
        {
            if (node == null) return null;
            var result = Array.Find(node.Idents.ToArray(), t => t.Name == name);
            if (result != null) return result;
            return CheckUpperScopesForIdent(node.Parent, name);
        }

        public IdentTableEntity FindIdent(string name)
        {
            return CheckUpperScopesForIdent(this, name);
        }

        public TypeTableEntity FindType(string name)
        {
            return CheckUpperScopesForType(this, name);
        }
    }

    public class IdentScopeHelper
    {
        IdentScopeTree root ;
        ErrorController errorController;

        public IdentScopeTree cur;

        public IdentScopeHelper(ErrorController ec)
        {
            errorController = ec;
            root = new IdentScopeTree(ec);
            cur = root;
            InitRootScope();
        }

        public void OpenScope()
        {
            var oldCur = cur;
            cur = new IdentScopeTree(errorController);
            oldCur.Children.Add(cur);
            cur.Parent = oldCur;
        }

        public void CloseScope()
        {
            if (cur.Parent != null) cur = cur.Parent;
        }

        private void InitRootScope()
        {
            foreach (var t in Models.typeNames)
            {
                root.AddType(TypeTableTypeEnums.SCALAR, t.Value);
            }

            root.AddIdent("false", IdentUsageType.PROGRAM, Models.typeNames[Models.Types.BOOL]).BoolVal = false;
            root.AddIdent("true", IdentUsageType.PROGRAM, Models.typeNames[Models.Types.BOOL]).BoolVal = true;
            root.AddIdent("maxint", IdentUsageType.PROGRAM, Models.typeNames[Models.Types.INT]).IntVal = int.MaxValue;
            root.AddIdent("writeln", IdentUsageType.PROGRAM, Models.typeNames[Models.Types.VOID]);
        }
    }
}
