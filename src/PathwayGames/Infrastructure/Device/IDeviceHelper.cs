using PathwayGames.Sensors;

namespace PathwayGames.Infrastructure.Device
{
    public interface IDeviceHelper
    {
        int MachineNameToPPI(string machineName);

        EyeGazeCompensation MachineNameToEyeGazeCompensation(string machineName);
    }
}
