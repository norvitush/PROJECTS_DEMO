namespace VOrb
{
    public class BitOperations
    {
        public Restart_settings Value { get; private set; }
        public BitOperations() => this.Value = Restart_settings.None;
        public BitOperations(params Restart_settings[] values) {
            this.Value = Restart_settings.None;
            foreach (var vl in values)
            {
                this.Value |= vl;
            }
           
        }
        public void Set(int val) => this.Value = (Restart_settings)val;
        public void Add(Restart_settings value) => this.Value |= value;
        public void Remove(Restart_settings value) => this.Value ^= value;
        public bool Contains(Restart_settings value) => (this.Value & value) == value;
        public override string ToString() => this.Value.ToString("G");
    }
}