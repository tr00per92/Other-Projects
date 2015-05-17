namespace AbcSlider.UnitTests
{
    using AbcControls;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class AbcSliderTests
    {
        private const double DefaultMinimum = -100;
        private const double DefaultValue = 0;
        private const double DefaultMaximum = 100;
        private AbcSlider slider;

        [TestInitialize]
        public void Initialize()
        {
            this.slider = new AbcSlider
            {
                Minimum = DefaultMinimum,
                Value = DefaultValue,
                Maximum = DefaultMaximum
            };
        }

        [TestMethod]
        public void TestInitializationValues()
        {
            Assert.AreEqual(DefaultMinimum, this.slider.Minimum);
            Assert.AreEqual(DefaultValue, this.slider.Value);
            Assert.AreEqual(DefaultMaximum, this.slider.Maximum);
        }

        [TestMethod]
        public void TestSetValidValueShouldChangeValue()
        {
            const double newValue = 10;
            this.slider.Value = newValue;
            Assert.AreEqual(newValue, this.slider.Value);
        }

        [TestMethod]
        public void TestSetValueMoreThanMaximumShouldChangeValueToMaximum()
        {
            this.slider.Value = this.slider.Maximum + 1;
            Assert.AreEqual(this.slider.Maximum, this.slider.Value);
        }

        [TestMethod]
        public void TestSetValueLessThanMinimumShouldChangeValueToMinimum()
        {
            this.slider.Value = this.slider.Minimum - 1;
            Assert.AreEqual(this.slider.Minimum, this.slider.Value);
        }

        [TestMethod]
        public void TestSetValidMinimumShouldChangeMinimum()
        {
            var newMinimum = this.slider.Maximum - 50;
            this.slider.Minimum = newMinimum;
            Assert.AreEqual(newMinimum, this.slider.Minimum);
        }

        [TestMethod]
        public void TestSetMinimumMoreThanMaximumShouldChangeMinimumToMaximumMinusOne()
        {
            var newMinimum = this.slider.Maximum + 50;
            this.slider.Minimum = newMinimum;
            Assert.AreEqual(this.slider.Maximum - 1, this.slider.Minimum);
        }

        [TestMethod]
        public void TestSetMinimumMoreThanValueShouldChangeValueToMinimum()
        {
            var newMinimum = this.slider.Value + 5;
            this.slider.Minimum = newMinimum;
            Assert.AreEqual(newMinimum, this.slider.Minimum);
            Assert.AreEqual(newMinimum, this.slider.Value);
        }

        [TestMethod]
        public void TestSetValidMaximumShouldChangeMaximum()
        {
            const double newMaximum = 255;
            this.slider.Maximum = newMaximum;
            Assert.AreEqual(newMaximum, this.slider.Maximum);
        }

        [TestMethod]
        public void TestSetMaximumLessThanMinimumShouldChangeMaximumToMinimumPlusOne()
        {
            var newMaximum = this.slider.Minimum - 22;
            this.slider.Maximum = newMaximum;
            Assert.AreEqual(this.slider.Minimum + 1, this.slider.Maximum);
        }

        [TestMethod]
        public void TestSetMaximumLessThanValueShouldChangeValueToMaximum()
        {
            var newMaximum = this.slider.Value - 11;
            this.slider.Maximum = newMaximum;
            Assert.AreEqual(newMaximum, this.slider.Maximum);
            Assert.AreEqual(newMaximum, this.slider.Value);
        }
    }
}
