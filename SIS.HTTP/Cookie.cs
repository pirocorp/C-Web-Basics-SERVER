namespace SIS.HTTP
{
    public class Cookie
    {
        public Cookie(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; }

        public string Value { get; }

        public override string ToString()
        {
            return $"{this.Name}={this.Value}";
        }
    }
}
