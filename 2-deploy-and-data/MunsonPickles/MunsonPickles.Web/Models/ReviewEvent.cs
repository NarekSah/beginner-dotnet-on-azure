namespace MunsonPickles.Web.Models
{
    public class ReviewEvent
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public bool HasPhotos { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
