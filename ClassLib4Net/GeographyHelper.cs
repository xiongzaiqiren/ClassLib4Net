using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClassLib4Net
{
    /// <summary>
	/// 地理坐标及位置助手
	/// 熊学浩
	/// 2016-05-20
	/// </summary>
    public class GeographyHelper
    {
        #region MBR-1
        /// <summary>
        /// 赤道半径（m）
        /// </summary>
        public const double Ea = 6378137;
        /// <summary>
        /// 极半径（m）
        /// </summary>
        public const double Eb = 6356725;

        private static void GetLatLon(double LAT, double LON, double distance, double angle, out double newLon, out double newLat)
        {
            double dx = distance * 1000 * Math.Sin(angle * Math.PI / 180.0);
            double dy = distance * 1000 * Math.Cos(angle * Math.PI / 180.0);
            double ec = Eb + (Ea - Eb) * (90.0 - LAT) / 90.0;
            double ed = ec * Math.Cos(LAT * Math.PI / 180);
            newLon = (dx / ed + LON * Math.PI / 180.0) * 180.0 / Math.PI;
            newLat = (dy / ec + LAT * Math.PI / 180.0) * 180.0 / Math.PI;
        }

        /// <summary>
        /// 这个根据一个经纬度坐标、距离然后求另外一个经纬度坐标的作用，主要就是确定一个最小外包矩形(Minimum bounding rectangle，简称MBR)。例如，我要找一个坐标点(lat,lon)的5公里范围内的所有商户信息、景点信息等。这个MBR就是一个最大的范围，这个矩形是包含5公里范围内所有这些有效信息的一个最小矩形。利用公式，求出四个方向0度、90度、180度、270度方向上的四个坐标点就可以得到这个MBR
        /// </summary>
        /// <param name="centorlatitude">中心点纬度</param>
        /// <param name="centorLogitude">中心点经度</param>
        /// <param name="distance">距离（km）</param>
        /// <param name="maxLatitude">最大纬度</param>
        /// <param name="minLatitude">最小纬度</param>
        /// <param name="maxLongitude">最大经度</param>
        /// <param name="minLongitude">最小经度</param>
        public static void GetRectRange(double centorlatitude, double centorLogitude, double distance, out double maxLatitude, out double minLatitude, out double maxLongitude, out double minLongitude)
        {
            double temp = 0.0;
            GetLatLon(centorlatitude, centorLogitude, distance, 0, out temp, out maxLatitude);
            GetLatLon(centorlatitude, centorLogitude, distance, 180, out temp, out minLatitude);
            GetLatLon(centorlatitude, centorLogitude, distance, 90, out maxLongitude, out temp);
            GetLatLon(centorlatitude, centorLogitude, distance, 270, out minLongitude, out temp);
        }

        #endregion

        #region MBR-2
        /// <summary>
        /// 这个根据一个经纬度坐标、距离然后求另外一个经纬度坐标的作用，主要就是确定一个最小外包矩形(Minimum bounding rectangle，简称MBR)。例如，我要找一个坐标点(lat,lon)的5公里范围内的所有商户信息、景点信息等。这个MBR就是一个最大的范围，这个矩形是包含5公里范围内所有这些有效信息的一个最小矩形。利用公式，求出四个方向0度、90度、180度、270度方向上的四个坐标点就可以得到这个MBR
        /// </summary>
        /// <param name="centorlatitude">中心点纬度</param>
        /// <param name="centorLogitude">中心点经度</param>
        /// <param name="distance">距离（km）</param>
        /// <param name="maxLatitude">最大纬度</param>
        /// <param name="minLatitude">最小纬度</param>
        /// <param name="maxLongitude">最大经度</param>
        /// <param name="minLongitude">最小经度</param>
        public static void GetRectRange2(double centorlatitude, double centorLogitude, double distance, out double maxLatitude, out double minLatitude, out double maxLongitude, out double minLongitude)
        {
            double temp = 0.0;
            GetLatLon2(centorlatitude, centorLogitude, distance, 0, out maxLatitude, out temp);
            GetLatLon2(centorlatitude, centorLogitude, distance, 180, out minLatitude, out temp);
            GetLatLon2(centorlatitude, centorLogitude, distance, 90, out temp, out maxLongitude);
            GetLatLon2(centorlatitude, centorLogitude, distance, 270, out temp, out minLongitude);
        }

        /// <summary>
        /// where    φ is latitude, λ is longitude, θ is the bearing (clockwise from north),
        /// δ is the angular distance d/R; d being the distance travelled, R the earth’s radius
        /// bearing 方位 0，90，180，270
        /// </summary>
        private static void GetLatLon2(double lat, double lon, double d, double bearing, out double lat2, out double lon2)
        {
            lat2 = 0.0;
            lon2 = 0.0;
            double R = 6378.137; //赤道半径（km）
            var φ1 = ConvertDegreesToRadians(lat);
            var λ1 = ConvertDegreesToRadians(lon);
            var θ = ConvertDegreesToRadians(bearing);
            var φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(d / R) + Math.Cos(φ1) * Math.Sin(d / R) * Math.Cos(θ));
            var λ2 = λ1 + Math.Atan2(Math.Sin(θ) * Math.Sin(d / R) * Math.Cos(φ1), Math.Cos(d / R) - Math.Sin(φ1) * Math.Sin(φ2));
            λ2 = (λ2 + 3 * Math.PI) % (2 * Math.PI) - Math.PI; // normalise to -180..+180°
            lat2 = ConvertRadiansToDegrees(φ2);
            lon2 = ConvertRadiansToDegrees(λ2);
        }

        /// <summary>
        /// 角度转换成弧度
        /// </summary>
        /// <param name="degrees">角度</param>
        /// <returns></returns>
        public static double ConvertDegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
        /// <summary>
        /// 弧度转换成角度
        /// </summary>
        /// <param name="radian">弧度</param>
        /// <returns></returns>
        public static double ConvertRadiansToDegrees(double radian)
        {
            return radian * 180.0 / Math.PI;
        }
        #endregion

        #region Distance
        /// <summary>
        /// 球面上任意两点之间的距离计算公式，Haversine公式采用了正弦函数，即使距离很小，也能保持足够的有效数字。
        /// </summary>
        /// <param name="theta">两点经度的差值</param>
        /// <returns></returns>
        public static double HaverSin(double theta)
        {
            var v = Math.Sin(theta / 2);
            return v * v;
        }

        /// <summary>
        /// 地球半径 平均值，千米km
        /// </summary>
        public const double EARTH_RADIUS = 6371.0;

        /// <summary>
        /// 给定的经度1，纬度1；经度2，纬度2. 计算2个经纬度之间的距离。
        /// </summary>
        /// <param name="lat1">经度1</param>
        /// <param name="lon1">纬度1</param>
        /// <param name="lat2">经度2</param>
        /// <param name="lon2">纬度2</param>
        /// <returns>距离（公里、千米）</returns>
        public static double Distance(double lat1, double lon1, double lat2, double lon2)
        {
            //用haversine公式计算球面两点间的距离。
            //经纬度转换成弧度
            lat1 = ConvertDegreesToRadians(lat1);
            lon1 = ConvertDegreesToRadians(lon1);
            lat2 = ConvertDegreesToRadians(lat2);
            lon2 = ConvertDegreesToRadians(lon2);

            //差值
            var vLon = Math.Abs(lon1 - lon2);
            var vLat = Math.Abs(lat1 - lat2);

            //h is the great circle distance in radians, great circle就是一个球体上的切面，它的圆心即是球心的一个周长最大的圆。
            var h = HaverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * HaverSin(vLon);

            var distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));

            return distance;
        }
        #endregion

    }
}
