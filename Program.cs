
class Program
{
    static void Main()
    {
        string path=@"std.ma";
        
        Ma.ExecuteFile(path);
        // op.showOrigin();
        // Console.ReadKey();
        Ma.Interact();

        // Ma.Execute("(def very f x (f (f (f x))))");
        // Ma.Execute("(def ao x (append x aowu))");
        // string code="(very (very ao (very ao miao)))";
        
        // List<string> s=Ma.CutB(code);
        // for(int i = 0; i < s.Count; ++i)
        // {
        //     Console.WriteLine(s[i]);
        // }

        // string code="(len (cat a (cat b c)))";

        // Op op=Ma.Parse("");
        // op.show();
        // Console.WriteLine(op.ToString());

        // while(op.ExecuteStep())
        // {
        //     Console.Clear();
        //     op.show();
        //     Console.ReadKey();
        // }

        // op.Execute();
        // Console.WriteLine(op);
    }
}
// string code="""
// ((def feb 
//     x 
//     (if 
//         (= x 0) 
//         0 
//         (if 
//             (= x 1) 
//             1 
//             (+ 
//                 (feb (- x 1)) 
//                 (feb (- x 2))
//             )
//         )
//     )
// ) 
// 10)
// """;
//"((def mul2 x (+ x x)) 10)";
//((def add2 y (+ ((def add1 x (+ x 1)) y) 1)) 10)
//((def c x y (if (!= x 0) (c (- x 1) (+ y x)) y)) 10 0)
//((def feb x (if (= x 0) 0 (if (= x 1) 1 (+ (feb (- x 1)) (feb (- x 2)))))) 10)
//((def fib n (if (< n 2) n (+ (fib (- n 1)) (fib (- n 2))))) 10)
//((def feb n ((def febp a b n (if (> n 0) (febp b (+ a b) (- n 1)) a)) 0 1 n)) 10)
//((def fact n (if (= n 0) 1 (* n (fact (- n 1))))) 10)
//(((def curry_add x (def addx y (+ x y))) 10) 11)
//(def and x y (if (= x True) (if (= y True) True False) False))
//(def or x y (if (= x True) True (if (= y True) True False)))
//(def not x (if (= x True) False True))

//Ma.Execute("(def addsub op (rp x y (+ x y) (- x y) op))");
//Ma.Execute("(def takeout op (rp x (nocall x) x op))");
//string code="(takeout (addsub (nocall (+ 3 5))))";

// Ma.Execute("(def cons x y (lam con m (if (= m 0) x y)))");
// Ma.Execute("(def left c (c 0))");
// Ma.Execute("(def right c (c 1))");
// string code="(right (cons 1 2))";

//((def f c x y (c x y)) (lam a b (+ a b)) 10 2)

// Ma.Execute("(def add x y (cons (+ (left x) (left y)) (+ (right x) (right y))))");
// string code="(add (cons 1 2) (cons 3 4))";

// Ma.Execute("(def addd (lam a (lam b (lam c (lam d (+ (+ a b) (+ c d)))))))");
// string code="(((((addd) 1) 2) 3) 4)";

// lam(a,lam(b,lam(c,lam(d,+(+(a,b),+(c,d))))))(1)(2)(3)(4)
// ((lam x y ((lam y x (- x y)) x y)) 2 3)

//def(takeout op rp(x nocall(x) x op))
//def(myif cond then else takeout(cond()(nocall(then),nocall(else))))

// Ma.Execute("(def takeout op (rp x (nocall x) x op))");
// Ma.Execute("(def myif cond then else (takeout (cond then else)))");
// Ma.Execute("(def true x y x)");
// Ma.Execute("(def false x y y)");
// string code="(myif true (nocall (+ 1 2)) (nocall (+ 3 4)))";

// Ma.Execute("(def takeout op (rp x (nocall x) x op))");
// Ma.Execute("(def myif cond then else (call (cond then else)))");
// Ma.Execute("(def true x y x)");
// Ma.Execute("(def false x y y)");
// string code="(myif true (lam (+ 1 2)) (lam (+ 3 4)))";

//(((lam f (f f)) (lam f (lam n (if (= n 0) 1 (* n ((f f) (- n 1))))))) 4)//匿名递归阶乘

//((def fiblist n l t (if (= n t) l (fiblist (+ n 1) (append l (feb n)) t))) 0 list 10)