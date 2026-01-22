class Program
{
    static void Main()
    {
        string code="(cat a (cat b c))";

        Op op=Ma.Parse(code);
        op.show();
        Console.WriteLine(op);
        
        // foreach(var o in op.ExecuteSingleStep())
        // {
        //     Console.WriteLine(o);
        // }

        // while(op.inp!=null && op.HasDefine())
        // {
        //     op.ExecuteStep();
        //     Console.WriteLine(op);
        // }

        op.Execute();
        Console.WriteLine(op);
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