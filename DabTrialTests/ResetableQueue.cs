using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DabTrialTests
{
    public class ResetableQueue<T> : IEnumerable, IEnumerable<T>, ICollection<T>, ICollection
    {
        T[] _queue;
        int _currentIndex;
        int _totalCount;
        const int _defaultCapacity=0;
        const int _defaultFirstResize = 4;
        public ResetableQueue() : this(_defaultCapacity)
        {
        }

        public ResetableQueue(int capacity)
        {
            _queue = new T[capacity];
        }

        public ResetableQueue(IEnumerable<T> list) : this()
        {
            Enqueue(list);
        }

        public int Count
        {
            get { return _totalCount - _currentIndex; }
        }

        public int Capacity
        {
            get { return _queue.Length; }
        }

        public void Enqueue(T item)
        {
            if (_totalCount >= _queue.Length)
            {

                if (_queue.Length == 0) 
                {
                    Array.Resize(ref _queue, _defaultFirstResize); 
                }
                else
                {
                    Array.Resize(ref _queue, _queue.Length * 2);
                }
            }
            _queue[_totalCount++] = item;
        }

        public void Enqueue(IEnumerable<T> items)
        {
            var c = items as ICollection<T>;
            if (c==null)
            {
                foreach (T i in items) { Enqueue(i); }
            }
            else
            {
                int index = _totalCount;
                _totalCount += c.Count;
                if (_totalCount > _queue.Length)
                {
                    Array.Resize(ref _queue, NearestPower2(_totalCount));
                }
                c.CopyTo(_queue, index);
            }
        }

        static int NearestPower2(int count)
        {
            return (int)Math.Pow(2, Math.Ceiling(Math.Log(count, 2)));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            CopyTo((Array)array, arrayIndex);
        }

        public void CopyTo(Array array, int arrayIndex)
        {
            Array.Copy(_queue, _currentIndex, array, arrayIndex, Count);
        }

        public void ResetPoint()
        {
            _totalCount = Count;
            var newQueue = new T[NearestPower2(_totalCount)];
            Array.Copy(_queue, _currentIndex, newQueue, 0, _totalCount);
            _queue = newQueue;
            _currentIndex = 0;
        }

        public void Reset()
        {
            _currentIndex = 0;
        }

        public T Dequeue()
        {
            if (_currentIndex >= _queue.Length) 
            { 
                throw new InvalidOperationException("ResetableQueue dequed more items than have been enqued in collection"); 
            }
            return _queue[_currentIndex++];
        }

        public T Peek()
        {
            if (_currentIndex >= _queue.Length)
            {
                throw new InvalidOperationException("ResetableQueue peek with no items remaining in collection");
            }
            return _queue[_currentIndex];
        }

        IEnumerable<T> CreateEnumerator()
        {
            for (int i=_currentIndex;i<_totalCount;i++)
            {
                yield return _queue[i];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return CreateEnumerator().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return CreateEnumerator().GetEnumerator();
        }

        public bool IsSynchronized { get { return false; } }

        public bool IsReadOnly { get { return false; } }

        public object SyncRoot { get { return _queue.SyncRoot; } }

        void ICollection<T>.Add(T item)
        {
            Enqueue(item);
        }

        public void Clear()
        {
            Array.Clear(_queue,0,_queue.Length);
        }

        public bool Contains(T item)
        {
            return Array.IndexOf(_queue, item) >= 0;
        }


        //
        // Summary:
        //     Sets the capacity to the actual number of elements in the System.Collections.Generic.Queue<T>,
        //     if that number is less than 90 percent of current capacity.
        public void TrimExcess()
        {
            if (Count < 0.9*_queue.Length)
            {
                var newArray = new T[_totalCount];
                Array.Copy(_queue, 0, newArray,0,newArray.Length);
                _queue = newArray;
            }
        }

        public T[] ToArray()
        {
            var returnVar = new T[Count];
            CopyTo(returnVar, 0);
            return returnVar;
            
        }
        /// <summary>
        /// purely to implement ICollection<T>
        /// not performance tested
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool ICollection<T>.Remove(T item)
        {
            int index = Array.IndexOf(_queue, item);
            if (index < 0) { return false; }
            var tmpqueue = new T[_queue.Length];
            Array.Copy(_queue, 0, tmpqueue, 0,index);
            Array.Copy(_queue, index + 1, tmpqueue, index, _totalCount - index - 1);
            _queue = tmpqueue;
            --_totalCount;
            if (index <= _currentIndex) { --_currentIndex; }
            return true;
        }
    }
}
