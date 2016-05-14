using Common.Contracts;
using Common.Utility;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests
{
    [TestFixture]
    public class BinarySerializerTests
    {
        private IFixture _fixture;
        [Test]
        public void TestSerialization()
        {
            _fixture = new Fixture();
            var deltas = _fixture.Create<Control>();
            var serializer = new BinarySerializer();
            var res = serializer.Deserialize(serializer.Serialize(deltas));

            res.ShouldBeEquivalentTo(deltas);
        }
    }
}
