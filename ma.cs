static class Ma
{
    static char[] separator=[' ','\n','\r'];
    static List<string> Cut(string code)
    {
        List<string> output=[];
        string temp="";
        int jumpflag=0;
        for(int i = 1; i < code.Length-1; ++i)
        {
            if(code[i]=='(') ++ jumpflag;
            if(code[i]==')') -- jumpflag;
            if(jumpflag==0 && Array.Exists<char>(separator,(char x)=>x==code[i]))
            {
                if(temp.Length!=0)
                    output.Add(temp);
                temp="";
            }
            else
            {
                temp+=code[i];
            }
        }
        if(temp!="") output.Add(temp);
        return output;
    }

    public static Op Parse(string code)    //(* (+ 1 2) (- 3 (/ 4 5)))
    {
        Op op=new Op();
        if (code[0] == '(' && code[code.Length - 1] == ')')
        {
            List<string> codes=Cut(code);
            if (codes[0][0] == '(' && codes[0][codes[0].Length - 1] == ')')     //((def mul2 x (+ x x)) 10)
            {
                op.name="call";
                op.inp=new Op[codes.Count];

                for(int i = 0; i < codes.Count; ++i)
                {
                    op.inp[i]=Parse(codes[i]);
                }
                
            }
            else
            {
                op.name=codes[0];
                op.inp=new Op[codes.Count-1];

                for(int i = 1; i < codes.Count; ++i)
                {
                    op.inp[i-1]=Parse(codes[i]);
                }
            }
        }
        else
        {
            op.name=code;
            op.inp=null;
        }
        return op;
    }

    public static Op Execute(string code)
    {
        Op op=Parse(code);
        op.Execute();
        return op;
    }
}

struct Op
{
    public string name;
    public Op[]? inp;
    public static Dictionary<string, (Op[],Op)> defines=new Dictionary<string, (Op[],Op)>();    //formInp,define

    public Op()
    {
        name="";
    }

    public bool HasDefine()
    {
        return defines.ContainsKey(name)||Lib.lib.ContainsKey(name);
    }

    public void Explain()
    {
        if(defines.ContainsKey(name))
        ExpDic();
        else if(Lib.lib.ContainsKey(name))
        ExpLib();
    }

    public void Execute()
    {
        while (inp != null && HasDefine())
        {
            if (name=="def" || name=="eq" || name=="rp") Explain();
            else if (name == "if")
            {
                inp[0].Execute();
                Explain();
            }
            else
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    inp[i].Execute();
                }
                Explain();
            }
            
        }
    }

    public void ExecuteStep(int deep=0)
    {
        while (inp != null && HasDefine())
        {
            if (name=="def" || name=="eq" || name=="rp") Explain();
            else if (name == "if")
            {
                inp[0].ExecuteStep(deep+1);
                Explain();
            }
            else
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    inp[i].ExecuteStep(deep+1);
                }
                Explain();
            }
            if (deep == 0)
            {
                break;
            }
        }
    }

    // public IEnumerable<Op> ExecuteSingleStep()
    // {
    //     while (inp != null && HasDefine())
    //     {
    //         if (name=="def" || name=="eq" || name == "rp")
    //         {
    //             Explain();
    //             yield return this;
    //         } 
    //         else if (name == "if")
    //         {
    //             inp[0].ExecuteSingleStep();
    //             Explain();
    //             yield return this;
    //         }
    //         else
    //         {
    //             for(int i = 0; i < inp.Length; ++i)
    //             {
    //                 inp[i].ExecuteSingleStep();
    //             }
    //             Explain();
    //             yield return this;
    //         }
            
    //     }
    // }

    public void ExpDic()
    {
        Op d;
        Op[] form;
        (form,d)=defines[name];
        d=d.Copy();
        for(int i = 0; i < form.Length; ++i)
        {
            d.Replace(form[i],inp[i]);
        }
        ShallowCopyToThis(d);
    }

    public void ExpLib()
    {
        Op d=Lib.lib[name](this);
        ShallowCopyToThis(d);
    }

    public void Replace(Op target,Op content) //def
    {
        if (name == target.name)
        {
            if (inp == null)
            {
                Op def=content.Copy();
                ShallowCopyToThis(def);
            }
            else
            {
                if (content.name == "lam")  //((def f c x y (c x y)) (lam x y (+ x y)) 10 2)
                {
                    Op c=new Op(){name="call"};
                    c.inp=new Op[inp.Length+1];
                    c.inp[0]=content.Copy();
                    for(int i = 0; i < inp.Length; ++i)
                    {
                        c.inp[i+1]=inp[i];
                    }
                    ShallowCopyToThis(c);
                }
                else
                {
                    name=content.name;
                }
            }
        }
        else
        {
            if(inp!=null)
            for(int i = 0; i < inp.Length; i++)
            {
                inp[i].Replace(target,content);
            }
        }
    }

    public void ReplaceName(string target,string content)
    {
        if (name == target)
        {
            name=content;
        }
        else
        {
            if(inp!=null)
            for(int i = 0; i < inp.Length; i++)
            {
                inp[i].ReplaceName(target,content);
            }
        }
    }

    public bool isAllEq(Op op)
    {
        if (name == op.name)
        {
            if(inp==null &&op.inp==null) return true;
            else if(inp !=null && op.inp !=null && inp.Length==op.inp.Length)
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    if(!inp[i].isAllEq(op.inp[i]))return false;
                }
                return true;
            }
            else return false;
        }
        else return false;
    }

    public Op Copy()
    {
        Op op=new Op();
        op.name=name;
        if (inp != null)
        {
            op.inp=new Op[inp.Length];
            for(int i = 0; i < inp.Length; ++i)
            {
                op.inp[i]=inp[i].Copy();
            }
        }
        return op;
    }

    public void ShallowCopyToThis(Op op)
    {
        name=op.name;
        inp=op.inp;
    }

    public override string ToString()
    {
        if (inp == null)
        {
            return name;
        }
        else
        {
            string t="("+name;
            for(int i = 0; i < inp.Length; ++i)
            {
                t+=" "+inp[i].ToString();
            }
            t+=")";
            return t;
        }
    }

    public void show(int deep=0)
    {
        for(int i = 0; i < deep; ++i)
        {
            Console.Write("     ");
        }
        Console.WriteLine(name);
        if(inp!=null)
        for(int i = 0; i < inp.Length; ++i)
        {
            inp[i].show(deep+1);
        }
    }
}