namespace QueryPack.Projections.Infrastructure.Binders
{
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal class MultiMemberValueBinder : IMemberValueBinder
    {
        private MemberExpression _parentMemberExpression;
        private List<(MemberExpression, MemberExpression)> _memberExpressions;

        public MultiMemberValueBinder(MemberExpression memberExpression)
        {
            _memberExpressions = new List<(MemberExpression, MemberExpression)>();
            _parentMemberExpression = memberExpression;
        }

        public void AddMemberExpression(MemberExpression sourceExpression)
        {
            var destinationExpression = Expression.PropertyOrField(_parentMemberExpression, sourceExpression.Member.Name);
            if (!_memberExpressions.Contains((sourceExpression, destinationExpression)))
                _memberExpressions.Add((sourceExpression, destinationExpression));
        }

        public void Bind(List<MemberBinding> memberBindings)
        {
            var propertyInfo = _parentMemberExpression.GetPropertyOrFieldInfo();
            var expression = Assign(propertyInfo.Item2, _memberExpressions.ToArray());

            memberBindings.Add(Expression.Bind(_parentMemberExpression.Member, expression));
        }

        private Expression Assign(Type type, IEnumerable<(MemberExpression, MemberExpression)> initArgs)
        {
            var @new = Expression.New(type);

            return Expression.MemberInit(@new, initArgs.Select(arg =>
            {
                var argMember = arg.Item2.Member;

                return Expression.Bind(argMember, arg.Item1);
            }));
        }
    }

}
