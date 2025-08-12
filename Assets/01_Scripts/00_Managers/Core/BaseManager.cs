using Cysharp.Threading.Tasks;

public abstract class BaseManager
{
    public abstract UniTask Init();
    public abstract UniTask Dispose();
} 