namespace Horseshoe.NET.Events
{
    public delegate void EasyNotifier<T>(T message);

    public delegate void EasyNotifier<T1, T2>(T1 messagePart1, T2 messagePart2);

    public delegate void EasyNotifier<T1, T2, T3>(T1 messagePart1, T2 messagePart2, T3 messagePart3);
}
