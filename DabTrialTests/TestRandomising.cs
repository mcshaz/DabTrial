using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DabTrial.Domain.Services;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using System.IO;
using Moq;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using DabTrial.Infrastructure.Utilities.Randomisation;

namespace DabTrialTests
{
    [TestClass]
    public class TestRandomising
    {
        [TestMethod]
        public void TestNewBlockProbIntervention()
        {
            var partService = new TrialParticipantService(new EmptyValidationShell());
            Console.WriteLine("probability of intervention:{0}",partService.NewBlockProbIntervention());
        }
        [TestMethod]
        public void TestRallocFromCurrentDb()
        {
            string conStr;
            List<TrialParticipant> participants;

            using (var db = new DataContext())
            {
                participants = db.TrialParticipants.Include(p => p.RespiratorySupportAtRandomisation).ToList();
                conStr = db.Database.Connection.ConnectionString;
            }
            int realCount = participants.Count;
            Dictionary<RandomParams, ResetableQueue<bool>> blocks = new Dictionary<RandomParams, ResetableQueue<bool>>(64);
            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT [StratID],[Rx]FROM [dabtrial_com_participantdata].[dbo].[Ralloc]", con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        byte currentStrata = 0;
                        var allStrata = AllParams();
                        ResetableQueue<bool> q=null;
                        while (reader.Read())
                        {
                            byte st = (byte)reader["StratID"];
                            if (currentStrata != st)
                            {
                                q=new ResetableQueue<bool>(250);
                                currentStrata = st;
                                blocks.Add(allStrata.Pop(), q);
                            }
                            q.Enqueue((bool)reader["Rx"]);
                        }
                    }
                }
                foreach (var p in participants)
                {
                    p.IsInterventionArm = blocks[new RandomParams(p)].Dequeue();
                }
                foreach (var v in blocks.Values)
                {
                    v.ResetPoint();
                }
                TestRandomisation(conStr, participants, RandomMethod.Ralloc, p =>
                    {
                        p.IsInterventionArm = blocks[new RandomParams(p)].Dequeue();
                    },
                    () => 
                    {
                        foreach (var v in blocks.Values)
                        {
                            v.Reset();
                        }
                    });
            }
        }
        [TestMethod]
        public void TestNormalisation()
        {
            var p = NextParticipant(new RandomAdaptor());
            using(var db = new DataContext())
            {
                var g = Normalisation.GetPInterventionUsingGScale(db.Database, Normalisation.GetTableName(typeof(TrialParticipant), db), 0.5,
                    Normalisation.SetArguments<TrialParticipant>(p, pa => pa.IsInterventionArm,
                    true,
                    new WeightArg<TrialParticipant> { Property = t => t.HasChronicLungDisease, Weight = 2 },
                    new WeightArg<TrialParticipant> { Property = t => t.HasCyanoticHeartDisease, Weight = 4 },
                    new WeightArg<TrialParticipant> { Property = t => t.RespSupportTypeId, Weight = 1 },
                    new WeightArg<TrialParticipant> { Property = t => t.StudyCentreId, Weight = 2 }));
                Console.WriteLine(g);
            }
        }
        [TestMethod]
        public void TestNormalisationBegg()
        {
            var p = NextParticipant(new RandomAdaptor());
            using (var db = new DataContext())
            {
                var g = Normalisation.BiasToInterventionBegg(db.Database, Normalisation.GetTableName(typeof(TrialParticipant), db),
                    Normalisation.SetArguments<TrialParticipant>(p, pa => pa.IsInterventionArm,
                    true,
                    new WeightArg<TrialParticipant> { Property = t => t.HasChronicLungDisease, Weight = 2 },
                    new WeightArg<TrialParticipant> { Property = t => t.HasCyanoticHeartDisease, Weight = 4 },
                    new WeightArg<TrialParticipant> { Property = t => t.RespSupportTypeId, Weight = 1 },
                    new WeightArg<TrialParticipant> { Property = t => t.StudyCentreId, Weight = 2 }));
                Console.WriteLine(g);
            }
        }
        [TestMethod]
        public void TestMovementFromCurrentDb()
        {
            var rnd = new RandomAdaptor();
            var mockSet = new Mock<DbSet<TrialParticipant>>();
            string conStr;
            List<TrialParticipant> participants;
            IQueryable<TrialParticipant> data;
            List<RespiratorySupportType> respSupport;
            using (var db = new DataContext())
            {
                participants = db.TrialParticipants.Include(p=>p.RespiratorySupportAtRandomisation).ToList();
                respSupport = db.RespiratorySupportTypes.OrderBy(r=>r.RespSupportTypeId).ToList();
                conStr = db.Database.Connection.ConnectionString;
            }

            data = participants.AsQueryable();

            mockSet.As<IQueryable<TrialParticipant>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<TrialParticipant>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<TrialParticipant>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<TrialParticipant>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<IDataContext>();
            mockContext.Setup(c => c.TrialParticipants).Returns(mockSet.Object); 

            using (var ps = new TrialParticipantService(new EmptyValidationShell(),mockContext.Object))
            {
                TestRandomisation(conStr, participants, RandomMethod.InUse,p =>
                    {
                        p.RespiratorySupportAtRandomisation = respSupport[p.RespSupportTypeId - 1];
                        ps.CreateAllocation(p, rnd);
                    });
                
            }
        }
        const int finalTrialSize = 306;
        static void TestRandomisation(string connectionString, List<TrialParticipant> participants, RandomMethod method, Action<TrialParticipant> createAllocation, Action onIteration=null)
        {
            int realCount = participants.Count;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT [Iteration],[ParticipantNo],[StudyCentreId],[RespSupportTypeId],[HasChronicLungDisease],[HasCyanoticHeartDisease]FROM [dbo].[Strata] ORDER BY [Iteration], [ParticipantNo]",con))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        short currentIteration = 1;
                        while (reader.Read())
                        {
                            short it = (short)reader["iteration"];
                            if (currentIteration != it)
                            {
                                UpdateSummary(participants, currentIteration, con, method);
                                participants.RemoveRange(realCount, finalTrialSize - realCount);
                                currentIteration = it;
                                if (onIteration != null) { onIteration(); }
                            }

                            TrialParticipant p = new TrialParticipant
                            {
                                StudyCentreId = (byte)reader["StudyCentreId"],
                                RespSupportTypeId = (byte)reader["RespSupportTypeId"], 
                                HasChronicLungDisease = (bool)reader["HasChronicLungDisease"],
                                HasCyanoticHeartDisease = (bool)reader["HasCyanoticHeartDisease"]
                            };

                            createAllocation(p);

                            participants.Add(p);
                        }
                        UpdateSummary(participants, currentIteration, con, method);
                    }
                }
            }
        }
        enum RandomMethod { InUse = 1, Ralloc =2 }
        static void UpdateSummary(List<TrialParticipant> participants, short currentIteration, SqlConnection con, RandomMethod method)
        {
            var tally = '(' + string.Join(") , (",
            (from pa in participants
             group pa by pa.IsInterventionArm into g
             select (((int)method).ToString() + ',' + currentIteration + ',' +
                 (g.Key ? '1' : '0') + ',' +
                 g.Count() + ',' +
                 g.Count(s => s.StudyCentreId == 1) + ',' +
                 g.Count(s => s.StudyCentreId == 2) + ',' +
                 g.Count(s => s.StudyCentreId == 3) + ',' +
                 g.Count(s => s.StudyCentreId == 4) + ',' +
                 g.Count(s => s.RespSupportTypeId == 2) + ',' +
                 g.Count(s => s.RespSupportTypeId == 3) + ',' +
                 g.Count(s => s.RespSupportTypeId == 4) + ',' +
                 g.Count(s => s.RespSupportTypeId == 5) + ',' +
                 g.Count(s => s.HasChronicLungDisease) + ',' +
                 g.Count(s => s.HasCyanoticHeartDisease)))) + ')';

            using (var insertTally = new SqlCommand("INSERT INTO [dabtrial_com_participantdata].[dbo].[Tally] ([RandomisationMethod],[Iteration],[IsInterventionArm],[Total],[StudySite1],[StudySite2],[StudySite3],[StudySite4],[RespSupport2],[RespSupport3],[Respsupport4],[RespSupport5],[HasChronicLungDisease],[HasCyanoticHeartDisease]) VALUES " + tally, con))
            {
                insertTally.ExecuteNonQuery();
            }
        }
        static TrialParticipant NextParticipant(RandomAdaptor rnd)
        {
            DateTime dummyDate = new DateTime(635594612978369071);
            TrialParticipant returnVar = new TrialParticipant
            {
                EnrollingClinicianId = 2, 
                WeeksGestationAtBirth=40, 
                Weight=4, 
                IcuAdmission = dummyDate, 
                LocalTimeRandomised = dummyDate,
                Dob=dummyDate, 
                HospitalId=new string('a',44)
            };
            double d = rnd.NextDouble();
            if (d<0.6667)
            {
                returnVar.StudyCentreId=1;
            }
            else if(d<0.8267)
            {
                returnVar.StudyCentreId=2;
            }
            else if (d<0.92)
            {
                returnVar.StudyCentreId=3;
            }
            else
            {
                returnVar.StudyCentreId=4;
            }

            returnVar.HasChronicLungDisease = rnd.NextDouble() > 0.92;
            returnVar.HasCyanoticHeartDisease = rnd.NextDouble() > 0.9867;

            d = rnd.NextDouble();
            if (d < 0.0133)
            {
                returnVar.RespSupportTypeId = 2;
            }
            else if (d < 0.6267)
            {
                returnVar.RespSupportTypeId = 3;
            }
            else if (d < 0.9333)
            {
                returnVar.RespSupportTypeId = 4;
            }
            else
            {
                returnVar.RespSupportTypeId = 5;
            }

            return returnVar;
        }
        static Stack<RandomParams> AllParams()
        {
            var returnVar = new Stack<RandomParams>(48);
            for (int c=0;c<=1;c++)
            {
                for (int l = 0; l <= 1; l++)
                {
                    for (byte s = 1; s <= 4; s++)
                    {
                        for (byte r = 1; r <= 3; r++)
                        {
                            returnVar.Push(new RandomParams(s, r, c>0, l>0));
                        }
                    }
                }
            }
            return returnVar;
        }
    }
    internal class RandomParams
    {
        public byte StudySiteId { get; private set; }
        public byte RespSupportStrata { get; private set; }
        public bool HasCyanoticHeart { get; private set; }
        public bool HasChronicLung { get; private set; }

        public RandomParams(TrialParticipant p) : this((byte)p.StudyCentreId, GetStrata(p.RespSupportTypeId), p.HasCyanoticHeartDisease, p.HasChronicLungDisease)
        {
        }

        static byte GetStrata(int respSupportTypeId)
        {
            switch (respSupportTypeId)
            {
                case 1:
                case 2:
                    return 1;
                case 3:
                case 4:
                    return 2;
                case 5:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public RandomParams(byte studySiteId, byte respSupportStrata, bool hasCyanoticHeartDiaseas, bool hasChronicLungDisease)
        {
            StudySiteId = studySiteId;
            RespSupportStrata = respSupportStrata;
            HasCyanoticHeart = hasCyanoticHeartDiaseas;
            HasChronicLung = hasChronicLungDisease;
            _hashCode = (HasCyanoticHeart?131072:0) | (HasChronicLung?65536:0) | StudySiteId << 8 | RespSupportStrata;
        }

        int _hashCode;
        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(RandomParams) && _hashCode == obj.GetHashCode();
        }
    }
}
