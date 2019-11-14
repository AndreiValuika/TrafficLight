using System;

namespace TrafficLight.Lib
{
    public enum TrafficColor
    {
        Red,
        Yellow,
        Green
    }
    public enum WorkMode
    {
        Normal,
        Hand,
        SafeMode,
        Off
    }
    public class TrafficLight
    {
        private TrafficColor _currentLight;
        private TrafficColor _previousLight;
        private Battery _battery;
        private WorkMode _workMode;

        const string ALARM_MESSAGE = "Battery low!!!";
        const int LOW_BATTERY_LEVEL = 11;

        public void SetWorkMode(WorkMode workMode)
        {
            _workMode = workMode;
        }

        public TrafficLight(Battery battery)
        {
            ChangeBattery(battery);
        }

        public string GetStatus()
        {
            return $" Color={_currentLight};" +
                   $" Work mode = {_workMode}" +
                   $" Battery = {CheckBattery()}%.";
        }

        private void SetDefault()
        {
            _currentLight = TrafficColor.Yellow;
            _previousLight = TrafficColor.Green;
            _workMode = WorkMode.Normal;
        }

        /// <summary>
        /// Set current color .
        /// WorkMode must be "Hand".
        /// </summary>
        public void SetCurrentLight(TrafficColor color)
        {
            if (_workMode != WorkMode.Hand)
            {
                throw new Exception("Wrong mode!");
            }
            _currentLight = color;
        }

        /// <summary>
        /// Change current color from order "Red-Yellow-Green Green-Yellow-Red".
        /// WorkMode must be "Normal".
        /// </summary>
        public void NextColor()
        {
            if (_workMode != WorkMode.Normal)
            {
                throw new Exception("Wrong mode!");
            }
            var tempColor = _currentLight;
            switch (_currentLight)
            {
                case TrafficColor.Red:
                case TrafficColor.Green:
                    _currentLight = TrafficColor.Yellow;
                    break;
                case TrafficColor.Yellow:
                    _currentLight = _previousLight == TrafficColor.Green ?
                                     TrafficColor.Red : TrafficColor.Green;
                    break;
            }
            _previousLight = tempColor;
        }

        private int CheckBattery()
        {
            int actualEnergy = _battery.Energy;
            if (actualEnergy < LOW_BATTERY_LEVEL)
            {
                Notify?.Invoke(this, ALARM_MESSAGE + "     " + actualEnergy.ToString() + "%");
                _workMode = WorkMode.SafeMode;
            }
            return actualEnergy;
        }

        /// <summary>
        /// Change actual battery and set default fields.
        /// </summary>
        /// <param name="battery"></param>
        public void ChangeBattery(Battery battery)
        {
            _battery = battery;
            SetDefault();
        }
        /// <summary>
        /// Message to master.
        /// </summary>
        public event EventHandler<string> Notify;
    }
}
