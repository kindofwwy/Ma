delegate Op MaFunc(Op op);
delegate object Dya(int a,int b);
delegate object DyaStr(string a,string b);
delegate object MonStr(string a);
static class Lib
{
    public static Dictionary<string,MaFunc> lib;

    static Lib()
    {
        lib=new Dictionary<string, MaFunc>();

        lib["def"]=def;
        lib["call"]=call;
        lib["if"]=If;
        //lambda (lam args body)
        lib["+"]=Dyadic((int x,int y)=>x+y);
        lib["-"]=Dyadic((int x,int y)=>x-y);
        lib["*"]=Dyadic((int x,int y)=>x*y);
        lib["/"]=Dyadic((int x,int y)=>x/y);
        lib[">"]=Dyadic((int x,int y)=>x>y);
        lib["<"]=Dyadic((int x,int y)=>x<y);
        lib["="]=Dyadic((string x,string y)=>x==y);
        lib["!="]=Dyadic((string x,string y)=>x!=y);
        lib["and"]=Dyadic((string x,string y)=>x==true.ToString()&&y==true.ToString());
        lib["or"]=Dyadic((string x,string y)=>x==true.ToString()||y==true.ToString());
        lib["not"]=Mono((string x)=>{if(x==true.ToString())return false;else return true;});

        lib["cat"]=Dyadic((string x,string y)=>x+y);
        lib["namelen"]=Mono((string x)=>x.Length);
        lib["nameat"]=strAt;

        lib["eq"]=AllEq;
        lib["rp"]=Replace;
        lib["len"]=Len;
        lib["at"]=At;
        lib["append"]=Append;
        lib["rename"]=Rename;
        lib["atlist"]=atList;
    }

    static Op def(Op op)
    {
        if(op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
        Op d=new Op();
        d.name=op.inp[0].name;
        Op[] form=[];
        if (op.inp.Length > 2)
        {
            form=new Op[op.inp.Length-2];
            for(int i = 1; i < op.inp.Length - 1; ++i)
            {
                form[i-1]=op.inp[i];
            }
        }
        Op define=op.inp[op.inp.Length-1];
        Op.defines[d.name]=(form,define);
        return d;
    }

    static Op call(Op op)
    {
        if(op.inp.Length<1) Log.ExcepWrongParaNum(op,1);
        if(op.inp[0].name=="def") op.inp[0].Explain();
        else if (op.inp[0].name == "lam")
        {
            Op lam=op.inp[0];
            Op lambody=lam.inp[lam.inp.Length-1].Copy();
            for(int i = 1; i < op.inp.Length; ++i)
            {
                lambody.Replace(lam.inp[i-1],op.inp[i]);
            }
            return lambody;
        }
        Op d=op.inp[0].Copy();
        d.inp=new Op[op.inp.Length-1];
        for(int i = 1; i < op.inp.Length; ++i)
        {
            d.inp[i-1]=op.inp[i];
        }
        return d;
    }

    static Op If(Op op)
    {
        if(op.inp.Length<3) Log.ExcepWrongParaNum(op,3);
        Op d=new Op();
        if (op.inp[0].name == true.ToString())
        {
            d=op.inp[1];
        }
        else if (op.inp[0].name == false.ToString())
        {
            d=op.inp[2];
        }
        else
        {
            Log.Excep(op,$"if err: except {true} or {false}, but {op.inp[0].name}");
        }
        return d;
    }

    static MaFunc Dyadic(Dya func)
    {
        Op dya(Op op)
        {
            if(op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
            Op d=new Op();
            if(int.TryParse(op.inp[0].name,out int a))
            {
                if(int.TryParse(op.inp[1].name,out int b))
                {
                    d.name=func(a,b).ToString() ?? "None";
                    return d;
                }
            }
            Log.Excep(op,$"{op.name} err: except int, but {op.inp[0].name} and {op.inp[1].name}");
            return op;
        }
        return dya;
    }

    static MaFunc Dyadic(DyaStr func)
    {
        Op dya(Op op)
        {   
            if(op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
            Op d=new Op();
            d.name=func(op.inp[0].name,op.inp[1].name).ToString() ?? "None";
            return d;
        }
        return dya;
    }

    static MaFunc Mono(MonStr func)
    {
        Op mon(Op op)
        {
            if(op.inp.Length<1) Log.ExcepWrongParaNum(op,1);
            Op d=new Op();
            d.name=func(op.inp[0].name).ToString() ?? "None";
            return d;
        }
        return mon;
    }

    static Op AllEq(Op op)
    {
        if(op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
        Op a=op.inp[0];
        Op b=op.inp[1];
        Op d=new Op();
        d.name=a.isAllEq(b).ToString();
        return d;
    }

     static bool select_part(Op target,string[] args,Op be,ref Op?[] assumeArgs)
    {
        //匹配（对子项递归使用）
        //若assumeArgs只有名字，最后只替换名字，不替换子项
        /*  1.如果op未建档
                如果target还有子项 op建档，只有名字
                    遍历be子项确认是否匹配
                如果target没有子项 op建档，全部
            2.如果op已经建档
                如果op有子项，allEq op子项和be子项
                如果op没有子项，确认op be名字是否相同
                    如果target还有子项，遍历be子项确认是否匹配
            3.如果op无关档案
                确认op be名字是否相同
		            如果target还有子项，遍历be子项确认是否匹配*/
        int ind=Array.FindIndex(args,(string x) => x == target.name);

        if (ind != -1 && assumeArgs[ind]==null)
        {
            
            if(target.inp==null)
            {
                assumeArgs[ind]=be.Copy();
                return true;
            }
            else if(be.inp !=null && target.inp.Length == be.inp.Length)
            {
                assumeArgs[ind]=new Op(){name=be.name};
                for(int i = 0; i < target.inp.Length; ++i)
                {
                    if(!select_part(target.inp[i],args,be.inp[i],ref assumeArgs)) return false;
                }
                return true;
            }
            else return false;
        }
        else if (ind != -1 && assumeArgs[ind] != null && assumeArgs[ind].Value.name==be.name)
        {
            Op assume=assumeArgs[ind].Value;
            if(assume.inp==null && be.inp == null)
            {
                return true;
            }
            else if(assume.inp == null && target.inp != null && be.inp != null && target.inp.Length == be.inp.Length)
            {
                for(int i = 0; i < target.inp.Length; ++i)
                {
                    if(!select_part(target.inp[i],args,be.inp[i],ref assumeArgs)) return false;
                }
                return true;
            }
            else if(assume.inp!=null)
            {
                return assume.isAllEq(be);
            }
            else return false;
        }
        else if (target.name == be.name)
        {
            if(target.inp==null && be.inp == null)
            {
                return true;
            }
            else if(target.inp !=null && be.inp !=null && target.inp.Length == be.inp.Length)
            {
                for(int i = 0; i < target.inp.Length; ++i)
                {
                    if(!select_part(target.inp[i],args,be.inp[i],ref assumeArgs)) return false;
                }
                return true;
            }
            else return false;
        }
        else return  false;
    }

    static bool select(Op target,string[] args,ref Op be,Op content)
    {
        Op?[] assumeArgs=new Op?[args.Length];
        if(select_part(target,args,be,ref assumeArgs))
        {
            for(int i = 0; i < assumeArgs.Length; ++i)
            {
                if (!assumeArgs[i].HasValue)
                {
                    continue;
                }
                else if (assumeArgs[i].Value.inp == null)
                {
                    content.ReplaceName(args[i],assumeArgs[i].Value.name);
                }
                else
                {
                    content.ReplaceOnly(new Op(){name=args[i]},assumeArgs[i].Value);
                }
            }
            be.ShallowCopyToThis(content);
            return true;
        }
        else if(be.inp != null)
        {
            for(int i = 0; i < be.inp.Length; ++i)
            {
                if(select(target,args,ref be.inp[i], content))
                {
                    return true;
                }
            }
            return false;
        }
        else return false;
    }

    static Op Replace(Op op)
    {
        //(rp 通配符 目标结构 替换结构 目标) 通配符可多项,可省略
        if (op.inp.Length<3) Log.ExcepWrongParaNum(op,3);
        string[] args=new string[op.inp.Length-3];
        for(int i = 0; i < op.inp.Length-3; ++i)
        {
            args[i]=op.inp[i].name;
        }
        Op be=op.inp[op.inp.Length-1].Copy();
        Op content=op.inp[op.inp.Length-2].Copy();
        Op target=op.inp[op.inp.Length-3];
        
        select(target,args,ref be,content);

        return be;
    }

    static Op strAt(Op op)
    {
        if (op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
        string name=op.inp[0].name;
        if(int.TryParse(op.inp[1].name,out int index))
        {
            index=index<0 ? index - index/name.Length*name.Length + name.Length : index;
            index=index % name.Length;
            return new Op(){name=name[index].ToString()};
        }
        else
        {
            Log.Excep(op,$"except int as index,but {op.inp[1]}");
            return op;
        }
    }

    static Op Len(Op op)
    {
        if (op.inp != null && op.inp[0].inp != null)
        {
            Op d=new Op();
            d.name=op.inp[0].inp.Length.ToString();
            return d;
        }
        else return new Op(){name="None"};
    }

    static Op At(Op op)
    {
        if (op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
        else if (op.inp[0].inp==null) Log.Excep(op,"no item inside");
        else if(op.inp[0].inp.Length==0)  return new Op(){name="None"};
        else if(int.TryParse(op.inp[1].name,out int index))
        {
            Op[] ops=op.inp[0].inp;
            if(index>ops.Length-1 || index<-ops.Length) Log.Excep(op,"out of range");
            index=index<0 ? index + ops.Length : index;
            index=index % ops.Length;
            Op d=ops[index].Copy();
            return d;
        }
        else
        {
            Log.Excep(op,$"except int as index,but {op.inp[1]}");
        }
        return op;
    }
    
    static Op Append(Op op)
    {
        if (op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
        Op be=op.inp[0];
        //Op item=op.inp[1];
        Op d=new Op(){name=op.inp[0].name};
        if (be.inp == null)
        {
            d.inp=new Op[op.inp.Length-1];
            Array.Copy(op.inp,1,d.inp,0,d.inp.Length);
        }
        else
        {
            d.inp=new Op[be.inp.Length+op.inp.Length-1];
            Array.Copy(be.inp,d.inp,be.inp.Length);
            Array.Copy(op.inp,1,d.inp,be.inp.Length,op.inp.Length-1);
        }
        return d;

    }

    static Op Rename(Op op)
    {
        if (op.inp.Length<2) Log.ExcepWrongParaNum(op,2);
        Op body=op.inp[0].Copy();
        body.name=op.inp[1].name;
        return body;
    }

    static Op atList(Op op)
    {
        Op d=At(op);
        if(d.inp==null || d.inp.Length==0) return new Op(){name="list",inp=[d]};
        Op d2=new Op(){name="list"};
        d2.inp=new Op[d.inp.Length+1];
        d2.inp[0]=new Op(){name=d.name};
        for(int i=0; i < d.inp.Length; ++i)
        {
            d2.inp[i+1]=d.inp[i];
        }
        return d2;
    }
}