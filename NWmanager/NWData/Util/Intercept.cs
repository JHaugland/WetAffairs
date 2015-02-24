using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWData.Units;
using TTG.NavalWar.NWComms;
using TTG.NavalWar.NWComms.Entities;

namespace TTG.NavalWar.NWData
{
    [Serializable]
    public class Intercept
    {
        public Coordinate impactPoint = new Coordinate();
        public double bulletHeading_deg;
        int ROBOT_RADIUS = 1; // TODO:Find out WTF this is!
        protected Coordinate bulletStartingPoint = new Coordinate();
        protected Coordinate targetStartingPoint = new Coordinate();
        public double targetHeading;
        public double targetVelocity;
        public double bulletPower;
        public double angleThreshold;
        public double distance;
        public double distanceMeters;
        protected double impactTime;
        protected double angularVelocity_rad_per_sec;
        #region "Constructors"

        public void calculate(

       // Initial bullet position x coordinate 
       double xb,
            // Initial bullet position y coordinate
       double yb,
            // Initial target position x coordinate
       double xt,
            // Initial target position y coordinate
       double yt,
            // Target heading
       double tHeading,
            // Target velocity
       double vt,
            // Power of the bullet that we will be firing
       double bPower,
            // Angular velocity of the target
       double angularVelocity_deg_per_sec
      )
        {
            impactPoint.SetFromXY(0, 0);
            
            angularVelocity_rad_per_sec = ConvertDegreesToRadians(angularVelocity_deg_per_sec);

            bulletStartingPoint.SetFromXY(xb, yb);
            targetStartingPoint.SetFromXY(xt, yt);

            targetHeading = tHeading;
            targetVelocity = vt;
            bulletPower = bPower;
            double vb = 20 - 3 * bulletPower;

            double dX, dY;

            // Start with initial guesses at 10 and 20 ticks
            impactTime = getImpactTime(10, 20, 0.01);
           

            impactPoint = getEstimatedPosition(impactTime);

            dX = (impactPoint.X - bulletStartingPoint.X);
            dY = (impactPoint.Y - bulletStartingPoint.Y);

            distance = Math.Sqrt(dX * dX + dY * dY);

            

            bulletHeading_deg = ConvertRadiansToDegrees(Math.Atan2(dX, dY));
            angleThreshold = ConvertRadiansToDegrees(Math.Atan(ROBOT_RADIUS / distance));
        }

        protected Coordinate getEstimatedPosition(double time)
        {

            double x = targetStartingPoint.X +
               targetVelocity * time * Math.Sin(ConvertDegreesToRadians(targetHeading));
            double y = targetStartingPoint.Y +
               targetVelocity * time * Math.Cos(ConvertDegreesToRadians(targetHeading));
            Coordinate nc = new Coordinate();
            nc.SetFromXY(x, y);
            return nc;
        }

        private double f(double time)
        {

            double vb = 20 - 3 * bulletPower;

            Coordinate targetPosition = getEstimatedPosition(time);
            double dX = (targetPosition.X - bulletStartingPoint.X);
            double dY = (targetPosition.Y - bulletStartingPoint.Y);

            return Math.Sqrt(dX * dX + dY * dY) - vb * time;
        }

        private double getImpactTime(double t0,
          double t1, double accuracy)
        {

            double X = t1;
            double lastX = t0;
            int iterationCount = 0;
            double lastfX = f(lastX);

            while ((Math.Abs(X - lastX) >= accuracy) &&
              (iterationCount < 15))
            {

                iterationCount++;
                double fX = f(X);

                if ((fX - lastfX) == 0.0) break;

                double nextX = X - fX * (X - lastX) / (fX - lastfX);
                lastX = X;
                X = nextX;
                lastfX = fX;
            }

            return X;
        }
        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
        public static double ConvertRadiansToDegrees(double radians)
        {
            double degrees = (180 / Math.PI) * radians;
            return (degrees);
        }
    }
}
        #endregion