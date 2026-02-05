static class Ma
{
    static char[] separator=[' ','\n','\r',','];
    static List<string> Cut(string code)    //for (+ 1 2)
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

    static List<string> CutB(string code)    //for +(1,2)
    {
        List<string> output=[];
        string temp="";

        int jumpflag=0;
        bool isArgs= code[0] == '(' && code[code.Length - 1] == ')';
        if(isArgs) code=code.Substring(1,code.Length-2);
        for( int i=0; i < code.Length; ++i)
        {
            if (code[i] == '(')
            {
                if ((!isArgs)&&jumpflag == 0)
                {
                    if(temp.Length!=0)
                        output.Add(temp);
                    temp="";
                }
                ++ jumpflag;
            }
            else if(code[i]==')') -- jumpflag;
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

    static string CutHeadTail(string code)
    {
        int start=0;
        int end=code.Length-1;
        for(int i = start; i < code.Length; ++i)
        {
            if(!Array.Exists<char>(separator,(char x) => x == code[i]))
            {
                start=i;
                break;
            }
        }
        for(int i = end; i >= 0; --i)
        {
            if(!Array.Exists<char>(separator,(char x) => x == code[i]))
            {
                end=i;
                break;
            }
        }
        return code.Substring(start,end-start+1);
    }

    public static Op Parse(string code)    //(* (+ 1 2) (- 3 (/ 4 5)))
    {
        Op op=new Op();
        code=CutHeadTail(code);
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

    public static Op ParseB(string code)
    {
        Op op=new Op();
        List<string> codes=CutB(code);
        if (codes.Count > 2)
        {
            List<string> inp=CutB(codes[codes.Count-1]);
            op.name="call";
            op.inp=new Op[inp.Count+1];
            string front="";
            for(int i = 0; i < codes.Count - 1; ++i)
            {
                front+=codes[i];
            }
            op.inp[0]=ParseB(front);
            for(int i = 0; i < inp.Count; ++i)
            {
                op.inp[i+1]=ParseB(inp[i]);
            }
        }
        else if (codes.Count == 2)
        {
            op.name=codes[0];
            List<string> inp=CutB(codes[1]);
            op.inp=new Op[inp.Count];
            for(int i = 0; i < inp.Count; ++i)
            {
                op.inp[i]=ParseB(inp[i]);
            }
        }
        else
        {
            op.name=codes[0];
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
    static string[] NoCallSub=["def","eq","rp","len"];

    public Op()
    {
        name="";
    }

    public bool HasDefine()
    {
        return defines.ContainsKey(name)||Lib.lib.ContainsKey(name);
    }

    bool isNoCallSub()
    {
        string name=this.name;
        return Array.Exists<string>(NoCallSub,(string x)=>x==name);
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
            if (isNoCallSub()) Explain();
            else if (name == "if")
            {
                inp[0].Execute();
                Explain();
            }
            else if (name == "append" || name == "at")
            {
                inp[1].Execute();
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

    public bool ExecuteStep()
    {
        if (inp != null && HasDefine())
        {
            if (isNoCallSub()) 
            {
                Explain();
                return true;
            }
            else if (name == "if")
            {
                if(!inp[0].ExecuteStep())
                    Explain();
                return true;
            }
            else if (name == "append" || name == "at")
            {
                if(!inp[1].ExecuteStep())
                    Explain();
                return true;
            }
            else
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    if (inp[i].ExecuteStep())
                    {
                        return true;
                    }
                }
                Explain();
                return true;
            }
        }
        else
        {
            return false;
        }
    }

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

    bool isNeedAlpha(Op target) //((lam x y ((lam y x (- x y)) x y)) 2 3)
    {
        if ((name == "def" || name == "lam" || name == "rp") && inp!=null)
        {
            bool ignore=false;
            int bias=name=="rp"?3:1;
            for(int i = 0; i < inp.Length-bias; ++i)
            {
                if (inp[i].name == target.name)
                {
                    ignore=true;
                    break;
                }
            }
            return ignore;
        }
        else return false;
    }

    public void Replace(Op target,Op content) //def
    {
        if (!isNeedAlpha(target))
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
                    for(int i = 0; i < inp.Length; ++i)
                    {
                        inp[i].Replace(target,content);
                    } 
                }
            }
            else
            {
                if(inp!=null)
                {
                    for(int i = 0; i < inp.Length; ++i)
                    {
                        inp[i].Replace(target,content);
                    } 
                }
            }
        }
    }

    public void ReplaceOnly(Op target,Op content) //rp
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
                name=content.name;
            }
        }
        else
        {
            if(inp!=null)
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    inp[i].ReplaceOnly(target,content);
                } 
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

    public string ToStringB()
    {
        if (inp == null)
        {
            return name;
        }
        else
        {
            string t=name;
            t+="(";
            for(int i = 0; i < inp.Length; ++i)
            {
                if(i==0)
                    t+=inp[i].ToStringB();
                else
                    t+=","+inp[i].ToStringB();
            }
            t+=")";
            return t;
        }
    }

    static ConsoleColor[] colors=[ConsoleColor.Cyan,ConsoleColor.Magenta,ConsoleColor.Yellow];
    public void show(int deep=0,int lastcount=0,bool islast=false)
    {
        for(int i = 0; i < deep; ++i)
        {
            Console.Write("     ");
        }
        Console.ForegroundColor=colors[deep%colors.Length];
        if (isnosub())
        {
            Console.Write(this);
        }
        else if(inp != null)
        {
            Console.Write("(");
        }
        if(isnosub() && islast)
        {
            for(int i = 0; i < lastcount; ++i)
            {
                Console.ForegroundColor=colors[(deep-i-1)%colors.Length];
                Console.Write(")");
            }
        }
        if (isnosub())
        {
            Console.Write("\n");
        }
        else
        {
            Console.WriteLine(name);

            if (inp != null)
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    if (i == inp.Length - 1)
                    {
                        inp[i].show(deep+1,lastcount+1,true);
                    }
                    else
                    {
                        inp[i].show(deep+1,0,false);
                    }
                    
                }
            }
        }
        
        
    }

    bool isnosub()
    {
        if(inp==null)  return true;
        else
        {
            for(int i = 0; i < inp.Length; ++i)
            {
                if(inp[i].inp!=null) return false;
            }
            return true;
        }
    }
}