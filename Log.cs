static class Log
{
    static Op Err(string message)
    {
        return new Op{name="err",inp=[new Op(){name=message}]};
    }

    public static Op Excep(string message)
    {
        Console.WriteLine(message);
        Console.ReadKey();
        return Err(message);
    }

    public static Op Excep(Op op,string message)
    {
        message=$"from_{op.name}:"+message;
        Console.WriteLine($"At {op}");
        return Excep(message);
    }

    public static Op ExcepWrongParaNum(Op op,int exceptNum)
    {
        return Excep(op,$"except_number_of_parameters_at_least_{exceptNum}/_but_receive_{op.inp.Length}.");
    }

    public static Op ExcepNoItem(Op op)
    {
        return Excep(op,$"no_item_inside.");
    }

    public static Op ExcepNotFound(Op op)
    {
        return Excep(op,$"not_found.");
    }

    public static Op ExcepIndex(Op op,string some)
    {
        return Excep(op,$"except_int_as_index/but_{some}.");
    }

    public static Op OutOfRange(Op op,int? ind=null)
    {
        if(ind!=null)
            return Excep(op,$"{ind}_is_out_of_range");
        return Excep(op,"out_of_range");
    }
}