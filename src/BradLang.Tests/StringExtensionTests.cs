using Xunit;

namespace BradLang.Tests
{
    public class StringExtensionTests
    {
        [Fact]
        public void StringExtensions_Unintent_RemovesLeadingIndent()
        {
            var text = @"
                {
                    var a = 10

                    if (a < 100)
                    {
                        a = 5
                    }
                }
            ";

            var expected = 
@"{
    var a = 10

    if (a < 100)
    {
        a = 5
    }
}";

            var result = text.Unindent();

            Assert.Equal(expected, result);
        }
    }
}
