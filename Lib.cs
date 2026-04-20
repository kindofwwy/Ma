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

        lib["def"]=Def;
        lib["defn"]=Defnocall;
        lib["call"]=Call;
        lib["if"]=If;
        //lambda (lam args body)
        lib["+"]=Dyadic((int x,int y)=>x+y);
        lib["-"]=Dyadic((int x,int y)=>x-y);
        lib["*"]=Dyadic((int x,int y)=>x*y);
        lib["/"]=Div;
        lib[">"]=Dyadic((int x,int y)=>x>y);
        lib["<"]=Dyadic((int x,int y)=>x<y);
        lib["="]=Dyadic((string x,string y)=>x==y);
        lib["!="]=Dyadic((string x,string y)=>x!=y);

        lib["namecat"]=Dyadic((string x,string y)=>x+y);
        lib["namelen"]=Mono((string x)=>x.Length);
        lib["nameat"]=strAt;

        lib["eq"]=AllEq;
        lib["rp"]=Replace;
        lib["rpall"]=RpAll;
        lib["len"]=Len;
        lib["at"]=At;
        lib["set"]=Set;
        lib["append"]=Append;
        lib["rename"]=Rename;
        lib["atlist"]=atList;
        lib["lookup"]=LookUp;
        lib["remove"]=Remove;

        lib["wait"]=Wait;
        lib["exp"]=Exp;
        lib["exe"]=Exe;
        lib["step"]=Step;

        lib["first"]=First;
        lib["last"]=Last;
        lib["rest"]=Rest;
        lib["insert"]=Insert;
        lib["pop"]=Pop;

        lib["catch"]=Catch;
        lib["raise"]=Raise;
    }

    static Op Def(Op op)
    {
        if(op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        Op d=op.inp[0];
        Op[] form=[];
        if (op.inp.Length > 2)
        {
            form=new Op[op.inp.Length-2];
            Array.Copy(op.inp,1,form,0,form.Length);
        }
        Op define=op.inp[op.inp.Length-1];
        Op.defines[d.name]=(form,define);
        return d;
    }

    static Op Defnocall(Op op)
    {
        Op d=Def(op);
        if(!Op.NoCallSub.Exists((string x)=>x==d.name))
            Op.NoCallSub.Add(d.name);
        return d;
    }

    static Op Call(Op op)
    {
        if(op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op be=op.inp[0];
        if (be.name == "lam" && be.inp!=null && be.inp.Length>0)
        {
            Op lam=be;
            Op lambody=lam.inp[lam.inp.Length-1].Copy();
            Op[] lamargs=new Op[lam.inp.Length-1];
            Array.Copy(lam.inp,0,lamargs,0,lam.inp.Length-1);
            Op[] lamRealargs=new Op[lam.inp.Length-1];
            Array.Copy(op.inp,1,lamRealargs,0,lam.inp.Length-1);
            lambody.Replaces(lamargs,lamRealargs);
            return lambody;
        }
        be.inp=new Op[op.inp.Length-1];
        Array.Copy(op.inp,1,be.inp,0,be.inp.Length);
        return be;
    }

    static Op If(Op op)
    {
        if(op.inp.Length<3) return Log.ExcepWrongParaNum(op,3);
        Op d;
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
            return Log.Excep(op,$"except_{true}_or_{false}/but_{op.inp[0].name}");
        }
        return d;
    }

    static MaFunc Dyadic(Dya func)
    {
        Op dya(Op op)
        {
            if(op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
            Op d=new Op();
            if(int.TryParse(op.inp[0].name,out int a))
            {
                if(int.TryParse(op.inp[1].name,out int b))
                {
                    d.name=func(a,b).ToString() ?? "None";
                    return d;
                }
            }
            return Log.Excep(op,$"except_int/but_{op.inp[0].name}_and_{op.inp[1].name}");
        }
        return dya;
    }

    static Op Div(Op op)
    {
        if(op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        if(op.inp[1].name=="0") return Log.Excep(op,"division_by_zero");
        Op d=Dyadic((int x,int y)=>x/y)(op);
        return d;
    }

    static MaFunc Dyadic(DyaStr func)
    {
        Op dya(Op op)
        {   
            if(op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
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
            if(op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
            Op d=new Op();
            d.name=func(op.inp[0].name).ToString() ?? "None";
            return d;
        }
        return mon;
    }

    static Op AllEq(Op op)
    {
        if(op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
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
            List<Op> tars=[];
            List<Op> cons=[];

            for(int i = 0; i < assumeArgs.Length; ++i)
            {
                if (assumeArgs[i].HasValue)
                {
                    tars.Add(new Op(){name=args[i]});
                    cons.Add(assumeArgs[i].Value);
                }
            }
            content.ReplacesOnly(tars.ToArray(),cons.ToArray());
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

    static void selectAll(Op target,string[] args,ref Op be,Op content)
    {
        Op?[] assumeArgs=new Op?[args.Length];

        if(be.inp != null)
        {
            for(int i = 0; i < be.inp.Length; ++i)
            {
                selectAll(target,args,ref be.inp[i], content);
            }
        }
        if(select_part(target,args,be,ref assumeArgs))
        {
            List<Op> tars=[];
            List<Op> cons=[];

            for(int i = 0; i < assumeArgs.Length; ++i)
            {
                if (assumeArgs[i].HasValue)
                {
                    tars.Add(new Op(){name=args[i]});
                    cons.Add(assumeArgs[i].Value);
                }
            }
            Op c=content.Copy();
            c.ReplacesOnly(tars.ToArray(),cons.ToArray());
            be.ShallowCopyToThis(c);
        }
    }

    static Op Replace(Op op)
    {
        //(rp 通配符 目标结构 替换结构 目标) 通配符可多项,可省略
        if (op.inp.Length<3) return Log.ExcepWrongParaNum(op,3);
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

    static Op RpAll(Op op)
    {
        //(rpall 通配符 目标结构 替换结构 目标) 通配符可多项,可省略
        if (op.inp.Length<3) return Log.ExcepWrongParaNum(op,3);
        string[] args=new string[op.inp.Length-3];
        for(int i = 0; i < op.inp.Length-3; ++i)
        {
            args[i]=op.inp[i].name;
        }
        Op be=op.inp[op.inp.Length-1].Copy();
        Op content=op.inp[op.inp.Length-2].Copy();
        Op target=op.inp[op.inp.Length-3];
        
        selectAll(target,args,ref be,content);

        return be;
    }

    static Op strAt(Op op)
    {
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        string name=op.inp[0].name;
        if(int.TryParse(op.inp[1].name,out int index))
        {
            index=index<0 ? index - index/name.Length*name.Length + name.Length : index;
            index=index % name.Length;
            return new Op(){name=name[index].ToString()};
        }
        else
        {
            return Log.ExcepIndex(op,op.inp[1].ToString());
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
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        Op[]? ops=op.inp[0].inp;
        if (ops==null || ops.Length==0) return Log.ExcepNoItem(op);
        else if(int.TryParse(op.inp[1].name,out int index))
        {
            if(index>ops.Length-1 || index<-ops.Length) return Log.OutOfRange(op,index);
            index=index<0 ? index + ops.Length : index;
            index=index % ops.Length;
            Op d=ops[index].Copy();
            return d;
        }
        else
        {
            return Log.ExcepIndex(op,op.inp[1].ToString());
        }
    }

    static Op Set(Op op)
    {
        if (op.inp.Length<3) return Log.ExcepWrongParaNum(op,3);
        Op[]? ops=op.inp[0].inp;
        if (ops==null || ops.Length==0) return Log.ExcepNoItem(op);
        else if(int.TryParse(op.inp[1].name,out int index))
        {
            if(index>ops.Length-1 || index<-ops.Length) return Log.OutOfRange(op,index);
            index=index<0 ? index + ops.Length : index;
            index=index % ops.Length;
            ops[index]=op.inp[2];
            op.name=op.inp[0].name;
            op.inp=ops;
            
            return op;
        }
        else
        {
            return Log.ExcepIndex(op,op.inp[1].ToString());
        }
    }
    
    static Op Append(Op op)
    {
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
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
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        Op body=op.inp[0];
        body.name=op.inp[1].name;
        return body;
    }

    static Op atList(Op op)
    {
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
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

    static Op Exp(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op d=op.inp[0];
        d.Explain();
        return d;
    }

    static Op Exe(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op d=op.inp[0];
        if (d.ExecuteStep())
        {
            return new Op{name="exe",inp=[d]};
        }
        else
        {
            return d;  
        }
        
    }

    static Op Step(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op d=op.inp[0];
        d.ExecuteStep();
        return d;
    }

    static bool waitStep(Op op,out Op d)
    {
        d=new Op();
        if (op.inp != null)
        {
            if (op.name == "wait")
            {
                return false;
            }
            if (op.name == "expif" && (op.inp[0].name==true.ToString() || op.inp[0].name==false.ToString()))
            {
                op.name="if";
                op.Explain();
                d=op;
                return true;
            }
            for(int i = 0; i < op.inp.Length; ++i)
            {
                if(waitStep(op.inp[i],out d))
                {
                    op.inp[i]=d;
                    d=op;
                    return true;
                }
            }
            if (op.name == "exp" || op.name == "exe" || op.name == "step")
            {
                op.Explain();
                d=op;
                return true;
            }
        }
        return false;
    }

    static Op Wait(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        if (waitStep(op.inp[0],out Op d))
        {
            return new Op{name="wait",inp=[d]};
        }
        else
        {
            return op.inp[0];
        }
        
    }

    static Op Rest(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op be=op.inp[0];
        if (be.inp==null || be.inp.Length==0) return Log.ExcepNoItem(op);
        Op d=new Op(){name=be.name};
        d.inp=new Op[be.inp.Length-1];
        Array.Copy(be.inp,1,d.inp,0,d.inp.Length);
        return d;
    }

    static Op Insert(Op op)
    {
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        Op d=new Op(){name=op.inp[1].name};
        Op be=op.inp[1];
        if (be.inp != null)
        {
            d.inp=new Op[be.inp.Length+1];
            d.inp[0]=op.inp[0];
            Array.Copy(be.inp,0,d.inp,1,be.inp.Length);
        }
        else
        {
            d.inp=[op.inp[0]];
        }
        return d;
    }

    static Op Pop(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op be=op.inp[0];
        if (be.inp == null || be.inp.Length == 0) return Log.ExcepNoItem(op);
        Op d=new Op(){name=be.name};
        d.inp=new Op[be.inp.Length-1];
        Array.Copy(be.inp,0,d.inp,0,be.inp.Length-1);
        return d;
    }

    static Op LookUp(Op op)
    {
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        Op[]? be=op.inp[0].inp;
        if (be==null) return Log.ExcepNoItem(op);
        Op tar=op.inp[1];
        for(int i = 0; i < be.Length; ++i)
        {
            if (be[i].name == tar.name)
            {
                Op d=be[i];
                return d;
            }
        }
        return Log.ExcepNotFound(op);
    }

    static Op Catch(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        return op.inp[0];
    }

    static Op Raise(Op op)
    {
        return Log.Err(op.inp);
    }

    static Op Remove(Op op)
    {
        if (op.inp.Length<2) return Log.ExcepWrongParaNum(op,2);
        Op[]? ops=op.inp[0].inp;
        if (ops==null || ops.Length==0) return Log.ExcepNoItem(op);
        else if(int.TryParse(op.inp[1].name,out int index))
        {
            if(index>ops.Length-1 || index<-ops.Length) return Log.OutOfRange(op,index);
            index=index<0 ? index + ops.Length : index;
            index=index % ops.Length;
            Op[] newinp=new Op[ops.Length-1];
            Array.Copy(ops,0,newinp,0,index);
            Array.Copy(ops,index+1,newinp,index,ops.Length-index-1);

            op=op.inp[0];
            op.inp=newinp;
            return op;
        }
        else
        {
            return Log.ExcepIndex(op,op.inp[1].ToString());
        }
    }

    static Op First(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op d=op.inp[0];
        if (d.inp==null || d.inp.Length==0) return Log.ExcepNoItem(op);
        else return d.inp[0];
    }

    static Op Last(Op op)
    {
        if (op.inp.Length<1) return Log.ExcepWrongParaNum(op,1);
        Op d=op.inp[0];
        if (d.inp==null || d.inp.Length==0) return Log.ExcepNoItem(op);
        else return d.inp[d.inp.Length-1];
    }
    // static Op Make(Op op)
    // {
    //     if (op.inp.Length<3) return Log.ExcepWrongParaNum(op,3);
    //     Op[] x=[op.inp[1]];
    //     Op[] y=(op.inp[2].name==op.inp[0].name?op.inp[2].inp : [op.inp[2]]) ?? [];
    //     if(op.inp[1].name=="None") x=[];
    //     if(op.inp[2].name=="None") y=[];
        
    //     Op d=new Op(){name=op.inp[0].name};
    //     d.inp=new Op[x.Length+y.Length];
    //     Array.Copy(x,d.inp,x.Length);
    //     Array.Copy(y,0,d.inp,x.Length,y.Length);
    //     return d;
    // }
}