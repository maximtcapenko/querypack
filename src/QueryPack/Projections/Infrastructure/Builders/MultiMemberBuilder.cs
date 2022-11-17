namespace QueryPack.Projections.Infrastructure.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class MultiMemberBuilder : IMemberBuilder
    {
        private readonly MemberExpression _parentMemberExpression;
        private readonly List<MemberExpression> _memberExpressions;

        public MultiMemberBuilder(MemberExpression memberExpression)
        {
            _memberExpressions = new List<MemberExpression>();
            _parentMemberExpression = memberExpression;
        }

        public void AddMemberExpression(MemberExpression memberExpression)
        {
            if (!_memberExpressions.Contains(memberExpression))
                _memberExpressions.Add(memberExpression);
        }

        public void Build(ITypeBuilder typeBuilder)
        {
            var propertyInfo = _parentMemberExpression.Member as PropertyInfo; // TODO: fix it
            var type = CreateType(_memberExpressions);

            typeBuilder.DefineField(propertyInfo.Name, type);
        }

        private Type CreateType(IEnumerable<MemberExpression> initArgs)
        {
            var typeBuilder = AnonymousTypeBuilder.GetBuilder();

            foreach (var arg in initArgs)
            {
                var property = arg.Member as PropertyInfo;
                typeBuilder.DefineField(property.Name, property.PropertyType);
            }

            return typeBuilder.CreateType();
        }
    }

}
