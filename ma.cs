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
            if (code[i] == '(') ++ jumpflag;
            if (code[i] == ')') -- jumpflag;

            if(jumpflag==0 && Array.Exists<char>(separator,(char x)=>x==code[i])) 
            {
                if(temp.Length!=0)
                    output.Add(temp);
                temp="";
            }
            else if(code[i] == '(' && jumpflag == 1)
            {
                if(temp.Length!=0)
                    output.Add(temp);
                temp="(";
            }
            else if(code[i] == ')' && jumpflag == 0)
            {
                temp+=")";
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
            if(codes.Count==0) return new Op{name="None"};
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
            if (codes.Count==0) op.name="None";
            else op.name=codes[0];
            op.inp=null;
        }
        return op;
    }

    public static Op ParseMix(string code)
    {
        code=CutHeadTail(code);
        if (code[0] == '(')
        {
            return Parse(code);
        }
        return ParseB(code);
    }

    public static List<string> CutCode(string code)
    {
        List<string> codes=[];
        string tempcode="";
        int quotenum=0;
        bool jump=false;
        for(int i = 0; i < code.Length; ++i)
        {
            if (code[i] == '(')
            {
                quotenum++;
            }
            else if (code[i] == ')')
            {
                quotenum--;
                if (quotenum == 0 && tempcode!="" && (!(i<code.Length-1) || code[i+1]!='(')) //some(...)(...) nocut
                {
                    codes.Add(tempcode+")");
                    tempcode="";
                    continue;
                }
            }
            else if (code[i] == '#')
            {
                jump=true;
                continue;
            }
            else if (jump && (code[i] == '\n' || code[i]=='\r'))
            {
                jump=false;
                continue;
            }
            else if((code[i] == '\n' || code[i]=='\r') && quotenum == 0 && tempcode!="")
            {
                codes.Add(tempcode);
                tempcode="";
                continue;
            }

            if (!jump && !(code[i] == '\n' || code[i]=='\r'))
            {
                tempcode+=code[i];
            }
        }
        return codes;
    }

    public static int ShowOpHint(Op op,int deep=0,int current=0,int target=0)
    {
        if (deep == 0)
        {
            if(!op.ExeLineNumber(ref target)) target=-1;
        }
        for(int i = 0; i < deep; ++i)
        {
            Console.Write("     ");
        }
        if (current == target)
        {
            Console.ForegroundColor=ConsoleColor.Yellow;
        }
        Console.WriteLine(op.name+((op.inp!=null && op.inp.Length==0)?"()":""));
        int line=current;
        if (op.inp != null)
        {
            
            for(int i = 0; i < op.inp.Length; ++i)
            {
                line++;
                line=ShowOpHint(op.inp[i],deep+1,line,target);
            }
        }
        if (current == target)
        {
            Console.ForegroundColor=ConsoleColor.White;
        }
        return line;
    }

    public static Op Execute(string code)
    {
        Op op=ParseMix(code);
        op.Execute();
        return op;
    }

    public static Op ExecuteFile(string path)
    {
        string code=File.ReadAllText(path);
        List<string> codes=CutCode(code);
        Op op=new Op();
        for(int i = 0; i < codes.Count; ++i)
        {
            op=Execute(codes[i]);
        }
        return op;
    }

    public static void Interact()
    {
        Modes modes=Modes.execute;
        bool ispause=false;
        Op op=new Op();
        while (true)
        {
            Console.Write(">>");
            string input=Console.ReadLine()??"";
            if (input != "")
            {
                if (input[0] == ':')
                {
                    string[] command=CutHeadTail(input.Substring(1)).Split([' ']);
                    if (command[0] == "q")
                    {
                        break;
                    }
                    else if (command[0] == "load")
                    {
                        ExecuteFile(command[1]);
                        Console.WriteLine("done");
                    }
                    else if (command[0] == "step")
                    {
                        ispause=false;
                        modes=Modes.step;
                        Console.WriteLine("step mode");
                    }
                    else if (command[0] == "execute")
                    {
                        ispause=false;
                        modes=Modes.execute;
                        Console.WriteLine("execute mode");
                    }
                    else if (command[0] == "pause")
                    {
                        ispause=true;
                        modes=Modes.step;
                        Console.WriteLine("pause mode");
                    }
                    else if (command[0] == "show")
                    {
                        op.show();
                        Console.ForegroundColor=ConsoleColor.White;
                    }
                    else if (command[0] == "demo")
                    {
                        modes=Modes.demo;
                        Console.WriteLine("demo mode");
                    }
                }
                else
                {
                    try
                    {
                        if (modes==Modes.step)
                        {
                            op=ParseMix(input);
                            while (op.ExecuteStep())
                            {
                                Console.WriteLine(op.ToStringB());
                                if(ispause) Console.ReadKey();
                            }
                        }
                        else if (modes==Modes.demo)
                        {
                            op=ParseMix(input);
                            while (op.ExecuteStep())
                            {
                                ShowOpHint(op);
                                if(ispause) Console.ReadKey();
                                else Thread.Sleep(1000);
                                Console.Clear();
                            }
                            ShowOpHint(op);
                        }
                        else
                        {
                            op=Execute(input);
                            Console.WriteLine(op.ToStringB()); 
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }   
            }
        }
    }
}

enum Modes
{
    execute,
    step,
    demo
}
struct Op
{
    public string name;
    public Op[]? inp;
    public static Dictionary<string, (Op[],Op)> defines=new Dictionary<string, (Op[],Op)>();    //formInp,define
    public static List<string> NoCallSub=["def","eq","wait","exp","defn"];

    public Op()
    {
        name="";
    }

    public bool HasDefine()
    {
        return defines.ContainsKey(name)||Lib.lib.ContainsKey(name);
    }

    public bool isNoCallSub()
    {
        string name=this.name;
        return NoCallSub.Exists((string x)=>x==name); //Array.Exists<string>(NoCallSub,(string x)=>x==name);
    }

    public void Explain()
    {
        int errind=Array.FindIndex<Op>(inp,(Op x) => x.name == "err");
        if(errind!=-1)
        {
            if(name=="catch")
                inp[errind].name="cerr";
            ShallowCopyToThis(inp[errind]);
        }
        else
        {
            if(defines.ContainsKey(name))
            ExpDic();
            else if(Lib.lib.ContainsKey(name))
            ExpLib();
        }
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
            else if (name == "rp" | name == "rpall")
            {
                inp[inp.Length-1].Execute();
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
                if (!inp[0].ExecuteStep())
                    Explain();
                return true;
            }
            else if (name == "rp" | name == "rpall")
            {
                if (!inp[inp.Length - 1].ExecuteStep())
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

    public bool ExeLineNumber(ref int line,int current=0,bool countonly=false)  //下次step会执行的代码行号
    {
        if (countonly)
        {
            if (inp != null)
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    line++;
                    inp[i].ExeLineNumber(ref line,line,countonly);
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        if (inp != null && HasDefine())
        {
            if (isNoCallSub()) 
            {
                return true;
            }
            else if (name == "if")
            {
                if(!inp[0].ExeLineNumber(ref line,line+1))
                    line=current;
                return true;
            }
            else if (name == "rp" | name == "rpall")
            {
                for(int i = 0; i < inp.Length-1; ++i)
                {
                    line++;
                    inp[i].ExeLineNumber(ref line,line,true);
                }
                line++;
                if(!inp[inp.Length-1].ExeLineNumber(ref line,line))
                    line=current;
                return true;
            }
            else
            {
                for(int i = 0; i < inp.Length; ++i)
                {
                    line++;
                    if (inp[i].ExeLineNumber(ref line,line))
                    {
                        return true;
                    }
                }
                line=current;
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    public int CountLines()
    {
        if(inp==null) return 1;
        else
        {
            int s=0;
            for(int i = 0; i < inp.Length; ++i)
            {
                s+=inp[i].CountLines();
            }
            return s;
        }
    }

    public void ExpDic()
    {
        Op d;
        Op[] form;
        (form,d)=defines[name];
        d=d.Copy();
        if(inp.Length<form.Length) ShallowCopyToThis(Log.ExcepWrongParaNum(this,form.Length));
        else
        {
            d.Replaces(form,inp);
            ShallowCopyToThis(d);
        }
    }

    public void ExpLib()
    {
        Op d=Lib.lib[name](this);
        ShallowCopyToThis(d);
    }

    bool isNeedAlpha(Op target) //((lam x y ((lam y x (- x y)) x y)) 2 3)
    {
        if ((name == "def" || name == "lam" || name == "rp" || name == "rpall" || name == "defn") && inp!=null)
        {
            bool ignore=false;
            int bias=(name=="rp"|| name == "rpall")?3:1;
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

    public bool Replaces(Op[] targets,Op[] contents)
    {
        List<Op> newtar=new List<Op>();
        for(int j = 0; j < targets.Length; ++j)
        {
            Op target=targets[j];
            Op content=contents[j];
            if (!isNeedAlpha(target))
            {
                newtar.Add(target);
                if (name == target.name)
                {
                    if (inp == null)
                    {
                        Op def=content.Copy();
                        ShallowCopyToThis(def);
                    }
                    else
                    {
                        if (content.name == "lam" || content.inp != null)  //((def f c x y (c x y)) (lam x y (+ x y)) 10 2)
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
                            inp[i].Replaces(targets,contents);
                        }
                    }
                    return true;
                }
            }
        }
        if(inp!=null)
        {
            bool res=false;
            for(int i = 0; i < inp.Length; ++i)
            {
                res|=inp[i].Replaces(newtar.ToArray(), contents);
            }
            return res;
        }
        return false;
    }

    public bool ReplacesOnly(Op[] targets,Op[] contents)    //rp
    {
        for(int j = 0; j < targets.Length; ++j)
        {
            Op target=targets[j];
            Op content=contents[j];
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
                        name=content.name;
                        for(int i = 0; i < inp.Length; ++i)
                        {
                            inp[i].ReplacesOnly(targets,contents);
                        }
                    }
                    return true;
                }
            }
        }
        if(inp!=null)
        {
            bool res=false;
            for(int i = 0; i < inp.Length; ++i)
            {
                res|=inp[i].ReplacesOnly(targets, contents);
            }
            return res;
        }
        return false;
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

    public void showOrigin(int deep=0)
    {
        for(int i = 0; i < deep; ++i)
        {
            Console.Write("     ");
        }
        Console.WriteLine(name);
        if (inp != null)
        {
            for(int i = 0; i < inp.Length; ++i)
            {
                inp[i].showOrigin(deep+1);
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