using NUnit.Framework;
using TrafficLight.Lib;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using FluentAssertions;

namespace TrafficLight.Lib.Tests
{
    [TestFixture]
    public class TrafficLightTests
    {
        private TrafficLight _trafficLight;
        private Mock<Battery> _mock;

        [OneTimeSetUp]
        public void Init()
        {
            _mock = new Mock<Battery>();
            _trafficLight = new TrafficLight(_mock.Object);
        }
        [SetUp]
        public void Start()
        {
            _mock.Object.Energy = 100;
            _trafficLight.ChangeBattery(_mock.Object);
        }

        [TestCase(WorkMode.Hand, TrafficColor.Yellow, 50)]
        public void GetStatusTest(WorkMode workMode, TrafficColor color, int x)
        {
            _trafficLight.SetWorkMode(workMode);
            _trafficLight.SetCurrentLight(color);
            _trafficLight.GetStatus().Should().Be($" Color={color};" +
                                                  $" Work mode = {workMode}" +
                                                  $" Battery = {_mock.Object.Energy}%.");
        }

        [TestCase(TrafficColor.Green)]
        [TestCase(TrafficColor.Red)]
        public void Next_Color_Yellow_Test(TrafficColor color)
        {
            _trafficLight.SetWorkMode(WorkMode.Hand);
            _trafficLight.SetCurrentLight(color);
            _trafficLight.SetWorkMode(WorkMode.Normal);
            _trafficLight.NextColor();
            _trafficLight.GetStatus().Should().Contain("Yellow");
        }

        [Test]
        public void Next_Color_Green_To_Red_Test()
        {
            _trafficLight.SetWorkMode(WorkMode.Hand);
            _trafficLight.SetCurrentLight(TrafficColor.Green);
            _trafficLight.SetWorkMode(WorkMode.Normal);
            _trafficLight.NextColor();
            _trafficLight.NextColor();
            _trafficLight.GetStatus().Should().Contain("Red");
        }

        [Test]
        public void Next_Color_Red_To_Green_Test()
        {
            _trafficLight.SetWorkMode(WorkMode.Hand);
            _trafficLight.SetCurrentLight(TrafficColor.Red);
            _trafficLight.SetWorkMode(WorkMode.Normal);
            _trafficLight.NextColor();
            _trafficLight.NextColor();
            _trafficLight.GetStatus().Should().Contain("Green");
        }

        [Test]
        public void Set_Default_Test()
        {
           // _trafficLight.
            _trafficLight.ChangeBattery(_mock.Object);
            _trafficLight.GetStatus().Should().Be(" Color=Yellow;" +
                                                  " Work mode = Normal" +
                                                  " Battery = 100%.");
        }

        [Test]
        public void Notify_Test()
        {
            var wasCalled = false;
            _mock.Object.Energy = 10;
            _trafficLight.Notify += delegate (object o, string st)
            { 
                wasCalled = true;
            };
            _trafficLight.GetStatus();
            wasCalled.Should().BeTrue();
        }

        [TestCase(TrafficColor.Green)]
        [TestCase(TrafficColor.Red)]
        [TestCase(TrafficColor.Yellow)]
        public void SetCorrentLightTest(TrafficColor trafficColor)
        {
            _trafficLight.SetWorkMode(WorkMode.Hand);
            _trafficLight.SetCurrentLight(trafficColor);
            _trafficLight.GetStatus().Should().Contain(trafficColor.ToString());
        }
    }
}