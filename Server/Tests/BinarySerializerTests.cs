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

        [SetUp]
        public void SetUp()
        {
            _fixture = new Fixture();
        }

        [Test]
        public void TestImageRequestSerialization() => TestSerialization<ImageRequest, Request>();

        [Test]
        public void TestScoreRequestSerialization() => TestSerialization<ScoreRequest, Request>();

        [Test]
        public void TestResponseSerialization() => TestSerialization<Response, Response>();


        private void TestSerialization<TEntity, TSerializerEntity>() where TEntity : TSerializerEntity
        {
            var entity = _fixture.Create<TEntity>();
            var serializer =(ISerializer<TSerializerEntity>) new BinarySerializer();
            var res = serializer.Deserialize(serializer.Serialize(entity));

            res.ShouldBeEquivalentTo(entity);
        }
    }
}
