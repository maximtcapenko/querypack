namespace QueryPack.Projections.Infrastructure.Binders
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class ProjectionMemberValueBinder : IMemberValueBinder
    {
        private readonly MemberExpression _source;
        private readonly MemberExpression _destination;
        
        public ProjectionMemberValueBinder(MemberExpression source, MemberExpression desctination)
        {
            _source = source;
            _destination = desctination;
        }

        public void Bind(List<MemberBinding> memberBindings)
        {
            memberBindings.Add(Expression.Bind(_destination.Member as PropertyInfo, _source));
        }
    }
}