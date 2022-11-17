namespace QueryPack.Projections.Infrastructure.Builders
{
    using System;
    using System.Reflection;

    internal class RuntimeMemberBuilder : IMemberBuilder
    {
        private readonly string _name;
        private readonly Type _typeInfo;

        public RuntimeMemberBuilder(string name, Type typeInfo)
        {
            _name = name;
            _typeInfo = typeInfo;
        }

        public void Build(ITypeBuilder typeBuilder)
        {
            typeBuilder.DefineField(_name, _typeInfo);
        }
    }
}