namespace EventManagerBackend.Models.JSON
{
    public class Answer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public IList<string>? Options { get; set; }
    }
}
