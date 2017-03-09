using NUnit.Framework;

namespace SmartyPantsNet.Tests
{
    [TestFixture]
    public class SmartyPantsTests
    {
        readonly SmartyPants defaultSmartyPants = new SmartyPants();

        [Test]
        public void NothingTest()
        {
            Assert.AreEqual(
                "'Ice monkeys' throw \"space rocks\" -- when in space...",
                new SmartyPants(SmartyPants.ATTR_DO_NOTHING).Educate(
                    "'Ice monkeys' throw \"space rocks\" -- when in space..."
                )
            );
        }

        [Test]
        public void SingleQuoteTest()
        {
            Assert.AreEqual(
                "I’m from the ’80s",
                defaultSmartyPants.Educate(
                    "I'm from the '80s"
                )
            );
        }

        [Test]
        public void SingleQuotesTest()
        {
            Assert.AreEqual(
                "‘Magic’, said Steven ‘Rock Hard’ Jones as he booted up his ‘Commodore 64’",
                defaultSmartyPants.Educate(
                    "'Magic', said Steven 'Rock Hard' Jones as he booted up his 'Commodore 64'"
                )
            );
        }

        [Test]
        public void DoubleQuotesTest()
        {
            Assert.AreEqual(
                "“Magic”, said Steven “Rock Hard” Jones as he booted up his “Commodore 64”",
                defaultSmartyPants.Educate(
                    "\"Magic\", said Steven \"Rock Hard\" Jones as he booted up his \"Commodore 64\""
                )
            );
        }

        [Test]
        public void SingleBacktickQuotesTest()
        {
            Assert.AreEqual(
                "‘Magic’, said Steven ‘Rock Hard’ Jones as he booted up his ‘Commodore 64’",
                new SmartyPants("B").Educate(
                    "`Magic', said Steven `Rock Hard' Jones as he booted up his `Commodore 64'"
                )
            );
        }

        [Test]
        public void DoubleBacktickQuotesTest()
        {
            Assert.AreEqual(
                "“Magic”, said Steven “Rock Hard” Jones as he booted up his “Commodore 64”",
                defaultSmartyPants.Educate(
                    "``Magic'', said Steven ``Rock Hard'' Jones as he booted up his ``Commodore 64''"
                )
            );
        }

        [Test]
        public void NestedQuotesTest()
        {
            Assert.AreEqual(
                "He said, “‘Quoted’ words in a larger quote.”",
                defaultSmartyPants.Educate(
                    "He said, \"'Quoted' words in a larger quote.\""
                )
            );
        }

        [Test]
        public void EmDashTest()
        {
            Assert.AreEqual(
                "Now — and then",
                defaultSmartyPants.Educate(
                    "Now -- and then"
                )
            );
        }

        [Test]
        public void ShortEnDashTest()
        {
            Assert.AreEqual(
                "Now — and then. A story of the years 1980–2000.",
                new SmartyPants(SmartyPants.ATTR_LONG_EM_DASH_SHORT_EN).Educate(
                    "Now --- and then. A story of the years 1980--2000."
                )
            );
        }

        [Test]
        public void LongEnDashTest()
        {
            Assert.AreEqual(
                "Now — and then. A story of the years 1980–2000.",
                new SmartyPants(SmartyPants.ATTR_SHORT_EM_DASH_LONG_EN).Educate(
                    "Now -- and then. A story of the years 1980---2000."
                )
            );
        }

        [Test]
        public void EllipsisTest()
        {
            Assert.AreEqual(
                "I love kids… is that weird to say?",
                defaultSmartyPants.Educate(
                    "I love kids... is that weird to say?"
                )
            );
        }

        [Test]
        public void EscapedQuotesTest()
        {
            Assert.AreEqual(
                "Straight 'quotes' are not ‘cool’. Say what? Straight \"quotes\" are not “cool”!",
                defaultSmartyPants.Educate(
                    "Straight \\'quotes\\' are not 'cool'. Say what? Straight \\\"quotes\\\" are not \"cool\"!"
                )
            );
        }

        [Test]
        public void ManualSettingsTest()
        {
            Assert.AreEqual(
                "Now — and then. A ‘sublime’ story of the years 1980–2000…",
                new SmartyPants("qei").Educate(
                    "Now -- and then. A 'sublime' story of the years 1980---2000..."
                )
            );
        }

        [Test]
        public void StupefyTest()
        {
            Assert.AreEqual(
                "He said, \"'stupify' me\" -- or did he...?",
                new SmartyPants(SmartyPants.ATTR_STUPEFY).Educate(
                    "He said, “‘stupify’ me” — or did he…?"
                )
            );
        }

        [Test]
        public void UnicodeTest()
        {
            Assert.AreEqual(
                "ÆÆÆ “øøø” ååå — çecil",
                defaultSmartyPants.Educate(
                    "ÆÆÆ \"øøø\" ååå -- çecil"
                )
            );
            Assert.AreEqual(
                "ドラゴン “龍” 용 — ঘুড়ি বিশেষ",
                defaultSmartyPants.Educate(
                    "ドラゴン \"龍\" 용 -- ঘুড়ি বিশেষ"
                )
            );
        }
    }
}
