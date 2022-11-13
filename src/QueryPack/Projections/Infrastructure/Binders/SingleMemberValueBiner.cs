namespace QueryPack.Projections.Infrastructure.Binders
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SingleMemberValueBiner : IMemberValueBinder
    {
        private MemberExpression _sourceExpression;
        private MemberExpression _destinationExpression;

        public SingleMemberValueBiner(MemberExpression sourceExpression, MemberExpression destinationExpression)
        {
            _destinationExpression = destinationExpression;
            _sourceExpression = sourceExpression;
        }

        public void Bind(List<MemberBinding> memberBindings)
        {
            memberBindings.Add(Expression.Bind(_destinationExpression.Member, _sourceExpression));
        }
    }
}
