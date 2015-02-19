using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace DabTrial.Infrastructure.Utilities.Randomisation
{
    public class VariableWeight
    {
        public VariableWeight()
        {
            Weight = 1;
        }
        public string Name { get; set; }
        public double Weight { get; set; }
        public object CurrentValue { get; set; }
    }
    public class WeightArg<T>
    {
        public Expression<Func<T, object>> Property {get; set;}
        public double Weight {get; set;}
    }
    public class Factors
    {
        public string InterventionFieldName { get; set; }
        public object IsInterventionValue { get; set; }
        public IEnumerable<VariableWeight> Weights { get; set; }
    }
    public class ArmData
    {
        public double Intervention { get; set; }
        public double Control { get; set; } 
    }
    public static class Normalisation
    {
        public static Factors SetArguments<T>(T obj, Expression<Func<T, object>> intervention, object isInterventionValue, params WeightArg<T>[] weights) where T : class
        {
            return new Factors
            {
                Weights = weights.Select(w=>new VariableWeight {
                    Name = GetCorrectPropertyName<T>(w.Property).Member.Name,
                    Weight = w.Weight,
                    CurrentValue = w.Property.Compile()(obj)
                }).ToList(),
                InterventionFieldName = GetCorrectPropertyName<T>(intervention).Member.Name,
                IsInterventionValue = isInterventionValue
            };
        }
        //http://stackoverflow.com/questions/12420466/unable-to-cast-object-of-type-system-linq-expressions-unaryexpression-to-type
        static MemberExpression GetCorrectPropertyName<T>(Expression<Func<T, object>> expression)
        {
            return (expression.Body as MemberExpression) ?? (MemberExpression)((UnaryExpression)expression.Body).Operand;
        }
        public static double GetPInterventionUsingGScale(Database db, string participantTable,double t,Factors factors)
        {
            if (t>1 || t<=0)
            {
                throw new ArgumentOutOfRangeException("t must be > 0 and <= 1");
            }
            var g = GetGPocock(db, participantTable, factors);
            return (1-(t*g.Intervention/(g.Intervention+g.Control)))/(2 - t);
        }
        //would suggest 5/6
        public static bool? BiasToInterventionMinG(Database db, string participantTable, Factors factors)
        {
            var g = GetGPocock(db, participantTable, factors);
            if (g.Intervention==g.Control)
            {
                return null;
            }
            return g.Intervention < g.Control;
        }
        //public enum MinimisationMethod {  PocockAndSimon, BeggAndIglewicz }
        static ArmData GetGPocock(Database db, string participantTable,Factors factors)
        {
            string sqlCount = string.Format(
                "SELECT SUM(CASE WHEN t.[{{0}}]=@factor AND t.[{0}]=@intervention THEN 1 ELSE 0 END) " +
                "- SUM(CASE WHEN t.[{{0}}]=@factor AND t.[{0}]<>@intervention THEN 1 ELSE 0 END) " +
                "FROM {1} as t" , factors.InterventionFieldName,participantTable);
            var interv = (ICloneable)new SqlParameter("@intervention", factors.IsInterventionValue);
            var returnVar = new ArmData();
            foreach (var v in factors.Weights)
            {
                var interventionDif = db.SqlQuery<int>(string.Format(sqlCount, v.Name), new SqlParameter("@factor",v.CurrentValue), interv.Clone()).FirstOrDefault(); // when positive, more interventions than controls
                returnVar.Intervention += Math.Abs(interventionDif+1)*v.Weight;
                returnVar.Control += Math.Abs(interventionDif-1)*v.Weight;
            }
            return returnVar;
        }

        public static bool? BiasToInterventionBegg(Database db, string participantTable,Factors factors)
        {
            string sqlCount = string.Format(
                "SELECT SUM(CASE WHEN t.[{{0}}]<>@factor AND t.[{0}]<>@intervention THEN 1 ELSE 0 END) " +
                "- SUM(CASE WHEN t.[{{0}}]<>@factor AND t.[{0}]=@intervention THEN 1 ELSE 0 END) " +
                "- SUM(CASE WHEN t.[{{0}}]=@factor AND t.[{0}]<>@intervention THEN 1 ELSE 0 END) " +
                "+ SUM(CASE WHEN t.[{{0}}]=@factor AND t.[{0}]=@intervention THEN 1 ELSE 0 END) " +
                "FROM {1} as t", factors.InterventionFieldName, participantTable);
            var interv = (ICloneable)new SqlParameter("@intervention", factors.IsInterventionValue);
            var sum = factors.Weights.Select(v => db.SqlQuery<int>(string.Format(sqlCount, v.Name), new SqlParameter("@factor", v.CurrentValue), interv.Clone()).FirstOrDefault())
                .Sum();
            if (sum == 0) { return null; }
            return sum < 0;
        }

        //http://romiller.com/2014/04/08/ef6-1-mapping-between-types-tables/
        public static string GetTableName(Type type, DbContext context)
        {
            var metadata = ((IObjectContextAdapter)context).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }
    }
}