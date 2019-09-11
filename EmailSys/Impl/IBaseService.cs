

namespace EmailSys.Impl
{
   public interface IBaseService
    {

        bool IsInitialed { get; }

        bool Run();
    }
}
