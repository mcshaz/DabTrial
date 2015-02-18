using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace DabTrialTests
{
    [TestClass]
    public class ResetablQueueTest
    {
        [TestMethod]
        public void TestResetableQueue()
        {
            var a = new List<int>();

            var e = new ResetableQueue<int>(Enumerable.Range(1,4));

            Assert.AreEqual(1, e.Dequeue());
            e.ResetPoint();
            Assert.AreEqual(2, e.Dequeue());
            e.Reset();
            Assert.AreEqual(2, e.Dequeue());

            e.Enqueue(Enumerable.Range(5, 13));

            CollectionAssert.AreEquivalent(Enumerable.Range(3, 15).ToList(), e);

            Assert.AreEqual(3, e.Dequeue());

            ((ICollection<int>)e).Remove(3);
            ((ICollection<int>)e).Remove(2);
            ((ICollection<int>)e).Remove(7);

            CollectionAssert.AreEquivalent((new int[] {4,5,6}).Concat(Enumerable.Range(8,10)).ToList(), e);

            e.Enqueue(Enumerable.Range(18, 13));
            Assert.AreEqual(4, e.Dequeue());

            ((ICollection<int>)e).Remove(9);
            Assert.AreEqual(5, e.Dequeue());

            e.TrimExcess();

            Assert.AreEqual(25,e.Capacity);
            Assert.AreEqual(6, e.Dequeue());
        }
    }
}
