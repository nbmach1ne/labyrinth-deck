using Godot;
using System.Collections.Generic;

namespace LabyrinthDeck
{
    public class PriorityQueue<T> where T: Object, System.IComparable<T>
    {
        private List<T> _data;

        public int Count
        {
            get { return _data.Count; }
        }

        public PriorityQueue()
        {
            _data = new List<T>();
        }

        public void Put(T item)
        {
            _data.Add(item);

            int ci = _data.Count - 1;
            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (_data[ci].CompareTo(_data[pi]) >= 0)
                {
                    break;
                }

                T tmp = _data[ci];
                _data[ci] = _data[pi];
                _data[pi] = tmp;
                ci = pi;
            }
        }

        public T Get()
        {
            int li = _data.Count - 1;
            T frontItem = _data[0];
            _data[0] = _data[li];
            _data.RemoveAt(li);

            --li;
            int pi = 0;
            while (true)
            {
                int ci = (pi * 2) + 1;
                if (ci > li)
                {
                    break;
                }

                int rc = ci + 1;
                if (rc <= li && _data[rc].CompareTo(_data[ci]) < 0)
                {
                    ci = rc;
                }
                if (_data[pi].CompareTo(_data[ci]) <= 0)
                {
                    break;
                }

                T tmp = _data[pi];
                _data[pi] = _data[ci];
                _data[ci] = tmp;
                pi = ci;
            }

            return frontItem;
        }
    }
}