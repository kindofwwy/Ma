
Ma是一个只由函数组成的语言！

像+或是123、abc在这里都会被视作是函数。
你可以把它包裹在括号里，来调用它。例如(myfun)就是调用函数myfun。
你可以在括号内加入更多的函数，用空格隔开它们。例如(myfun hello world)。里面的hello和world是另外的两个函数，我们称它们是myfun的参数。
函数被调用之后，会根据定义将自身替换成另一个函数。例如(+ 1 2)在调用后会变为3。
你可以在函数里面加入要调用的函数。例如(+ 1 (+ 2 3))。它在被调用时会先将(+ 2 3)调用，替换成5，即(+ 1 (+ 2 3)) => (+ 1 5)，然后自己再被调用(+ 1 5) => 6。
你可以使用def函数来定义一个函数。例如(def hello world) => hello。在这之后，当你调用hello，它会被替换成world，(hello) => world。其中hello的部分被称为函数名，world的部分被称为函数体。
def函数里面可以加入更多的参数。例如(def fun x (havefun x))。在这之后，当你调用fun时，需要补充一个参数来填充x。例如(fun party)，在调用后，函数体(havefun x)里的x会被替换为party。(fun party) => (havefun party)。def里面的x的部分被称为形式参数，而(fun party)里的party的部分被称为实际参数。
你可以用匿名函数lam来做到类似的事。例如(lam x (havefun x))。不过这个函数没有名字，你需要使用调用函数call来调用它，并把它和它的实际参数填入call里面，例如(call (lam x (havefun x)) party) => (havefun party)。你还可以这么写((lam x (havefun x)) party)，解释器会把它自动转换为前面的写法。


术语：
函数：Ma语言的基本组成单位。一个函数的组成包括名字和多个参数。存在已定义的函数和未定义的函数。存在没有参数的函数。
参数：另一些函数。指(f args)中args的部分。
已定义的函数：系统函数以及使用def进行定义的函数。
未定义的函数：包括数字和True、lam等不在已定义函数的范围里的函数。
函数的调用：调用f，可以这样做(f args)，运行时会使自身替换为自身的输出。如果要调用一个参数数量为0的函数g，可以这样做(g)
没有参数的函数：当函数不处于被调用的位置时，为没有参数的函数。例如(+ 1 2)里面的1和2。若函数的参数数量为0，但被调用，比如(f)，则视作存在参数，而不是没有参数。
函数的调用顺序：当已定义函数拥有参数时，会对该函数的参数从左到右进行调用，之后再调用该函数。未定义函数不会被调用。部分函数存在特殊调用顺序。


系统函数：
(def name args body) => name
定义一个函数。其中args的数量不定（可以取0个）。
在调用时，会将body里面和args同名的函数按照顺序替换成传入的参数，之后将自身替换为body。
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

(at a index) => b
获取a的第index个子项。第一项的index为0，后续依次递增。支持index使用-1返回a的倒数第1个子项。

(append a b) => (a b)
给a添加一个或多个子项。会添加在最右侧。若a没有子项，也会添加一个子项。
例子：
(append list 1 2 3) => (list 1 2 3)

(rename a name) => name
把a的名字改成name。a的子项会保留。

(atlist a index) => (list b b-args)
获取a的第index个子项，获取的结果会被转化为list形式，即名字为list，参数首项为a的第index项的名字，剩下的项为a的第index项的参数的未定义函数。
例子：
(atlist (numlist 1 2 3) 2) => (list 3)
(atlist (list (a)) 0) => (list (a))
(atlist (list (* (+ 1 2) 3)) 0) => (list * (+ 1 2) 3)


约定的未定义函数：
True
False
None

(list name args)
用来装载函数的结构。

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

对于希望不进行调用的函数，可以将它包裹在一个未定义函数里。
例子：
(nocall (+ 1 2)) //(+ 1 2)不会被调用