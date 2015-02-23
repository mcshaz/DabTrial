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
using MathNet.Numerics.LinearAlgebra;
using System.Text;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Collections;

namespace DabTrial.Infrastructure.Utilities.Randomisation
{
    public class Covariable
    {
        public Covariable()
        {
            Weight = 1;
        }
        public string Name { get; set; }
        public object CurrentValue { get; set; }
        public IEnumerable LevelsOf { get; set; }
        public double Weight { get; set; }
    }
    public class CovariableArgument<T>
    {
        public Expression<Func<T, object>> Property {get; set;}
        public double Weight {get; set;} //only required for Pocock
        public IEnumerable LevelsOf { get; set; } //only required for Faraggi
    }

    public class Factors
    {
        public string InterventionFieldName { get; set; }
        public object IsInterventionValue { get; set; }
        public Type MappedClass {get; set;}
        public IEnumerable<Covariable> Covariables { get; set; }
    }
    public class ArmData
    {
        public double Intervention { get; set; }
        public double Control { get; set; } 
    }
    public static class Normalisation
    {
        public static Factors SetArguments<T>(T obj, Expression<Func<T, object>> intervention, object isInterventionValue, params CovariableArgument<T>[] covariables) where T : class
        {
            return new Factors
            {
                Covariables = covariables.Select(w=>new Covariable {
                    Name = GetCorrectPropertyName<T>(w.Property).Member.Name,
                    Weight = w.Weight,
                    CurrentValue = w.Property.Compile()(obj),
                    LevelsOf = w.LevelsOf
                }).ToList(),
                InterventionFieldName = GetCorrectPropertyName<T>(intervention).Member.Name,
                IsInterventionValue = isInterventionValue,
                MappedClass = typeof(T)
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
        //public enum MinimisationMethod {  PocockAndSimon, BeggAndIglewicz, FaraggiAndReiser }
        static ArmData GetGPocock(Database db, string participantTable,Factors factors)
        {
            string sqlCount = string.Format(
                "SELECT SUM(CASE WHEN t.[{{0}}]=@factor AND t.[{0}]=@intervention THEN 1 ELSE 0 END) " +
                "- SUM(CASE WHEN t.[{{0}}]=@factor AND t.[{0}]<>@intervention THEN 1 ELSE 0 END) " +
                "FROM {1} AS t" , factors.InterventionFieldName,participantTable);
            var interv = (ICloneable)new SqlParameter("@intervention", factors.IsInterventionValue);
            var returnVar = new ArmData();
            foreach (var v in factors.Covariables)
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
                "FROM {1} AS t", factors.InterventionFieldName, participantTable);
            var interv = (ICloneable)new SqlParameter("@intervention", factors.IsInterventionValue);
            var sum = factors.Covariables.Select(v => db.SqlQuery<int>(string.Format(sqlCount, v.Name), new SqlParameter("@factor", v.CurrentValue), interv.Clone()).FirstOrDefault())
                .Sum();
            if (sum == 0) { return null; }
            return sum < 0;
        }

        public static bool? BiasToInterventionFaraggi(DbContext db, Factors factors)
        {
            return BiasToInterventionFaraggi((SqlConnection)db.Database.Connection, GetTableName(factors.MappedClass, db), factors);
        }
        public static bool? BiasToInterventionFaraggi(SqlConnection con, string participantTable,Factors factors)
        {
            StringBuilder selectSql = new StringBuilder("SELECT");
            Queue<object> args = new Queue<object>();
            Queue<float> newParticipantVals = new Queue<float>();
            foreach (var w in factors.Covariables)
            {
                foreach (var i in w.LevelsOf)
                {
                    selectSql.AppendFormat(" CASE WHEN t.[{0}] = @p{1} THEN 1 ELSE -1 END,", w.Name, args.Count);
                    args.Enqueue(w.CurrentValue);
                    newParticipantVals.Enqueue(i.Equals(w.CurrentValue)?1:-1);
                }
            }
            selectSql.Length-=1;
            selectSql.AppendFormat(" FROM {0} AS t", participantTable);
            var covariates = con.SelectMatrix<float>(selectSql.ToString(),args.ToArray());
            var newPatientCoariates = Matrix<float>.Build.Dense(1, newParticipantVals.Count, newParticipantVals.ToArray());
            var diag = Matrix<float>.Build.DiagonalOfDiagonalVector(((newParticipantVals.Count + (covariates.TransposeAndMultiply(newPatientCoariates) - 1)) / (2 * newParticipantVals.Count)).Column(0));

            var allocations = con.SelectMatrix<float>(string.Format("SELECT CASE WHEN t.[{0}] = @intervention THEN 1 ELSE -1 END FROM {1} AS t", factors.InterventionFieldName, participantTable), new SqlParameter("@intervention", factors.IsInterventionValue));
            var zn = ((allocations.TransposeThisAndMultiply(diag)*covariates).TransposeAndMultiply(newPatientCoariates))[0,0];
            Console.WriteLine(zn);
            if (zn == 0) { return null; }
            return zn < 0;
        }

        static object[] ObjectToParameter(object[] sqlParams)
        {
            object[] returnVar = null;
            for (int i = 0; i < sqlParams.Length; i++)
            {
                if (i == 0)
                {
                    if (sqlParams[i] is SqlParameter)
                    {
                        return sqlParams;
                    }
                    else
                    {
                        returnVar = new object[sqlParams.Length];
                    }
                }
                returnVar[i] = new SqlParameter('p' + i.ToString(), sqlParams[i]);
            }
            return returnVar;
        }
        static Matrix<T> SelectMatrix<T> (this SqlConnection con, string command, params object[] sqlParams) where T : struct, global::System.IEquatable<T>, global::System.IFormattable
        {
            var res = SelectColumnMajor<T>(con, command, sqlParams);
            return Matrix<T>.Build.Dense(res.Rows, res.Cols, res.Matrix);
        }

        static ColumnMajor<T> SelectColumnMajor<T>(this SqlConnection con, string command, params object[] sqlParams) where T : struct, global::System.IEquatable<T>, global::System.IFormattable
        {
            Queue<T>[] returnCols;
            var typeofT = typeof(T);
            con.Open();
            using (var cmd = new SqlCommand(command, con))
            {
                cmd.Parameters.AddRange(ObjectToParameter(sqlParams));
                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.Read())
                    {
                        returnCols = new Queue<T>[reader.FieldCount];
                        for (int i = 0; i < returnCols.Length; i++)
                        {
                            returnCols[i] = new Queue<T>();
                        }
                        do
                        {
                            for (int i = 0; i < returnCols.Length; i++)
                            {
                                returnCols[i].Enqueue((T)System.Convert.ChangeType(reader[0], typeofT));
                            }
                        } while (reader.Read());
                    }
                    else
                    {
                        return ColumnMajor<T>.EmptyColumn();
                    }
                }
            }

            var returnVar = new ColumnMajor<T>(returnCols.First().Count, returnCols.Length);
            for (int c = 0; c < returnVar.Cols;c++ )
            {
                returnCols[c].CopyTo(returnVar.Matrix, returnVar.Rows * c);
            }
            return returnVar;
        }

        static IEnumerable<T[]> ReaderAllContents<T>(SqlDataReader reader)
        {
            var ttype = typeof(T);
            while (reader.Read())
            {
                T[] row = new T[reader.FieldCount];
                for (int i = 0; i < row.Length;i++ )
                {
                    row[i] = (T)System.Convert.ChangeType(reader[i], ttype);
                }
                yield return row;
            }
        }

        /*
        public DataSet GetResultReport()
        {
            DataSet retVal = new DataSet();
            EntityConnection entityConn = (EntityConnection)context.Connection;
            SqlConnection sqlConn = (SqlConnection)entityConn.StoreConnection;
            SqlCommand cmdReport = new SqlCommand([YourSpName], sqlConn);
            SqlDataAdapter daReport = new SqlDataAdapter(cmdReport);
            using (cmdReport)
            {
                SqlParameter questionIdPrm = new SqlParameter("QuestionId", questionId);
                cmdReport.CommandType = CommandType.StoredProcedure;
                cmdReport.Parameters.Add(questionIdPrm);                
                daReport.Fill(retVal);
            }                  

            return retVal;
        }
        */
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

    public class ColumnMajor<T>
    {
        public ColumnMajor(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Matrix = new T[rows*cols];
        }

        public T[] Matrix { get; set; }
        public int Rows {get; set;}
        public int Cols { get; set; }

        public static ColumnMajor<T> EmptyColumn()
        {
            return new ColumnMajor<T> (0, 0);
        }
    }
}