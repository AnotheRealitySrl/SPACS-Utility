namespace Virtuademy.SDK.Core.Utilities
{
    public class Field
    {
        public string name;

        public object value;

        public Field()
        {
        }

        public Field(string name, object value)
        {
            this.name = name;
            this.value = value;
        }
    }
}