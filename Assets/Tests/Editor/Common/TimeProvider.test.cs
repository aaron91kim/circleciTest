using System;
using NUnit.Framework;
using XGolf.Game.Component;

namespace XGolf.Tests.Module
{
    [TestFixture]
    public class TimeProviderTests
    {
        private TimeProvider _timeProvider;
        [SetUp]
        public void SetUp()
        {
            _timeProvider = new TimeProvider();
        }

        #region TryParse
        [Test, Description("parse formatted time string to UTC seconds")]
        public void TryParse()
        {
            bool hasParsed = _timeProvider.TryParse("2023-08-26", out var actual);
            Assert.AreEqual(1693008000, actual);
            Assert.True(hasParsed);
        }
        #endregion

        #region IsExpired
        [Test, Description("check the time is expired")]
        public void IsExpired()
        {
            var actual = _timeProvider.IsExpired(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 60 * 5);
            Assert.True(actual);
        }
        #endregion
    }
}