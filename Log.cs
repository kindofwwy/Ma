static class Log
{
    public static void Excep(string message)
    {
        Console.WriteLine(message);
        Thread.Sleep(10000);
    }

    public static void Excep(Op op,string message)
    {
        Console.WriteLine($"at {op}");
        Excep(message);
    }

    public static void ExcepWrongParaNum(Op op,int exceptNum)
    {
        Excep(op,$"{op.name} err: except number of parameters at least {exceptNum}, but receive {op.inp.Length}");
    }
}