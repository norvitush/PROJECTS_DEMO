using System;
using System.Collections;
using System.Linq;

namespace GoldenSoft.Column21.Core
{
    public class CardsSetBitData : IEnumerable
    {
        public CardsSet Value { get; private set; }
        public CardsSetBitData() => this.Value = CardsSet.None;
        public CardsSetBitData(params CardsSet[] values)
        {
            this.Value = CardsSet.None;
            foreach (var vl in values)
            {
                this.Value |= vl;
            }

        }
        public void Set(int val) => this.Value = (CardsSet)val;
        public void Add(CardsSet value) => this.Value |= value;
        public void Remove(CardsSet value) => this.Value ^= value;
        public bool Contains(CardsSet value) => (this.Value & value) == value;

        public int IntValue => (int)Value;
        public override string ToString() => this.Value.ToString("G");

        public string ToStringAsBits()
        {
            int t = (int)Value;
            var mas = System.BitConverter.GetBytes(t).Reverse();
            string outp = "";
            foreach (var bt in mas)
            {
                string ByteString = System.Convert.ToString(bt, 2).PadLeft(8, '0');
                outp += "[" + ByteString + "]";
            }
         
            return outp;
        }

        public System.Collections.Generic.IEnumerator<CardsSet> GetEnumerator()
        {
            Array arr = Enum.GetValues(typeof(CardsSet));

            foreach (CardsSet item in arr)
            {
                if (Contains(item) && item != CardsSet.None)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return GetEnumerator();
        }
    }
}
