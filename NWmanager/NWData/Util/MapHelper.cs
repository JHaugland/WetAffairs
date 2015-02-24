using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTG.NavalWar.NWData;
using TTG.NavalWar.NWData.GamePlay;
using TTG.NavalWar.NWData.Util;
using TTG.NavalWar.NWData.OrderSystem;
using TTG.NavalWar.NWComms.Entities;
using TTG.NavalWar.NWComms;
using System.Diagnostics;
using TTG.NavalWar.NWData.Units;

namespace TTG.NavalWar.NWData.Util
{
    // Original written by Bryan Reynolds. 
    // http://bryan.reynoldslive.com/post/Latitude2c-Longitude2c-Bearing2c-Cardinal-Direction2c-Distance2c-and-C.aspx
    public static class MapHelper
    {
        public const double RADIUS_OF_EARTH_KM = 6378.137;      // WGS84 major axis

        private const double WGS84_MAJOR_AXIS = 6378137;
        private const double WGS84_MINOR_AXIS = 6356752.3142;

        private const double WGS84_MAJOR_AXIS_SQUARED = WGS84_MAJOR_AXIS * WGS84_MAJOR_AXIS;
        private const double WGS84_MINOR_AXIS_SQUARED = WGS84_MINOR_AXIS * WGS84_MINOR_AXIS;

        private const double WGS84_FLATTENING = 1 / 298.257223563;
        private const double WGS84_ONE_MINUS_FLATTENING = 1 - WGS84_FLATTENING;

        private const double MILES_TO_KILOMETERS = 1.609344;
        private const double MILES_TO_NEUTICAL = 0.8684;
        private const double MILES_TO_METERS = MILES_TO_KILOMETERS * 1000.0;

        /// <summary>
        /// Class is used in a calculation to determin cardinal point enumeration values from degrees.
        /// </summary>
        private struct CardinalRanges
        {
            public GameConstants.DirectionCardinalPoints CardinalPoint;
            /// <summary>
            /// Low range value associated with the cardinal point.
            /// </summary>
            public double LowRange;
            /// <summary>
            /// High range value associated with the cardinal point.
            /// </summary>
            public double HighRange;
        }

        private struct HeightDepthRanges
        {
            public GameConstants.HeightDepthPoints HeightDepthPoint;
            public double LowRangeMeters;
            public double HighRangeMeters;
        }        

        public static int GetHighestValue(int a, int b)
        {
            if (a > b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public static int GetHighestValue(int a, int b, int c)
        {
            int[] values = new int[3];
            values[0] = a;
            values[1] = b;
            values[2] = c;
            return values.Max();
        }

        public static int GetLowestValue(int a, int b)
        {
            if (a < b)
            {
                return a;
            }
            else
            {
                return b;
            }
        }

        public static int GetLowestValue(int a, int b, int c)
        {
            int[] values = new int[3];
            values[0] = a;
            values[1] = b;
            values[2] = c;
            return values.Min();
        }
        /// <summary>
        /// Converts meters to neutical miles
        /// </summary>
        /// <param name="meters"></param>
        /// <returns></returns>
        public static double ToNmilesFromMeters(this double meters)
        {
            return meters / 1852.0;
        }

        ///// <summary>
        ///// Converts neutical miles to meters.
        ///// </summary>
        ///// <param name="nm"></param>
        ///// <returns></returns>
        //public static double ToMetersFromNmiles(this double nm)
        //{
        //    return nm * 1852.0;
        //}

        /// <summary>
        /// Converts kilometer per hour to meter per second
        /// </summary>
        /// <param name="kph"></param>
        /// <returns></returns>
        public static double ToMperSecFromKph(this double kph)
        {
            return kph / 3.6;
        }

        /// <summary>
        /// Converts degrees to Radians.
        /// </summary>
        /// <returns>Returns a radian from degrees.</returns>
        public static double ToRadian(this double degree)
        {
            return (degree * Math.PI / 180.0);
        }
        /// <summary>
        /// To signed degrees ranging from -180 to 180 from a radian value.
        /// </summary>
        /// <returns>Returns degrees from radians.</returns>
        public static double ToDegreeSignedLongitude(this double radian)
        {
            double degreeUnsigned = radian * 180.0 / Math.PI;
            return Math.IEEERemainder(degreeUnsigned, 360.0);
        }

        /// <summary>
        /// To signed degrees ranging from -90 to 90 from a radian value.
        /// </summary>
        /// <returns>Returns degrees from radians.</returns>
        public static double ToDegreeSignedLatitude(this double radian)
        {
            double degreeUnsigned = radian * 180.0 / Math.PI; 
            return Math.IEEERemainder(degreeUnsigned, 180.0);
        }

        /// <summary>
        /// Returns degrees ranging from 0 to 360 from a radian value.
        /// </summary>
        /// <param name="radian"></param>
        /// <returns></returns>
        public static double ToDegreeBearing(this double radian)
        {
            return ((radian * 180.0 / Math.PI) + 360.0) % 360.0;
        }

        public static PositionInfo Offset(this PositionInfo pos, double bearingDeg, double distanceM)
        {
            var position = new Position(pos);
            var npos = position.Offset(bearingDeg, distanceM);
            return npos.GetPositionInfo();
        }

        //public static string ToDegMinSecString(this double degrees)
        //{
        //    var d = Math.Abs(degrees);  // (unsigned result ready for appending compass dir'n)
        //    d += 1/7200;  // add ½ second for rounding
        //    double deg = Math.Floor(d);
        //    double min = Math.Floor((d-deg)*60);
        //    double sec = Math.Floor((d-deg-min/60)*3600);
        //    string sdeg = deg.ToString();
        //    string smin = min.ToString();
        //    string ssec = sec.ToString();

        //    // add leading zeros if required
        //    if (deg<100) 
        //    {
        //      sdeg = '0' + sdeg; 
        //    }
        //    if (deg<10) 
        //    {
        //      sdeg = '0' + sdeg;
        //    }
        //    if (min<10) 
        //    {
        //      smin = '0' + smin;
        //    }
        //    if (sec<10) 
        //    {
        //      ssec = '0' + ssec;
        //    }
        //    return string.Format("{0}\u00B0 {1}' {2}\"", sdeg, smin, ssec);
            
        //}

        public static double ToSecFromHours(double hour)
        {
            return hour * 3600;
        }

        public static double Clamp(this double value, double minValue, double maxValue)
        {
            if (value < minValue)
            {
                return minValue;
            }
            else if (value > maxValue)
            {
                return maxValue;
            }
            else
            {
                return value;
            }

        }

        public static int Clamp(this int value, int minValue, int maxValue)
        {
            if (value < minValue)
            {
                return minValue;
            }
            else if (value > maxValue)
            {
                return maxValue;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Calculates the relative properties of two positions, including relative movement and relative bearing
        /// </summary>
        /// <param name="firstPos"></param>
        /// <param name="secondPos"></param>
        /// <returns></returns>
        public static PositionRelationship CalculatePositionRelationship(
            Position firstPos, double firstSpeedKph, Position secondPos, double secondSpeedKph)
        {
            PositionRelationship posRel = new PositionRelationship();
            posRel.RelativeBearing = GetRelativeBearing(firstPos, secondPos);
            if (secondSpeedKph > firstSpeedKph)
            {
                posRel.RelativeSpeed = -1;
            }
            else if (firstSpeedKph > secondSpeedKph)
            {
                posRel.RelativeSpeed = 1;
            }
            else
            {
                posRel.RelativeSpeed = 0;
            }
            return posRel;
        }

        public static GameConstants.RelativeBearing GetRelativeBearing(Position firstPos, Position secondPos)
        {
            if (firstPos == null || secondPos == null || !secondPos.HasBearing)
            {
                return GameConstants.RelativeBearing.UnknownOrUndefined;
            }
            //double firstBearingDeg = (double)firstPos.BearingDeg;
            double secondBearingDeg = (double)secondPos.BearingDeg;
            double distanceNowM = CalculateDistanceM(firstPos.Coordinate, secondPos.Coordinate);
            var futurePos = secondPos.Offset((double)secondPos.BearingDeg, 1000);
            double distanceThenM = CalculateDistanceM(firstPos.Coordinate, futurePos.Coordinate);
            if (distanceThenM > distanceNowM)
            {
                return GameConstants.RelativeBearing.MovingAway;
            }
            else if (Math.Abs(distanceNowM - distanceThenM) < 5)
            {
                return GameConstants.RelativeBearing.MovingParallel;
            }
            else
            {
                return GameConstants.RelativeBearing.MovingTowards;
            }

        }
        /// <summary>
        /// Calculates the distance between two points of latitude and longitude.
        /// Great Link - http://www.movable-type.co.uk/scripts/latlong.html
        /// </summary>
        /// <param name="coordinate1">First coordinate.</param>
        /// <param name="coordinate2">Second coordinate.</param>
        /// <param name="unitsOfLength">Sets the return value unit of length.</param>
        public static double CalculateDistanceUnit(Coordinate coordinate1, Coordinate coordinate2, 
            GameConstants.UnitsOfLength unitsOfLength)
        {
            var theta = coordinate1.LongitudeDeg - coordinate2.LongitudeDeg;
            var distance = Math.Sin(coordinate1.LatitudeRad) * Math.Sin(coordinate2.LatitudeRad) +
                           Math.Cos(coordinate1.LatitudeRad) * Math.Cos(coordinate2.LatitudeRad) *
                           Math.Cos(theta.ToRadian());

            distance = Math.Acos(distance);
            distance = distance.ToDegreeBearing();
            distance = distance * 60 * 1.1515;

            if (unitsOfLength == GameConstants.UnitsOfLength.Meter)
            {
                distance = distance * MILES_TO_METERS;
            }
            else if (unitsOfLength == GameConstants.UnitsOfLength.Kilometer)
            {
                distance = distance * MILES_TO_KILOMETERS;
            }
            else if (unitsOfLength == GameConstants.UnitsOfLength.NauticalMiles)
            {
                distance = distance * MILES_TO_NEUTICAL;
            }
            return (distance);

        }

        public static double CalculateDistanceRoughM(Coordinate coordinate1, Coordinate coordinate2)
        { 
            //sqrt(x * x + y * y)
            //where x = 69.1 * (lat2 - lat1) 
            //and y = 53.0 * (lon2 - lon1) 
            //y = 69.1 * (lon2 - lon1) * cos(lat1/57.3) 
            double x = 69.1 * (coordinate2.LatitudeDeg - coordinate1.LatitudeDeg);
            double y = 69.1 * (coordinate2.LongitudeDeg - coordinate1.LongitudeDeg) 
                * Math.Cos(coordinate1.LatitudeDeg / 57.3);
            double resultMi = Math.Sqrt((x * x) + (y * y)) ;
            return resultMi * 1609.344;
        }

        public static double CalculateDistanceApproxM(Coordinate coordinate1, Coordinate coordinate2)
        {
            double lat1 = coordinate1.LatitudeRad;
            double lat2 = coordinate2.LatitudeRad;
            double lon1 = coordinate1.LongitudeRad;
            double lon2 = coordinate2.LongitudeRad;
            double d = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2) +
                              Math.Cos(lat1) * Math.Cos(lat2) *
                              Math.Cos(lon2 - lon1)) * RADIUS_OF_EARTH_KM;
            //double d = Math.Acos(Math.Sin(coordinate1.Latitude)*Math.Sin(coordinate2.Latitude) + 
            //                  Math.Cos(coordinate1.Latitude)*Math.Cos(coordinate2.Latitude) *
            //                  Math.Cos(coordinate2.Longitude - coordinate1.Longitude)) * RADIUS_OF_EARTH_KM;
            return d * 1000.0;
        }

        public static bool IsDistanceShorterThan(Position position1, Position position2, double maxDistanceM, 
            out double distance3DM)
        {
            if (position1 == null || position2 == null)
            {
                distance3DM = double.MaxValue;
                return false;
            }
            double distanceRoughM = CalculateDistanceRoughM(position1.Coordinate, position2.Coordinate);
            if (distanceRoughM > maxDistanceM * 1.2)
            {
                distance3DM = distanceRoughM;
                return false;
            }
            distance3DM = CalculateDistance3DM(position1, position2);
            return (distance3DM < maxDistanceM);
        }

        public static double CalculateDistance3DM(Position position1, Position position2)
        {
            if (position1 == null || position2 == null)
            {
                return double.MaxValue; //hmm
            }

            double distanceM = CalculateDistanceM( position1.Coordinate, position2.Coordinate );

            if ( position1.HasHeightOverSeaLevel && position2.HasHeightOverSeaLevel &&
                Math.Abs( position1.HeightOverSeaLevelM.Value - position2.HeightOverSeaLevelM.Value ) > 1 &&
                distanceM < 100000 ) //the maximum distance where height has any practical influence on 3d distance
            {
                double heightM = Math.Abs( position1.HeightOverSeaLevelM.Value - position2.HeightOverSeaLevelM.Value );
                distanceM = Math.Sqrt( Math.Pow( distanceM, 2 ) + Math.Pow( heightM, 2 ) );
            }

            return distanceM;
        }

        public static double CalculateDistanceM(Coordinate coordinate1, Coordinate coordinate2)
        {
            return CalculateDistanceUnit(coordinate1, coordinate2, GameConstants.UnitsOfLength.Meter);

        }
        /// <summary>
        /// Calculates (rightly) the maximum distance an observer can see on a curved earth surface, given the height of 
        /// an observer and a target above sea level (in meters). Does not account for visibility or obstructions.
        /// Uses formula calculating exact distance to horizon. See http://en.wikipedia.org/wiki/Horizon#Exact_formula
        /// </summary>
        /// <param name="heightObserverM"></param>
        /// <param name="heightTargetM"></param>
        /// <returns></returns>
        public static double CalculateMaxLineOfSightM(double heightObserverM, double heightTargetM)
        {
            double height = heightObserverM + heightTargetM;
            //double distanceToHorizon = Math.Sqrt((2.0 * RADIUS_OF_EARTH_KM * 1000.0) + Math.Pow(height, 2));
            var distanceToHorizon = 3.86 * Math.Sqrt(height);
            return distanceToHorizon * 1000.0;
        }

        public static double CalculateMaxRadarLineOfSightM(double heightObserverM, double heightTargetM)
        {
            //http://www.its.bldrdoc.gov/fs-1037/dir-029/_4333.htm

            double radarHorizonRangeKm = 4.12 * (Math.Sqrt(heightObserverM) + Math.Sqrt(heightTargetM));
            return radarHorizonRangeKm * 1000.0;
            ////Rule of thumb: for radar line of sight, assume earth radius is 4/3rds of real
            //double Height = heightObserverM + heightTargetM;
            //double DistanceToHorizon = Math.Sqrt((2 * (4.0/3.0) * RADIUS_OF_EARTH_KM * 1000) + Math.Pow(Height, 2));
            //return DistanceToHorizon;


        }

        // The directional names are also routinely and very conveniently associated with 
        // the degrees of rotation in the unit circle, a necessary step for navigational 
        // calculations (derived from trigonometry) and/or for use with Global 
        // Positioning Satellite (GPS) Receivers. The four cardinal directions 
        // correspond to the following degrees of a compass:
        //
        // North (N): 0° = 360° 
        // East (E): 90° 
        // South (S): 180° 
        // West (W): 270° 
        // An ordinal, or intercardinal, direction is one of the four intermediate 
        // compass directions located halfway between the cardinal directions.
        //
        // Northeast (NE), 45°, halfway between north and east, is the opposite of southwest. 
        // Southeast (SE), 135°, halfway between south and east, is the opposite of northwest. 
        // Southwest (SW), 225°, halfway between south and west, is the opposite of northeast. 
        // Northwest (NW), 315°, halfway between north and west, is the opposite of southeast. 
        // These 8 words have been further compounded, resulting in a total of 32 named 
        // (and numbered) points evenly spaced around the compass. It is noteworthy that 
        // there are languages which do not use compound words to name the points, 
        // instead assigning unique words, colors, and/or associations with phenomena of the natural world.

        /// <summary>
        /// Method extension for Doubles. Converts a degree to a cardinal point enumeration.
        /// </summary>
        /// <returns>Returns a cardinal point enumeration representing a compass direction.</returns>
        public static GameConstants.DirectionCardinalPoints ToCardinalMark(this double degree)
        {
            var CardinalRanges = new List<CardinalRanges>
                       {
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.N, LowRange = 0, HighRange = 22.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.NE, LowRange = 22.5, HighRange = 67.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.E, LowRange = 67.5, HighRange = 112.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.SE, LowRange = 112.5, HighRange = 157.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.S, LowRange = 157.5, HighRange = 202.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.SW, LowRange = 202.5, HighRange = 247.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.W, LowRange = 247.5, HighRange = 292.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.NW, LowRange = 292.5, HighRange = 337.5},
                         new CardinalRanges {CardinalPoint = GameConstants.DirectionCardinalPoints.N, LowRange = 337.5, HighRange = 360.1}
                       };


            if (!(degree >= 0 && degree <= 360))
            {
                throw new ArgumentOutOfRangeException("degree",
                                                      "Degree value must be greater than or equal to 0 and less than or equal to 360.");
            }

            return CardinalRanges.Find(p => (degree >= p.LowRange && degree < p.HighRange)).CardinalPoint;
        }

        public static GameConstants.DirectionCardinalPoints ToCardinalMark( this float degree )
        {
            double d = degree;
            return d.ToCardinalMark();
        }

        public static GameConstants.HeightDepthPoints ToHeightDepthMark(this double heightDepthM)
        {
            if (heightDepthM >= GameConstants.HEIGHT_MAXIUMUM_M)
            {
                heightDepthM = GameConstants.HEIGHT_MAXIUMUM_M;
            }
            if (heightDepthM <= GameConstants.DEPTH_MAXIMUM_M)
            {
                heightDepthM = GameConstants.DEPTH_MAXIMUM_M;
            }

            var DepthRanges = new List<HeightDepthRanges>
            {
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.VeryDeep, 
                                        LowRangeMeters = GameConstants.DEPTH_MAXIMUM_M - 1, 
                                        HighRangeMeters = GameConstants.DEPTH_VERY_DEEP_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.Deep, 
                                        LowRangeMeters = GameConstants.DEPTH_DEEP_MIN_M, 
                                        HighRangeMeters = GameConstants.DEPTH_MEDIUM_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.MediumDepth, 
                                        LowRangeMeters = GameConstants.DEPTH_MEDIUM_MIN_M, 
                                        HighRangeMeters = GameConstants.DEPTH_SHALLOW_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.ShallowDepth, 
                                        LowRangeMeters = GameConstants.DEPTH_SHALLOW_MIN_M, 
                                        HighRangeMeters = GameConstants.DEPTH_PERISCOPE_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.PeriscopeDepth, 
                                        LowRangeMeters = GameConstants.DEPTH_PERISCOPE_MIN_M, 
                                        HighRangeMeters = GameConstants.DEPTH_SURFACE_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.Surface, 
                                        LowRangeMeters = GameConstants.DEPTH_SURFACE_MIN_M, 
                                        HighRangeMeters = GameConstants.HEIGHT_VERY_LOW_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.VeryLow, 
                                        LowRangeMeters = GameConstants.HEIGHT_VERY_LOW_MIN_M, 
                                        HighRangeMeters = GameConstants.HEIGHT_LOW_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.Low, 
                                        LowRangeMeters = GameConstants.HEIGHT_LOW_MIN_M, 
                                        HighRangeMeters = GameConstants.HEIGHT_MEDIUM_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.MediumHeight, 
                                        LowRangeMeters = GameConstants.HEIGHT_MEDIUM_MIN_M, 
                                        HighRangeMeters = GameConstants.HEIGHT_HIGH_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.High, 
                                        LowRangeMeters = GameConstants.HEIGHT_HIGH_MIN_M, 
                                        HighRangeMeters = GameConstants.HEIGHT_VERY_HIGH_MIN_M },
                new HeightDepthRanges { HeightDepthPoint = GameConstants.HeightDepthPoints.VeryHigh, 
                                        LowRangeMeters = GameConstants.HEIGHT_VERY_HIGH_MIN_M, 
                                        HighRangeMeters = GameConstants.HEIGHT_MAXIUMUM_M + 1 },

            };
            return DepthRanges.Find(p => heightDepthM >= p.LowRangeMeters && heightDepthM < p.HighRangeMeters).HeightDepthPoint;
        }

        public static GameConstants.HeightDepthPoints ToHeightDepthMark( this float heightDepthM )
        {
            double d = heightDepthM;
            return d.ToHeightDepthMark();
        }

        public static double GetElevationMFromHeightDepthMark(GameConstants.HeightDepthPoints heightDepthMark)
        {
            switch (heightDepthMark)
            {
                case GameConstants.HeightDepthPoints.MaxDepth:
                    return GameConstants.DEPTH_MAXIMUM_M;
                case GameConstants.HeightDepthPoints.VeryDeep:
                    return GameConstants.DEPTH_VERY_DEEP_MIN_M;
                case GameConstants.HeightDepthPoints.Deep:
                    return GameConstants.DEPTH_DEEP_MIN_M;
                case GameConstants.HeightDepthPoints.MediumDepth:
                    return GameConstants.DEPTH_MEDIUM_MIN_M;
                case GameConstants.HeightDepthPoints.ShallowDepth:
                    return GameConstants.DEPTH_SHALLOW_MIN_M;
                case GameConstants.HeightDepthPoints.PeriscopeDepth:
                    return GameConstants.DEPTH_PERISCOPE_MIN_M;
                case GameConstants.HeightDepthPoints.Surface:
                    return 0;
                case GameConstants.HeightDepthPoints.VeryLow:
                    return GameConstants.HEIGHT_VERY_LOW_MIN_M;
                case GameConstants.HeightDepthPoints.Low:
                    return GameConstants.HEIGHT_LOW_MIN_M;
                case GameConstants.HeightDepthPoints.MediumHeight:
                    return GameConstants.HEIGHT_MEDIUM_MIN_M;
                case GameConstants.HeightDepthPoints.High:
                    return GameConstants.HEIGHT_HIGH_MIN_M;
                case GameConstants.HeightDepthPoints.VeryHigh:
                    return GameConstants.HEIGHT_VERY_HIGH_MIN_M;
                case GameConstants.HeightDepthPoints.MaxHeight:
                    return GameConstants.HEIGHT_MAXIUMUM_M;
                default:
                    return 0;
            }
        }

        //public static GameConstants.HeightDepthPoints GetHeightDepthPointsFromMeters(double meters)
        //{
        //    if (meters >= GameConstants.HEIGHT_VERY_HIGH_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.VeryHigh;
        //    }
        //    else if (meters >= GameConstants.HEIGHT_HIGH_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.High;
        //    }
        //    else if (meters >= GameConstants.HEIGHT_MEDIUM_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.MediumHeight;
        //    }
        //    else if (meters >= GameConstants.HEIGHT_LOW_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.Low;
        //    }
        //    else if (meters >= GameConstants.HEIGHT_VERY_LOW_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.VeryLow;
        //    }
        //    else if (meters >= GameConstants.DEPTH_SURFACE_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.Surface;
        //    }
        //    else if (meters >= GameConstants.DEPTH_PERISCOPE_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.PeriscopeDepth;
        //    }
        //    else if (meters >= GameConstants.DEPTH_SHALLOW_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.ShallowDepth;
        //    }
        //    else if (meters >= GameConstants.DEPTH_MEDIUM_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.MediumDepth;
        //    }
        //    else if (meters >= GameConstants.DEPTH_DEEP_MIN_M)
        //    {
        //        return GameConstants.HeightDepthPoints.Deep;
        //    }
        //    else
        //    {
        //        return GameConstants.HeightDepthPoints.VeryDeep;
        //    }
        //}

        /// <summary>
        /// Accepts two coordinates in degrees. Returns bearing from c1 to c2 in degrees.
        /// </summary>
        /// <returns>A double value in degrees.  From 0 to 360.</returns>
        public static double CalculateBearingDegrees(Coordinate coordinate1, Coordinate coordinate2)
        {
            var latitude1 = coordinate1.LatitudeRad;
            var latitude2 = coordinate2.LatitudeRad;

            //var longitudeDifference = (coordinate2.Longitude - coordinate1.Longitude).ToRadian();
            var longitudeDifference = coordinate2.LongitudeRad - coordinate1.LongitudeRad;

            //var y = Math.sin(dLon) * Math.cos(lat2);
            //var x = Math.cos(lat1) * Math.sin(lat2) -
            //        Math.sin(lat1) * Math.cos(lat2) * Math.cos(dLon);
            //var brng = Math.atan2(y, x).toBrng();

            var y = Math.Sin(longitudeDifference) * Math.Cos(latitude2);
            var x = Math.Cos(latitude1) * Math.Sin(latitude2) -
                    Math.Sin(latitude1) * Math.Cos(latitude2) *
                    Math.Cos(longitudeDifference);
            var result = Math.Atan2(y, x).ToDegreeBearing(); 
            return (result + 360) % 360;
        }

        public static bool IsAngleWithinRangeDeg(double angleToCheckDeg, double angle1Deg, double angle2Deg)
        {
            return IsAngleWithinRangeRad(angleToCheckDeg.ToRadian(), angle1Deg.ToRadian(), angle2Deg.ToRadian());
        }

        public static bool IsAngleWithinRangeRad(double angleToCheckRad, double angle1Rad, double angle2Rad)
        {
            // map angles to -180 to 180, assumes angles are in [-540,540]
            if (angleToCheckRad < -Math.PI)
            {
                angleToCheckRad += Math.PI * 2;
            }
            else if (angleToCheckRad >= Math.PI)
            {
                angleToCheckRad -= Math.PI * 2;
            }

            if (angle1Rad < -Math.PI) 
            { 
                angle1Rad += Math.PI * 2; 
            }
            else if (angle1Rad > Math.PI) 
            {
                angle1Rad -= Math.PI * 2; 
            }

            if (angle2Rad < -Math.PI)
            { 
                angle2Rad += Math.PI * 2; 
            }
            else if (angle2Rad > Math.PI)
            {
                angle2Rad -= Math.PI * 2;
            }

            // check if region is within sector
            if (angle1Rad <= angle2Rad)
            {
                return ((angleToCheckRad >= angle1Rad) && (angleToCheckRad <= angle2Rad));
            }
            else
            {
                return ((angleToCheckRad >= angle1Rad) || (angleToCheckRad <= angle2Rad));
            }
        }

        /// <summary>
        /// Determines whether a coordinate is within a region specified with a polygon
        /// </summary>
        /// <param name="region"></param>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static bool IsWithinRegion(Region region, Coordinate coordinate)
        {
            //Raytracing. See http://www.alienryderflex.com/polygon/
            int numPoints = region.Coordinates.Count;
            double latRad = coordinate.LatitudeRad;
            double lonRad = coordinate.LongitudeRad;
            bool isInPoly = false;
            int j = numPoints - 1;
            for (int i = 0; i < numPoints; i++)
            {
                var coord1 = region.Coordinates[i];
                var coord2 = region.Coordinates[j];
                if (coord1.LongitudeRad < lonRad
                    && coord2.LongitudeRad >= lonRad
                    || coord2.LongitudeRad < lonRad
                    && coord1.LongitudeRad >= lonRad)
                {
                    if (coord1.LatitudeRad + (lonRad - coord1.LongitudeRad) 
                        / (coord2.LongitudeRad - coord1.LongitudeRad)
                        * (coord2.LatitudeRad - coord1.LatitudeRad) < latRad)
                    {
                        isInPoly = !isInPoly;
                    }
                }
                j = i;
            }
            return isInPoly;
        }

        /// <summary>
        /// Generates a square of coordinates based on a central position and the lenght of the sides in meters. Will return a list of 4 coordinates.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="lenghtM"></param>
        /// <returns></returns>
        public static List<Coordinate> CreateCoordinateSquare(Coordinate coordinate, double lenghtM)
        {
            var list = new List<Coordinate>();
            var diagonalM = Math.Sqrt((lenghtM * lenghtM) + (lenghtM * lenghtM));
            Position pos = new Position(coordinate);
            var nwCorner = pos.Offset(360 - 45, diagonalM);
            list.Add(nwCorner.Coordinate);
            var neCorner = pos.Offset(45, diagonalM);
            list.Add(neCorner.Coordinate);
            var seCorner = pos.Offset(90 + 45, diagonalM);
            list.Add(seCorner.Coordinate);
            var swCorner = pos.Offset(180 + 45, diagonalM);
            list.Add(swCorner.Coordinate);
            return list;
        }

        /// <summary>
        /// Generates a grid of coordinates centered on the submitted coordinate, containing
        /// the designated number of cells, with each coordinate a designated distance apart.
        /// </summary>
        /// <param name="coordinate">Center of grid</param>
        /// <param name="noOfCoordinates">Number of coordinates. If not square, grid may be larger</param>
        /// <param name="distanceM">Distance between coordinates in meter</param>
        /// <returns>List of coordinates</returns>
        public static List<Coordinate> CreateCoordinateGrid(Coordinate coordinate, int noOfCoordinates, double distanceM)
        {
            var list = new List<Coordinate>();
            int listWidth = (int)Math.Sqrt(noOfCoordinates);
            if (listWidth < 1)
            {
                listWidth = 1;
            }
            Position pos = new Position(coordinate);
            var nwCorner = pos.Offset(360-45, distanceM * (listWidth / 2));
            for (int ew = 1; ew <= listWidth; ew++)
            {
                for (int ns = 1; ns <= listWidth; ns++)
                { 
                    var distEw = distanceM * (ew - 1);
                    var distNs = distanceM * (ns - 1);
                    var posElement = nwCorner.Offset(90, distEw).Offset(180, distNs);
                    list.Add(posElement.Coordinate.Clone());
                }
            }
            return list;
        }
        
        public static double GetMaxValue(double value1, double value2)
        {
            return (value1 > value2 ? value1 : value2);
        }

        public static double GetMinValue(double value1, double value2)
        {
            return (value1 < value2 ? value1 : value2);
        }

        /// <summary>
        /// Determines whether a coord is within a sector calculated on a bearing from a original coordinate.
        /// </summary>
        /// <param name="posCenter">Coordinate that is the origin of the sector</param>
        /// <param name="centerDeg">The line (in degrees) determining the center of the sector, from posCenter</param>
        /// <param name="sectorRangeDeg">The range of the sector in degrees. If value is 30, 15 deg on each side.</param>
        /// <param name="posToCheck">Coordinate of position to check</param>
        /// <returns></returns>
        public static bool IsWithinSector(Coordinate posCenter, double centerDeg, double sectorRangeDeg, Coordinate posToCheck)
        {
            if (sectorRangeDeg >= 360)
            {
                return true;
            }
            double angleToPosToChechDeg = CalculateBearingDegrees(posCenter, posToCheck);
            double angleToPosToCheckRad = angleToPosToChechDeg.ToRadian();
            
            double halfSector = sectorRangeDeg.ToRadian() / 2;
            return IsAngleWithinRangeRad(angleToPosToCheckRad, 
                (centerDeg.ToRadian() - halfSector), 
                (centerDeg.ToRadian() + halfSector));
        }

        /// <summary>
        /// Calculates the sum of two bearing degrees.
        /// </summary>
        /// <param name="platformBearingDeg"></param>
        /// <param name="componentBearingDeg"></param>
        /// <returns></returns>
        public static double CalculateCombinedBearingDeg(double platformBearingDeg, double componentBearingDeg)
        {
            return (platformBearingDeg.ToRadian() + componentBearingDeg.ToRadian()).ToDegreeBearing();
        }

        public static Coordinate CalculateNewPosition2(Coordinate coord, double bearingDeg, double distanceM) 
        {
            //http://www.movable-type.co.uk/scripts/latlong-vincenty-direct.html
            double s = distanceM;
            double alpha1 = bearingDeg.ToRadian();
            double sinAlpha1 = Math.Sin(alpha1), cosAlpha1 = Math.Cos(alpha1);

            double tanU1 = WGS84_ONE_MINUS_FLATTENING * Math.Tan(coord.LatitudeRad);
            double cosU1 = 1 / Math.Sqrt((1 + tanU1 * tanU1)), sinU1 = tanU1 * cosU1;
            double sigma1 = Math.Atan2(tanU1, cosAlpha1);
            double sinAlpha = cosU1 * sinAlpha1;
            double cosSqAlpha = 1 - sinAlpha * sinAlpha;
            double uSq = cosSqAlpha * (WGS84_MAJOR_AXIS_SQUARED - WGS84_MINOR_AXIS_SQUARED) / (WGS84_MINOR_AXIS_SQUARED);
            double A = 1 + uSq / 16384 * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
            double B = uSq / 1024 * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));

            double sigmaOrg = s / (WGS84_MINOR_AXIS * A);
            double sigma = sigmaOrg, sigmaP = 2 * Math.PI;
            double sinSigma=0;
            double cosSigma=0;
            double cos2SigmaM=0;
            while (Math.Abs(sigma - sigmaP) > 1e-12)
            {
                cos2SigmaM = Math.Cos(2 * sigma1 + sigma);
                double cos2SigmaMSq = cos2SigmaM * cos2SigmaM;
                sinSigma = Math.Sin(sigma); 
                cosSigma = Math.Cos(sigma);
                double deltaSigma = B * sinSigma * (cos2SigmaM + B / 4 * (cosSigma * (-1 + 2 * cos2SigmaMSq) -
                  B / 6 * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaMSq)));
                sigmaP = sigma;
                sigma = sigmaOrg + deltaSigma;
            }

            double cosU1Sigma = cosU1 * cosSigma;
            double sinU1Sigma = sinU1 * sinSigma;

            double tmp = sinU1Sigma - cosU1Sigma * cosAlpha1;
            double lat2 = Math.Atan2(sinU1 * cosSigma + cosU1 * sinSigma * cosAlpha1,
                WGS84_ONE_MINUS_FLATTENING * Math.Sqrt(sinAlpha * sinAlpha + tmp * tmp));
            double lambda = Math.Atan2(sinSigma * sinAlpha1, cosU1Sigma - sinU1Sigma * cosAlpha1);
            double C = WGS84_FLATTENING / 16 * cosSqAlpha * (4 + WGS84_FLATTENING * (4 - 3 * cosSqAlpha));
            double L = lambda - (1 - C) * WGS84_FLATTENING * sinAlpha *
                (sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM)));

            double revAz = Math.Atan2(sinAlpha, -tmp);  // final bearing
            //coord = new Coordinate();
            //coord.LatitudeRad = lat2;
            //coord.LongitudeRad = L;
            //return coord;
            double lon1 = coord.LongitudeDeg;
            lon1 += L.ToDegreeSignedLongitude(); //TODO: This can still cause lon < -180
            return new Coordinate(lat2.ToDegreeSignedLatitude(), lon1);
        }

        public static Coordinate CalculateNewPositionApprox(Coordinate oldPos, double bearingDeg, double distanceMeters)
        {
            //again, from: http://www.movable-type.co.uk/scripts/latlong.html
            //d/R is the angular distance (in radians), where d is the distance travelled and R is the earth’s radius

            //var lat2 = Math.asin(Math.sin(lat1) * Math.cos(d / R) +
            //          Math.cos(lat1) * Math.sin(d / R) * Math.cos(brng));
            //var lon2 = lon1 + Math.atan2(Math.sin(brng) * Math.sin(d / R) * Math.cos(lat1),
            //                             Math.cos(d / R) - Math.sin(lat1) * Math.sin(lat2));

            double bearingRad = bearingDeg.ToRadian();
            double Lat1 = oldPos.LatitudeRad;
            double Lon1 = oldPos.LongitudeRad;
            double AngularDistance = distanceMeters / WGS84_MAJOR_AXIS;

            double sinLat1 = Math.Sin( Lat1 );
            double cosLat1 = Math.Cos( Lat1 );

            double sinAngularDistance = Math.Sin( AngularDistance );
            double cosAngularDistance = Math.Cos( AngularDistance );

            double lat2 = ( Math.Asin( sinLat1 * cosAngularDistance ) +
                          ( cosLat1 * sinAngularDistance * Math.Cos( bearingRad ) ) );

            double lon2 = Lon1 + ( Math.Atan2( Math.Sin( bearingRad ) * sinAngularDistance * cosLat1,
                          cosAngularDistance - ( sinLat1 * Math.Sin( lat2 ) ) ) );

            return new Coordinate(lat2.ToDegreeSignedLatitude(), lon2.ToDegreeSignedLongitude());
        }

        public static Coordinate CalculatePositionInRange(Coordinate coord1, Coordinate coord2, double desiredRangeM)
        {
            double CurrentDistanceM = CalculateDistanceM(coord1, coord2);
            double DistanceToClose = CurrentDistanceM - desiredRangeM;
            if(DistanceToClose < 1)
            {
                return coord1;
            }
            double AngleDeg = CalculateBearingDegrees(coord1, coord2);
            return CalculateNewPosition2(coord1, AngleDeg, DistanceToClose); 
        }

        public static Coordinate CalculateMidpoint(Coordinate coord1, Coordinate coord2)
        {
            //var Bx = Math.cos(lat2) * Math.cos(dLon);
            //var By = Math.cos(lat2) * Math.sin(dLon);
            //var lat3 = Math.atan2(Math.sin(lat1) + Math.sin(lat2),
            //                      Math.sqrt((Math.cos(lat1) + Bx) * (Math.cos(lat1) + Bx) +
            //                             By * By));
            //var lon3 = lon1.toRad() + Math.atan2(By, Math.cos(lat1) + Bx);

            double dLat = (coord2.LatitudeDeg - coord1.LatitudeDeg).ToRadian();
            double dLon = (coord2.LongitudeDeg - coord1.LongitudeDeg).ToRadian();
            double Bx = Math.Cos(coord2.LatitudeDeg) * Math.Cos(dLon);
            double By = Math.Cos(coord2.LatitudeDeg) * Math.Sin(dLon);
            double Lat3 = Math.Atan2(Math.Sin(coord1.LatitudeDeg) + Math.Sin(coord2.LatitudeDeg),
                          Math.Sqrt((Math.Cos(coord1.LatitudeDeg) + Bx) * Math.Cos(coord1.LatitudeDeg) + Bx) +
                          (By * By));
            double Lon3 = coord1.LongitudeDeg.ToRadian() + Math.Atan2(By, Math.Cos(coord1.LatitudeDeg) + Bx);

            Coordinate NewCoord = new Coordinate(Lat3, Lon3);
            //TODO: Check for conversion to Radian!
            return NewCoord;
        }

        public static PositionOffset CalculateOffsetFromPosition(Position position, Position positionFollowed)
        {
            if (position == null || positionFollowed == null 
                || position.Coordinate == null || positionFollowed.Coordinate == null
                || !position.HasBearing || !positionFollowed.HasBearing)
            {
                return null;
            }
            PositionOffset offset = new PositionOffset();
            if (position.HasHeightOverSeaLevel && positionFollowed.HasHeightOverSeaLevel)
            {
                offset.UpM = (double)positionFollowed.HeightOverSeaLevelM - (double)position.HeightOverSeaLevelM;
            }
            //TODO: Implement RightM and ForwardM
            
            return offset;
        }

        /// <summary>
        /// Calculates the elevation angle between two positions. 0 is defined as vertically aligned 
        /// (height is equal). If HeightOverSeaLevel is undefined, 0 is also returned. Angle will be negative if 
        /// pos2 is lower than pos1, otherwise positive.
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static double CalculateElevationAngleDeg(Position pos1, Position pos2)
        {
            if (pos1 == null || pos2 == null || !pos1.HasHeightOverSeaLevel || !pos2.HasHeightOverSeaLevel)
            {
                return 0;
            }
            double distanceM = MapHelper.CalculateDistance3DM(pos1, pos2);
            double heightDiffM = Math.Abs((double)pos1.HeightOverSeaLevelM - (double)pos2.HeightOverSeaLevelM);
            double angleRad = Math.Tan(heightDiffM / distanceM);
            if (pos1.HeightOverSeaLevelM <= pos2.HeightOverSeaLevelM)
            {
                return angleRad.ToDegreeBearing();
            }
            else
            {
                return (angleRad.ToDegreeBearing()) * -1;
            }

        }
        public static Position CalculatePositionFromOffset2(Position positionToFollow, PositionOffset offset)
        {
            //Position pos = UnitToFollow.Position;
            //Position desiredPos = new Position();
            Debug.Assert(positionToFollow != null && positionToFollow.Coordinate != null, "PositionToFollow should not be null.");
            Debug.Assert(offset != null, "PositionOffset should not be null.");
            double bearingFollowRad = 0;
            if (positionToFollow.HasBearing)
            {
                bearingFollowRad = positionToFollow.BearingDeg.Value.ToRadian();
            }
            double distanceM = Math.Sqrt(Math.Pow(offset.RightM, 2) + Math.Pow(offset.ForwardM, 2));
            //double relativeBearingToTargetRad = Math.Acos(Math.Abs(offset.RightM) / Math.Abs(distanceM));//Math.Atan2(offset.ForwardM, offset.RightM);
            double relativeBearingToTargetRad = Math.Atan2(offset.ForwardM, offset.RightM);
            if (double.IsNaN(relativeBearingToTargetRad))
            {
                return positionToFollow.Clone();
            }
            double relativeBearingToTargetDeg = -(relativeBearingToTargetRad * 180 / Math.PI - 90);

            //double slope = offset.ForwardM / offset.RightM;
            //double angleRad = Math.Atan(slope);
            //if (offset.RightM >= 0 && offset.ForwardM < 0) //SE
            //{
            //    relativeBearingToTargetRad += Math.PI / 2;
            //}
            //else if (offset.ForwardM >= 0 && offset.RightM < 0) //NW
            //{
            //    double newDeg = 360 - relativeBearingToTargetRad.ToDegreeBearing();
            //    relativeBearingToTargetRad = newDeg.ToRadian();
            //}
            //else if (offset.RightM < 0 && offset.ForwardM < 0) //SW
            //{
            //    relativeBearingToTargetRad += Math.PI;
            //}
            //double relativeBearingToTargetDeg = relativeBearingToTargetRad.ToDegreeBearing();

            double completeBearingDeg = MapHelper.CalculateCombinedBearingDeg(
                relativeBearingToTargetDeg, bearingFollowRad.ToDegreeBearing());
            Coordinate newCoordinate = MapHelper.CalculateNewPosition2(positionToFollow.Coordinate, completeBearingDeg, distanceM);
            Position pos = new Position(newCoordinate);
            pos.BearingDeg = positionToFollow.BearingDeg;
            if(positionToFollow.HasHeightOverSeaLevel)
            {
                pos.HeightOverSeaLevelM = positionToFollow.HeightOverSeaLevelM + offset.UpM;
            }
            return pos; 
        }

        //[Obsolete]
        //private static Position CalculatePositionFromOffset(Position positionToFollow, PositionOffset offset)
        //{
        //    //Position pos = UnitToFollow.Position;
        //    Position desiredPos = new Position();
        //    Debug.Assert(positionToFollow != null, "PositionToFollow should not be null.");
        //    Debug.Assert(offset != null, "PositionOffset should not be null.");
        //    double bearingRad = 0;
        //    if (positionToFollow.HasBearing)
        //    {
        //        bearingRad = positionToFollow.BearingDeg.Value.ToRadian();
        //    }
        //    //_collisionMap.x += Math.sin(_player.rotation * (Math.PI / 180)) * _player.Speed * -1;
        //    //_collisionMap.y += Math.cos(_player.rotation * (Math.PI / 180)) * _player.Speed;
        //    double diagonal = 90.0;
        //    double DistanceM = Math.Sqrt(Math.Pow(offset.RightM, 2) + Math.Pow(offset.ForwardM, 2));
        //    if (offset.RightM < 0 || offset.ForwardM < 0)//Hmm... TI
        //    {
        //        DistanceM *= -1;
        //    }
        //    if (offset.RightM < 0)
        //    {
        //        diagonal = 180.0;
        //    }

        //    Coordinate NotNormalizedPosition = MapHelper.CalculateNewPosition2(positionToFollow.Coordinate, diagonal,
        //        DistanceM);

        //    double BearingToPositionRad = MapHelper.CalculateBearingDegrees(positionToFollow.Coordinate, NotNormalizedPosition).ToRadian();
        //    BearingToPositionRad += bearingRad; //or + ?

        //    // rotate direction
        //    if (offset.RightM < 0 || offset.ForwardM < 0)
        //    {
        //        //TODO:HMMM...  
        //        double bearingDeg = BearingToPositionRad.ToDegreeBearing();
        //        bearingDeg = bearingDeg + 180 > 360 ? bearingDeg + 180 - 360 : bearingDeg + 180;
        //        double bearRad = bearingDeg.ToRadian();
        //        BearingToPositionRad = bearRad;
        //    }

        //    desiredPos = new Position(MapHelper.CalculateNewPosition2(positionToFollow.Coordinate, 
        //        BearingToPositionRad.ToDegreeBearing(), DistanceM));
        //    desiredPos.HeightOverSeaLevelM = positionToFollow.HeightOverSeaLevelM + offset.UpM;
        //    desiredPos.SetNewBearing((double)positionToFollow.BearingDeg);

        //    return desiredPos;
        //}


        //public static InterceptionData CalculateInterceptionCoordinate(Coordinate interceptorCoordinate,
        //    double interceptorSpeedKph, Coordinate targetCoordinate, double targetBearingDeg,
        //    double targetSpeedKph, double desiredDistanceToTargetM)
        //{

        //}

        /// <summary>
        /// Privates for lat long to pixel format
        /// for image tiles width 256 * 256 size.
        /// </summary>
        private static double tileSize = 256;
        private static double tiles;
        private static double circumrefrence;
        private static double radius;

        /// <summary>
        /// Used to get the X pixel value for worldmap.
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="zoomLevel"></param>
        /// <returns></returns>
        public static double LongitudeToXvalue(double longitude, int zoomLevel)
        {
            tiles = Math.Pow(2, zoomLevel);
            circumrefrence = tileSize * tiles;
            radius = circumrefrence / (2 * Math.PI);

            double Longitude = longitude * (Math.PI / 180);
            return (radius * Longitude);
        }
        /// <summary>
        /// Used to get the X pixel value for worldmap.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="ZoomLevel"></param>
        /// <returns></returns>
        public static double LongitudeToXvalue(Coordinate coord, int ZoomLevel)
        {
            tiles = Math.Pow(2, ZoomLevel);
            circumrefrence = tileSize * tiles;
            radius = circumrefrence / (2 * Math.PI);
            double longitude = coord.LongitudeDeg * (Math.PI / 180);
            return (radius * longitude);
        }
        /// <summary>
        /// Used to get the Y pixel value for worldmap.
        /// </summary>
        /// <param name="Latitude"></param>
        /// <param name="ZoomLevel"></param>
        /// <returns></returns>
        public static double LatitudeToYvalue(double Latitude, int ZoomLevel)
        {
            // REFRANCE: http://cfis.savagexi.com/2006/05/03/google-maps-deconstructed
            tiles = Math.Pow(2, ZoomLevel);
            circumrefrence = tileSize * tiles;
            radius = circumrefrence / (2 * Math.PI);
            var latitude = Latitude * (Math.PI / 180);
            double Y = radius / 2.0 * Math.Log((1.0 + Math.Sin(latitude)) / (1.0 - Math.Sin(latitude)));
            return Y * -1;
        }
        /// <summary>
        /// Used to get the Y pixel value for worldmap.
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="ZoomLevel"></param>
        /// <returns></returns>
        public static double LatitudeToYvalue(Coordinate coord, int ZoomLevel)
        {
            // REFRANCE: http://cfis.savagexi.com/2006/05/03/google-maps-deconstructed
            tiles = Math.Pow(2, ZoomLevel);
            circumrefrence = tileSize * tiles;
            radius = circumrefrence / (2 * Math.PI);
            var latitude = coord.LatitudeDeg * (Math.PI / 180);
            double Y = radius / 2.0 * Math.Log((1.0 + Math.Sin(latitude)) / (1.0 - Math.Sin(latitude)));
            return Y * -1;
        }

        public static double xToLong(double x)
        {
            // REFRANCE: http://cfis.savagexi.com/2006/05/03/google-maps-deconstructed
            tiles = Math.Pow(2, 9);
            circumrefrence = tileSize * tiles;
            radius = circumrefrence / (2 * Math.PI);
            double longRadians = x / radius;
            double longDegrees = ToDegreeSignedLongitude(longRadians);

            /* The user could have panned around the world a lot of times.
            Lat long goes from -180 to 180.  So every time a user gets 
            to 181 we want to subtract 360 degrees.  Every time a user
            gets to -181 we want to add 360 degrees. */

            double rotations = Math.Floor((longDegrees + 180) / 360);
            double longitude = longDegrees - (rotations * 360);
            return longitude;

        }

        public static double yToLat(double y)
        {
            tiles = Math.Pow(2, 9);
            circumrefrence = tileSize * tiles;
            radius = circumrefrence / (2 * Math.PI);
             var latitude =  (Math.PI/2) - (2 * Math.Atan(Math.Exp(-1.0 * y / radius)));
             return ToDegreeSignedLatitude(latitude);
        }

        public static double CalculateAcceleration(double PAtTMinus2, double PAtTMinus1, double PAtTMinus0)
        {
            return PAtTMinus2 - 2.0 * PAtTMinus1 + PAtTMinus0;
        }

        public static double CalculateVelocity(double PAtTMinus2, double PAtTMinus1, double PAtTMinus0)
        {
            return PAtTMinus2 / 2.0 - 2.0 * PAtTMinus1 + 3.0 * PAtTMinus0 / 2.0;
        }

        public static bool Intercept(double targetAxelerationX, double targetaxelerationY, double targetAxelerationZ, double targetVelocityX, double targetVelocityY, double targetVelocityZ, double targetPositionX, double targetPositionY, double targetPositionZ, double shooterVelocityX, double shooterVelocityY, double shooterVelocityZ, double shooterPositionX, double shooterPositionY, double shooterPositionZ, double intersepterLounchAcceleration, double intercepterLounchVelocity, double intercepterLouncherLenght, int intersepeterTimeStepFuel, double tolerance, out double[,] vectors,out double angle)
        {
            double[] garbage;
            
            return Intercept(targetAxelerationX, targetaxelerationY, targetAxelerationZ, targetVelocityX, targetVelocityY, targetVelocityZ, targetPositionX, targetPositionY, targetPositionZ, shooterVelocityX, shooterVelocityY, shooterVelocityZ, shooterPositionX, shooterPositionY, shooterPositionZ, intersepterLounchAcceleration, intercepterLounchVelocity, intercepterLouncherLenght, intersepeterTimeStepFuel, tolerance, out vectors, out garbage, out angle);
        }

        public static bool Intercept(double targetAxelerationX, double targetaxelerationY, double targetAxelerationZ, double targetVelocityX, double targetVelocityY, double targetVelocityZ, double targetPositionX, double targetPositionY, double targetPositionZ, double shooterVelocityX, double shooterVelocityY, double shooterVelocityZ, double shooterPositionX, double shooterPositionY, double shooterPositionZ, double intersepterLounchAcceleration, double intercepterLounchVelocity, double intercepterLouncherLenght, int intersepeterTimeStepFuel, double tolerance, out double[,] vectors, out double[] tui, out double angle)
        {
            targetVelocityX -= shooterVelocityX;
            targetVelocityY -= shooterVelocityY;
            targetVelocityZ -= shooterVelocityZ;
            targetPositionX -= shooterPositionX;
            targetPositionY -= shooterPositionY;
            targetPositionZ -= shooterPositionZ;
            if (tolerance < 1.0)
                tolerance = 1.0;
            if (intersepeterTimeStepFuel < 0)
                intersepeterTimeStepFuel = 0;
            if (intersepeterTimeStepFuel == 0)
                intersepterLounchAcceleration = 0.0;
            double tA = 0.25 * (targetAxelerationX * targetAxelerationX + targetaxelerationY * targetaxelerationY + targetAxelerationZ * targetAxelerationZ);
            double tB = targetAxelerationX * targetVelocityX + targetaxelerationY * targetVelocityY + targetAxelerationZ * targetVelocityZ;
            double tC = targetVelocityX * targetVelocityX + targetAxelerationX * targetPositionX + targetVelocityY * targetVelocityY + targetaxelerationY * targetPositionY + targetVelocityZ * targetVelocityZ + targetAxelerationZ * targetPositionZ;
            double tD = 2.0 * (targetVelocityX * targetPositionX + targetVelocityY * targetPositionY + targetVelocityZ * targetPositionZ);
            double tE = targetPositionX * targetPositionX + targetPositionY * targetPositionY + targetPositionZ * targetPositionZ;
            double fsA = tA - 0.25 * intersepterLounchAcceleration * intersepterLounchAcceleration;
            double fsB = tB - intersepterLounchAcceleration * intercepterLounchVelocity;
            double fsC = tC - intersepterLounchAcceleration * intercepterLouncherLenght - intercepterLounchVelocity * intercepterLounchVelocity;
            double fsD = tD - 2.0 * intercepterLounchVelocity * intercepterLouncherLenght;
            double fsE = tE - intercepterLouncherLenght * intercepterLouncherLenght;
            double ssC = tC - intersepterLounchAcceleration * intersepterLounchAcceleration * intersepeterTimeStepFuel * intersepeterTimeStepFuel - 2.0 * intersepterLounchAcceleration * intercepterLounchVelocity * intersepeterTimeStepFuel - intercepterLounchVelocity * intercepterLounchVelocity;
            double ssD = tD + intersepterLounchAcceleration * intersepterLounchAcceleration * intersepeterTimeStepFuel * intersepeterTimeStepFuel * intersepeterTimeStepFuel + intersepterLounchAcceleration * intercepterLounchVelocity * intersepeterTimeStepFuel * intersepeterTimeStepFuel - 2.0 * (intersepterLounchAcceleration * intersepeterTimeStepFuel + intercepterLounchVelocity) * intercepterLouncherLenght;
            double ssE = tE - 0.25 * intersepterLounchAcceleration * intersepterLounchAcceleration * intersepeterTimeStepFuel * intersepeterTimeStepFuel * intersepeterTimeStepFuel * intersepeterTimeStepFuel + intersepterLounchAcceleration * intercepterLouncherLenght * intersepeterTimeStepFuel * intersepeterTimeStepFuel - intercepterLouncherLenght * intercepterLouncherLenght;
            List<double> fsroots = rootsOf(fsA, fsB, fsC, fsD, fsE);
            List<double> fsrootsCA = rootsOf(0.0, 4.0 * fsA, 3.0 * fsB, 2.0 * fsC, fsD);
            List<double> ssroots = rootsOf(tA, tB, ssC, ssD, ssE);
            List<double> ssrootsCA = rootsOf(0.0, 4.0 * tA, 3.0 * tB, 2.0 * ssC, ssD);
            List<double> roots = new List<double>();
            foreach (double root in fsroots)
                if (root >= 0.0 && root <= intersepeterTimeStepFuel && !roots.Contains(root))
                    roots.Add(root);
            foreach (double root in ssroots)
                if (root > intersepeterTimeStepFuel && !roots.Contains(root))
                    roots.Add(root);
            foreach (double root in fsrootsCA)
                if (tolerance >= Math.Abs(fsA * root * root * root * root + fsB * root * root * root + fsC * root * root + fsD * root + fsE) && root >= 0.0 && root <= intersepeterTimeStepFuel && !roots.Contains(root))
                    roots.Add(root);
            foreach (double root in ssrootsCA)
                if (tolerance >= Math.Abs(tA * root * root * root * root + tB * root * root * root + ssC * root * root + ssD * root + ssE) && root > intersepeterTimeStepFuel && !roots.Contains(root))
                    roots.Add(root);
            roots.Sort();
            tui = roots.ToArray();
            bool able = roots.Count != 0;
            if (!able)
            {
                List<double> fsattempts = new List<double>();
                List<double> ssattempts = new List<double>();
                foreach (double root in fsrootsCA)
                    if (root >= 0.0 && root <= intersepeterTimeStepFuel && !fsattempts.Contains(root))
                        fsattempts.Add(root);
                fsattempts.Add(0);
                foreach (double root in ssrootsCA)
                    if (root > intersepeterTimeStepFuel && !ssattempts.Contains(root))
                        ssattempts.Add(root);
                double[] keys = new double[fsattempts.Count + ssattempts.Count];
                double[] items = new double[fsattempts.Count + ssattempts.Count];
                for (int i = 0; i != fsattempts.Count; i++)
                {
                    keys[i] = Math.Abs(fsA * fsattempts[i] * fsattempts[i] * fsattempts[i] * fsattempts[i] + fsB * fsattempts[i] * fsattempts[i] * fsattempts[i] + fsC * fsattempts[i] * fsattempts[i] + tD * fsattempts[i] + tE);
                    items[i] = fsattempts[i];
                }
                for (int i = 0; i != ssattempts.Count; i++)
                {
                    keys[fsattempts.Count + i] = Math.Abs(tA * ssattempts[i] * ssattempts[i] * ssattempts[i] * ssattempts[i] + tB * ssattempts[i] * ssattempts[i] * ssattempts[i] + ssC * ssattempts[i] * ssattempts[i] + ssD * ssattempts[i] + ssE);
                    items[fsattempts.Count + i] = ssattempts[i];
                }
                Array.Sort(keys, items);
                roots.AddRange(items);
            }
            vectors = new double[roots.Count, 3];
            for (int i = 0; i != roots.Count; i++)
            {
                double x;
                double y;
                double z;
                if (intersepterLounchAcceleration == 0.0 && intercepterLounchVelocity == 0.0)
                {
                    x = shooterVelocityX;
                    y = shooterVelocityY;
                    z = shooterVelocityZ;
                }
                else
                {
                    x = 0.5 * targetAxelerationX * roots[i] * roots[i] + targetVelocityX * roots[i] + targetPositionX;
                    y = 0.5 * targetaxelerationY * roots[i] * roots[i] + targetVelocityY * roots[i] + targetPositionY;
                    z = 0.5 * targetAxelerationZ * roots[i] * roots[i] + targetVelocityZ * roots[i] + targetPositionZ;
                }
                double m = Math.Sqrt((x * x + y * y + z * z));
                if (m < double.Epsilon)
                    m = 1.0;
                vectors[i, 0] = x / m;
                vectors[i, 1] = y / m;
                vectors[i, 2] = z / m;
            }
            angle = Math.Atan2(vectors[0, 0], vectors[0, 1]);
            return able;
        }

        private static List<double> rootsOf(double A, double B, double C, double D, double E)
        {
            List<double> roots = new List<double>();
            if (A == 0.0)
            {
                if (B == 0.0)
                {
                    if (C == 0.0)
                    {
                        if (D == 0.0)
                        {
                            if (E == 0.0)
                                roots.Add(0.0);
                        }
                        else
                            roots.Add(-D / E);
                    }
                    else
                        roots.AddRange(new double[] { (-D + Math.Sqrt(D * D - 4 * C * E)) / (2.0 * C), (-D - Math.Sqrt(D * D - 4 * C * E)) / (2.0 * C) });
                }
                else
                {
                    C /= B;
                    D /= B;
                    E /= B;
                    double F = (3.0 * D - C * C) / 3.0;
                    double G = (2.0 * C * C * C - 9.0 * C * D + 27.0 * E) / 27.0;
                    double H = (G * G) / 4.0 + (F * F * F) / 27.0;
                    if (H > 0)
                    {
                        double intermediate = -G / 2.0 + Math.Sqrt(H);
                        double m = intermediate < 0.0 ? -Math.Pow(-intermediate, 1.0 / 3.0) : Math.Pow(intermediate, 1.0 / 3.0);
                        intermediate -= 2.0 * Math.Sqrt(H);
                        double n = intermediate < 0.0 ? -Math.Pow(-intermediate, 1.0 / 3.0) : Math.Pow(intermediate, 1.0 / 3.0);
                        roots.Add(m + n - C / 3.0);
                    }
                    else
                    {
                        double intermediate = Math.Sqrt(G * G / 4.0 - H);
                        double rc = intermediate < 0.0 ? -Math.Pow(-intermediate, 1.0 / 3.0) : Math.Pow(intermediate, 1.0 / 3.0);
                        double theta = Math.Acos(-G / (2.0 * intermediate)) / 3.0;
                        roots.AddRange(new double[] { 2.0 * rc * Math.Cos(theta) - C / 3.0, -rc * (Math.Cos(theta) + Math.Sqrt(3.0) * Math.Sin(theta)) - C / 3.0, -rc * (Math.Cos(theta) - Math.Sqrt(3.0) * Math.Sin(theta)) - C / 3.0 });
                    }
                    if (F + G + H == 0.0)
                    {
                        double intermediate = E < 0.0 ? Math.Pow(-E, 1.0 / 3.0) : -Math.Pow(E, 1.0 / 3.0);
                        roots.Clear();
                        roots.AddRange(new double[] { intermediate, intermediate, intermediate });
                    }
                }
            }
            else
            {
                B /= A;
                C /= A;
                D /= A;
                E /= A;
                double F = C - (3.0 * B * B) / 8.0;
                double G = D + B * B * B / 8.0 - (B * C) / 2.0;
                double H = E - 3.0 * B * B * B * B / 256.0 + B * B * C / 16.0 - B * D / 4.0;
                double b = F / 2.0;
                double c = (F * F - 4.0 * H) / 16.0;
                double d = (G * G) / -64.0;
                double f = (3.0 * c - b * b) / 3.0;
                double g = (2.0 * b * b * b - 9.0 * b * c + 27.0 * d) / 27.0;
                double h = (g * g) / 4.0 + (f * f * f) / 27.0;
                double y1;
                double y2r;
                double y2i;
                double y3r;
                double y3i;
                if (h > 0.0)
                {
                    double intermediate = -g / 2.0 + Math.Sqrt(h);
                    double m = intermediate < 0.0 ? -Math.Pow(-intermediate, 1.0 / 3.0) : Math.Pow(intermediate, 1.0 / 3.0);
                    intermediate -= 2.0 * Math.Sqrt(h);
                    double n = intermediate < 0.0 ? -Math.Pow(-intermediate, 1.0 / 3.0) : Math.Pow(intermediate, 1.0 / 3.0);
                    y1 = m + n - b / 3.0;
                    y2r = (m + n) / -2.0 - b / 3.0;
                    y2i = ((m - n) / 2.0) * Math.Sqrt(3.0);
                    y3r = (m + n) / -2.0 - b / 3.0;
                    y3i = ((m - n) / 2.0) * Math.Sqrt(3.0);
                }
                else
                {
                    double intermediate = Math.Sqrt((g * g / 4.0 - h));
                    double rc = intermediate < 0.0 ? -Math.Pow(-intermediate, 1.0 / 3.0) : Math.Pow(intermediate, 1.0 / 3.0);
                    double theta = Math.Acos((-g / (2.0 * intermediate))) / 3.0;
                    y1 = 2.0 * rc * Math.Cos(theta) - b / 3.0;
                    y2r = -rc * (Math.Cos(theta) + Math.Sqrt(3.0) * Math.Sin(theta)) - b / 3.0;
                    y2i = 0.0;
                    y3r = -rc * (Math.Cos(theta) - Math.Sqrt(3.0) * Math.Sin(theta)) - b / 3.0;
                    y3i = 0.0;
                }
                if (f + g + h == 0.0)
                {
                    double intermediate = d < 0.0 ? Math.Pow(-d, 1.0 / 3.0) : -Math.Pow(d, 1.0 / 3.0);
                    y1 = intermediate;
                    y2r = intermediate;
                    y2i = 0.0;
                    y3r = intermediate;
                    y3i = 0.0;
                }
                double p;
                double q;
                if (h <= 0.0)
                {
                    int zeroCheck = 0;
                    double[] cubicRoots = new double[] { y1, y2r, y3r };
                    Array.Sort(cubicRoots);
                    p = Math.Sqrt(cubicRoots[1]);
                    q = Math.Sqrt(cubicRoots[2]);
                    if (Math.Round(y1, 13) == 0.0)
                    {
                        p = Math.Sqrt(y2r);
                        q = Math.Sqrt(y3r);
                        zeroCheck = 1;
                    }
                    if (Math.Round(y2r, 13) == 0.0)
                    {
                        p = Math.Sqrt(y1);
                        q = Math.Sqrt(y3r);
                        zeroCheck += 2;
                    }
                    if (Math.Round(y3r, 13) == 0.0)
                    {
                        p = Math.Sqrt(y1);
                        q = Math.Sqrt(y2r);
                        zeroCheck += 4;
                    }
                    switch (zeroCheck)
                    {
                        case (3):
                            p = Math.Sqrt(y3r);
                            break;
                        case (5):
                            p = Math.Sqrt(y2r);
                            break;
                        case (6):
                            p = Math.Sqrt(y1);
                            break;
                    }
                    if (Math.Round(y1, 13) < 0.0 || Math.Round(y2r, 13) < 0.0 || Math.Round(y3r, 13) < 0.0)
                    {
                        if (E == 0.0)
                            roots.Add(0.0);
                    }
                    else
                    {
                        double r;
                        if (zeroCheck < 5)
                            r = G / (-8.0 * p * q);
                        else
                            r = 0.0;
                        double s = B / 4.0;
                        roots.AddRange(new double[] { p + q + r - s, p - q - r - s, -p + q - r - s, -p - q + r - s });
                    }
                }
                else
                {
                    double r2mod = Math.Sqrt(y2r * y2r + y2i * y2i);
                    double y2mod = Math.Sqrt((r2mod - y2r) / 2.0);
                    double x2mod = y2i / (2.0 * y2mod);
                    p = x2mod + y2mod;
                    double r3mod = Math.Sqrt(y3r * y3r + y3i * y3i);
                    double y3mod = Math.Sqrt((r3mod - y3r) / 2.0);
                    double x3mod = y3i / (2.0 * y3mod);
                    q = x3mod + y3mod;
                    double r = G / (-8.0 * (x2mod * x3mod + y2mod * y3mod));
                    double s = B / 4.0;
                    roots.AddRange(new double[] { x2mod + x3mod + r - s, -x2mod - x3mod + r - s });
                }
            }
            for (int i = 0; i != roots.Count; i++)
                if (double.IsInfinity(roots[i]) || double.IsNaN(roots[i]))
                    roots.RemoveAt(i--);
            roots.Sort();
            return roots;
        }





    }
}
