
namespace VizsgaRemekWpf.Services
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : class;
    }
}
