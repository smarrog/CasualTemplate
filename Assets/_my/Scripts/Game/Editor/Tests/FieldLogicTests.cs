using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Smr.Common;

namespace Game.Tests {
    public class FieldLogicTests {
        private FieldLogic _fieldLogic;
        private IGiftLogic _giftLogic;
        private ISettings _settings;
        private ISaveData _saveData;
        private ISignalBus _signalBus;

        [SetUp]
        public void Init() {
            _giftLogic = Substitute.For<IGiftLogic>();
            var metaSettings = new MetaSettings {
                Field = new FieldSettings {
                    Levels = new List<LevelData> {
                        new() { Title = "1" },
                        new() { Title = "2" },
                        new() { Title = "3" },
                        new() { Title = "4" },
                        new() { Title = "5" },
                        new() { Title = "6" }
                    }
                }
            };
            _settings = Substitute.For<ISettings>();
            _settings.Meta.Returns(metaSettings);
            _saveData = Substitute.For<ISaveData>();
            _signalBus = Substitute.For<ISignalBus>();
            _fieldLogic = new FieldLogic(_giftLogic, _settings, _saveData, _signalBus);
        }

        [TestCase(1, 0)]
        [TestCase(4, 3)]
        [TestCase(6, 5)]
        public void GetLevelData_Simple(int level, int expectedIndex) {
            var levelData = _fieldLogic.GetLevelData(level);
            Assert.AreEqual(_settings.Meta.Field.Levels[expectedIndex], levelData);
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(7)]
        public void GetLevelData_InvalidLevel_ReturnsNull(int level) {
            Assert.IsNull(_fieldLogic.GetLevelData(level));
        }
    }
}