namespace BonLib.Managers
{

    public interface IManager
    {
        void BindDependencies();
        void ResolveDependencies();
        void SubscribeToEvents();
        void Initialize();
        void LateInitialize();
        void Dispose();
    }

}