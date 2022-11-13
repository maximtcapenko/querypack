namespace QueryPack.Projections.Infrastructure
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal interface IMemberValueBinder
    {
        void Bind(List<MemberBinding> memberBindings);
    }
}
