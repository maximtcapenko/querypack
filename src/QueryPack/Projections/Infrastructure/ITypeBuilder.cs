namespace QueryPack.Projections.Infrastructure
{
    using System;

    public interface ITypeBuilder
    {
        void DefineField(string name, Type type);
    }
}