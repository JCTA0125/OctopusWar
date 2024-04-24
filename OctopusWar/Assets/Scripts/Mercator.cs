//-----------------------------------------------------------------------
// <copyright file="Mercator.cs" company="Google">
//
// Copyright 2022 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace Google.CreativeLab.BalloonPop 
{
    using System;
    using System.Collections.Generic;

    public class Mercator 
    {
        
        //  ######     #    #######    #       ####### #     # ######  #######  #####  
        //  #     #   # #      #      # #         #     #   #  #     # #       #     # 
        //  #     #  #   #     #     #   #        #      # #   #     # #       #       
        //  #     # #     #    #    #     #       #       #    ######  #####    #####  
        //  #     # #######    #    #######       #       #    #       #             # 
        //  #     # #     #    #    #     #       #       #    #       #       #     # 
        //  ######  #     #    #    #     #       #       #    #       #######  #####  
                                                                            

        // ---------------------------
        public struct GeoCoordinate 
        {
            public double latitude;
            public double longitude;
            public double altitude;

            private const double DegreesToRadians = Math.PI/180.0;
            private const double RadiansToDegrees = 180.0/ Math.PI;
            private const double EarthRadius = 6378137.0;

            public GeoCoordinate(double lat, double lng, double alt = 0) 
            {
                this.latitude = lat;
                this.longitude = lng;
                this.altitude = alt;
            }
            /// <summary>
            /// �� GeoCoordinate�� �ٸ� GeoCoordinate�� ������ ���� �� �浵 ��ǥ ���� �Ÿ��� ��ȯ�մϴ�.
            /// </summary>
            /// <returns>
            /// �� ��ǥ ���� �Ÿ�(���� ����)�Դϴ�.
            /// </returns>
            /// <param name="other">�Ÿ��� ����� ��ġ�� ��Ÿ���� GeoCoordinate�Դϴ�.</param>
            public double GetDistanceTo(GeoCoordinate other)
            {
                // ��ǥ ���� NaN���� Ȯ���Ͽ� ���� ó���մϴ�.
                if (double.IsNaN(latitude) || double.IsNaN(longitude) || double.IsNaN(other.latitude) ||
                    double.IsNaN(other.longitude))
                {
                    throw new ArgumentException("Argument latitude or longitude is not a number");
                }

                // �� ��ǥ�� �ٸ� ��ǥ ���� ������ ���� ������ ����մϴ�.
                var d1 = latitude * (Math.PI / 180.0);
                var num1 = longitude * (Math.PI / 180.0);
                var d2 = other.latitude * (Math.PI / 180.0);
                var num2 = other.longitude * (Math.PI / 180.0) - num1;

                // Haversine ������ ����Ͽ� �� ���� ���� �Ÿ��� ����մϴ�.
                var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
                         Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);

                // ���� �������� ������� �Ÿ��� ����ϰ� ��ȯ�մϴ�.(����)
                return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
            }

            /// <summary>
            /// �־��� �������� ������ �Ÿ�(����)�� ����(��)�� ���� �������� ����մϴ�.
            /// �� �޼���� ������ ������ �������� ����Ͽ� �������� ����մϴ�.
            /// </summary>
            /// <param name="source">�����</param>
            /// <param name="range">���� ������ �Ÿ�</param>
            /// <param name="bearing">�� ������ ������</param>
            /// <returns>�������� �־��� �Ÿ��� ������ ���� �������Դϴ�.</returns>
            public GeoCoordinate CalculateDerivedPosition(double range, double bearing)
            {
                // ������� ������ �浵�� �������� ��ȯ�մϴ�.
                var latA = this.latitude * DegreesToRadians;
                var lonA = this.longitude * DegreesToRadians;

                // ���� �Ÿ��� ����մϴ�.
                var angularDistance = range / EarthRadius;
                // ���� ������ ����մϴ�.
                var trueCourse = bearing * DegreesToRadians;

                // ���ο� ��ġ�� ������ ����մϴ�.
                var lat = Math.Asin(
                    Math.Sin(latA) * Math.Cos(angularDistance) +
                    Math.Cos(latA) * Math.Sin(angularDistance) * Math.Cos(trueCourse));

                // �浵 ������ ����մϴ�.
                var dlon = Math.Atan2(
                    Math.Sin(trueCourse) * Math.Sin(angularDistance) * Math.Cos(latA),
                    Math.Cos(angularDistance) - Math.Sin(latA) * Math.Sin(lat));

                // ���ο� ��ġ�� �浵�� ����մϴ�.
                var lon = ((lonA + dlon + Math.PI) % (Math.PI * 2)) - Math.PI;

                // ���ο� GeoCoordinate �ν��Ͻ��� �����Ͽ� ��ȯ�մϴ�.
                return new GeoCoordinate(
                    lat * RadiansToDegrees,
                    lon * RadiansToDegrees,
                    this.altitude);
            }

        }
        public struct GeoPoint {
            public double x;
            public double y;
            public GeoPoint(double x, double y) 
            {
                this.x = x;
                this.y = y;
            }
            public override string ToString() 
            {
                return $"GeoPoint: [x: {this.x}, y: {this.y}]";
            }
        }
        public struct MapTile : IEquatable<MapTile> {
            public int x;
            public int y;
            public int z;
            public MapTile(int x, int y, int z) 
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }
            public override string ToString() 
            {
                return $"MapTile: [x: {this.x}, y: {this.y}, z: {this.z}]";
            }
            public string XYHash {
                get { return MapTile.XYHashString(this.x, this.y); }
            }
            static public string XYHashString(int x, int y) 
            {
                return $"{x}#{y}";
            }
            // -------------------------
            // IEquatable<MapTile>
            public override bool Equals(object obj) => this.Equals((MapTile)obj);
            public override int GetHashCode() 
            {
                int hash = 17;
                hash = hash * 31 + x;
                hash = hash * 31 + y;
                hash = hash * 31 + z;
                return hash;
            }
            public bool Equals(MapTile tile) 
            {
                return this.x == tile.x && this.y == tile.y && this.z == tile.z;
            }
            // -------------------------
        }
        public struct TileBounds {
            public GeoCoordinate sw;
            public GeoCoordinate ne;
            public TileBounds(GeoCoordinate sw, GeoCoordinate ne) 
            {
                this.sw = sw;
                this.ne = ne;
            }
        }
        // ---------------------------

        //  #     # ####### ####### #     # ####### ######   #####  
        //  ##   ## #          #    #     # #     # #     # #     # 
        //  # # # # #          #    #     # #     # #     # #       
        //  #  #  # #####      #    ####### #     # #     #  #####  
        //  #     # #          #    #     # #     # #     #       # 
        //  #     # #          #    #     # #     # #     # #     # 
        //  #     # #######    #    #     # ####### ######   #####  
                                                         

        public static GeoPoint FromLatLngToPoint(GeoCoordinate coord) 
        {
            double siny = Math.Min(
                Math.Max(Math.Sin((coord.latitude * (Math.PI / 180.0))), -0.9999),
                0.9999
            );
            GeoPoint p = new GeoPoint();
            p.x = 128.0 + coord.longitude * (256.0 / 360.0);
            p.y = 128.0 + 0.5 * Math.Log((1.0 + siny) / (1.0 - siny)) * -(256.0 / (2.0 * Math.PI));
            return p;
        }

        public static GeoCoordinate FromPointToLatLng(GeoPoint point) 
        {
            GeoCoordinate coord = new GeoCoordinate();
            coord.latitude = (2.0 * Math.Atan(Math.Exp((point.y - 128.0) / -(256.0 / (2.0 * Math.PI)))) -
                             Math.PI / 2.0) /
                             (Math.PI / 180.0);
            coord.longitude = (point.x - 128.0) / (256.0 / 360.0);
            return coord;
        }

        public static MapTile GetTileAtLatLng(double lat, double lng, float zoom) 
        {
            return GetTileAtLatLng(new GeoCoordinate(lat, lng), zoom);
        }

        public static MapTile GetTileAtLatLng(GeoCoordinate geoCoord, float zoom) 
        {
            int t = (int)Math.Pow(2, zoom);
            float s = 256.0f / t;
            GeoPoint p = FromLatLngToPoint(geoCoord);
            return new MapTile((int)Math.Floor(p.x / s), 
                               (int)Math.Floor(p.y / s), 
                               (int)zoom);
        }

        public static List<MapTile> GetSurroundingTileList(MapTile tile, bool includeCenterTile = true) 
        {
            List<MapTile> tiles = new List<MapTile>();
            if (includeCenterTile) tiles.Add(tile);
            tiles.Add(new MapTile(tile.x, tile.y - 1, tile.z)); // topTile
            tiles.Add(new MapTile(tile.x + 1, tile.y, tile.z)); // rightTile
            tiles.Add(new MapTile(tile.x, tile.y + 1, tile.z)); // bottomTile
            tiles.Add(new MapTile(tile.x - 1, tile.y, tile.z)); // leftTile
            tiles.Add(new MapTile(tile.x + 1, tile.y - 1, tile.z)); // topRightTile
            tiles.Add(new MapTile(tile.x + 1, tile.y + 1, tile.z)); // bottomRightTile
            tiles.Add(new MapTile(tile.x - 1, tile.y + 1, tile.z)); // bottomLeftTile
            tiles.Add(new MapTile(tile.x - 1, tile.y - 1, tile.z)); // topLeftTile
            return tiles;
        }

        public static List<string> GetSurroundingTileXYHashList(MapTile tile, bool includeCenterTile = true) 
        {
            return Mercator.GetSurroundingTileList(tile, includeCenterTile)
                .ConvertAll(
                    new Converter<MapTile, string>(
                        (MapTile t) => t.XYHash)
                    );
        }

        public static TileBounds GetTileBounds(MapTile tile) 
        {
            NormalizeTile(ref tile);
            
            int t = (int)Math.Pow(2, tile.z);
            float s = 256.0f / t;
            GeoPoint swPoint = new GeoPoint(tile.x * s, tile.y * s + s);
            GeoPoint nePoint = new GeoPoint(tile.x * s + s, tile.y * s);
            return new TileBounds(FromPointToLatLng(swPoint), FromPointToLatLng(nePoint));
        }

        public static void NormalizeTile(ref MapTile tile) 
        {
            int t = (int)Math.Pow(2, tile.z);
            tile.x = (int)(((tile.x % t) + t) % t);
            tile.y = (int)(((tile.y % t) + t) % t);
        }
    }

}