namespace QueryPack.Projections.Infrastructure.Builders
{
    using System.Linq.Expressions;
    using System.Reflection;

    internal class SingleMemberBuilder : IMemberBuilder
    {
        private readonly MemberExpression _memberExpression;

        public SingleMemberBuilder(MemberExpression memberExpression)
        {
            _memberExpression = memberExpression;
        }

        public void Build(ITypeBuilder typeBuilder)
        {
            var property = _memberExpression.Member as PropertyInfo;
            typeBuilder.DefineField(property.Name, property.PropertyType);
        }
    }
}
