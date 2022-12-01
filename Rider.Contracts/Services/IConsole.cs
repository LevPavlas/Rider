namespace Rider.Contracts.Services
{
    public interface IConsole
    {
        void WriteLine(string msg);
		void WriteError(string msg);
		void WriteWarning(string msg);
	}
}