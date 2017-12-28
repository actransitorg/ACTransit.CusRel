using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using LinqKit;

namespace ACTransit.CusRel.Repositories.Search
{
    public static class SearchPredicates
    {
        private static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod(@"Contains", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(string) }, null);
        private static readonly MethodInfo BooleanEqualsMethod = typeof(bool).GetMethod(@"Equals", BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(bool) }, null);
        private static readonly string[] GreaterThanSymbols = new[] { "from", "start", "begin" };
        private static readonly string[] LessThanSymbols = new[] { "to", "stop", "end" };

        public static List<string> SplitPascalCaseDescending(this string value)
        {
            value = value.Trim();
            var re = new Regex("([A-Z])");
            var matches = re.Matches(value);
            var result = new List<string> { value };
            for (var idx = matches.Count - 1; idx >= 0; idx--)
            {
                Match match = matches[idx];
                if (match.Index == 0) continue;
                var substring = value.Substring(0, match.Index);
                result.Add(substring);

            }
            return result;
        }

        public static Expression<Func<TDbType, bool>> BuildPredicate<TDbType, TSearchCriteria>(TSearchCriteria searchCriteria)
        {
            var predicate = PredicateBuilder.True<TDbType>();

            // Iterate the search criteria properties
            var searchCriteriaPropertyInfos = searchCriteria.GetType().GetProperties();
            foreach (var propInfo in searchCriteriaPropertyInfos)
            {
                var dbFieldNameFull = GetDbFieldName(propInfo);
                var dbType = typeof(TDbType); // Get the target DB type (table)
                var possibleFieldNames = SplitPascalCaseDescending(dbFieldNameFull);
                var levelsUp = 0;
                while (levelsUp < Math.Min(2, possibleFieldNames.Count))
                {
                    var dbFieldName = possibleFieldNames[levelsUp]; // Get the name of the DB field, which may not be the same as the property name.
                    try
                    {
                        var dbFieldMemberInfo = dbType.GetMember(dbFieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance).Single(); // Get a MemberInfo for the type's field (ignoring case so "FirstName" works as well as "firstName")

                        // STRINGS
                        if (propInfo.PropertyType == typeof(string))
                        {
                            levelsUp = byte.MaxValue;
                            predicate = ApplyStringCriterion(searchCriteria, propInfo, dbType, dbFieldMemberInfo, predicate);
                        }
                        // BOOLEANS
                        else if (propInfo.PropertyType == typeof(bool?) || propInfo.PropertyType == typeof(bool))
                        {
                            levelsUp = byte.MaxValue;
                            predicate = ApplyBoolCriterion(searchCriteria, propInfo, dbType, dbFieldMemberInfo, predicate);
                        }
                        // INT
                        else if (propInfo.PropertyType == typeof(int?) || propInfo.PropertyType == typeof(int))
                        {
                            levelsUp = byte.MaxValue;
                            predicate = ApplyIntCriterion(searchCriteria, propInfo, dbType, dbFieldMemberInfo, predicate);
                        }
                        // DATETIME
                        else if (propInfo.PropertyType == typeof(DateTime?))
                        {
                            var equalitySymbol = dbFieldNameFull.Replace(dbFieldName, "").ToLower();
                            levelsUp = byte.MaxValue;
                            predicate = ApplyDateTimeCriterion(searchCriteria, propInfo, dbType, dbFieldMemberInfo, predicate, equalitySymbol);
                        }
                    }
                    catch
                    {
                       
                    }
                    levelsUp++;
                }
            }

            return predicate;
        }

        private static Expression<Func<TDbType, bool>> ApplyStringCriterion<TDbType, TSearchCriteria>(TSearchCriteria searchCriteria, PropertyInfo propInfo, Type dbType, MemberInfo dbFieldMemberInfo, Expression<Func<TDbType, bool>> predicate)
        {
            // Check if a search criterion was provided
            var criteria = propInfo.GetValue(searchCriteria) as string;
            if (string.IsNullOrWhiteSpace(criteria)) return predicate;
            var dbTypeParameter = Expression.Parameter(dbType, @"x"); // "and" it to the predicate, creating an "x" as TDbType.  e.g. predicate = predicate.And(x => x.firstName.Contains(searchCriterion.FirstName)); ...
            var dbFieldMember = Expression.MakeMemberAccess(dbTypeParameter, dbFieldMemberInfo);
            var criterionConstant = new Expression[] { Expression.Constant(criteria) }; // Create the criterion as a constant
            var call = Expression.Call(dbFieldMember, StringContainsMethod, criterionConstant); // Create the MethodCallExpression like x.firstName.Contains(criterion)
            var lambda = Expression.Lambda(call, dbTypeParameter) as Expression<Func<TDbType, bool>>;  // Create a lambda like x => x.firstName.Contains(criterion)
            return predicate.And(lambda);  // Apply!
        }

        private static Expression<Func<TDbType, bool>> ApplyBoolCriterion<TDbType, TSearchCriteria>(TSearchCriteria searchCriteria, PropertyInfo propInfo, Type dbType, MemberInfo dbFieldMemberInfo, Expression<Func<TDbType, bool>> predicate)
        {
            // Check if a search criterion was provided
            var criteria = propInfo.GetValue(searchCriteria) as bool?;
            var dbTypeParameter = Expression.Parameter(dbType, @"x"); // "and" it to the predicate, creating an "x" as TDbType.  e.g. e.g. predicate = predicate.And(x => x.isActive.Contains(searchCriterion.IsActive)); ...
            var dbFieldMember = Expression.MakeMemberAccess(dbTypeParameter, dbFieldMemberInfo);
            var criterionConstant = new Expression[] { Expression.Constant(criteria) }; // Create the criterion as a constant
            var call = Expression.Call(dbFieldMember, BooleanEqualsMethod, criterionConstant); // Create the MethodCallExpression like x.isActive.Equals(criterion)
            var lambda = Expression.Lambda(call, dbTypeParameter) as Expression<Func<TDbType, bool>>;  // Create a lambda like x => x.isActive.Equals(criterion)
            return predicate.And(lambda); // Apply!
        }

        private static Expression<Func<TDbType, bool>> ApplyIntCriterion<TDbType, TSearchCriteria>(TSearchCriteria searchCriteria, PropertyInfo propInfo, Type dbType, MemberInfo dbFieldMemberInfo, Expression<Func<TDbType, bool>> predicate)
        {
            // Check if a search criterion was provided
            var criteria = propInfo.GetValue(searchCriteria) as int?;
            var dbTypeParameter = Expression.Parameter(dbType, @"x"); // "and" it to the predicate, creating an "x" as TDbType.  e.g. e.g. predicate = predicate.And(x => x.isActive.Contains(searchCriterion.IsActive)); ...
            var dbFieldMember = Expression.MakeMemberAccess(dbTypeParameter, dbFieldMemberInfo);
            var criterionConstant = new Expression[] { Expression.Constant(criteria) }; // Create the criterion as a constant
            var call = Expression.Call(dbFieldMember, BooleanEqualsMethod, criterionConstant); // Create the MethodCallExpression like x.isActive.Equals(criterion)
            var lambda = Expression.Lambda(call, dbTypeParameter) as Expression<Func<TDbType, bool>>;  // Create a lambda like x => x.isActive.Equals(criterion)
            return predicate.And(lambda); // Apply!
        }

        private static Expression<Func<TDbType, bool>> ApplyDateTimeCriterion<TDbType, TSearchCriteria>(TSearchCriteria searchCriteria, PropertyInfo propInfo, Type dbType, MemberInfo dbFieldMemberInfo, Expression<Func<TDbType, bool>> predicate, string equalitySymbol)
        {
            // Check if a search criterion was provided
            var criteria = propInfo.GetValue(searchCriteria) as DateTime?;
            var dbTypeParameter = Expression.Parameter(dbType, @"x"); // "and" it to the predicate, creating an "x" as TDbType.  e.g. e.g. predicate = predicate.And(x => x.Prop1 > searchCriterion); ...
            var dbFieldMember = Expression.MakeMemberAccess(dbTypeParameter, dbFieldMemberInfo);

            Expression body;
            if (GreaterThanSymbols.Contains(equalitySymbol))
                body = Expression.GreaterThanOrEqual(Expression.Constant(criteria), Expression.Constant(criteria));
            else if (LessThanSymbols.Contains(equalitySymbol))
                body = Expression.LessThanOrEqual(Expression.Constant(criteria), Expression.Constant(criteria));
            else
                body = Expression.Constant(criteria);

            var criterionConstant = new Expression[] { Expression.Constant(criteria) }; // Create the criterion as a constant
            var call = Expression.Call(dbFieldMember, BooleanEqualsMethod, criterionConstant); // Create the MethodCallExpression like x.isActive.Equals(criterion)

            var lambda = Expression.Lambda(call, dbTypeParameter) as Expression<Func<TDbType, bool>>;  // Create a lambda like x => x.Prop1 > criterion
            return predicate.And(lambda); // Apply!
        }

       
        private static string GetDbFieldName(PropertyInfo propInfo)
        {
            var fieldMapAttribute = propInfo.GetCustomAttributes(typeof(DbFieldMapAttribute), false).FirstOrDefault();
            var dbFieldName = fieldMapAttribute != null ? ((DbFieldMapAttribute)fieldMapAttribute).Field : propInfo.Name;
            return dbFieldName;
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class DbFieldMapAttribute : Attribute
        {
            public string Field { get; set; }
            public DbFieldMapAttribute(string field)
            {
                Field = field;
            }
        }
    }
}
