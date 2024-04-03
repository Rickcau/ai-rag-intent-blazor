namespace BlazorChat.Models
{
    public class ChatItemBase
    {
        private string? _content;
        private string? _htmlcontent;
        public string? Content
        {
            get { return _content; }
            set
            {
                _content = value;
                _htmlcontent = ConvertToHtml(value);
            }
        }

        public string? HtmlContent { get { return _htmlcontent;  } }

        private string? ConvertToHtml(string text)
        {
            // Replace newline characters with HTML line breaks
            return text?.Replace("\r\n", "<br>").Replace("\n", "<br>");
        }
    }
}
