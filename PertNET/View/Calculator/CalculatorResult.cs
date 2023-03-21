namespace PertNET.View
{
    public class CalculatorResult
    {

        public CalculatorResult(int id, string content, decimal total)
        {
            this.Id = id;
            this.Content = content;
            this.Total = total;
        }

        public int Id { get; set; }

        public string Content { get; set; }

        public decimal Total { get; set; }
    }
}
