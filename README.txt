
Ma是一个只由函数组成的语言！

术语：
函数：Ma语言的基本组成单位。一个函数的组成包括名字和多个参数。存在已定义的函数和未定义的函数。存在没有参数的函数。
参数：另一些函数。指(f args)中args的部分。
已定义的函数：系统函数以及使用def进行定义的函数。
未定义的函数：包括数字和True和lam等不在已定义函数的范围里的函数。
函数的调用：调用f，可以这样做(f args)，运行时会使自身替换为自身的输出。如果要调用一个参数数量为0的函数g，可以这样做(g)
没有参数的函数：当函数不处于被调用的位置时，为没有参数的函数。例如(+ 1 2)里面的1和2。
子项：与参数同义。若函数的参数数量为0，但被调用，比如(f)，则视作存在子项。
函数的调用顺序：当已定义函数拥有子项时，会对该函数的子项从左到右进行调用，之后再调用该函数。未定义函数不会被调用。部分函数存在特殊调用顺序。


系统函数：
(def name args body) => name
定义一个函数。其中args的数量不定（可以取0个）。
在调用时，会将body里面和args同名的函数按照顺序替换成传入的参数。
存在特殊调用顺序：内部不会进行调用。
例子：
(def add x y (+ x y)) => add ; (add 1 2) => 3 ；(add (- 3 2) (* 1 4)) => (add 1 4) => 5
(def pi 314) => pi ; (pi) => 314

(call name args) => (name args)
调用函数。其中args的数量不定（可以取0个）。

(if cond then else) => then 或 else
条件判断。
存在特殊调用顺序：会先调用cond，然后根据cond为True或False将自身替换成then或else。

(+ a b) => c
(- a b) => c
(* a b) => c
(/ a b) => c
四则运算。其中a和b和c为整数。

(> a b) => True 或 False
(< a b) => True 或 False
大小比较。其中a和b为整数。

(= a b) => True 或 False
判断相等。若a和b名字相同则返回True，否则False。
(！= a b) => True 或 False
判断不相等。若a和b名字相同则返回False，否则True。

(and a b) => True 或 False
(or a b) => True 或 False
(not a) => True 或 False
逻辑运算。其中a和b为True或False。

(eq a b) => True 或 False
比较两个函数的整体是否相等。会对函数的名字，以及每个子项进行比较。
存在特殊调用顺序：内部不会进行调用。

(rp args target struct x) => y
对x内部结构进行替换。rp会使用由args和其它函数组成的target在x里进行从浅到深，从左到右的匹配，并将第一个匹配到的部分替换成由args和其它函数组成的struct，并返回。
其中target和struct中由args组成的部分会被视为可以匹配任意函数。若args在struct中出现在进行调用的位置，则会无视匹配到的子项，只替换成匹配到的名字。
存在特殊调用顺序：内部不会进行调用。
例子：
(rp a (not a) a (not True)) => True
(rp x y (+ x y) (- x y) (+ (+ 1 2) (* 3 4))) => (- (+ 1 2) (* 3 4)) => -9
(rp f x (f x) (x f) (True (not some))) => (not True) => False   //这里的x只会替换名字

(len a) => b 或 None
得到a的子项的项数。若a不是一个被调用的函数，返回None。
存在特殊调用顺序：内部不会进行调用。

(at a index) => b
得到a的第index个子项。第一项的index为0，后续依次递增。
存在特殊调用顺序：会先调用index，然后调用自身。a不会在其中被调用。

(append a b) => (a b) 或 (a ... b)
给a添加一个子项。会添加在最右侧。若a没有子项，也会添加一个子项。
存在特殊调用顺序：会先调用b，然后调用自身。a不会在其中被调用。


未定义函数：
True
False
None

(lam args body)
(call (lam ...) x) => y
匿名函数。可以作为call的参数调用。
在调用时，会将body里面和args同名的函数按照顺序替换成传入的参数。


特别写法：
当调用的函数是一个需要返回后调用的函数时，解释器会自动转换为后面的写法。
((def f args) some) => (call (def f args) some)
((lam args) some) => (call (lam args) some)

对于存在特殊调用顺序的函数，若要使用正常调用顺序，可以将它包裹在def里。
例子：
(def normlen x (len x)) ; (normlen (+ 2 3)) => (normlen 5) => None