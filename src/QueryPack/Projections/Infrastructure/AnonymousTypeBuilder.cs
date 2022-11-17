namespace QueryPack.Projections.Infrastructure
{
    using Extensions;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    internal class AnonymousTypeBuilder : ITypeBuilder
    {
        private static AssemblyName _assemblyName = new AssemblyName() { Name = "AnonymousTypes" };
        private static ModuleBuilder _moduleBuilder = null;
        private static ConcurrentDictionary<string, Type> _typeCache = new ConcurrentDictionary<string, Type>();

        private readonly Dictionary<string, Type> _definedMembers = new Dictionary<string, Type>();

        static AnonymousTypeBuilder()
        {
            _moduleBuilder = AssemblyBuilder
                .DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run)
                .DefineDynamicModule(_assemblyName.Name);
        }

        public static AnonymousTypeBuilder GetBuilder()
        {
            return new AnonymousTypeBuilder();
        }

        public void DefineField(string name, Type type)
        {
            _definedMembers[name] = type;
        }

        public Type CreateType()
        {
            var name = string.Join(";", _definedMembers.Select(e => $"{e.Key}_{e.Value.Name}"));
            var key = $"DynamicDTO_{name.Hash()}";
            if (!_typeCache.ContainsKey(key))
            {
                var typeBuilder = GetTypeBuilder(key);

                foreach (var member in _definedMembers)
                {
                    var visitor = new PropertyTypeVisitor(member.Key, member.Value);
                    visitor.Visit(typeBuilder);
                }
                _typeCache.AddOrUpdate(key, typeBuilder.CreateType(), (k, t) => t);
            }

            return _typeCache[key];
        }

        private TypeBuilder GetTypeBuilder(string typeName, params Type[] interfaces)
        {
            var typeBuilder = _moduleBuilder.DefineType(typeName, TypeAttributes.Public |
                TypeAttributes.Class | TypeAttributes.Serializable);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public |
                MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
            
            foreach (var @interface in interfaces)
            {
                typeBuilder.AddInterfaceImplementation(@interface);
            }

            return typeBuilder;
        }
    }
}
