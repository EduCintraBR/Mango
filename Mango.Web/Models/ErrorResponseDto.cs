namespace Mango.Web.Models
{
    public class ErrorResponseDto
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Status { get; set; }
        public object? Errors { get; set; }
    }
}
