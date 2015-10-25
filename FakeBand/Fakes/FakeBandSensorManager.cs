using Microsoft.Band.Sensors;
using System;

namespace MSBandAzure.Services.Fakes
{
    public class FakeBandSensorManager : IBandSensorManager
    {
        public IBandSensor<IBandAccelerometerReading> Accelerometer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBandSensor<IBandCaloriesReading> Calories
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBandContactSensor Contact
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBandSensor<IBandDistanceReading> Distance
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBandSensor<IBandGyroscopeReading> Gyroscope
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        IBandSensor<IBandHeartRateReading> _heartRate = new FakeHeartRateSensor();
        public IBandSensor<IBandHeartRateReading> HeartRate
        {
            get
            {
                return _heartRate;
            }
        }

        public IBandSensor<IBandPedometerReading> Pedometer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBandSensor<IBandSkinTemperatureReading> SkinTemperature
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IBandSensor<IBandUVReading> UV
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
