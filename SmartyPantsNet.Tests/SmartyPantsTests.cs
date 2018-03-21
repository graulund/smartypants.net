using Xunit;

namespace SmartyPantsNet.Tests
{
    public class SmartyPantsTests
    {
        readonly SmartyPants defaultSmartyPants = new SmartyPants();

        [Fact]
        public void NothingTest()
        {
            Assert.Equal(
                "'Ice monkeys' throw \"space rocks\" -- when in space...",
                new SmartyPants(SmartyPants.ATTR_DO_NOTHING).Educate(
                    "'Ice monkeys' throw \"space rocks\" -- when in space..."
                )
            );
        }

        [Fact]
        public void SingleQuoteTest()
        {
            Assert.Equal(
                "I’m from the ’80s",
                defaultSmartyPants.Educate(
                    "I'm from the '80s"
                )
            );
        }

        [Fact]
        public void SingleQuotesTest()
        {
            Assert.Equal(
                "‘Magic’, said Steven ‘Rock Hard’ Jones as he booted up his ‘Commodore 64’",
                defaultSmartyPants.Educate(
                    "'Magic', said Steven 'Rock Hard' Jones as he booted up his 'Commodore 64'"
                )
            );
        }

        [Fact]
        public void DoubleQuotesTest()
        {
            Assert.Equal(
                "“Magic”, said Steven “Rock Hard” Jones as he booted up his “Commodore 64”",
                defaultSmartyPants.Educate(
                    "\"Magic\", said Steven \"Rock Hard\" Jones as he booted up his \"Commodore 64\""
                )
            );
        }

        [Fact]
        public void SingleBacktickQuotesTest()
        {
            Assert.Equal(
                "‘Magic’, said Steven ‘Rock Hard’ Jones as he booted up his ‘Commodore 64’",
                new SmartyPants("B").Educate(
                    "`Magic', said Steven `Rock Hard' Jones as he booted up his `Commodore 64'"
                )
            );
        }

        [Fact]
        public void DoubleBacktickQuotesTest()
        {
            Assert.Equal(
                "“Magic”, said Steven “Rock Hard” Jones as he booted up his “Commodore 64”",
                defaultSmartyPants.Educate(
                    "``Magic'', said Steven ``Rock Hard'' Jones as he booted up his ``Commodore 64''"
                )
            );
        }

        [Fact]
        public void NestedQuotesTest()
        {
            Assert.Equal(
                "He said, “‘Quoted’ words in a larger quote.”",
                defaultSmartyPants.Educate(
                    "He said, \"'Quoted' words in a larger quote.\""
                )
            );
        }

        [Fact]
        public void EmDashTest()
        {
            Assert.Equal(
                "Now — and then",
                defaultSmartyPants.Educate(
                    "Now -- and then"
                )
            );
        }

        [Fact]
        public void ShortEnDashTest()
        {
            Assert.Equal(
                "Now — and then. A story of the years 1980–2000.",
                new SmartyPants(SmartyPants.ATTR_LONG_EM_DASH_SHORT_EN).Educate(
                    "Now --- and then. A story of the years 1980--2000."
                )
            );
        }

        [Fact]
        public void LongEnDashTest()
        {
            Assert.Equal(
                "Now — and then. A story of the years 1980–2000.",
                new SmartyPants(SmartyPants.ATTR_SHORT_EM_DASH_LONG_EN).Educate(
                    "Now -- and then. A story of the years 1980---2000."
                )
            );
        }

        [Fact]
        public void EllipsisTest()
        {
            Assert.Equal(
                "I love kids… is that weird to say?",
                defaultSmartyPants.Educate(
                    "I love kids... is that weird to say?"
                )
            );
        }

        [Fact]
        public void EscapedQuotesTest()
        {
            Assert.Equal(
                "Straight 'quotes' are not ‘cool’. Say what? Straight \"quotes\" are not “cool”!",
                defaultSmartyPants.Educate(
                    "Straight \\'quotes\\' are not 'cool'. Say what? Straight \\\"quotes\\\" are not \"cool\"!"
                )
            );
        }

        [Fact]
        public void ManualSettingsTest()
        {
            Assert.Equal(
                "Now — and then. A ‘sublime’ story of the years 1980–2000…",
                new SmartyPants("qei").Educate(
                    "Now -- and then. A 'sublime' story of the years 1980---2000..."
                )
            );
        }

        [Fact]
        public void StupefyTest()
        {
            Assert.Equal(
                "He said, \"'stupify' me\" -- or did he...?",
                new SmartyPants(SmartyPants.ATTR_STUPEFY).Educate(
                    "He said, “‘stupify’ me” — or did he…?"
                )
            );
        }

        [Fact]
        public void UnicodeTest()
        {
            Assert.Equal(
                "ÆÆÆ “øøø” ååå — çecil",
                defaultSmartyPants.Educate(
                    "ÆÆÆ \"øøø\" ååå -- çecil"
                )
            );
            Assert.Equal(
                "ドラゴン “龍” 용 — ঘুড়ি বিশেষ",
                defaultSmartyPants.Educate(
                    "ドラゴン \"龍\" 용 -- ঘুড়ি বিশেষ"
                )
            );
        }
    }
}
