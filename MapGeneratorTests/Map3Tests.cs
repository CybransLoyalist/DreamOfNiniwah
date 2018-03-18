using Assets.Scripts;
using NUnit.Framework;

namespace MapGeneratorTests
{
    [TestFixture]
    public class Map3Tests
    {
        private Map3 _sut;
        private IMapChunk[,] _chunks;
        private IMapChunk _chunk00;
        private IMapChunk _chunk01;
        private IMapChunk _chunk11;
        private IMapChunk _chunk10;

        const int xMapResolution = 4;
        private const int chunksXCount = 2;
        private const int chunksXSize = 2;

        [SetUp]
        public void SetUp()
        {
            _chunk00 = new MapChunk();
            _chunk01 = new MapChunk();
            _chunk11 = new MapChunk();
            _chunk10 = new MapChunk();
            _chunks = new IMapChunk[chunksXCount, chunksXCount];
            _chunks[0, 0] = _chunk00;
            _chunks[0, 1] = _chunk01;
            _chunks[1, 1] = _chunk11;
            _chunks[1, 0] = _chunk10;

            _sut = new Map3(
                xMapResolution,
                xMapResolution,
                _chunks,
                chunksXSize,
                chunksXSize);
        }

        [Test]
        public void GetChunksOfTile00_ShallGetChunk00()
        {
            var result = _sut.GetChunksOfTile(0, 0);
            Assert.AreEqual(0, result.Count);
            Assert.AreEqual(_chunk00, result[0]);
        }
    }
}