using PathwayGames.Infrastructure.Device;
using PathwayGames.iOS.Infrastructure.Device;
using PathwayGames.Sensors;
using System.Collections.Generic;

[assembly: Xamarin.Forms.Dependency(typeof(DeviceHelper))]
namespace PathwayGames.iOS.Infrastructure.Device
{
    public class DeviceHelper : IDeviceHelper
    {
        public int MachineNameToPPI(string machineName)
        {
            Dictionary<string, int> machinePPIs = new Dictionary<string, int>() {
                // iPhone 11 Pro
                { "iPhone12,3", 458 } , 
                // iPhone 11 Pro Max
                { "iPhone12,5", 458 } , 
                // iPhone XS
                { "iPhone11,2", 458 } , 
                // iPhone XS Max
                { "iPhone11,4", 458 } , 
                { "iPhone11,6", 458 } , 
                // iPhone X
                { "iPhone10,3", 458 } , 
                { "iPhone10,6", 458 } , 

                // iPhone 8 Plus
                { "iPhone10,2", 401 } , 
                { "iPhone10,5", 401 } , 
                // iPhone 7 Plus
                { "iPhone9,2", 401 } ,  
                { "iPhone9,4", 401 } ,  
                // iPhone 6S Plus
                { "iPhone8,2", 401 } ,  
                // iPhone 6 Plus
                { "iPhone7,1", 401 } ,  

                // iPhone 11
                { "iPhone12,1", 326 },
                // iPhone XR
                { "iPhone11,8", 326 },

                // iPhone 8
                { "iPhone10,1", 326 },
                { "iPhone10,4", 326 },
                // iPhone 7
                { "iPhone9,1", 326 },
                { "iPhone9,3", 326 },
                // iPhone 6S
                { "iPhone8,1", 326 },
                // iPhone 6
                { "iPhone7,2", 326 },
                
                // iPhone SE
                { "iPhone8,4", 326 },
                // iPhone 5S
                { "iPhone6,1", 326 },
                { "iPhone6,2", 326 },
                // iPhone 5C
                { "iPhone5,3", 326 },
                { "iPhone5,4", 326 },
                // iPhone 5
                { "iPhone5,1", 326 },
                { "iPhone5,2", 326 },
                // iPod Touch (6th generation)
                { "iPod7,1", 326 },
                // iPod Touch (5th generation)
                { "iPod5,1", 326 },
                
                // iPhone 4S
                { "iPhone4,1", 326 },

                // iPad Mini (5th generation)
                { "iPad11,1", 326 },
                { "iPad11,2", 326 },
                // iPad Mini 4
                { "iPad5,1", 326 },
                { "iPad5,2", 326 },
                // iPad Mini 3
                { "iPad4,7", 326 },
                { "iPad4,8", 326 },
                { "iPad4,9", 326 },
                // iPad Mini 2
                { "iPad4,4", 326 },
                { "iPad4,5", 326 },
                { "iPad4,6", 326 },

                // iPad (7th generation)
                { "iPad7,11", 264 },
                { "iPad7,12", 264 },
                // iPad Air (3rd generation)
                { "iPad11,3", 264 },
                { "iPad11,4", 264 },
                // iPad Pro (12.9″, 3rd generation)
                { "iPad8,5", 264 },
                { "iPad8,6", 264 },
                { "iPad8,7", 264 },
                { "iPad8,8", 264 },
                // iPad Pro (11″)
                { "iPad8,1", 264 },
                { "iPad8,2", 264 },
                { "iPad8,3", 264 },
                { "iPad8,4", 264 },
                // iPad (6th generation)
                { "iPad7,5", 264 },
                { "iPad7,6", 264 },
                // iPad Pro (10.5″)
                { "iPad7,3", 264 },
                { "iPad7,4", 264 },
                // iPad Pro (12.9″, 2nd generation)
                { "iPad7,1", 264 },
                { "iPad7,2", 264 },
                // iPad (5th generation)
                { "iPad6,11", 264 },
                { "iPad6,12", 264 },
                // iPad Pro (12.9″)
                { "iPad6,7", 264 },
                { "iPad6,8", 264 },
                // iPad Pro (9.7″)
                { "iPad6,3", 264 },
                { "iPad6,4", 264 },
                // iPad Air 2
                { "iPad5,3", 264 },
                { "iPad5,4", 264 },
                // iPad Air
                { "iPad4,1", 264 },
                { "iPad4,2", 264 },
                { "iPad4,3", 264 },
                // iPad (4th generation)
                { "iPad3,4", 264 },
                { "iPad3,5", 264 },
                { "iPad3,6", 264 },
                // iPad (3rd generation)
                { "iPad3,1", 264 },
                { "iPad3,2", 264 },
                { "iPad3,3", 264 },

                // iPad Mini
                { "iPad2,5", 163 },
                { "iPad2,6", 163 },
                { "iPad2,7", 163 },

                // iPad 2
                { "iPad2,1", 132 },
                { "iPad2,2", 132 },
                { "iPad2,3", 132 },
                { "iPad2,4", 132 },
            };

            if (machinePPIs.TryGetValue(machineName, out var ppi)) {
                return ppi;
            } else { 
                return 0;
            }
        }

        public EyeGazeCompensation MachineNameToEyeGazeCompensation(string machineName)
        {
            Dictionary<string, EyeGazeCompensation> machineCompensations = new Dictionary<string, EyeGazeCompensation>() {
                // iPhone X
                { "iPhone10,3", new EyeGazeCompensation(83.39f, 398.5f) } ,
                { "iPhone10,6", new EyeGazeCompensation(83.39f, 398.5f) } ,
                // iPhone Xs
                { "iPhone11,2", new EyeGazeCompensation(83.39f, 398.5f) } ,
                // iPhone 11 Pro
                { "iPad8,1", new EyeGazeCompensation(46.58f, 378.15f) } ,
                { "iPad8,2", new EyeGazeCompensation(46.58f, 378.15f) } ,
                { "iPad8,3", new EyeGazeCompensation(46.58f, 378.15f) } ,
                { "iPad8,4", new EyeGazeCompensation(46.58f, 378.15f) } ,
            };

            if (machineCompensations.TryGetValue(machineName, out var compensation))
            {
                return compensation;
            }
            else
            {
                return new EyeGazeCompensation(0f, 0f);
            }
        }
    }
}