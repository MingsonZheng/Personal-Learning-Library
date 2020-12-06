using System.ComponentModel.DataAnnotations;

namespace HelloApi
{
    public class MyOption
    {
        public const string PETER = "Peter";

        public const string JACK = "Jack";

        public string Name { get; set; }

        [Range(1, 20)]
        public int Age { get; set; }
    }
}
