namespace VendingMachineCore
{
    public interface ICashDenomination
    {
        /// <summary>
        /// Friendly name representation
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Value of the denomination in pence
        /// </summary>
        int Value { get; }
    }

    internal class CashDenomination : ICashDenomination
    {
        public CashDenomination(string name, int value)
        {
            this.FriendlyName = name;
            this.Value = value;
        }

        public string FriendlyName { get; private set; }

        public int Value { get; private set; }

        public override bool Equals(object obj)
        {
            CashDenomination other = obj as CashDenomination;
            if (obj == null) return false;
            return this.FriendlyName == other.FriendlyName;
        }
        public override int GetHashCode()
        {
            return FriendlyName.GetHashCode();
        }

    }
}
