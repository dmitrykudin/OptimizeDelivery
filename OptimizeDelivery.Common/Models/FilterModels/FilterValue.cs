namespace Common.Models.FilterModels
{
    public class FilterValue<T>
    {
        public FilterValue()
        {
            IsSet = false;
        }

        public FilterValue(T value)
        {
            Value = value;
            IsSet = true;
        }

        public bool IsSet { get; set; }

        public T Value { get; set; }
    }
}